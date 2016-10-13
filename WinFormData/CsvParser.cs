using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace WinFormData
{
    public class TestResult
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public static class CsvParser
    {
        private static Dictionary<string, double> dict;

        public static Dictionary<string, double> Dict
        {
            get { return dict ?? (dict = new Dictionary<string, double>()); }
        }

        public static double GetTotal()
        {
            return Dict.Sum(x => x.Value);
        }

        public static List<TestResult> GetDictionaryValues()
        {
            return Dict.Select(item => new TestResult { Name = item.Key, Value = item.Value }).OrderBy(x => x.Name).ToList();
        }

        public static void ClearDictionary()
        {
            dict = new Dictionary<string, double>();
        }

        public static int GetDictCount()
        {
            return Dict.Count;
        }

        public static void UpdateDictionary(string path)
        {
            using (var parser = new TextFieldParser(path))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();
                    if (fields != null && fields.Length == 2)
                    {
                        var name = fields[0];
                        var value = double.Parse(fields[1]);

                        if (Dict.ContainsKey(name))
                        {
                            double dictValue;
                            dict.TryGetValue(name, out dictValue);
                            if (value > dictValue || value < dictValue)
                            {
                                Dict[name] = value;
                            }
                        }
                        else
                        {
                            Dict.Add(name, value);
                        }
                    }
                }
            }
        }
    }
}
