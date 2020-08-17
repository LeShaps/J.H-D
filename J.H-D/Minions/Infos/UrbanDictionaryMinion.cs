using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using J.H_D.Data;

using UrbanDefinition = J.H_D.Data.Response.UrbanDefinition;

namespace J.H_D.Minions.Infos
{
    static class UrbanDictionaryMinion
    {
        private static string RapidAPIHost = "mashape-community-urban-dictionary.p.rapidapi.com";
        private static string DefineAdress = "https://mashape-community-urban-dictionary.p.rapidapi.com/define?term=";

        public static async Task<FeatureRequest<UrbanDefinition?, Error.Urban>> SearchForWordAsync(string search)
        {
            dynamic result;
            UrbanDefinition? Defintion;

            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("X-RapidAPI-Host", RapidAPIHost);
                hc.DefaultRequestHeaders.Add("X-RapidAPI-Key", Program.p.RapidAPIKey);

                result = JsonConvert.DeserializeObject(await (await hc.GetAsync($"{DefineAdress}{search}")).Content.ReadAsStringAsync());
            }

            if (result["list"].Count == 0)
                return new FeatureRequest<UrbanDefinition?, Error.Urban>(null, Error.Urban.WordNotFound);

            JArray Results = result["list"];
            dynamic Usable = Results[0];
            Defintion = new UrbanDefinition
            {
                Author = Usable.author,
                Definition = Usable.definition,
                Link = Usable.permalink,
                Word = Usable.word,
                Exemples = Usable.example
            };

            return new FeatureRequest<UrbanDefinition?, Error.Urban>(Defintion, Error.Urban.None);
        }
    }
}
