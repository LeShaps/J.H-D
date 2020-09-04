using Discord;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace J.H_D.Db
{
    public class Db
    {
        public RethinkDB R1 { get; }

        private Connection conn;
        private string DbName;

        private const string defaultAvailability = "1111111111111111";
        private const string GuildTableName = "Guilds";

        public Dictionary<ulong, string> Languages { private set; get; }
        public Dictionary<ulong, string> Prefixs { private set; get; }
        public Dictionary<ulong, bool> Anonymize { private set; get; }
        public Dictionary<ulong, string> Availlability { private set; get; } // To change tbh

        public Db()
        {
            R1 = RethinkDB.R;
            Languages = new Dictionary<ulong, string>();
            Prefixs = new Dictionary<ulong, string>();
            Availlability = new Dictionary<ulong, string>();
            Anonymize = new Dictionary<ulong, bool>();
        }

        public async Task InitAsync()
            => await InitAsync("jhdatabase").ConfigureAwait(false);

        public async Task InitAsync(string dbName)
        {
            DbName = dbName;
            conn = await R1.Connection().ConnectAsync();
            if (!await R1.DbList().Contains(DbName).RunAsync<bool>(conn))
                R1.DbCreate(DbName).Run(conn);
            if (!await R1.Db(dbName).TableList().Contains(GuildTableName).RunAsync<bool>(conn))
                R1.Db(dbName).TableCreate(GuildTableName).Run(conn);
        }

        public async Task InitGuildAsync(IGuild guild)
        {
            string guildIdStr = guild.Id.ToString();
            if (await R1.Db(DbName).Table(GuildTableName).GetAll(guildIdStr).Count().Eq(0).RunAsync<bool>(conn))
            {
                await R1.Db(DbName).Table(GuildTableName).Insert(R1.HashMap("id", guildIdStr)
                    .With("Prefix", "jh.")
                    .With("Language", "en")
                    .With("Availability", defaultAvailability)
                    ).RunAsync(conn);
            }
            dynamic json = await R1.Db(DbName).Table(GuildTableName).Get(guildIdStr).RunAsync(conn);
            Languages.Add(guild.Id, (string)json.Languages);
            Prefixs.Add(guild.Id, (string)json.Prefix);
            string availability = (string)json.Availability;
            if (availability == null)
                Availlability.Add(guild.Id, defaultAvailability);
        }
    }
}
