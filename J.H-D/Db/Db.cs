using Discord;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Db
{
    public partial class Db
    {
        private RethinkDB R;
        public RethinkDB GetR() => R;

        private Connection conn;
        private string DbName;

        private static readonly string defaultAvailability = "1111111111111111";

        public Dictionary<ulong, string> Languages { private set; get; }
        public Dictionary<ulong, string> Prefixs { private set; get; }
        public Dictionary<ulong, bool> Anonymize { private set; get; }
        public Dictionary<ulong, string> Availlability { private set; get; } // To change tbh

        public Db()
        {
            R = RethinkDB.R;
            Languages = new Dictionary<ulong, string>();
            Prefixs = new Dictionary<ulong, string>();
            Availlability = new Dictionary<ulong, string>();
            Anonymize = new Dictionary<ulong, bool>();
        }

        public async Task InitAsync()
            => await InitAsync("jhdatabase");

        public async Task InitAsync(string dbName)
        {
            DbName = dbName;
            conn = await R.Connection().ConnectAsync();
            if (!await R.DbList().Contains(DbName).RunAsync<bool>(conn))
                R.DbCreate(DbName).Run(conn);
            if (!await R.Db(dbName).TableList().Contains("Guilds").RunAsync<bool>(conn))
                R.Db(dbName).TableCreate("Guilds").Run(conn);
        }

        public async Task InitGuild(IGuild guild)
        {
            string guildIdStr = guild.Id.ToString();
            if (await R.Db(DbName).Table("Guilds").GetAll(guildIdStr).Count().Eq(0).RunAsync<bool>(conn))
            {
                await R.Db(DbName).Table("Guilds").Insert(R.HashMap("id", guildIdStr)
                    .With("Prefix", ".jh")
                    .With("Language", "en")
                    .With("Availability", defaultAvailability)
                    ).RunAsync(conn);
            }
            dynamic json = await R.Db(DbName).Table("Guilds").Get(guildIdStr).RunAsync(conn);
            Languages.Add(guild.Id, (string)json.Languages);
            Prefixs.Add(guild.Id, (string)json.Prefix);
            string availability = (string)json.Availability;
            if (availability == null)
                Availlability.Add(guild.Id, defaultAvailability);
        }
    }
}
