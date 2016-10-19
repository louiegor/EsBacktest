using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace WinFormData
{
    public class TestResult
    {
        public string Name { get; set; }
        public double Profit { get; set; }
        public double CurrentPrice { get; set; }
        public double ProfitPercent { get; set; }
        public double WinRatio { get; set; }

    }

    public class Data
    {
        public double Profit { get; set; }
        public double WinRatio { get; set; }
        public double CurrentPrice { get; set; }
    }

    public static class CsvParser
    {
        private static Dictionary<string, Data> dict;

        public static Dictionary<string, Data> Dict
        {
            get { return dict ?? (dict = new Dictionary<string, Data>()); }
        }

        public static double GetTotal()
        {
            return Dict.Aggregate<KeyValuePair<string, Data>, double>(0, (current, item) => current + item.Value.Profit);
        }

        public static int GetFilterTotalStocks(double winRatio, double profitPercent)
        {
            return GetFilterDictionary(winRatio, profitPercent).Count();
        }

        public static List<TestResult> GetDictionaryValues()
        {
            return
                Dict.Select(
                    item =>
                    new TestResult
                        {
                            Name = item.Key,
                            Profit = Math.Round(item.Value.Profit,2),
                            CurrentPrice = Math.Round(item.Value.CurrentPrice,2),
                            ProfitPercent = Math.Round(100*item.Value.Profit/item.Value.CurrentPrice,2),
                            WinRatio = Math.Round(item.Value.WinRatio,2)
                        }).OrderBy(x => x.Name).ToList();
        }

        public static List<TestResult> GetFilterDictionary(double winRatio, double profitPercent)
        {
            var result = GetDictionaryValues();
            result = result.Where(x => x.WinRatio >= winRatio && 100*x.Profit/x.CurrentPrice >= profitPercent).ToList();

            return result;
        }

        public static double GetFilterProfit(double winRatio, double profitPercent)
        {
            var result =
                GetDictionaryValues().Where(x => x.WinRatio >= winRatio && 100*x.Profit / x.CurrentPrice >= profitPercent).Sum(x=>x.Profit);

            return result;
        }

        public static List<TestResult> GetUnFilterDictionary(double winRatio, double profitPercent)
        {
            var result =
                GetDictionaryValues().Where(x => x.WinRatio < winRatio || 100*x.Profit / x.CurrentPrice < profitPercent).ToList();

            return result;
        }

        public static void ClearDictionary()
        {
            dict = new Dictionary<string, Data>();
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
                    //Process row - Symbol,Profit,WinRatio%,CurrentPrice
                    string[] fields = parser.ReadFields();
                    if (fields != null && fields.Length == 4)
                    {
                        var name = fields[0];
                        var newprofit = double.Parse(fields[1]);
                        var winratio = double.Parse(fields[2]);
                        var currentPrice = double.Parse(fields[3]);

                        if (Dict.ContainsKey(name))
                        {
                            Data dictValue;
                            Dict.TryGetValue(name, out dictValue);
                            double dictprofit = 0;
                            if (dictValue != null) dictprofit = dictValue.Profit;

                            if (newprofit > dictprofit || newprofit < dictprofit)
                            {
                                Dict[name].Profit = newprofit;
                                Dict[name].WinRatio = winratio;
                                Dict[name].CurrentPrice = currentPrice;
                            }
                        }
                        else
                        {
                            var data = new Data
                                {
                                    Profit = newprofit,
                                    CurrentPrice = currentPrice,
                                    WinRatio = winratio
                                };
                            Dict.Add(name, data);
                        }
                    }

                }
            }
        }
    }
}
