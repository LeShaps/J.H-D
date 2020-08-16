using BooruSharp.Booru;
using BooruSharp.Search.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using J.H_D.Data;

namespace J.H_D.Minions.NSFW
{
    public static class BooruMinion
    {
        public enum BooruType
        {
            Danbooru,
            E621,
            E926,
            Gelbooru,
            Konachan,
            Realbooru,
            R34,
            Safebooru,
            Sakugabooru,
            SankakuComplex,
            Yandere
        }

        public static readonly Dictionary<BooruType, Type> WebsiteEndpoints = new Dictionary<BooruType, Type>
        {
            {BooruType.Danbooru, typeof(DanbooruDonmai) },
            {BooruType.E621, typeof(E621) },
            {BooruType.E926, typeof(E926) },
            {BooruType.Gelbooru, typeof(Gelbooru) },
            {BooruType.Konachan, typeof(Konachan) },
            {BooruType.Realbooru, typeof(Realbooru) },
            {BooruType.R34, typeof(Rule34) },
            {BooruType.SankakuComplex, typeof(SankakuComplex) },
            {BooruType.Yandere, typeof(Yandere) }
        };


        public class BooruOptions
        {
            public BooruType Booru;
            public string[] SearchQuery;
            public bool AllowNSFW { private set; get; }

            public BooruOptions(BooruType Website, string[] Search, bool Allow)
            {
                Booru = Website;
                SearchQuery = Search;
                AllowNSFW = Allow;
            }
        }

        public static async Task<FeatureRequest<SearchResult, Error.Booru>> GetBooruImageAsync(BooruOptions options)
        {
            Type Booru = WebsiteEndpoints[options.Booru];
            var BooruSearch = (ABooru)Activator.CreateInstance(Booru);

            if (options.AllowNSFW == false)
                options.SearchQuery.Append("Safe");

            SearchResult Result = await BooruSearch.GetRandomPostAsync(options.SearchQuery);

            if (Result.fileUrl == null)
                return new FeatureRequest<SearchResult, Error.Booru>(Result, Error.Booru.NotFound);

            return new FeatureRequest<SearchResult, Error.Booru>(Result, Error.Booru.None);
        }

        public static async Task<FeatureRequest<BooruSharp.Search.Tag.SearchResult, Error.Booru>> GetTag(BooruType Booru, string Id)
        {
            Type BType = WebsiteEndpoints[Booru];
            var BooruWebsite = (ABooru)Activator.CreateInstance(BType);

            var TagResult = await BooruWebsite.GetTagAsync(Id);
            return new FeatureRequest<BooruSharp.Search.Tag.SearchResult, Error.Booru>(TagResult, Error.Booru.None);
        }

        public static async Task<FeatureRequest<List<BooruSharp.Search.Tag.SearchResult>, Error.Booru>> GetTags(BooruType Booru, string[] Tags, 
            BooruSharp.Search.Tag.TagType OnlyType = BooruSharp.Search.Tag.TagType.Metadata)
        {
            Type BType = WebsiteEndpoints[Booru];
            var BooruWebsite = (ABooru)Activator.CreateInstance(BType);

            List<BooruSharp.Search.Tag.SearchResult> FoundTags = new List<BooruSharp.Search.Tag.SearchResult>();
            foreach (string Tag in Tags)
            {
                FoundTags.Add(await BooruWebsite.GetTagAsync(Tag));
            }

            if (OnlyType != BooruSharp.Search.Tag.TagType.Metadata)
                return new FeatureRequest<List<BooruSharp.Search.Tag.SearchResult>, Error.Booru>(
                    FoundTags.Where(x => x.type == OnlyType).ToList(),
                    Error.Booru.None);

            else
                return new FeatureRequest<List<BooruSharp.Search.Tag.SearchResult>, Error.Booru>(FoundTags, Error.Booru.None);
        }
    }
}
