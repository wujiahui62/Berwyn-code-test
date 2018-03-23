using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessCvs
{
    class Program
    {
        static void Main(string[] args)
        {
            var records = new List<Record>();
            long largeNumber = 0;
            long numberOflargeRow = 0;
            var largeNumberString = string.Empty;
            var allGuidDict = new Dictionary<string, int>();
            var duplicatDict = new Dictionary<string, int>();
            var duplicateGuid = string.Empty;
            long lengthofVal3 = 0;
            long recordCount = 0;

            using (var reader = new StreamReader(@"C:\test.csv"))
            {
                int i = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (i != 0 && !string.IsNullOrEmpty(line))
                    {
                        recordCount += 1;
                        var values = line.Split(',');

                        values = RemoveQouteSign(values);

                        var record = new Record
                        {
                            GUID = values[0],
                            Val1 = Convert.ToInt64(values[1]),
                            Val2 = Convert.ToInt64(values[2]),
                            Val3 = values[3]
                        };

                        if (record.Val1 + record.Val2 > largeNumber)
                        {
                            largeNumber = record.Val1 + record.Val2;
                            largeNumberString = record.GUID;
                            numberOflargeRow = recordCount;
                        }
                        lengthofVal3 += record.Val3.Length;
                        if (allGuidDict.ContainsKey(record.GUID))
                        {
                            if (!duplicatDict.ContainsKey(record.GUID))
                            {
                                duplicatDict.Add(record.GUID,0);
                            }
                            duplicateGuid = record.GUID;
                        }
                        else
                        {
                            allGuidDict.Add(record.GUID, 0);
                        }
                        records.Add(record);
                    }
                    i++;
                }
            }
            var averageLength = (lengthofVal3 * 1.0 / recordCount);
            var outputTotal = new List<string>();
            outputTotal.Add("The largest sum of Val1 and Val2 for any single row in the CSV, as well as the GUID for that row. Guid:" +largeNumberString + ",Row:" + numberOflargeRow + ",Sum:" + largeNumber);
            outputTotal.Add("Any Duplicate GUID values:" + duplicateGuid);
            outputTotal.Add("The average length of Val3 across all input rows:" + averageLength);
            File.WriteAllLines(@"C:\OutputTotal.csv", outputTotal.ToArray());
            var output = new List<string>();
            output.Add(@"GUID,Val1+Val2,IsDuplicateGuid (Y or N),Is Val3 length greater than the average length of Val3 (Y or N)");
            output.AddRange(records.Select(x => string.Format("{0},{1},{2},{3}", x.GUID, x.Val1 + x.Val2,
                duplicatDict.ContainsKey(x.GUID)?"Y":"N", x.Val3.Length > averageLength ? "Y" : "N")).ToList());
            File.WriteAllLines(@"C:\Output.csv", output.ToArray());

        }
        private static string[] RemoveQouteSign(string[] ar)
        {
            for (int i = 0; i < ar.Count(); i++)
            {
                ar[i] = ar[i].Trim();
                if (ar[i].StartsWith("\"") || ar[i].StartsWith("'")) ar[i] = ar[i].Substring(1);
                if (ar[i].EndsWith("\"") || ar[i].EndsWith("'")) ar[i] = ar[i].Substring(0, ar[i].Length - 1);

            }
            return ar;
        }
    }

    internal class Record
    {
        public string GUID { get; set; }
        public long Val1 { get; set; }
        public long Val2 { get; set; }
        public string Val3 { get; set; }

    }
}
