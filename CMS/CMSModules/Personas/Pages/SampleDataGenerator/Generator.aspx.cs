using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Personas;
using CMS.SiteProvider;
using CMS.UIControls;

using Newtonsoft.Json.Linq;

using System;
using System.Linq;
using System.Net;


/// <summary>
/// Generates various Online marketing data. This page is for internal purposes only.
/// </summary>
public partial class CMSModules_Personas_Pages_SampleDataGenerator_Generator : CMSDeskPage
{
    #region "Infrastructure"

    private static class StaticRandom
    {
        private static Random random = new Random();


        public static int Next()
        {
            return random.Next();
        }


        public static int Next(int maxValue)
        {
            return random.Next(maxValue);
        }


        public static int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }


        public static double NextDouble()
        {
            return random.NextDouble();
        }
    }


    private class FirstLetterCapitalizer
    {
        public string CapitalizeFirstLetters(string toUpper)
        {
            return string.Join(" ", toUpper.Split(' ').Where(word => word.Length > 2).Select(s => char.ToUpper(s[0]) + s.Substring(1)));
        }
    }


    private class BulkInsertion
    {
        public static void Insert(IEnumerable<BaseInfo> data)
        {
            if (data == null || !data.Any())
            {
                return;
            }

            var baseInfo = data.First();
            var generalizedInfo = baseInfo.Generalized;
            string tableName = DataClassInfoProvider.GetClasses().WhereEquals("ClassName", generalizedInfo.ObjectClassName).FirstObject.ClassTableName;

            ConnectionHelper.BulkInsert(CreateDataTable(data, baseInfo), tableName);
        }


        private static IEnumerable<DataColumn> ExtranctDataColumns(BaseInfo info)
        {
            var type = info.GetType();

            return from c in info.ColumnNames
                   let prop = type.GetProperty(c)
                   where prop != null
                   select new DataColumn(c, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }


        private static DataTable CreateDataTable(IEnumerable<BaseInfo> data, BaseInfo info)
        {
            DataTable table = new DataTable();

            var columns = ExtranctDataColumns(info).ToArray();
            if (!columns.Any())
            {
                return table;
            }

            table.Columns.AddRange(columns);
            foreach (BaseInfo item in data)
            {
                DataRow row = table.NewRow();
                FillDataRow(item, row);
                table.Rows.Add(row);
            }

            return table;
        }


        private static void FillDataRow(BaseInfo item, DataRow row)
        {
            var generalizedInfo = item.Generalized;
            var prefix = generalizedInfo.ObjectClassName.Split('.').Last();

            foreach (string column in row.Table.Columns.OfType<DataColumn>().Select(c => c.ColumnName))
            {
                if (column == generalizedInfo.GUIDColumn)
                {
                    var guid = Guid.NewGuid();
                    item.SetValue(column, guid);
                    row[column] = guid;
                }
                else if ((column == generalizedInfo.TimeStampColumn) ||
                         (column == string.Format("{0}Created", prefix)))
                {
                    var date = DateTime.Now;
                    item.SetValue(column, date);
                    row[column] = date;
                }
                else
                {
                    row[column] = item.GetValue(column) ?? DBNull.Value;
                }
            }
        }
    }

    #endregion


    #region "Personal data"

    private class PersonalDataStructure
    {
        public string FirstName
        {
            get;
            set;
        }


        public string LastName
        {
            get;
            set;
        }


        public string Address
        {
            get;
            set;
        }


        public string City
        {
            get;
            set;
        }


        public UserGenderEnum Gender
        {
            get;
            set;
        }


        public string MobilePhone
        {
            get;
            set;
        }


        public string HomePhone
        {
            get;
            set;
        }


        public string ZIP
        {
            get;
            set;
        }
    }


    private interface IPersonalDataGenerator
    {
        PersonalDataStructure GeneratePersonalData(UserGenderEnum? gender = null);
    }


    private class RealPersonalDataGenerator : IPersonalDataGenerator
    {
        public PersonalDataStructure GeneratePersonalData(UserGenderEnum? gender = null)
        {
            string contactDataUrl = "http://api.randomuser.me/0.3/";

            if (gender != null)
            {
                contactDataUrl = URLHelper.AddParameterToUrl(contactDataUrl, "gender", gender.Value == UserGenderEnum.Female ? "female" : "male");
            }

            string jsonResponse = new WebClient().DownloadString(contactDataUrl);

            dynamic response = JObject.Parse(jsonResponse);
            dynamic user = response.results[0].user;

            var capitalizer = new FirstLetterCapitalizer();

            return new PersonalDataStructure()
            {
                Gender = user.gender == "male" ? UserGenderEnum.Male : UserGenderEnum.Female,
                FirstName = capitalizer.CapitalizeFirstLetters((string)user.name.first),
                LastName = capitalizer.CapitalizeFirstLetters((string)user.name.last),
                Address = capitalizer.CapitalizeFirstLetters((string)user.location.street),
                City = capitalizer.CapitalizeFirstLetters((string)user.location.city),
                MobilePhone = user.cell,
                HomePhone = user.phone,
                ZIP = user.location.zip,
            };
        }
    }

    #endregion


    #region "Generators"

    private class StupidPersonalDataGenerator : IPersonalDataGenerator
    {
        public PersonalDataStructure GeneratePersonalData(UserGenderEnum? gender = null)
        {
            int contactNumber = StaticRandom.Next();

            string name;
            string lastName = "Doe " + contactNumber;
            gender = gender ?? (UserGenderEnum)(contactNumber % 3);
            if (gender.Value == UserGenderEnum.Male)
            {
                name = "John";
            }
            else if(gender.Value == UserGenderEnum.Female)
            {
                name = "Jolene";
            }
            else
            {
                name = "Dog";
            }


            return new PersonalDataStructure()
            {
                Gender = gender.Value,
                FirstName = name,
                LastName = lastName,
                Address = name + " street",
                City = name + " city",
                MobilePhone = contactNumber.ToString(),
                HomePhone = (contactNumber * 2).ToString(),
                ZIP = (contactNumber * 3).ToString(),
            };
        }
    }


    private class SampleContactStatusesGenerator
    {
        private readonly int mSiteId;


        public SampleContactStatusesGenerator(int siteID)
        {
            mSiteId = siteID;
        }


        public void Generate()
        {
            var names = new List<string>
            {
                "VIP",
                "Interested",
                "Prospective client",
                "Not interested",
                "Waste",
            };

            foreach (var contactStatusName in names)
            {
                var cs = new ContactStatusInfo()
                {
                    ContactStatusDescription = "This client is " + contactStatusName,
                    ContactStatusDisplayName = contactStatusName,
                    ContactStatusName = ValidationHelper.GetCodeName(contactStatusName),
                    ContactStatusSiteID = mSiteId,
                };

                cs.Insert();
            }
        }
    }


    private class SampleContactsGenerator
    {
        private readonly IPersonalDataGenerator mPersonalDataGenerator;
        protected readonly int mSiteId;


        private readonly List<string> mCompanyNames = new List<string>
        {
            "xentypol",
            "grifteraagency",
            "rawerastudio",
            "webcreatives",
            "gleretexdesign",
            "zafix",
            "brevia",
            "gianto",
            "duramo",
            "fumla"
        };


        protected readonly List<ContactStatusInfo> mExistingContactStatuses;


        public SampleContactsGenerator(IPersonalDataGenerator personalDataGenerator, int siteID)
        {
            mPersonalDataGenerator = personalDataGenerator;
            mSiteId = siteID;
            mExistingContactStatuses = ContactStatusInfoProvider.GetContactStatuses().OnSite(mSiteId).ToList();
        }


        public void Generate(int contactsCount)
        {
            List<BaseInfo> contacts = new List<BaseInfo>();

            for (int i = 0; i < contactsCount; ++i)
            {
                contacts.Add(CreateContactInfo(mCompanyNames));
            }

            BulkInsertion.Insert(contacts);
        }


        protected string GetEmail(string firstName, string lastName)
        {
            return firstName + "." + lastName + "@" + mCompanyNames[StaticRandom.Next(mCompanyNames.Count)] + ".com";
        }


        protected ContactInfo CreateContactInfo(List<string> companyNames)
        {
            var realPerson = mPersonalDataGenerator.GeneratePersonalData();

            ContactInfo contact = new ContactInfo()
            {
                ContactGender = (int)realPerson.Gender,
                ContactFirstName = realPerson.FirstName,
                ContactLastName = realPerson.LastName,
                ContactEmail = realPerson.FirstName + "." + realPerson.LastName + "@" + companyNames[StaticRandom.Next(companyNames.Count)] + ".com",
                ContactMobilePhone = realPerson.MobilePhone,
                ContactHomePhone = realPerson.HomePhone,
                ContactAddress1 = realPerson.Address,
                ContactCity = realPerson.City,
                ContactCountryID = 271,
                ContactZIP = realPerson.ZIP,
                ContactIsAnonymous = true,
                ContactStatusID = mExistingContactStatuses[StaticRandom.Next(mExistingContactStatuses.Count)].ContactStatusID,
                ContactSiteID = mSiteId,
            };
            return contact;
        }
    }


    private interface ISamplePersonasGenerator
    {
        void GeneratePersonas(int siteId);
    }


    private class StupidPersonasGenerator : ISamplePersonasGenerator
    {
        public void GeneratePersonas(int siteId)
        {
            GenerateDisabledPersona(siteId);
            GenerateStupidMalePersona(siteId);
            GenerateStupidFemalePersona(siteId);
        }

        private void GenerateDisabledPersona(int siteId)
        {
            var stupidPerson = new StupidPersonalDataGenerator().GeneratePersonalData();

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = stupidPerson.FirstName + " " + stupidPerson.LastName + " (disabled)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = false
            };
            persona.Insert();
        }


        private void GenerateStupidMalePersona(int siteId)
        {
            var stupidPerson = new StupidPersonalDataGenerator().GeneratePersonalData(UserGenderEnum.Male);

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = stupidPerson.FirstName + " " + stupidPerson.LastName + " (male)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = true
            };
            persona.Insert();

            var rule = new RuleInfo
            {
                RuleScoreID = persona.PersonaScoreID,
                RuleDisplayName = "Is male",
                RuleName = "Rule-" + Guid.NewGuid(),
                RuleValue = 1000,
                RuleType = RuleTypeEnum.Attribute,
                RuleParameter = "ContactGender",
                RuleCondition = "<condition><attribute name=\"ContactGender\"><value>1</value></attribute><wherecondition>ContactGender = 1</wherecondition></condition>",
                RuleSiteID = siteId
            };
            rule.Insert();
        }


        private void GenerateStupidFemalePersona(int siteId)
        {
            var stupidPerson = new StupidPersonalDataGenerator().GeneratePersonalData(UserGenderEnum.Female);

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = stupidPerson.FirstName + " " + stupidPerson.LastName + " (female)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = true
            };
            persona.Insert();

            var rule = new RuleInfo
            {
                RuleScoreID = persona.PersonaScoreID,
                RuleDisplayName = "Is female",
                RuleName = "Rule-" + Guid.NewGuid(),
                RuleValue = 1000,
                RuleType = RuleTypeEnum.Attribute,
                RuleParameter = "ContactGender",
                RuleCondition = "<condition><attribute name=\"ContactGender\"><value>2</value></attribute><wherecondition>ContactGender = 2</wherecondition></condition>",
                RuleSiteID = siteId
            };
            rule.Insert();
        }
    }


    private class RealPersonasGenerator : ISamplePersonasGenerator
    {
        public void GeneratePersonas(int siteId)
        {
            GenerateDisabledPersona(siteId);
            GenerateRealMalePersona(siteId);
            GenerateRealFemalePersona(siteId);
        }


        private void GenerateDisabledPersona(int siteId)
        {
            var realPerson = new RealPersonalDataGenerator().GeneratePersonalData();

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = realPerson.FirstName + " " + realPerson.LastName + " (disabled)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = false
            };
            persona.Insert();
        }


        private void GenerateRealMalePersona(int siteId)
        {
            PersonalDataStructure personalData = new RealPersonalDataGenerator().GeneratePersonalData(UserGenderEnum.Male);

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = personalData.FirstName + " " + personalData.LastName + " (male)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = true
            };
            persona.Insert();

            var rule = new RuleInfo
            {
                RuleScoreID = persona.PersonaScoreID,
                RuleDisplayName = "Is male",
                RuleName = "Rule-" + Guid.NewGuid(),
                RuleValue = 1000,
                RuleType = RuleTypeEnum.Attribute,
                RuleParameter = "ContactGender",
                RuleCondition = "<condition><attribute name=\"ContactGender\"><value>1</value></attribute><wherecondition>ContactGender = 1</wherecondition></condition>",
                RuleSiteID = siteId
            };
            rule.Insert();
        }


        private void GenerateRealFemalePersona(int siteId)
        {
            PersonalDataStructure personalData = new RealPersonalDataGenerator().GeneratePersonalData(UserGenderEnum.Female);

            PersonaInfo persona = new PersonaInfo
            {
                PersonaDisplayName = personalData.FirstName + " " + personalData.LastName + " (female)",
                PersonaName = "Persona-" + Guid.NewGuid(),
                PersonaSiteID = siteId,
                PersonaPointsThreshold = 100,
                PersonaEnabled = true
            };
            persona.Insert();

            var rule = new RuleInfo
            {
                RuleScoreID = persona.PersonaScoreID,
                RuleDisplayName = "Is female",
                RuleName = "Rule-" + Guid.NewGuid(),
                RuleValue = 1000,
                RuleType = RuleTypeEnum.Attribute,
                RuleParameter = "ContactGender",
                RuleCondition = "<condition><attribute name=\"ContactGender\"><value>2</value></attribute><wherecondition>ContactGender = 2</wherecondition></condition>",
                RuleSiteID = siteId
            };
            rule.Insert();
        }
    }


    private class SampleScoresGenerator
    {
        public void GenerateScores(int scoresCount, int siteId)
        {
            List<ScoreInfo> scores = new List<ScoreInfo>();

            for (int i = 0; i < scoresCount; i++)
            {
                ScoreInfo score = new ScoreInfo
                {
                    ScoreDisplayName = "Score #" + i,
                    ScoreName = "Score-" + Guid.NewGuid(),
                    ScoreBelongsToPersona = false,
                    ScoreEnabled = true,
                    ScoreStatus = 0,
                    ScoreSiteID = siteId
                };
                score.Insert();
                scores.Add(score);
            }

            List<RuleInfo> rules = new List<RuleInfo>();

            foreach (var score in scores)
            {
                rules.AddRange(GenerateRules(score));
            }

            BulkInsertion.Insert(rules);
        }


        private IEnumerable<RuleInfo> GenerateRules(ScoreInfo score)
        {
            var rules = new List<RuleInfo>();

            int ruleCount = StaticRandom.Next(15, 25);
            for (int i = 0; i < ruleCount; i++)
            {
                var ruleInfo = GenerateRule(score.ScoreSiteID);
                ruleInfo.RuleScoreID = score.ScoreID;
                rules.Add(ruleInfo);
            }

            return rules;
        }


        private RuleInfo GenerateRule(int siteId)
        {
            var random = StaticRandom.NextDouble();
            RuleTypeEnum ruleType = random < 0.166 ?
                RuleTypeEnum.Attribute : random < 0.333 ?
                    RuleTypeEnum.Macro : RuleTypeEnum.Activity;

            switch (ruleType)
            {
                case RuleTypeEnum.Activity:
                    return GenerateActivityRule(siteId);

                case RuleTypeEnum.Attribute:
                    return GenerateAttributeRule(siteId);

                case RuleTypeEnum.Macro:
                    return GenerateMacroRule(siteId);
            }

            return null;
        }


        private RuleInfo GenerateActivityRule(int siteId)
        {
            return GeneratePageVisitRule(siteId);
        }


        private RuleInfo GenerateAttributeRule(int siteId)
        {
            return GenerateContactLastNameContainsRandomLetterRule(siteId);
        }


        private RuleInfo GenerateContactLastNameContainsRandomLetterRule(int siteId)
        {
            var rule = GenerateBaseRule(RuleTypeEnum.Attribute, siteId);
            char letter = (char)StaticRandom.Next(97, 122);

            rule.RuleParameter = "ContactLastName";
            rule.RuleCondition = string.Format("<condition><attribute name=\"ContactLastName\"><value>{0}</value><params><Operator>0</Operator></params></attribute><wherecondition>ISNULL([ContactLastName], '') LIKE N'%{0}%'</wherecondition></condition>", letter);
            rule.RuleDisplayName = string.Format("Attribute rule for {0} points - contact last name contains {1}", rule.RuleValue, letter);

            return rule;
        }


        private RuleInfo GenerateMacroRule(int siteId)
        {
            var rule = GenerateBaseRule(RuleTypeEnum.Macro, siteId);

            string value = StaticRandom.NextDouble() < 0.5 ? "true" : "false";
            rule.RuleCondition = string.Format("<condition><macro><value>{{%{0}%}}</value></macro></condition>", value);
            rule.RuleDisplayName = string.Format("Macro rule for {0} points which is always {1}", rule.RuleValue, value);

            return rule;
        }


        private RuleInfo GenerateBaseRule(RuleTypeEnum ruleType, int siteId)
        {
            return new RuleInfo
            {
                RuleType = ruleType,
                RuleSiteID = siteId,
                RuleValue = StaticRandom.Next(10, 1000),
                RuleIsRecurring = false,
                RuleName = "Rule-" + Guid.NewGuid()
            };
        }


        private RuleInfo GeneratePageVisitRule(int siteId)
        {
            var rule = GenerateBaseRule(RuleTypeEnum.Activity, siteId);

            rule.RuleParameter = "pagevisit";
            rule.RuleCondition = "<condition><activity name=\"pagevisit\"><field name=\"ActivityCreated\"><params><seconddatetime>1/1/0001 12:00:00 AM</seconddatetime></params></field><field name=\"ActivityURL\"><params><operator>0</operator></params></field><field name=\"ActivityTitle\"><params><operator>0</operator></params></field><field name=\"ActivityComment\"><params><operator>0</operator></params></field><field name=\"ActivityCampaign\"><params><operator>0</operator></params></field><field name=\"ActivityIPAddress\"><params><operator>0</operator></params></field><field name=\"ActivityURLReferrer\"><params><operator>0</operator></params></field><field name=\"Operator\"><value>0</value></field></activity><wherecondition>ActivityType='pagevisit'</wherecondition></condition>";
            rule.RuleDisplayName = string.Format("Page visit rule for {0} points", rule.RuleValue);

            return rule;
        }
    }


    private class SampleActivitiesGenerator
    {
        public int GenerateActivitiesForContacts(IEnumerable<ContactInfo> contacts, int mediumActivitiesCount, List<TreeNode> treeNodes)
        {
            var activities = new List<ActivityInfo>();
            DateTime created = DateTime.Now;

            foreach (var contact in contacts)
            {
                int activitiesCount = (int)(mediumActivitiesCount * StaticRandom.NextDouble() * 2);

                for (int i = 0; i < activitiesCount; i++)
                {
                    var treeNode = treeNodes[StaticRandom.Next(treeNodes.Count)];

                    var activityInfo = new ActivityInfo
                    {
                        ActivityCreated = created,
                        ActivityType = "pagevisit",
                        ActivityActiveContactID = contact.ContactID,
                        ActivityOriginalContactID = contact.ContactID,
                        ActivitySiteID = contact.ContactSiteID,
                        ActivityTitle = "Page visit on '" + treeNode.DocumentName + "' page",
                        ActivityItemID = 0,
                        ActivityItemDetailID = 0,
                        ActivityURL = treeNode.DocumentUrlPath,
                        ActivityNodeID = treeNode.NodeID,
                        ActivityValue = "",
                        ActivityIPAddress = "123.123.456.789",
                        ActivityCampaign = "",
                        ActivityURLReferrer = treeNode.DocumentUrlPath + "-totojereferrer",
                        ActivityCulture = treeNode.DocumentCulture,
                    };

                    activities.Add(activityInfo);
                }
            }
            BulkInsertion.Insert(activities);

            var activityIds = ActivityInfoProvider.GetActivities().WhereEquals("ActivityCreated", created).Select(a => a.ActivityID);

            var pageVisits = new List<PageVisitInfo>();

            foreach (var activityId in activityIds)
            {
                var pageVisitInfo = new PageVisitInfo
                {
                    PageVisitActivityID = activityId,
                    PageVisitMVTCombinationName = "",
                    PageVisitABVariantName = "",
                    PageVisitDetail = "?totojequerystring=prase",
                };

                pageVisits.Add(pageVisitInfo);
            }

            BulkInsertion.Insert(pageVisits);

            return activities.Count;
        }
    }


    private class SampleDataGenerator
    {
        public class GenerationOptions
        {
            public bool GenerateContactStatuses
            {
                get;
                set;
            }


            public int ContactsCount
            {
                get;
                set;
            }


            public bool ContactsWithRealNames
            {
                get;
                set;
            }


            public bool GeneratePersonas
            {
                get;
                set;
            }


            public int ScoresCount
            {
                get;
                set;
            }


            public int ActivitiesForEachExistingContactCount
            {
                get;
                set;
            }
        }


        private readonly int mSiteId;


        public Action<string> Information;
        public Action<string> Error;


        public SampleDataGenerator(int siteID)
        {
            mSiteId = siteID;
        }


        public void Generate(GenerationOptions options)
        {
            try
            {
                if (options.GenerateContactStatuses)
                {
                    if (ContactStatusInfoProvider.GetContactStatuses().OnSite(mSiteId).Any())
                    {
                        Information("Contact statuses already exists");
                    }
                    else
                    {
                        new SampleContactStatusesGenerator(mSiteId).Generate();
                        Information("Contact statuses generated");
                    }
                }
                if (options.ContactsCount > 0)
                {
                    IPersonalDataGenerator personalDataGenerator = options.ContactsWithRealNames ?
                        new RealPersonalDataGenerator() :
                        (IPersonalDataGenerator)new StupidPersonalDataGenerator();
                    SampleContactsGenerator generator = new SampleContactsGenerator(personalDataGenerator, mSiteId);
                    generator.Generate(options.ContactsCount);
                    Information(options.ContactsCount + " contacts generated");
                }
                if (options.GeneratePersonas)
                {
                    ISamplePersonasGenerator personaGenerator = options.ContactsWithRealNames ?
                        new RealPersonasGenerator() :
                        (ISamplePersonasGenerator)new StupidPersonasGenerator();
                    personaGenerator.GeneratePersonas(mSiteId);
                    Information("Sample personas generated");
                }
                if (options.ScoresCount > 0)
                {
                    new SampleScoresGenerator().GenerateScores(options.ScoresCount, mSiteId);
                    Information(options.ScoresCount + " scores generated");
                }
                if (options.ActivitiesForEachExistingContactCount > 0)
                {
                    var contacts = ContactInfoProvider.GetContacts().OnSite(mSiteId);

                    var documents = DocumentHelper.GetDocuments()
                        .PublishedVersion()
                        .OnSite(mSiteId);

                    int activitiesGenerated = new SampleActivitiesGenerator().GenerateActivitiesForContacts(contacts, options.ActivitiesForEachExistingContactCount, documents.ToList());

                    Information(activitiesGenerated + " activities generated");
                }
            }
            catch (Exception e)
            {
                Error(e.Message);
            }
        }
    }

    #endregion


    #region "System.Web"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            RedirectToAccessDenied("Online Marketing sample data generator is available only to the global administrator");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ShowWarning("This is Online Marketing sample data generator. This tool should be used for the internal purposes only. It wasn't fully tested and using it may corrupt your data. Internet connection is needed to generate contacts with real names and personas.");

        btnGenerate.Click += (_, __) =>
        {
            var generator = new SampleDataGenerator(SiteContext.CurrentSiteID)
            {
                Information = s => ShowInformation(s),
                Error = s => ShowError(s)
            };

            var stopwatch = Stopwatch.StartNew();

            generator.Generate(new SampleDataGenerator.GenerationOptions()
            {
                GenerateContactStatuses = chckCreateContactStatuses.Checked,
                ContactsCount = chckGenerateContacts.Checked ? txtContactsCount.Text.ToInteger(0) : 0,
                ContactsWithRealNames = chckContactRealNames.Checked,
                GeneratePersonas = txtGeneratePersonas.Checked,
                ScoresCount = chckGenerateScores.Checked ? txtScoresCount.Text.ToInteger(0) : 0,
                ActivitiesForEachExistingContactCount = chckGenerateActivities.Checked ? txtActivitiesCount.Text.ToInteger(0) : 0,
            });

            ShowInformation("Time elapsed: " + stopwatch.Elapsed);
        };
    }


    public override void ShowInformation(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        var messages = new MessagesPlaceHolder();
        pnlMessages.Controls.Add(messages);

        messages.ShowInformation(text, description, tooltipText, persistent);
    }


    public override void ShowWarning(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        var messages = new MessagesPlaceHolder();
        pnlMessages.Controls.Add(messages);

        messages.ShowWarning(text, description, tooltipText);
    }

    #endregion
}