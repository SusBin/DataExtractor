using Microsoft.VisualBasic.FileIO;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;

namespace DataExtractor.Helpers
{
    public class CSVHelper
    {

        public List<string> ReadUPRNs(IFormFile file, string header)
        {
            var uprns = new List<string>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvReader.Read();
                csvReader.ReadHeader();
                if (!csvReader.HeaderRecord.Contains(header))
                {
                    throw new ArgumentException($"The specified header '{header}' does not exist in the file.");
                }

                while (csvReader.Read())
                {
                    var record = csvReader.GetRecord<dynamic>() as IDictionary<string, object>;
                    if (record.ContainsKey(header))
                    {
                        uprns.Add(record[header].ToString());
                    }
                }
            }

            return uprns;
        }



        public List<Dictionary<string, string>> ExtractRows(IFormFile file, List<string> uprns, string header)
        {
            var data = new List<Dictionary<string, string>>();
            var uprnsSet = new HashSet<string>(uprns);

            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvReader.Read();
                csvReader.ReadHeader();
                if (!csvReader.HeaderRecord.Contains(header))
                {
                    throw new ArgumentException($"The specified header '{header}' does not exist in the file.");
                }

                while (csvReader.Read())
                {
                    var record = csvReader.GetRecord<dynamic>() as IDictionary<string, object>;
                    if (uprnsSet.Contains(record[header]))
                    {
                        var extractedRow = new Dictionary<string, string>();
                        foreach (var key in record.Keys)
                        {
                            extractedRow[key] = record[key].ToString();
                        }
                        data.Add(extractedRow);
                    }
                }
            }

            return data;
        }



        public byte[] WriteToCSV(List<Dictionary<string, string>> data)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var records = new List<dynamic>();
                foreach (var row in data)
                {
                    dynamic obj = new ExpandoObject();
                    var dict = (IDictionary<string, object>)obj;
                    foreach (var pair in row)
                    {
                        dict[pair.Key] = pair.Value;
                    }
                    records.Add(obj);
                }
                csvWriter.WriteRecords(records);
                writer.Flush();
                return memoryStream.ToArray();
            }
        }




        //public byte[] WriteToCSV(List<Dictionary<string, string>> data)
        //{
        //    using (var memoryStream = new MemoryStream())
        //    using (var writer = new StreamWriter(memoryStream, Encoding.UTF8))
        //    using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        //    {
        //        csvWriter.WriteRecords(data);
        //        writer.Flush();
        //        return memoryStream.ToArray();
        //    }
        //}


    }
}
