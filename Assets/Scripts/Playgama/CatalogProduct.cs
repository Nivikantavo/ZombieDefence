public class CatalogProduct
{
    public string id;
    public string priceValue;
    public string price;
    public string priceCurrencyCode;
    public string imageURI;

    public static CatalogProduct FromDictionary(System.Collections.Generic.Dictionary<string, string> item)
    {
        if (item == null)
            return null;

        CatalogProduct product = new CatalogProduct();
        product.id = GetValue(item, "id");
        product.priceValue = GetValue(item, "priceValue");
        product.price = GetValue(item, "price");
        product.priceCurrencyCode = GetValue(item, "priceCurrencyCode");
        product.imageURI = GetValue(item, "imageURI");

        if (string.IsNullOrEmpty(product.priceValue) == false)
            return product;

        if (string.IsNullOrEmpty(product.price) == false)
            product.priceValue = product.price;

        return product;
    }

    private static string GetValue(System.Collections.Generic.Dictionary<string, string> item, string key)
    {
        return item.TryGetValue(key, out string value) ? value : null;
    }
}
