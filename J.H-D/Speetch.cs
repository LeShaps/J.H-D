using Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D
{
    class Speetch
    {
        public static Embed FchanHelp = new EmbedBuilder()
        {
            Title = "Help Menu",
            Color = Discord.Color.DarkGreen,
            Fields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder()
                        {
                            Name = "__**Japanese Culture**__",
                            Value = ("a - Anime and Manga" + Environment.NewLine + "c - Anime/Cute" + Environment.NewLine + "w - Anime/Wallpapers" + Environment.NewLine + "m - Mecha" + Environment.NewLine + "cgl - Costplay & EGL" +
                            Environment.NewLine + "cm - Cute/Male" + Environment.NewLine + "f - Flash" + Environment.NewLine + "n - Transportation" + Environment.NewLine + "jp - Otaku Culture"),
                            IsInline = true,
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = "__**Video Games**__",
                            Value = ("v - Video Games" + Environment.NewLine + "vg - Video Games Generals" + Environment.NewLine + "vp - Pokémon" + Environment.NewLine + "vr - Retro Games"),
                            IsInline = true,
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = "__**Interests**__",
                            Value = ("co - Comics & Cartoons" + Environment.NewLine + "g - Technology" + Environment.NewLine + "tv - Television & Film" + Environment.NewLine + "k - Weapons" + Environment.NewLine +
                            "o - Auto" + Environment.NewLine + "an - Animals & Nature" + Environment.NewLine + "tg - Traditional Games" + Environment.NewLine + "sp - Sports" + Environment.NewLine + "asp - Alternative Sports" +
                            Environment.NewLine + "sci - Sciences & Math" + Environment.NewLine + "his - History and Humanities" + Environment.NewLine + "int - International" + Environment.NewLine + "out - Outdoors" + Environment.NewLine +
                            "toy - Toys"),
                            IsInline = true,
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = "__**Creative**__",
                            Value = ("i - Oekaki" + Environment.NewLine + "po - Papercraft & Origami" + Environment.NewLine + "p - Photography" + Environment.NewLine + "ck - Food & Cooking" + Environment.NewLine + "ic - Artwork/Critique" +
                            Environment.NewLine + "wg - Wallpapers/General" + Environment.NewLine + "lit - Literature" + Environment.NewLine + "mu - Music" + Environment.NewLine + "fa - Fashion" + Environment.NewLine +
                            "3 - 3DCG" + Environment.NewLine + "gd - Graphic Design" + Environment.NewLine + "diy - Do-It-Yourself" + Environment.NewLine + "wsg - Worksafe GIF" + Environment.NewLine + "qst - Quests"),
                            IsInline = true,
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = "__**Other**__",
                            Value = ("biz - Business & Finance" + Environment.NewLine + "trv - Travel" + Environment.NewLine + "fit - Fitness" + Environment.NewLine + "x - Paranormal" + Environment.NewLine + "adv - Advice" + Environment.NewLine
                            + "lgbt - LGBT" + Environment.NewLine + "mlp - Pony" + Environment.NewLine + "news - Current News" + Environment.NewLine + "wsr - Worksafe Requests" + Environment.NewLine + "vip - Very Important Posts"),
                            IsInline = true,
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = "__**Misc. (NSFW)**__",
                            Value = ("b - Random" + Environment.NewLine + "r9k - ROBOTT9001" + Environment.NewLine + "pol - Politicaly Incorrect" + Environment.NewLine + "bant - International/Random" + Environment.NewLine +
                            "soc - Cams & Meetups" + Environment.NewLine + "s4s - Shit 4chan Says"),
                            IsInline = true,
                        },
                        new EmbedFieldBuilder()
                        {
                            Name = "__**Adult**__",
                            Value = ("s - Sexy Beautiful Women" + Environment.NewLine + "hc - Hardcore" + Environment.NewLine + "hm - Handsome Men" + Environment.NewLine + "h - Hentai" + Environment.NewLine + "e - Ecchi" + Environment.NewLine +
                            "u - yuri" + Environment.NewLine + "d - Hentai/Alternative" + Environment.NewLine + "y - Yaoi" + Environment.NewLine + "t - Torrents" + Environment.NewLine + "hr - High Resolution" + Environment.NewLine +
                            "gif - Adult GIF" + Environment.NewLine + "aco - Adult Cartoons" + Environment.NewLine + "r - Adult Request"),
                            IsInline = true,
                        }
                    },
            ImageUrl = "http://img.2ch.sc/img/4_0217.png",
        }.Build();

        public static string FcUnknownChan = "Veuillez vérifier que vous avez demandé un chan existant, pour la liste de chan, executez 4nsfw help";

        public static string NfMovie = "Je n'ai pas trouvé ce film, veuillez vérifier l'orthographe ou essayer avec un autre";

        public static string Wait = "Vous recevrez ce que vous voulez dans quelques secondes";

        public static EmbedBuilder make_movieinfos(Movie movie)
        {
            Console.WriteLine(movie._name);
            Console.WriteLine(movie._releaseDate);
            Console.WriteLine(movie._overview);
            Console.WriteLine(movie._averageNote);
            Console.WriteLine(movie._posterpath);
            EmbedBuilder infos = new EmbedBuilder()
            {
                Title = movie._name,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Date de sortie",
                        IsInline = true,
                        Value = movie._releaseDate,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Synopsis",
                        IsInline = false,
                        Value = movie._overview,
                    }
                },
                Color = Color.DarkRed,
                Footer = new EmbedFooterBuilder()
                {
                    Text = "Note globale : " + movie._averageNote,
                },
                ImageUrl = movie._posterpath,
            };
            return (infos);
        }

        public static EmbedBuilder makemovie_moreinfo(Movie movie)
        {
            EmbedBuilder moreinfos = new EmbedBuilder()
            {
                Title = movie._name + " - Other infos",
                Color = Color.DarkRed,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Titre original",
                        Value = movie._originalTitle,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Langage original",
                        Value = movie._originalLanguage,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Budget",
                        Value = movie._budget + "$",
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Recettes",
                        Value = movie._revenue + "$",
                        IsInline = true,
                    }
                },
                ImageUrl = movie._backdropPath,
            };
            return (moreinfos);
        }

        public static EmbedBuilder anime_builder(Anime anime)
        {
            EmbedBuilder animeinfos = new EmbedBuilder()
            {
                Title = anime._name,
                Color = Color.DarkBlue,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Épisodes",
                        Value = anime._nb,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Débuté le ",
                        Value = anime._startDate,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Fini le",
                        Value = anime._endDate,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Synopsis",
                        Value = anime._synopsis,
                        IsInline = false,
                    },
                },
                ImageUrl = anime._image_link,
                Footer = new EmbedFooterBuilder()
                {
                    Text = ("Score : " + anime._rate + "/10"),
                },
            };
            return (animeinfos);
        }

        public static EmbedBuilder build_skimageinfos(sk_image image)
        {
            EmbedBuilder skinfos_builder = new EmbedBuilder()
            {
                Title = image._name,
                Color = Color.DarkMagenta,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Rating",
                        Value = image._rating,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Have loli",
                        Value = image._isLoli.ToString(),
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Author",
                        Value = image._author,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Tags",
                        Value = image.make_tagnamelist(),
                        IsInline = false,
                    },
                },
                Footer = new EmbedFooterBuilder()
                {
                    Text = "Source : " + image._source,
                },
            };
            return (skinfos_builder);
        }

        private static List<Tag> m_tagtypelist(List<Tag> tags, TagType wantedtype)
        {
            List<Tag> typeoftaglist = new List<Tag>();

            foreach (Tag t in tags)
            {
                if (t._type == wantedtype)
                    typeoftaglist.Add(t);
            }
            return (typeoftaglist);
        }

        private static string mtag_toline(List<Tag> tags)
        {
            string res = " ";

            foreach (Tag t in tags)
            {
                res += t._name + Environment.NewLine;
            }
            return (res);
        }

        private static string mtag_tolist(List<Tag> tags)
        {
            string res = " ";

            foreach(Tag t in tags)
            {
                res += t._name + ",";
            }
            if (res != " ")
                res.Substring(0, res.Length - 2);
            if (res.Length > 2000)
            {
                res.Substring(0, 1997);
                res += "...";
            }
            return (res);
        }

        public static EmbedBuilder tembuild_danwtag(dan_image image)
        {
            string original = mtag_toline(m_tagtypelist(image._tags, TagType.Copyright));
            string charac = mtag_toline(m_tagtypelist(image._tags, TagType.Character));
            string author = mtag_toline(m_tagtypelist(image._tags, TagType.Artist));

            if (original == " ")
                original = "Original";
            if (charac == " ")
                charac = "OC";
            if (author == " ")
                author = image._author;

            EmbedBuilder danbuilder = new EmbedBuilder()
            {
                Title = image._name,
                Color = Color.DarkMagenta,
                Description = "More infos about your last image",
                Url = image._file_url,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Œuvre(s) originale(s)",
                        Value = original,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Character(s)",
                        Value = charac,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Artist(s)",
                        Value = author,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Have loli",
                        Value = image._isLoli.ToString(),
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Rating",
                        Value = image._rating.ToString(),
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Is banned",
                        Value = image._is_banned.ToString(),
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Tags",
                        Value = image.make_tagnamelist(),
                        IsInline = false,
                    },
                },
                Footer = new EmbedFooterBuilder()
                {
                    Text = "Source : " + image._source,
                },
                ImageUrl = image._sample_url,
            };
            return (danbuilder);
        }

        public static EmbedBuilder tembuild_dan(dan_image image)
        {
            string original = mtag_toline(m_tagtypelist(image._tags, TagType.Copyright));
            string charac = mtag_toline(m_tagtypelist(image._tags, TagType.Character));
            string author = mtag_toline(m_tagtypelist(image._tags, TagType.Artist));

            if (original == " ")
                original = "Original";
            if (charac == " ")
                charac = "OC";
            if (author == " ")
                author = image._author;

            EmbedBuilder danbuilder = new EmbedBuilder()
            {
                Title = image._name,
                Color = Color.DarkMagenta,
                Description = "More infos about your last image",
                Url = image._file_url,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Œuvre(s) originale(s)",
                        Value = original,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Character(s)",
                        Value = charac,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Artist(s)",
                        Value = author,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Have loli",
                        Value = image._isLoli.ToString(),
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Rating",
                        Value = image._rating.ToString(),
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Is banned",
                        Value = image._is_banned.ToString(),
                        IsInline = true,
                    },
                },
                Footer = new EmbedFooterBuilder()
                {
                    Text = "Source : " + image._source,
                },
                ImageUrl = image._sample_url,
            };
            return (danbuilder);
        }

        public static EmbedBuilder konbuilderwtag(kon_image image)
        {
            string original = mtag_toline(m_tagtypelist(image._tags, TagType.Copyright));
            string charac = mtag_toline(m_tagtypelist(image._tags, TagType.Character));
            string author = mtag_toline(m_tagtypelist(image._tags, TagType.Artist));

            if (original == " ")
                original = "Original";
            if (charac == " ")
                charac = "OC";
            if (author == " ")
                author = image._author;

            EmbedBuilder konbuilder = new EmbedBuilder()
            {
                Title = image._name,
                Color = Color.DarkMagenta,
                Description = "More infos about your last image",
                Url = image._file_url,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Œuvre(s) originale(s)",
                        Value = original,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Character(s)",
                        Value = charac,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Artist(s)",
                        Value = author,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Have loli",
                        Value = image._isLoli.ToString(),
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Rating",
                        Value = image._rating.ToString(),
                        IsInline = true,
                    },
                },
                Footer = new EmbedFooterBuilder()
                {
                    Text = "Source : " + image._source,
                },
                ImageUrl = image._sample_url,
            };
            return (konbuilder);
        }

        public static EmbedBuilder konbuilder(kon_image image)
        {
            string original = mtag_toline(m_tagtypelist(image._tags, TagType.Copyright));
            string charac = mtag_toline(m_tagtypelist(image._tags, TagType.Character));
            string author = mtag_toline(m_tagtypelist(image._tags, TagType.Artist));

            if (original == " ")
                original = "Original";
            if (charac == " ")
                charac = "OC";
            if (author == " ")
                author = image._author;

            EmbedBuilder konbuilder = new EmbedBuilder()
            {
                Title = image._name,
                Color = Color.DarkMagenta,
                Description = "More infos about your last image",
                Url = image._file_url,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Œuvre(s) originale(s)",
                        Value = original,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Character(s)",
                        Value = charac,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Artist(s)",
                        Value = author,
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Have loli",
                        Value = image._isLoli.ToString(),
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Rating",
                        Value = image._rating.ToString(),
                        IsInline = true,
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Tags",
                        Value = image.make_tagnamelist(),
                        IsInline = false,
                    },
                },
                Footer = new EmbedFooterBuilder()
                {
                    Text = "Source : " + image._source,
                },
                ImageUrl = image._sample_url,
            };
            return (konbuilder);
        }
    }
}
