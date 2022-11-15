using CsvHelper.Configuration.Attributes;

namespace SimpleTyrePricing;

[Delimiter(";")]
public class PriceMatchResult
{
    public bool NeedsAdjusting { get; private init; }
    public string? YourProductCode { get; private init; }
    public decimal YourPrice { get; private init; }
    public decimal PriceTransportDelta { get; private init; }
    public decimal PriceAdjusted { get; private init; }
    public CountryIsoCode CountryIsoCode { get; private init; }

    public static IEnumerable<PriceMatchResult> FromFormatIn(IEnumerable<PriceMatchModel> records,
        IEnumerable<BuyModel> buyModels)
    {
        const decimal adjustedPriceStepSize = 0.25m;

        return (from record in records
            let priceAdjusted = record.RankNumber != 1
                ? record.YourPrice - (record.YourPriceInclTransport - record.CheapestPriceWithTransport) -
                  adjustedPriceStepSize
                : 0
            let buyModelRecord = buyModels.Where(x => x.ProductCode == record.YourProductCode).ToList()
            from buyRecord in buyModelRecord
            select new PriceMatchResult
            {
                NeedsAdjusting = record.RankNumber != 1 || priceAdjusted >= buyRecord.Price,
                YourProductCode = record.YourProductCode,
                YourPrice = record.YourPrice,
                PriceTransportDelta = record.RankNumber != 1
                    ? record.YourPriceInclTransport - record.CheapestPriceWithTransport
                    : 0,
                PriceAdjusted = priceAdjusted,
                CountryIsoCode = record.CountryIsoCode
            }).ToList();
    }
}

[Delimiter(",")]
public class PriceMatchModel
{
    public string? Brand { get; set; }
    public string? Product { get; set; }
    public string? ProfileCode { get; set; }
    public string? Ean { get; set; }
    public string? YourProductCode { get; set; }
    public string? Season { get; set; }
    public decimal CheapestPrice { get; set; }
    public decimal CheapestPriceWithTransport { get; set; }
    public decimal StockCheapestPrice { get; set; }
    public decimal YourPrice { get; set; }
    public decimal YourPriceInclTransport { get; set; }
    public int YourStock { get; set; }
    public int RankNumber { get; set; }
    public int CompetitorCount { get; set; }
    [Name("SupplierID")] public int SupplierId { get; set; }
    public CurrencyCode CurrencyCode { get; set; }
    public CountryIsoCode CountryIsoCode { get; set; }
}

[Delimiter(";")]
public class BuyModel
{
    public string? ProductCode { get; set; }
    public decimal Price { get; set; }
}

public enum CurrencyCode
{
    EUR,
    GBP,
    USD
}

public enum CountryIsoCode
{
    LV,
    UK,
    DE,
    FR,
    ES,
    IT,
    BE,
    NL,
    PL,
    PT,
    SE,
    FI,
    DK,
    NO,
    AT,
    CH,
    CZ,
    HU,
    RO,
    SK,
    SI,
    BG,
    EE,
    LT,
    LU,
    MT,
    GR,
    HR,
    IE,
    CY,
    TR,
    AL,
    BA,
    BY
}
