using Microsoft.VisualBasic;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace Telegram
{
    public class BotClientV2:IDisposable
    {
        private TelegramBotClient botClient;   
        
        public User botUser = new User();

        
        public TelegramBotClient.OnMessageHandler MessageHandler { set; get; }
        public TelegramBotClient.OnErrorHandler ErrorHandler { set; get; }
        public TelegramBotClient.OnUpdateHandler UpdateHandler { set; get; }

        public delegate Task CustomMessageHandler(BotClientV2 Bot, Message message, UpdateType type);
        public CustomMessageHandler customMessageHandler {get;set;}
        public CallbackQuery CallbackQuery { set; get; }
        

        public TelegramBotClientOptions BotOptions;
        private ReceiverOptions ReceiverOptions;

        

        public bool Debug;

        public BotClientV2(TelegramBotClientOptions BotOptions, ReceiverOptions ReceiverOptions)
        {
            this.BotOptions = BotOptions;

            try
            {

                botClient = new TelegramBotClient(BotOptions);
                this.ReceiverOptions = ReceiverOptions;

                
            }
            catch(Exception ex) 
            { 
                if(Debug)
                Console.WriteLine(ex.Message);
            }
            

            

            if(botClient != null)
            {
                Init();
                botClient.OnMessage += OnMessage;
                botClient.OnMessage += OnCustomMessage;
                botClient.OnError += OnError;
                botClient.OnUpdate += OnUpdate;
                
            }
            
            
        }

        public Task OnCustomMessage(Message msg, UpdateType type)
        {
            return customMessageHandler.Invoke(this, msg, type);
        }

        public async Task OnMessage(Message msg, UpdateType type)
        {
            
            await MessageHandler.Invoke(msg, type);

        }

        public async Task OnError(Exception exception, HandleErrorSource source)
        {
            await ErrorHandler.Invoke(exception, source);
        }
 
        public async Task OnUpdate(Update update)
        {
            await UpdateHandler.Invoke(update);
        }


        public async Task SendText(int chatId, string text)
        {
            await this.botClient.SendMessage(chatId, text);
        }


        public async Task<Message> SendByteObject(int ChatId,Spread<byte> Bytes, string? caption)
        {
            using (var ms = new MemoryStream(Bytes.ToArray()))
            {
                if (ms.CanSeek)
                {
                    ms.Position = 0;
                }
            var inp = InputFile.FromStream(ms, "img.png");
            return await this.botClient.SendPhoto(ChatId, inp);
            }
            
        }

        public async Task<Message> SendStreamObject(int ChatId, Stream stream, string? caption)
        {
            stream.Position = 0;    
            var inp = InputFile.FromStream(stream);
            return await this.botClient.SendPhoto(ChatId, inp);


        }

        public async Task<Message> SendFile(int ChatId, string filePath, string? caption)
        {
            return await this.botClient.SendPhoto(ChatId, InputFile.FromString(filePath), caption);
        }


        async void Init()
        {
            this.botUser = await this.botClient.GetMe();
            
        }

        

        public TelegramBotClientOptions GetOptions => this.BotOptions;

        public ReceiverOptions GetReceiverOptions=>this.ReceiverOptions;

        public string Name => botUser.Username;

        public TelegramBotClient GetBot=> this.botClient;

        public void Dispose()
        {

            this.botClient = null;
            
        }

    }
}
