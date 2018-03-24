using System;
using System.Text.RegularExpressions;
using eu.europa.ec;

/// <summary>
/// Description résumée de EUVatChecker
/// </summary>
public class EUVatChecker
{
    public static string retName = string.Empty, retAddress = string.Empty;


    public EUVatChecker()
    {
        //
        // TODO: ajoutez ici la logique du constructeur
        //
    }

    public static bool Check(string vatstring)
    {
        bool result = false;
        string vat, country;
        DateTime retDate;
        vatstring = vatstring.ToUpper();
        Regex rgx = new Regex("[^A-Z0-9]");
        vatstring = rgx.Replace(vatstring, string.Empty); 
        if (IsValid(vatstring))
        {
            country = vatstring.Substring(0, 2);
            vat = vatstring.Substring(2);
            checkVatService checker = new checkVatService();
            try
            {
                retDate = checker.checkVat(ref country, ref vat, out result, out retName, out retAddress);
            }
            catch
            {
                result = false;
            }
        }
        return result;
    }

    private static bool IsValid(string vatstring)
    {
        string country = string.Empty;
        if (string.IsNullOrEmpty(vatstring))
        {
            return false;
        }

        if (vatstring.Length < 3)
        {
            return false;
        }

        country = vatstring.Substring(0, 2);
        string pattern = "[A-Z]";
        Match match = Regex.Match(country, pattern);
        return match.Success;
    }
}