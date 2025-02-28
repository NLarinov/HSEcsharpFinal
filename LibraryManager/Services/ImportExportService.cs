using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using TkachevProject4.Models;

namespace TkachevProject4.Services;

public class ImportExportService
    {
        public List<Book> ImportFromFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            
            return extension switch {
                ".json" => ImportFromJson(filePath),
                ".csv" => ImportFromCsv(filePath),
                _ => throw new NotSupportedException("Unsupported file format")
            };
        }

        private List<Book> ImportFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Book>>(json) ?? new List<Book>();
        }

        private List<Book> ImportFromCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<Book>().ToList();
        }

        public void ExportToFile(List<Book> books, string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            
            switch(extension) {
                case ".json":
                    ExportToJson(books, filePath);
                    break;
                case ".csv":
                    ExportToCsv(books, filePath);
                    break;
                default:
                    throw new NotSupportedException("Unsupported file format");
            }
        }

        private void ExportToJson(List<Book> books, string filePath)
        {
            var json = JsonConvert.SerializeObject(books, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        private void ExportToCsv(List<Book> books, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(books);
        }
    }