using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Minions.Responses
{
    class UrbanDictionaryMinion
    {
        private static string RapidAPIHost = "mashape-community-urban-dictionary.p.rapidapi.com";
        private static string DefineAdress = "https://mashape-community-urban-dictionary.p.rapidapi.com/define?term=";

        public static async Task<FeatureRequest<Response.UrbanDefinition, Error.Urban>> SearchForWord(string search)
        {
            dynamic result;
            Response.UrbanDefinition Defintion;

            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("X-RapidAPI-Host", RapidAPIHost);
                hc.DefaultRequestHeaders.Add("X-RapidAPI-Key", Program.p.RapidAPIKey);

                result = JsonConvert.DeserializeObject(await (await hc.GetAsync($"{DefineAdress}{search}")).Content.ReadAsStringAsync());
            }

            if (result["list"] == null)
                return new FeatureRequest<Response.UrbanDefinition, Error.Urban>(null, Error.Urban.WordNotFound);

            JArray Results = result["list"];
            dynamic Usable = Results[0];
            Defintion = new Response.UrbanDefinition()
            {
                Author = Usable.author,
                Definition = Usable.definition,
                Link = Usable.permalink,
                Word = Usable.word,
                Exemples = Usable.example
            };

            return new FeatureRequest<Response.UrbanDefinition, Error.Urban>(Defintion, Error.Urban.None);
        }
    }
}
