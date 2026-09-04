using System;
using System.Globalization;

public class CatalogProduct
{
    public const string PlaygamaCurrencyCode = "Gam";
    private const decimal UsdPerGam = 0.1m;

    public string id;
    public string priceValue;
    public string price;
    public string priceCurrencyCode;
    public string imageURI;
    public string priceCurrencyImage;

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
        product.priceCurrencyImage = GetValue(item, "priceCurrencyImage");

        if (string.IsNullOrEmpty(product.priceValue) && string.IsNullOrEmpty(product.price) == false)
            product.priceValue = product.price;

        return product;
    }

    public bool LooksLikeUsd()
    {
        if (ContainsToken(priceCurrencyCode, "USD")
            || ContainsToken(priceCurrencyCode, "$")
            || ContainsToken(price, "USD")
            || ContainsToken(price, "$")
            || ContainsToken(priceValue, "USD")
            || ContainsToken(priceValue, "$"))
        {
            return true;
        }

        return TryParseDecimal(priceValue, out decimal value)
            && value > 0m
            && value != decimal.Truncate(value);
    }

    public static string ToGamPriceLabel(CatalogProduct product, IapProductDefinition definition)
    {
        if (TryGetConfiguredGamAmount(definition, out int configuredGam))
            return FormatGamLabel(configuredGam);

        if (product == null || product.TryGetGamAmount(out int gam) == false)
        {
            if (product == null)
                return string.Empty;

            return string.IsNullOrEmpty(product.priceValue) ? product.price : product.priceValue;
        }

        return FormatGamLabel(gam);
    }

    public static string FormatGamLabel(int gam)
    {
        return gam.ToString(CultureInfo.InvariantCulture) + " " + PlaygamaCurrencyCode;
    }

    private bool TryGetGamAmount(out int gam)
    {
        gam = 0;
        if (TryParseDecimal(priceValue, out decimal value) == false
            && TryParseDecimal(price, out value) == false)
        {
            return false;
        }

        if (value <= 0m)
            return false;

        bool labeledGam = ContainsToken(priceCurrencyCode, PlaygamaCurrencyCode)
            || ContainsToken(price, PlaygamaCurrencyCode)
            || ContainsToken(priceValue, PlaygamaCurrencyCode);

        if (labeledGam || LooksLikeUsd() == false)
        {
            gam = Math.Max(1, (int)decimal.Round(value, 0, MidpointRounding.AwayFromZero));
            return true;
        }

        gam = Math.Max(1, (int)decimal.Round(value / UsdPerGam, 0, MidpointRounding.AwayFromZero));
        return true;
    }

    private static bool TryGetConfiguredGamAmount(IapProductDefinition definition, out int gam)
    {
        gam = 0;
        if (definition == null || string.IsNullOrEmpty(definition.EditorPriceLabel))
            return false;

        if (TryParseDecimal(definition.EditorPriceLabel, out decimal value) == false || value <= 0m)
            return false;

        gam = Math.Max(1, (int)decimal.Round(value, 0, MidpointRounding.AwayFromZero));
        return true;
    }

    private static bool ContainsToken(string text, string token)
    {
        return string.IsNullOrEmpty(text) == false
            && text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool TryParseDecimal(string text, out decimal value)
    {
        value = 0m;
        if (string.IsNullOrEmpty(text))
            return false;

        char[] buffer = new char[text.Length];
        int length = 0;
        for (int i = 0; i < text.Length; i++)
        {
            char character = text[i];
            if (character == '$' || char.IsLetter(character))
                continue;

            buffer[length] = character;
            length++;
        }

        string numeric = new string(buffer, 0, length).Trim();
        return decimal.TryParse(numeric, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    private static string GetValue(System.Collections.Generic.Dictionary<string, string> item, string key)
    {
        return item.TryGetValue(key, out string value) ? value : null;
    }
}
