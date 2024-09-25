using System.Net;
using System.Net.Sockets;

Server server = new Server();

class Server
{
    private TcpListener tcpListener;
    private Thread listenThread;

    public Server()
    {
        this.tcpListener = new TcpListener(IPAddress.Any, 80);
        this.listenThread = new Thread(new ThreadStart(ClientListener));
        this.listenThread.Start();
    }

    private void ClientListener()
        {
            this.tcpListener.Start();

            while (true)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();
                Thread clientThread = new Thread(new ParameterizedThreadStart(ClientCommunication));
                clientThread.Start(client);
            }
        }

    private void ClientCommunication(object client)
    {
        TcpClient tcpClient = (TcpClient)client;
        NetworkStream clientStream = tcpClient.GetStream();
        Console.WriteLine("Got connection");
        StreamReader clientStreamReader = new StreamReader(clientStream);

        while (true)
        {
            string input = clientStreamReader.ReadLine();

            if (input != null)
            {
                Console.WriteLine(clientStreamReader.ReadLine());
            }
            
        }
    }
}