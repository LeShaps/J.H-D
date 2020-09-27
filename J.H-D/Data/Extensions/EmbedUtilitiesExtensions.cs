using Discord;
using System.Collections.Generic;

namespace J.H_D.Data.Extensions.Discord
{
    public static class EmbedUtilitiesExtensions
    {
        public static Embed Copy(this Embed Original)
        {
            return Original.ToEmbedBuilder().Build();
        }

        public static Embed UpdateDescription(this Embed Original, string Update)
        {
            EmbedBuilder Builder = Original.ToEmbedBuilder();
            Builder.Description = Update;

            return Builder.Build();
        }

        public static Embed BuildParagraphs(this Embed Original, string Text, string Separator)
        {
            EmbedBuilder NewBuilder = Original.ToEmbedBuilder();
            List<EmbedFieldBuilder> FieldsBuilders = new List<EmbedFieldBuilder>();
            List<string> Parts = new List<string>();
            string CurrentParagraph = "";

            foreach (string Section in Text.Split(Separator))
            {
                if (CurrentParagraph.Length + Section.Length + Separator.Length > 1024) {
                    Parts.Add(CurrentParagraph);
                    CurrentParagraph = "";
                }
                CurrentParagraph += Section + Separator;
            }
            Parts.Add(CurrentParagraph);

            for (int i = 0; i < Parts.Count; i++)
            {
                FieldsBuilders.Add(new EmbedFieldBuilder
                {
                    Name = $"Part {i + 1}",
                    Value = Parts[i],
                    IsInline = true
                });
            }

            NewBuilder.Fields = FieldsBuilders;

            return NewBuilder.Build();
        }
    }
}
