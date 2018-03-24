using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

using CMS.Core;
using CMS.UIControls;
using CMS.IO;
using CMS.Base;
using CMS.Helpers;

[Title("Tests.Title")]
[UIElement(ModuleName.CMS, "Development.Tests")]
public partial class CMSModules_SystemDevelopment_Development_Tests_Default : GlobalAdminPage
{
    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        string path = Server.MapPath("~/App_Data/CodeTemplates/");

        // Prepare templates
        string classTemplate = File.ReadAllText(path + "TestClass.Template");
        string methodTemplate = File.ReadAllText(path + "TestMethod.Template");

        // Get the class instance
        string className = selClass.ClassName;
        string assemblyName = selClass.AssemblyName;

        if (selClass.IsValid())
        {
            Type cls = ClassHelper.GetClassType(assemblyName, className);
            if (cls != null)
            {
                string ns = "Custom";

                int dotIndex = className.LastIndexOfCSafe(".");
                if (dotIndex > 0)
                {
                    ns = className.Substring(0, dotIndex);
                    className = className.Substring(dotIndex + 1);
                }

                var methodsCode = new StringBuilder();

                if (radSingle.Checked)
                {
                    string methodName = txtMethodName.Text;
                    methodName = ValidationHelper.GetIdentifier(ValidationHelper.GetCodeName(methodName));
                    
                    // Single method
                    AddMethod(methodsCode, methodTemplate, methodName);
                }
                else
                {
                    // All methods
                    AddMethods(methodsCode, methodTemplate, cls);
                    AddProperties(methodsCode, methodTemplate, cls);
                }

                // Generate result
                string result = classTemplate
                    .Replace("##NAMESPACE##", ns)
                    .Replace("##CLASS##", className)
                    .Replace("##METHODS##", methodsCode.ToString());

                txtCode.Text = result;
            }

            // Generate class hierarchy
            if (!radSingle.Checked)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<ul>");

                int level = 0;
                int coveredLevel = (chkInherited.Checked ? ValidationHelper.GetInteger(txtInheritance.Text, Int32.MaxValue) : 0);
                
                if (coveredLevel < 0)
                {
                    coveredLevel = 0;
                }
                if (chkInherited.Checked)
                {
                    txtInheritance.Text = coveredLevel.ToString();
                }

                while (cls != null)
                {
                    sb.Append("<li", (coveredLevel >= level ? " style=\"color: green;\"" : ""), ">", cls.Name, (cls.IsAbstract ? " (abstract)" : ""), "</li>");
                    cls = cls.BaseType;

                    level++;
                }

                sb.Append("</ul>");

                ltlHierarchy.Text = sb.ToString();
            }
        }
        else
        {
            ShowError(GetString(selClass.ErrorMessage));
        }
    }


    /// <summary>
    /// Adds the methods from the given type
    /// </summary>
    /// <param name="methodsCode">Methods code</param>
    /// <param name="methodTemplate">Method template</param>
    /// <param name="cls">Type from which take the methods</param>
    protected void AddMethods(StringBuilder methodsCode, string methodTemplate, Type cls)
    {
        var flags = GetFlags(chkStatic.Checked, chkInstance.Checked);
        var generatedMethods = new SafeDictionary<string, bool>();
        List<Type> allowedClasses = GetAllowedClasses(cls);

        // Generate methods
        var methods = cls.GetMethods(flags).Where(m => !m.IsSpecialName);

        // Add the methods
        foreach (var method in methods)
        {
            if (allowedClasses.Contains(method.DeclaringType))
            {
                var methodName = method.Name;

                if (!generatedMethods[methodName])
                {
                    // Generate method code
                    AddMethod(methodsCode, methodTemplate, methodName);

                    generatedMethods[methodName] = true;
                }
            }
        }
    }


    /// <summary>
    /// Adds the methods from the given type
    /// </summary>
    /// <param name="methodsCode">Methods code</param>
    /// <param name="methodTemplate">Method template</param>
    /// <param name="cls">Type from which take the methods</param>
    protected void AddProperties(StringBuilder methodsCode, string methodTemplate, Type cls)
    {
        var flags = GetFlags(chkProp.Checked, chkStaticProp.Checked);
        var generatedMethods = new SafeDictionary<string, bool>();
        List<Type> allowedClasses = GetAllowedClasses(cls);

        // Generate properties
        var properties = cls.GetProperties(flags).Where(p => !p.IsSpecialName);

        foreach (var prop in properties)
        {
            if (allowedClasses.Contains(prop.DeclaringType))
            {
                var methodName = prop.Name;

                // Handle the indexer name
                if (methodName == "Item")
                {
                    methodName = prop.GetIndexParameters()[0].ParameterType.Name + "Indexer";
                }
                else
                {
                    methodName = "Property_" + methodName;
                }

                if (!generatedMethods[methodName])
                {
                    // Generate method code
                    AddMethod(methodsCode, methodTemplate, methodName);

                    generatedMethods[methodName] = true;
                }
            }
        }
    }


    /// <summary>
    /// Gets the flags for getting the items from class
    /// </summary>
    /// <param name="instance">Get instance members</param>
    /// <param name="stat">Get static members</param>
    private BindingFlags GetFlags(bool instance, bool stat)
    {
        var flags = BindingFlags.Public;

        if (instance)
        {
            flags |= BindingFlags.Instance;
        }
        if (stat)
        {
            flags |= BindingFlags.Static;
        }

        return flags;
    }


    /// <summary>
    /// Gets the list of types for which the items should be extracted
    /// </summary>
    /// <param name="cls">Class type</param>
    private List<Type> GetAllowedClasses(Type cls)
    {
        // Get allowed defining classes
        List<Type> allowedClasses = new List<Type>();
        allowedClasses.Add(cls);

        if (chkInherited.Checked)
        {
            int i = ValidationHelper.GetInteger(txtInheritance.Text, Int32.MaxValue);

            var inheritedType = cls.BaseType;

            while ((i > 0) && (inheritedType != null) && (inheritedType != typeof(object)))
            {
                allowedClasses.Add(inheritedType);

                inheritedType = inheritedType.BaseType;
                i--;
            }
        }

        return allowedClasses;
    }


    /// <summary>
    /// Adds the methods from the given type
    /// </summary>
    /// <param name="methodsCode">Methods code</param>
    /// <param name="methodTemplate">Method template</param>
    /// <param name="methodName">Method name</param>
    protected void AddMethod(StringBuilder methodsCode, string methodTemplate, string methodName)
    {
        string methodCode = methodTemplate
             .Replace("##METHOD##", methodName);

        if (methodsCode.Length > 0)
        {
            methodsCode.Append("\r\n");
        }

        methodsCode.Append(methodCode);
    }
}