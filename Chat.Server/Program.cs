using System.Net;
using System.Net.Sockets;

var server = new Server();
await server.Run();


public class Server
{
    private readonly TcpListener _tcpListener;

    public Server()
    {
        _tcpListener = new TcpListener(IPAddress.Any, 7077);
    }

    public async Task Run()
    {
        try
        {
            _tcpListener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                var client = new Client(tcpClient, this);
                Task.Run(client.ProcessAsync);
            }
        }
        catch (Exception ex)
        {

        }
    }

    public async Task SendAnswer(Client client, string message)
    {
        await client.Writer.WriteAsync(message);
        await client.Writer.FlushAsync();
    }
}

public class Client
{
    public Guid Id = Guid.NewGuid();
    public StreamReader Reader { get; set; }
    public StreamWriter Writer { get; set; }

    private TcpClient _tcpClient;
    private Server _server;

    public Client(TcpClient tcpClient, Server server)
    {
        _tcpClient = tcpClient;
        _server = server;
        Reader = new StreamReader(_tcpClient.GetStream());
        Writer = new StreamWriter(_tcpClient.GetStream());
    }

    public async Task ProcessAsync()
    {
        try
        {
            string userName = await Reader.ReadLineAsync();
            string message = $"{userName} вошел в чат";
            await _server.SendAnswer(this, message);
            Console.WriteLine(message);

            while (true)
            {
                try
                {
                    message = await Reader.ReadLineAsync();

                    if (message == null)
                        continue;

                    message = $"{userName}: {message}";
                    Console.WriteLine(message);
                    await _server.SendAnswer(this, message);
                }
                catch
                {

                }
            }
        }
        catch
        {

        }
    }
}