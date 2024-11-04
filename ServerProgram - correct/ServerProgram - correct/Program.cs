using ServerProgram___correct;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;

public class server
{
    static int bikeID = 0;
    public static Dictionary<int, Socket> bikeClients = new Dictionary<int, Socket>();
    public static Dictionary<string, Socket> bikeClients1 = new Dictionary<string, Socket>();
    public static Dictionary<Socket, string> keyList = new Dictionary<Socket, string>();
    public static DoctorHandler doctorHandler;
    public static List<BikeHandler> bikeHandlers = new List<BikeHandler>();
    private static DataStorage dataStorage;
    private static string privateKey;
    private static string publicKey;

    public static void Main()
    {
        try
        {
            dataStorage = new DataStorage("data.json");
            bikeID = dataStorage.getHighestID();

            (publicKey, privateKey) = encryption.GenerateKeys();

            Data data = new Data(10, 1.1, 2.2, 10, 3, 8, "name");
            TcpListener myList = new TcpListener(IPAddress.Any, 8001);
            myList.Start();
            Console.WriteLine("The server is running at port 8001...");
            Console.WriteLine("Waiting for a connection.....");

            while (true)
            {
                Socket s = myList.AcceptSocket();
                Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(s);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error..... " + e.StackTrace);
        }
    }

    private static void HandleClient(Object object1)
    {
        Socket socket = object1 as Socket;

        // send public key
        socket.Send(Encoding.ASCII.GetBytes(publicKey));

        // recieve public key client
        byte[] buffer = new byte[2048];
        int bytesRead = socket.Receive(buffer);
        byte[] result = new byte[bytesRead];
        Array.Copy(buffer, result, bytesRead);

        string clientKey = Encoding.ASCII.GetString(result);

        //what kind of client
        buffer = new byte[2048];
        bytesRead = socket.Receive(buffer);
        result = new byte[bytesRead];
        Array.Copy(buffer, result, bytesRead);
        String s = encryption.Decrypt(privateKey, result);

        Console.WriteLine("Received...");

        Console.WriteLine(s);

        if (s.StartsWith("f"))
        {
            HandleBike(socket,s, clientKey);
        }
        else if (s.StartsWith("d"))
        {
            HandleDoctor(socket, clientKey);
        } 
        else
        {
            socket.Send(encryption.Encrypt(clientKey, "Je bent geïntialiseerd als niks"));
        }
        socket.Close();
    }

    public static void HandleBike(Socket socket, string message, string clientKey)
    {
        keyList.Add(socket, clientKey);
        string name = (message.Split('|')[1]);
        bikeID++;
        bikeClients.Add(bikeID, socket);
        bikeClients1.Add(bikeID+": "+name, socket);
        Console.WriteLine("Fiets-client verbonden met ID: " + bikeID);

        if (doctorHandler != null)
        {
            doctorHandler.NotifyDoctorAboutNewBikeClient();
        }


        BikeHandler bikeHandler = new BikeHandler(socket, bikeID, dataStorage, clientKey, privateKey);
        if (doctorHandler != null)
        {
            bikeHandler.SetDoctorHandler(doctorHandler);
        }
        else
        {
            Console.WriteLine("doktor kan niet worden ingesteld.");
            bikeHandlers.Add(bikeHandler);
        }
        bikeHandler.HandleBike();

        bikeClients.Remove(bikeID);
    }

    public static void HandleDoctor(Socket socket, string docterKey)
    {
        doctorHandler = new DoctorHandler(socket,dataStorage, docterKey, privateKey);

        doctorHandler.SendBikeClientList();

        doctorHandler.HandleDoctor();

        if (bikeHandlers.Count > 0) 
        {
            foreach (var bikeHandler in bikeHandlers)
            {
                bikeHandler.SetDoctorHandler(doctorHandler);
            }
        }
        socket.Close();
    }
}


[Serializable]
public class Data
{
    private int id;
    private double speed;
    private double distance;
    private int time;
    private int heartBeat;
    private int resistance;
    private string name;

    public Data(int id, double speed, double distance, int time, int heartBeat, int resistance, string name)
    {
        this.ID = id;
        this.Speed = speed;
        this.Distance = distance;
        this.Time = time;
        this.HeartBeat = heartBeat;
        this.Resistance = resistance;
        this.name = name;
    }

    public int ID { get; set; }
    public double Speed { get; set; }
    public double Distance { get; set; }
    public int Time { get; set; }
    public int HeartBeat { get; set; }
    public int Resistance { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        return $"ID: {ID}, Speed: {Speed}, Distance: {Distance}, Time: {Time}, HeartBeat: {HeartBeat}";
    }
}