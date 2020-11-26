using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using J.H_D.Data;
using static J.H_D.Data.Error;
using static J.H_D.Data.Response;
using J.H_D.Data.Extensions;

namespace J.H_D.Minions.Websites
{
    class SCPMinion
    {
        readonly static Regex ArticleListRegex = new Regex("<li><a href=\"(/scp-[0-9]+)\">(.+)</a>(.+)</li>");
        readonly static Regex ImageRegex = new Regex("class=\"scp-image-block block-right\".+src=\"([^\"]+).+[^<]<div class=\"scp-image-caption\"[^<]+<p>([^<]+)");
        readonly static Regex InfosRegex = new Regex("(?=<strong>(.+)</strong>(.+)</p>)");

        private readonly static Dictionary<int, string> NumberPageAssociation = new Dictionary<int, string>
        {
            {999, "http://www.scpwiki.com/scp-series" },
            {1999, "http://www.scpwiki.com/scp-series-2"},
            {2999, "http://www.scpwiki.com/scp-series-3"},
            {3999, "http://www.scpwiki.com/scp-series-4" },
            {4999, "http://www.scpwiki.com/scp-series-5" },
            {5999, "http://www.scpwiki.com/scp-seires-6" }
        };

        public static async Task<FeatureRequest<SCPReport?, SCPError>> GetBasicReport(int SCPNumber)
        {
            string Article = await JHConfig.Asker.GetStringAsync($"http://www.scpwiki.com/scp-{SCPNumber.TrailingCharacters('0', 3)}");
            SCPReport Report = new SCPReport();

            foreach (Match ImgMatch in ImageRegex.Matches(Article))
            {
                if (!ImgMatch.Success) {
                    Report.HasImage = false;
                }

                Report.ImageLink = ImgMatch.Groups[1].Value;
                Report.ImageCaption = ImgMatch.Groups[2].Value;
                Report.HasImage = true;
            }

            foreach (Match InfoMatch in InfosRegex.Matches(Article))
            {
                if (InfoMatch.Groups[1].Value.Contains("Class")) {
                    Report.Class = InfoMatch.Groups[2].Value;
                } else if (InfoMatch.Groups[1].Value.Contains("Containment Procedures")) {
                    Report.Confinement = InfoMatch.Groups[2].Value + " [...]";
                } else if (InfoMatch.Groups[1].Value.Contains("Description")) {
                    Report.Description = InfoMatch.Groups[2].Value + " [...]";
                }
            }

            Report.Name = await GetObjectName(SCPNumber);
            Report.Number = SCPNumber;

            return new FeatureRequest<SCPReport?, SCPError>(Report, SCPError.None);
        }

        private static async Task<string> GetObjectName(int Number)
        {
            List<string> CurrentList = new List<string>();
            Dictionary<int, string> PageList = new Dictionary<int, string>();
            string LookForList = NumberPageAssociation.Where(x => x.Key > Number).FirstOrDefault().Value;
            string RawResult = await JHConfig.Asker.GetStringAsync(LookForList);

            foreach (Match m in ArticleListRegex.Matches(RawResult))
            {
                PageList.Add(int.Parse(m.Groups[2].Value.Replace("SCP-", "")), $"{ m.Groups[3].Value.Replace("- ", "")}");
            }
            return PageList[Number];
        }
    }
}
