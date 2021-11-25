using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using GasMon.Models;

namespace GasMon
{
    public class CsvManager
    {
        public static void CreateCsv(List<Notification> notifications)
        {
            using (var writer = new StreamWriter(@"C:\Training\GasMon\GasMon\file.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(notifications);
            }
        }

        public static void AppendCsv(List<Notification> notifications)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Don't write the header again.
                HasHeaderRecord = false,
            };
            using (var stream = File.Open(@"C:\Training\GasMon\GasMon\file.csv", FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(notifications);
            }
        }
    }
}