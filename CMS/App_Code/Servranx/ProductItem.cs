using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Description résumée de ProductItem
/// </summary>
public class ProductItem
{
	public ProductItem()
	{
		//
		// TODO: ajoutez ici la logique du constructeur
		//
	}
    public int ID { get; set; }
    public string Nom { get; set; }
    public int Qte { get; set; }
}