using CommunityToolkit.HighPerformance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace VL.TelegramUtils
{
    public class BotEventsHandler
    {
        public TelegramBotClient bot { get; private set; }
        CancellationTokenSource _cts = new CancellationTokenSource();
        Task _running;
        bool isRunning;
        public Func<Message, UpdateType, Message> OnMessageReceieved { set; get; }

        

        public BotEventsHandler(string Token)
        {
            bot = new TelegramBotClient(Token, cancellationToken: _cts.Token);
            isRunning = true;
            _running = RunAsync();

            bot.OnMessage += OnMessage;
        }

        public async Task RunAsync()
        {
            if (isRunning) return;
            

                bot.OnError += OnError;
                bot.OnMessage += OnMessage;
                bot.OnUpdate += OnUpdate;

            

        }

        public void Stop()
        {
            if (!isRunning) return;

            _cts.Cancel();
            try
            {

            }
            catch { }
            finally { }
            isRunning = false;
        }

        async Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception); // just dump the exception to the console
        }

        // method that handle messages received by the bot:
        public async Task OnMessage(Message msg, UpdateType type)
        {
            var ms = OnMessageReceieved.Invoke(msg, type);
        }

        // method that handle other types of updates received by the bot:
        async Task OnUpdate(Update update)
        {
            if (update is { CallbackQuery: { } query }) // non-null CallbackQuery
            {
                await bot.AnswerCallbackQuery(query.Id, $"You picked {query.Data}");
                await bot.SendMessage(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
            }
        }
    }
}
