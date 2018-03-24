using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS.CMSHelper;
using CMS.MacroEngine;

/// <summary>
/// Description résumée de LocalizedCountry
/// </summary>
public class LocalizedCountry
{
	public LocalizedCountry()
	{
		//
		// TODO: ajoutez ici la logique du constructeur
		//
	}

    public static DataSet LocalizeCountry(DataSet dsInput)
    {
        if (dsInput != null)
        {
            DataTable tblCountry = dsInput.Tables[0];
            foreach (DataRow drow in tblCountry.Rows)
            {
                drow["CountryDisplayName"] = MacroContext.CurrentResolver.ResolveMacros(drow["CountryDisplayName"].ToString());
            }
            tblCountry.DefaultView.Sort = "CountryDisplayName asc";
            DataTable tblCountrySorted = tblCountry.DefaultView.ToTable();
            dsInput.Tables.Clear();
            dsInput.Tables.Add(tblCountrySorted);
            
        }
        return dsInput;
    }
}