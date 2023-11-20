using System.Net.Sockets;

var host = "127.0.0.1";
int port = 7077;
using var tcpClient = new TcpClient();
Console.WriteLine("Введите свое имя:");
var name = Console.ReadLine();
StreamWriter writer = null;
StreamReader Reader = null;

try
{
    tcpClient.Connect(host, port);
    writer = new StreamWriter(tcpClient.GetStream());
    Reader = new StreamReader(tcpClient.GetStream());

    if (writer == null)
        return;

    Task.Run(() => ReceiveMessageAsync(Reader));
    await SendMessageAsync(writer);
}
catch
{

}

async Task ReceiveMessageAsync(StreamReader reader)
{
    while (true)
    {
        try
        {
            var message = await reader.ReadLineAsync();

            if (string.IsNullOrEmpty(message))
                continue;

            Print(message);
        }
        catch
        {

        }
    }
}

void Print(string message)
{
    Console.WriteLine(message);
}


async Task SendMessageAsync(StreamWriter writer)
{
    await writer.WriteLineAsync(name);
    await writer.FlushAsync();
    Console.WriteLine("Введите сообщение:");

    while (true)
    {
        var message = Console.ReadLine();
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
    }
}
