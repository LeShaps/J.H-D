using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

using J.H_D.Tools;
using J.H_D.Data;

using Anime = J.H_D.Data.Response.Anime;
using System.Globalization;

namespace J.H_D.Minions.Infos
{
    static class KitsuMinion
    {
        public static async Task<FeatureRequest<Anime?, Error.Anime>> SearchAnimeAsync(string[] Args)
        {
            string SearchName = Utilities.MakeQueryArgs(Args);
            if (SearchName.Length == 0)
                return new FeatureRequest<Anime?, Error.Anime>(null, Error.Anime.Help);

            dynamic json = JsonConvert.DeserializeObject(await JHConfig.KitsuClient.GetStringAsync($"https://kitsu.io/api/edge/anime?page[limit]=1&filter[text]=" + SearchName));


            if (json == null)
                return new FeatureRequest<Anime?, Error.Anime>(null, Error.Anime.NotFound);

            dynamic Results = json.data[0];
            dynamic Attributes = Results.attributes;

            Anime Response = new Anime
            {
                Id = Results.id,
                Synopsis = Attributes.synopsis,
                Title = Attributes.titles.en,
                LATitle = Attributes.titles.en_jp,
                OriginalTitle = Attributes.titles.ja_jp,
                Rating = Attributes.averageRating,
                StartDate = Attributes.startDate,
                EndDate = Attributes.endDate,
                AgeRating = Attributes.ageRating,
                Guideline = Attributes.ageRatingGuide,
                Status = Attributes.status,
                PosterImage = Attributes.posterImage.original,
                CoverImage = Attributes.coverImage.original,
                EpisodeCount = Attributes.episodeCount,
                EpLength = Attributes.episodeLength,
                HumanReadableWatchtime = GetReadableWatchtime((string)Attributes.totalLength),
                VideoUrl = new Uri($"https://youtube.com/watch?v={Attributes.youtubeVideoId}")
            };

            return new FeatureRequest<Anime?, Error.Anime>(Response, Error.Anime.None);
        }

        private static string GetReadableWatchtime(string minutes)
        {
            int Mins = int.Parse(minutes, CultureInfo.InvariantCulture);
            TimeSpan span = TimeSpan.FromMinutes(Mins);
            return span.ToString(@"hh\hmm", CultureInfo.InvariantCulture);
        }
    }
}
