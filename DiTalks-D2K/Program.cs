using System.Net;
using System.Net.Sockets;
using System.Text;
using Discord;
using Discord.WebSocket;
using System.Text.Json;
using System.Text.RegularExpressions;

var sendLock = new object();

var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

socket.Bind(new IPEndPoint(IPAddress.Any, 8080));
socket.Listen(10);

var sockets = new List<Socket>();

var client = new DiscordSocketClient();

client.Log += e =>
{
    Console.WriteLine(e);
    return Task.CompletedTask;
};

client.Ready += async () =>
{
    Console.WriteLine($"{client.CurrentUser} connected");
    await client.CreateGlobalApplicationCommandAsync(new SlashCommandBuilder
    {
        Name = "ditalks",
        Description = "어느 방에서든 카카오톡으로 메시지를 전송합니다.",
        Options = new List<SlashCommandOptionBuilder>()
        {
            new()
            {
                Name = "message",
                Type = ApplicationCommandOptionType.String,
                Description = "카카오톡에 전송할 메시지",
                IsRequired = true
            }
        }
    }.Build());
};

client.JoinedGuild += async e => await e.SystemChannel.SendMessageAsync("이름이가 서버에 가입했습니다! :D");

client.SlashCommandExecuted += async command =>
{
    switch (command.CommandName)
    {
        case "ditalks":
            if (command.User.IsWebhook || command.User.IsBot) return;

            var prcMsg = (string)command.Data.Options.Where(x => x.Name == "message").First();
            Console.WriteLine(prcMsg);

            try
            {
                foreach (var user in new Regex("<@![0-9]{18}>").Matches(prcMsg).First().Groups.Values.Select(x => x.Value))
                    prcMsg = prcMsg.Replace(user, client.GetUser(ulong.Parse(user[3..^1])).Username);
            }
            catch (Exception) { /* Ignore */ }

            TrySendMessageToSocket(new
            {
                Message = prcMsg,
                SendBy = command.User.Username,
                Channel = command.Channel.Name,
                IsForce = true
            });

            await command.RespondAsync((string)command.Data.Options.Where(x => x.Name == "message").First());
            break;
    }
};

client.MessageReceived += async message =>
{
    if (message.Author.IsBot || message.Author.IsWebhook) return;

    var prcMsg = message.Content;
    foreach (var user in message.MentionedUsers)
        prcMsg = prcMsg.Replace($"\u003C@!{user.Id}\u003E", $"@{user.Username}");

    TrySendMessageToSocket(new
    {
        Message = prcMsg,
        SendBy = message.Author.Username,
        Channel = message.Channel.Name,
        IsForce = false,
        File = message.Attachments.Select(x => new
        {
            Name = x.Filename,
            Url = x.Url
        }).ToArray()
    });
};

await client.LoginAsync(TokenType.Bot, new StreamReader("token.txt").ReadToEnd());
await client.StartAsync();

new Thread(() =>
{
    while (true)
    {
        Thread.Sleep(30000);
        TrySendMessageToSocket(new
        {
            Message = "ping"
        });
    }
}).Start();

while (true)
{
    sockets.Add(socket.Accept());
}

void TrySendMessageToSocket(object message)
{
    var smg = JsonSerializer.Serialize(message);
    Console.WriteLine($"send: {smg}");
    var r = new List<Socket>();
    sockets.ForEach(x => {
        try
        {
            lock (sendLock)
            {
                x.Send(Encoding.UTF8.GetBytes(smg));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Disconnected " + x.RemoteEndPoint.ToString());
            Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
            r.Add(x);
        }
    });
    foreach (var x in r)
        sockets.Remove(x);
}