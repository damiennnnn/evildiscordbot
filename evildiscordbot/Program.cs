using System;
using System.Globalization;
using DSharpPlus;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

//MTAwOTQ4NzU1OTc0OTAyMTc2Ng.Gei0Sd.CKhovYDNnvmhpgtevWyUOBJzbPlJZZtxGo1tIE
namespace evildiscordbot
{
    public class Program
    {
        private static ulong _AuthorId = 141601495819747329;
        private static string _Token = "";


        static void Main(string[] args)
        {
            _Token = args[0];
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task Countdown(DiscordChannel channel, DiscordClient client)
        {
            DateTime releaseTime = new DateTime(2022, 8, 18, 8, 15, 0);

            DateTime oldTime = new DateTime();
            bool firstMsg = false;
            while (DateTime.Now.Ticks < releaseTime.Ticks)
            {
                DateTime difference = new DateTime(releaseTime.Ticks - DateTime.Now.Ticks);
                
                async Task Print()
                {
                    var message = await client.SendMessageAsync(channel, "@everyone");
                    await message.ModifyAsync(string.Format("You have {0} hours {1} minutes {2} seconds and {3} milliseconds! :)", difference.Hour, difference.Minute,
                        difference.Second, difference.Millisecond));

                };

                if (difference.Second < (oldTime.Second  - 6))
                {
                    DiscordActivity activity =
                        new DiscordActivity(difference.ToLongTimeString(), ActivityType.Competing);
                    await client.UpdateStatusAsync(activity);
                    oldTime = difference;
                }
                
                if (
                    ((difference.Hour > 3) && (!firstMsg || difference.Hour < oldTime.Hour))
                    || (difference.Hour is > 0 and <= 3 && (difference.Minute < (oldTime.Minute -15)))
                    || ((difference.Hour == 0 && (difference.Minute + 5) < oldTime.Minute))
                    )
                {
                    firstMsg = true;
                    await Print();
                    oldTime = difference;
                }
            }
            
            var message = await client.SendMessageAsync(channel, "@everyone");
            await message.ModifyAsync("gg ez");
        }
        
        static async Task AsyncMain()
        {
            bool countdown = false;
            var botClient = new DiscordClient(new DiscordConfiguration()
            {
                Token = _Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            botClient.Ready += async (s, e) =>
            {
            };
            botClient.GuildAvailable += async (s, e) =>
            {
                botClient.Logger.LogInformation(new EventId(10), null, "Guild available: {Name} {Id}", e.Guild.Name, e.Guild.Id);
            };
            botClient.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower().StartsWith("dookie") && !countdown)
                {
                    countdown = true;
                    await botClient.SendMessageAsync(e.Message.Channel, "hello");
                    await Countdown(e.Message.Channel, botClient);

                }
            };
            
            DiscordActivity activity = new DiscordActivity("test", ActivityType.Watching);
            await botClient.ConnectAsync(activity: activity);


            await Task.Delay(-1);
        }
    }
}