using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Minions.Websites
{
    class InspirobotMinion
    {
        public static async Task<FeatureRequest<string, Error.InspirationnalError>> FeelInspiration()
        {
            string Ressource = null;

            using (HttpClient hc = new HttpClient())
            {
                Ressource = await hc.GetAsync("https://inspirobot.me/api?generate=true").Result.Content.ReadAsStringAsync();
            }

            if (Ressource == null)
                return new FeatureRequest<string, Error.InspirationnalError>(null, Error.InspirationnalError.Communication);
            return new FeatureRequest<string, Error.InspirationnalError>(Ressource, Error.InspirationnalError.None);
        }
    }
}
