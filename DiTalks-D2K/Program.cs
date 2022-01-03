using System.Net;
using System.Net.Sockets;
using System.Text;
using Discord;
using Discord.WebSocket;

var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Bind(new IPEndPoint(IPAddress.Any, 8080));
socket.Listen(10);

var clientSocket = socket.Accept();

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
    clientSocket.Send(Encoding.UTF8.GetBytes(message.Content));
};

await client.LoginAsync(TokenType.Bot, "NzQwODQ5NzIxMjQwOTc3NDM3.XyvAEQ.qT52IluqnMpK5p3fw2WwLeipLO4");
await client.StartAsync();
await Task.Delay(Timeout.Infinite);
