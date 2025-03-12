using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;

namespace LueJSON
{
    class LueJSON
    {
        struct Stat
        {
            public string Etnicity { get; }
            public int Count { get; set; }

            public Stat(string etnicity, int count)
            {
                Etnicity = etnicity;
                Count = count;
            }
        }

        static async Task<string> FetchFromURL(string url)
        {
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(url);
            return response;
        }

        static void PrintStats(in List<Stat> stats)
        {
            if (stats.Count > 0)
            {
                int longestEtn = stats.Select(stat => stat.Etnicity).OrderByDescending(etn => etn.Length).First().Count();
                int biggestCount = stats.Select(stat => stat.Count).Max().ToString().Count();

                string sFormat = $"|{{0,-{longestEtn}}}|{{1,-{biggestCount}}}|";

                string rowLine = $"|{new string('_', longestEtn)}|{new string('_', biggestCount)}|";

                Console.WriteLine($" {new string('_', longestEtn + biggestCount + 1)} ");

                foreach (Stat stat in stats)
                {
                    Console.WriteLine(string.Format(sFormat, stat.Etnicity, stat.Count));
                    Console.WriteLine(rowLine);
                }
            }
        }

        static void Main(string[] args)
        {
            string url = "https://data.cityofnewyork.us/api/views/25th-nujf/rows.json";
            string sJSON = "";

            try
            {
                sJSON = FetchFromURL(url).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Tieton haku osoitteesta:\n" + url + " epäonnistui");
                Console.WriteLine(e.Message);
            }

            try
            {
                using (JsonDocument doc = JsonDocument.Parse(sJSON))
                {
                    if (doc.RootElement.TryGetProperty("data", out JsonElement data))
                    {
                        string sData = data.GetRawText();

                        List<Stat> stats = new();


                        foreach (JsonElement name in data.EnumerateArray())
                        {
                            string etnicity = name[10].GetRawText().Trim('"');

                            if (!stats.Exists(stat => stat.Etnicity == etnicity))
                            {
                                stats.Add(new Stat(etnicity, 1));
                            }
                            else
                            {
                                int index = stats.FindIndex(stat => stat.Etnicity == etnicity);

                                if (stats[index].Etnicity != null)
                                {
                                    stats[index] = new Stat(etnicity, stats[index].Count + 1);
                                }
                            }
                        }
                        PrintStats(stats);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("JSON tiedoston jäsentäminen epäonnistui");
                Console.WriteLine(e.Message);
            }
        }
    }
}
