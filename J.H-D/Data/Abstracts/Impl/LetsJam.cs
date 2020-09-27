using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using J.H_D.Data.Exceptions;
using J.H_D.Data.Extensions;
using J.H_D.Data.Interfaces;
using static J.H_D.Data.Response;
using J.H_D.Data.Extensions.Discord;
using System.Globalization;

namespace J.H_D.Data.Abstracts.Impl
{
    class LetsJam : AGame
    {
        List<string> EmbedTexts = new List<string>();
        List<Tuple<string, bool>> Lyrics = new List<Tuple<string, bool>>();
        SongLyrics? Infos;
        List<string> WordsForTest = new List<string>();
        IUserMessage EmbedMessage;

        readonly List<Tuple<float, uint, string>> ScoreInfos = new List<Tuple<float, uint, string>>
        {
            new Tuple<float, uint, string>(0, Convert.ToUInt32("A5A597", 16), "No medal"),
            new Tuple<float, uint, string>(70, Convert.ToUInt32("B8722D", 16), "the bronze medal, great job!"),
            new Tuple<float, uint, string>(80, Convert.ToUInt32("C6C6C6", 16), "the Silver medal, keep it up!"),
            new Tuple<float, uint, string>(90, Convert.ToUInt32("FFDB19", 16), "the gold medal, so close!")
        };

        private readonly Regex Pattern = new Regex("[\\w\\S]");
        private readonly int MaxScore = 10_000;
        private readonly float TimePerWord = 0.9f;

        public LetsJam(IMessageChannel Channel, IUser _, ISetup Setup, IPostMode PostMode) : base(Channel, _, Setup, PostMode)
        {
            var LyricsInfos = Setup.GetLoadedParameters() as Tuple<SongLyrics?, List<Tuple<string, bool>>>;
            Lyrics = LyricsInfos.Item2;
            Infos = LyricsInfos.Item1;
            foreach (var Word in Lyrics) {
                string UsableWord = Word.Item1.ToWordOnly();
                if (UsableWord != null)
                    WordsForTest.Add(UsableWord.ToLowerInvariant());
            }
        }

        protected override Task CheckAnswerInternalAsync(string Answer)
        {
            List<int> ReplaceInstances = new List<int>();
            string LyricsToDiplay = null;

            if (Answer.IsNullOrEmpty())
                return Task.CompletedTask;

            
            foreach (string Word in Answer.Split(" "))
            {
                string WordVar = Word.ToWordOnly().ToLowerInvariant();

                if (Lyrics.Any(lyr => lyr.Item1 != "\n" && lyr.Item1.ToWordOnly().ToLowerInvariant() == WordVar)) {
                    ReplaceInstances.AddRange(Lyrics.IndexesOf(lyr => lyr.Item1 != "\n" && lyr.Item1.ToWordOnly().ToLowerInvariant() == WordVar));
                }
            }

            foreach (int Index in ReplaceInstances) {
                string OldWord = Lyrics[Index].Item1;
                Lyrics[Index] = new Tuple<string, bool>(OldWord, true);
            }
            ReplaceInstances.Clear();

            foreach (var lyr in Lyrics)
            {
                if (lyr.Item2)
                    LyricsToDiplay += " " + lyr.Item1;
                else
                    LyricsToDiplay += " " + Pattern.Replace(lyr.Item1, "-");
            }

            var Emb = (Embed)EmbedMessage.Embeds.First();

            if (LyricsToDiplay.Length < 2048)
            {
                Emb = UpdateEmbed(Emb, LyricsToDiplay);
                EmbedMessage.ModifyAsync(x => x.Embed = Emb);
            }
            else
            {
                Emb = UpdateEmbed(Emb);
                EmbedMessage.ModifyAsync(x => x.Embed = Emb.BuildParagraphs(LyricsToDiplay, "\n \n"));
            }

            if (Lyrics.Any(x => x.Item2 == false))
                throw new InvalidGameAnwserException("Continue");
            else
                throw new GameLostException("Win");
        }

        protected override void CheckReactionAsync(IMessage Message, IReaction Reaction)
        {
            throw new NotImplementedException();
        }

        protected override string GetAnswer()
        {
            string FinalLyrics = null;
            float ScorePercent = Score * 100 / MaxScore;
            var FinalScoreInfos = ScoreInfos.Where(sc => ScorePercent > sc.Item1).Last();


            foreach (var lyr in Lyrics)
            {
                if (lyr.Item2)
                    FinalLyrics += " " + lyr.Item1;
                else
                    FinalLyrics += " **" + lyr.Item1 + "**";
            }

            if (FinalLyrics.Length < 2048)
            {
                var Emb = (Embed)EmbedMessage.Embeds.First();
                Emb = UpdateEmbed(Emb, FinalLyrics);
                EmbedMessage.ModifyAsync(x => x.Embed = Emb);
            }
            else
            {
                Random rand = new Random();
                string Exemples = $"Here's some exemples of words you haven't found{Environment.NewLine}";

                var NotFounds = Lyrics.Where(lyr => lyr.Item2 == false && lyr.Item1 != "\n").ToList();
                if (NotFounds.Count >= 5)
                {
                    for (int i = 5; i > 0; i--)
                    {
                        Exemples += $"=> {NotFounds[rand.Next(NotFounds.Count) - 1].Item1}{Environment.NewLine}";
                    }
                }
                Lyrics.Clear();
                JHConfig.Games.Remove(this);
                return $"You've got {FinalScoreInfos.Item3}\n{Exemples}";
            }

            Lyrics.Clear();
            JHConfig.Games.Remove(this);
            return $"You've got {FinalScoreInfos.Item3}\nCheck the initial embed for the response!";
        }

        protected override int GetGameTimer()
            => (int)(TimePerWord * Lyrics.Count);

        protected override object GetNextPost()
        {
            string LyricsToDisplay = null;
            EmbedBuilder Builder;

            foreach (var lyr in Lyrics)
            {
                if (lyr.Item2)
                    LyricsToDisplay += " " + lyr.Item1;
                else
                    LyricsToDisplay += " " + Pattern.Replace(lyr.Item1, "-");
            }

            if (LyricsToDisplay.Length < 2048)
            {
                Builder = new EmbedBuilder
                {
                    Color = Color.DarkerGrey,
                    Description = LyricsToDisplay,
                    Title = $"You play on {Infos.Value.Artist} - {Infos.Value.SongName}",
                    Footer = new EmbedFooterBuilder
                    {
                        Text = $"{Score:N} pts / {MaxScore:N}"
                    }
                };
                return Builder.Build();
            }
            else
            {
                Builder = new EmbedBuilder
                {
                    Color = Color.DarkerGrey,
                    Title = $"You play on {Infos.Value.Artist} - {Infos.Value.SongName}",
                    Footer = new EmbedFooterBuilder
                    {
                        Text = $"{Score:N} pts / {MaxScore:N}"
                    }
                };

                return Builder.Build().BuildParagraphs(LyricsToDisplay, "\n \n");
            }

        }

        protected override string GetRules()
        {
            TimeSpan span = TimeSpan.FromSeconds(GetGameTimer());
            string Timer = span.ToString(@"mm\:ss", CultureInfo.InvariantCulture);
            return $"Write the lyrics to complete the song, you have {Timer}\nGood luck!";
        }

        protected override int GetScore()
        {
            float ScoreRatio = (float)MaxScore / Lyrics.Count;
            Score = (int)(Lyrics.Where(x => x.Item2 == true).Count() * ScoreRatio);
            return Score;
        }

        protected override string GetSuccessMessage()
            => "You know your jam!\nYou've found the whole song, congratulation";

        protected override Task ProccessSuccessAsync(string Answer)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateMessage(IUserMessage Message)
            => EmbedMessage ??= Message;
        
        private Embed UpdateEmbed(Embed Original, string Lyrics) {
            EmbedBuilder NewBuilder = Original.ToEmbedBuilder();
            Color EmbedColor = new Color(Convert.ToUInt32("A5A597", 16));
            float ScorePercent = Score * 100 / MaxScore;

            foreach (var c in ScoreInfos) {
                if (ScorePercent > c.Item1)
                    EmbedColor = new Color(c.Item2);
            }

            NewBuilder.Description = Lyrics;
            NewBuilder.Color = EmbedColor;
            NewBuilder.Footer.Text = $"{GetScore():N} pts / {MaxScore:N}";

            return NewBuilder.Build();
        }

        private Embed UpdateEmbed(Embed Original)
        {
            EmbedBuilder UpdateBuilder = Original.ToEmbedBuilder();
            Color EmbedColor = Color.DarkGrey;
            float ScorePercent = Score * 100 / MaxScore;

            foreach (var c in ScoreInfos)
            {
                if (ScorePercent > c.Item1)
                    EmbedColor = new Color(c.Item2);
            }

            UpdateBuilder.Color = EmbedColor;
            UpdateBuilder.Footer.Text = $"{GetScore():N} pts / {MaxScore:N}";

            return UpdateBuilder.Build();
        }
    }
}
