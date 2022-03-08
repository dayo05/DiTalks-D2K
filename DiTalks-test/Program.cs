using System.Net;
using System.Net.Sockets;
using System.Text;

var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Connect(new IPEndPoint(IPAddress.Parse("40.87.96.156"), 8080));
Console.WriteLine("Connected");
Console.OutputEncoding = Encoding.UTF8;
while (true)
{
    var buf = new byte[100000];
    socket.Receive(buf);
    Console.WriteLine(Encoding.UTF8.GetString(buf));
}