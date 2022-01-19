using System.Net;
using System.Net.Sockets;
using System.Text;
using Discord;
using Discord.WebSocket;
using System.Text.Json;

var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Bind(new IPEndPoint(IPAddress.Any, 8080));
socket.Listen(10);

var sockets = new List<Socket>();

var client = new DiscordSocketClient();
client.Log += e =>
{
    Console.WriteLine(e);
    return Task.CompletedTask;
};
client.Ready += () =>
{
    Console.WriteLine($"{client.CurrentUser} connected");
    return Task.CompletedTask;
};
client.MessageReceived += async message =>
{
    var r = new List<Socket>();
    sockets.ForEach(x => {
        try
        {
            x.Send(Encoding.UTF8.GetBytes(message.Content));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
            r.Add(x);
        }
    });
    foreach (var x in r)
        sockets.Remove(x);
};

await client.LoginAsync(TokenType.Bot, new StreamReader("token.txt").ReadToEnd());
await client.StartAsync();

while (true)
{
    sockets.Add(socket.Accept());
}