using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;

// 1. Haetaan JSON tiedosto osoitteesta merkkijonoon
// 2. Jäsennetään merkkijono JSON dokumentiksi
// 3. Etsitään "data" avain
// 4. luodaan tietorakenne johon tiedot tallennetaan
// 5. Käydään data avaimen sisältämiä []
// 
// osoite = sivun url osoite
// sJSON = NoudaOsoitteesta(osoite)
//
// dokumentti = JäsennäJSON(sJSON)
//
// Jos data JSON objekti löytyy dokumentista ja se on []
//      
//      lista tilastot<Tilasto>
//      
//      jokaista data objektin lapsiobjektia [] kohtaan
//          jos tilastot ei sisällä lapsiobjekti[10] 
//              lisää lapsiobjekt[10] tilastoihin, määrällä 1
//          jos taas se löytyy jo tilastoista
//              lisää löytyneen tilaston määrä yhdellä
//
// tietorakenne Tilasto
//      Etnisyys
//      määrä
//  

namespace LueJSON
{
    class LueJSON {

        /*struct Stat(string etnicity, int count) {*/
        /*    string etnicity;*/
        /*    int count;*/
        /*}*/

        struct Stat {
            string Etnicity;
            int Count;

            public Stat(string etnicity, int count) {
                Etnicity = etnicity;
                Count = count;
            }
        }

        static async Task<string> FetchFromURL(string url) {
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(url);
            return response;
        }

        static void Main(string[] args) {
            string url = "https://data.cityofnewyork.us/api/views/25th-nujf/rows.json";
            string sJSON;

            try {
                sJSON = FetchFromURL(url).Result;
            } catch (Exception e) {
                Console.WriteLine("Tieton haku osoitteesta:\n"+url+" epäonnistui");
                Console.WriteLine(e.Message);
            }

            try {
                using(JsonDocument doc = JsonDocument.Parse(sJSON))
                {
                    if (doc.RootElement.TryGetProperty("data", out JsonElement data)) {

                        List<Stat> stats = new();

                        foreach (JsonElement nameStats in data) {
                            string etnicity = nameStats[10];
                            
                            if (!stats.Exists(stat => stat.Etnicity == etnicity)) {
                                stats.Add(new Stat(etnicity, 1));
                            }
                            else {
                                stats.Find(stat => stat.Etnicity == etnicity).Count++;
                            }
                        }
                    }
                }
            } catch(Exception e) {
                Console.WriteLine("JSON tiedoston jäsentäminen epäonnistui");
                Console.WriteLine(e.Message);
            }
        }
    }
}
