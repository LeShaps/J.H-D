using Discord;
using Discord.Commands;
using J.H_D.Minions.Websites;
using J.H_D.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static J.H_D.Data.Response;

namespace J.H_D.Modules
{
    class SCPModule : ModuleBase
    {
        [Command("Report")]
        public async Task MakeReport([Remainder]int Number)
        {
            var Report = await SCPMinion.GetBasicReport(Number).ConfigureAwait(false);

            await ReplyAsync("", false, BuildReportBase(Report.Answer));
        }

        public Embed BuildReportBase(SCPReport Report)
        {
            EmbedBuilder builder = new EmbedBuilder
            {
                Title = $"#SCP-{Report.Number}: {Utilities.Clarify(Report.Name)}",
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = "Class",
                        Value = Utilities.Clarify(Report.Class),
                        IsInline = true
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Description",
                        Value = Utilities.Clarify(Report.Description),
                        IsInline = false
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Confinement procedures",
                        Value = Utilities.Clarify(Report.Confinement),
                        IsInline = false
                    }
                },
                Color = new Color(0xF0EFE3),
                Url = $"http://www.scpwiki.com/scp-{Report.Number}"
            };

            if (Report.HasImage)
            {
                builder.ImageUrl = Report.ImageLink;
                builder.Footer = new EmbedFooterBuilder
                {
                    Text = Report.ImageCaption
                };
            }

            return builder.Build();
        }
    }
}
