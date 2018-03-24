using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Controls;
using CMS.GlobalHelper;
using System.Data;
using CMS.SiteProvider;
using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.DataEngine;

using MaxMindGeoIP;
using CMS.Membership;
using CMS.Helpers;
using CMS.Localization;
using CMS.CustomTables;

public partial class LesEngages_CustomFilterActivities : CMSAbstractDataFilterControl
{
    private bool btnDateClicked = false;
    private bool btnDateDescClicked = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.drpTheme.SelectedValue = "0";
            this.drpLieu.SelectedValue = "0";
            SetFilter("Date");
            lbDate.Visible = false;
        }
    }

     private void GetTheme()
    {
        TreeProvider provider = new TreeProvider(MembershipContext.AuthenticatedUser);
        string currentCulture = CultureHelper.PreferredUICultureCode;
        string defaultCulture = ResourceStringInfoProvider.DefaultUICulture;


        var culture = CookieHelper.GetValue("CMSPreferredCulture");
        string documentTypes = "custom.categoriethematique";
        string orderby = "CategoriethematiqueLIBELLE";       
        DataSet data = provider.SelectNodes(SiteContext.CurrentSiteName, "/%", currentCulture, true, documentTypes,null,orderby);
        if (!DataHelper.DataSourceIsEmpty(data))
        {
            // Bind deparments to the drop-down list
            this.drpTheme.DataSource = data;
            this.drpTheme.DataTextField = "CategoriethematiqueLIBELLE";
            this.drpTheme.DataValueField = "CategoriethematiqueID";
            this.drpTheme.DataBind();
            RemoveDuplicateItems(drpTheme);
            // Add default '(tous)' value
            this.drpTheme.Items.Insert(0, new ListItem("FILTRER PAR THEMATIQUE","0"));             
        }
    }

    private void GetLieu()
    {
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(MembershipContext.AuthenticatedUser);

        string customTableClassName = "customtable.region";

        // Check if Custom table 'Sample table' exists

        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);

        if (customTable != null)
        {
            string where = "itemID<>'1'";
            string orderby= "Libelle";
            DataSet data = customTableProvider.GetItems(customTableClassName, where, orderby);
            if (!DataHelper.DataSourceIsEmpty(data))
            {
                // Bind lieu to the drop-down list
                this.drpLieu.DataSource = data;
                this.drpLieu.DataTextField = "Libelle";
                this.drpLieu.DataValueField = "ItemID";
                this.drpLieu.DataBind();
                RemoveDuplicateItems(drpLieu);

                // Add default '(tous)' value
                this.drpLieu.Items.Insert(0, new ListItem("FILTRER PAR REGION", "0"));

                // Add PRES DE CHEZ VOUS value
                this.drpLieu.Items.Insert(drpLieu.Items.Count , new ListItem("Près de chez vous", "1"));

            }
        }
    }
  
    /// <summary>
    /// Init event handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        // Create child controls
        SetupControl();


        base.OnInit(e);
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Check if the current request is a postback
        if (RequestHelper.IsPostBack())
        {
            // Apply the filter to the displayed data
            if (btnDateClicked)
            {
                SetFilter("Date");
            }

            if (btnDateDescClicked)
            {
                SetFilter("Date desc");
            }
            else
            {
                SetFilter();
            }
        }

        base.OnPreRender(e);
    }

    /// <summary>
    /// Setup the inner child controls
    /// </summary>
    private void SetupControl()
    {
        // Hide the filter if StopProcessing is enabled
        if (this.StopProcessing)
        {
            this.Visible = false;
        }


        // Initialize only if the current request is NOT a postback
        else if (!RequestHelper.IsPostBack())
        {
            GetTheme();
            GetLieu();
        }
    }

    /// <summary>
    /// Generates a WHERE condition and ORDER BY clause based on the current filtering selection
    /// </summary>
    private void SetFilter(string order=null)  
    {
        string where = null;

        // Generate a WHERE condition based on the selected theme
        if (this.drpTheme.SelectedValue != null)
        {
            int themeId = ValidationHelper.GetInteger(this.drpTheme.SelectedValue, 0);
            if (themeId > 0)
            {
                where = "Theme = " + themeId;
            }   
        }

        // Generate a WHERE condition based on the selected lieu
        if (this.drpLieu.SelectedValue != null)
        {
            int lieu = ValidationHelper.GetInteger(this.drpLieu.SelectedValue, 0);
            if (lieu == 1)
            {
                GeoLocalization.GeoLocalization geoLocalization = new GeoLocalization.GeoLocalization();
                //Location location = geoLocalization.GeoLocation("85.27.26.0");          // CHARLEROI
                //Location location = geoLocalization.GeoLocation("91.87.14.0");          // BRUSSELS 
                //Location location = geoLocalization.GeoLocation("94.111.168.0");            // GEMBLOUX -> Namur 
                //Location location = geoLocalization.GeoLocation("85.201.98.0");            // ARLON -> Luxembourg 
                //Location location = geoLocalization.GeoLocation("41.74.28.62");
                //Location location = geoLocalization.GeoLocation("212.68.206.6");
                string ipAdress = Request.UserHostAddress;
                //Response.Write("<script>alert('This is Alert " + ipAdress + "');</script>");
                Location location = geoLocalization.GeoLocation(ipAdress);
                if (location != null && !string.IsNullOrEmpty(location.city))
                {
                    // string reg = GetRegion(location.city.ToUpper());
                    string reg = geoLocalization.GetRegion(location.city.ToUpper());
                    if (reg == "0")
                    {
                        reg = "2";
                    }
                    if (!string.IsNullOrEmpty (reg))
                    {
                        where = "Region = " + reg;
                    }
                }
            }
            if (lieu > 1)
            {
                where = "Region = " + lieu;
            }
        }

        if (this.drpTheme.SelectedValue != null && this.drpLieu.SelectedValue != null)
        {
            int themeId = ValidationHelper.GetInteger(this.drpTheme.SelectedValue, 0);
            int lieu = ValidationHelper.GetInteger(this.drpLieu.SelectedValue, 0);
            if (themeId>0 && lieu > 0)
            {
                where = "Theme = " + themeId + "AND Region= " + lieu;
            }
        }

        if (this.drpTheme.SelectedValue == "0" && this.drpLieu.SelectedValue == "0")
        {
            where = "Theme > 0 AND Region > 0";
        }
        
        if (where != null)
        {
            // Set where condition
            this.WhereCondition = where;
        }

        if (order != null)
        {
            // Set orderBy clause
            this.OrderBy = order;
        }

        // Raise the filter changed event
        this.RaiseOnFilterChanged();
    }

    private string GetRegion(string City)
    {
        City = City.Trim();
        string result = string.Empty;
        if (string.IsNullOrEmpty(City)) return string.Empty; 
        // Creates a new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(MembershipContext.AuthenticatedUser);

        string customTableClassName = "customtable.GeoLoc";

        // Checks if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
        if (customTable != null)
        {
            // Prepares the parameters
            string where = string.Format("entId in (select entEntId from customtable_geoloc where entID in (select entEntId from customtable_geoloc where (entGeoLocName='{0}' OR entGeoLocName LIKE '{0} |%'  OR entGeoLocName LIKE '%| {0} |%'  OR entGeoLocName LIKE '%| {0}') AND cast(entId as int)>=1000))", City);
            te.Text = where;
            DataSet DataSet = customTableProvider.GetItems(customTableClassName, where,null );

            if (!DataHelper.DataSourceIsEmpty(DataSet))
            {
                // Gets the custom table item ID
                int entID = ValidationHelper.GetInteger(DataSet.Tables[0].Rows[0][0], 0);

                // Gets the custom table item
                CustomTableItem CustomTableItem = customTableProvider.GetItem(entID, customTableClassName);

                if (CustomTableItem != null)
                {
                    result = ValidationHelper.GetString(CustomTableItem.GetValue("entDDLValue"), "");
                }
            }
        }
        return result;
    }

    public static void RemoveDuplicateItems(DropDownList ddl)
    {

        for (int i = 0; i < ddl.Items.Count; i++)
        {
            ddl.SelectedIndex = i;
            string str = ddl.SelectedItem.ToString();
            for (int counter = i + 1; counter < ddl.Items.Count; counter++)
            {
                ddl.SelectedIndex = counter;
                string compareStr = ddl.SelectedItem.ToString();
                if (str == compareStr)
                {
                    ddl.Items.RemoveAt(counter);
                    counter = counter - 1;
                }
            }
        }
    }

    protected void lbDate_Click(object sender, EventArgs e)
    {
        btnDateClicked = true;
        btnDateDescClicked = false;
        lbDate.Visible = false;
        lbDateDesc.Visible = true;
        SetFilter("Date");
    }
    protected void lbDateDesc_Click(object sender, EventArgs e)
    {
        btnDateDescClicked = true;
        btnDateClicked = false;
        lbDateDesc.Visible = false;
        lbDate.Visible = true;
        SetFilter("Date desc");
    }
}