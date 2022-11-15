using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleTyrePricing;

public static class SimpleTyrePricing
{
    public static void Main()
    {
        var folderPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName);
        if (folderPath is null) throw new Exception("Something went very wrong. PID doesn't have a main module?");

        if (!Directory.Exists(Path.Join(folderPath, "Input")))
        {
            Directory.CreateDirectory(Path.Join(folderPath, "Input"));
            Directory.CreateDirectory(Path.Join(folderPath, "Output"));
            Console.WriteLine("Input and Output folders created. Add 220169*.csv and buy*.csv to Input.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(1);
        }
        
        var workingPath = Path.Join(folderPath, "Input");
        var directory = new DirectoryInfo(workingPath);
        var priceMatchModel = new List<PriceMatchModel>();
        var buyModel = new List<BuyModel>();

        foreach (var file in directory.GetFiles("220169*.csv"))
        {
            using var reader = new StreamReader(Path.Join(workingPath, file.Name));
            using var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

            var records = csvReader.GetRecords<PriceMatchModel>();
            if (records is null) throw new Exception("No records found");

            priceMatchModel.AddRange(records);
        }

        foreach (var file in directory.GetFiles("buy*.csv"))
        {
            using var reader = new StreamReader(Path.Join(workingPath, file.Name));
            using var csvReader = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

            var records = csvReader.GetRecords<BuyModel>();
            if (records is null) throw new Exception("No records found");

            buyModel.AddRange(records);
        }

        var priceMatchResult = PriceMatchResult.FromFormatIn(priceMatchModel, buyModel);
        using var writer = new StreamWriter(Path.Join(folderPath, "Output", "output.csv"));
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csvWriter.WriteRecords(priceMatchResult);
        
        Console.WriteLine("Magic complete. Press any key to exit.");
        Console.ReadKey();
    }
}
