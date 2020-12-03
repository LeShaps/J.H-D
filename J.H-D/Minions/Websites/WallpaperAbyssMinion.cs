using J.H_D.Data;
using System;
using System.Collections.Generic;
using System.Text;
using static J.H_D.Data.Response;

namespace J.H_D.Minions.Websites
{
    class WallpaperAbyssMinion
    {
        /*
        private readonly string ApiKey = JHConfig.APIKey["AlphaCoders"];
        private readonly string BaseAddress = $"https://wall.aphacoders.com/api2.0/get.php?info_level=3";

        public static FeatureRequest<WallPaper, Error.AbyssError> GetWallpaper()
        {

        }
        */
    }

    public class WallpaperQueryOptions
    {
        public string Search { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public WallpaperQueryOptions(string CommandQuery)
        {
            Search = CommandQuery.Split(' ')[0];
        }
    }
}
