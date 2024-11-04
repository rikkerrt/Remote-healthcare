using ServerProgram___correct;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;

public class server
{
    static int BikeID = 0;
    public static Dictionary<int, Socket> bikeClients = new Dictionary<int, Socket>();
    public static Dictionary<string, Socket> bikeClients1 = new Dictionary<string, Socket>();
    public static DoctorHandler doctorHandler;
    public static List<BikeHandler> bikeHandlers = new List<BikeHandler>();
    private static DataStorage DataStorage;

    public static void Main()
    {
        try
        {
            DataStorage = new DataStorage("data.json");
            BikeID = DataStorage.getHighestID();




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
        byte[] b = new byte[100];
        int k = socket.Receive(b);
        Console.WriteLine("Received...");

        String s = System.Text.Encoding.ASCII.GetString(b);
        Console.WriteLine(s);

        ASCIIEncoding asen = new ASCIIEncoding();

        if (s.StartsWith("f"))
        {
            HandleBike(socket,s);
        }
        else if (s.StartsWith("d"))
        {
            HandleDoctor(socket);
        }
        else
        {
            socket.Send(asen.GetBytes("Je bent geïntialiseerd als niks"));
        }
        socket.Close();
    }

    public static void HandleBike(Socket socket, string message)
    {
        string name = (message.Split('|')[1]);
        BikeID++;
        bikeClients1.Add(BikeID+": "+name, socket);
        Console.WriteLine("Fiets-client verbonden met ID: " + BikeID);

        if (doctorHandler != null)
        {
            doctorHandler.NotifyDoctorAboutNewBikeClient();
        }


        BikeHandler bikeHandler = new BikeHandler(socket, BikeID, DataStorage);
        if (doctorHandler != null)
        {
            bikeHandler.SetDoctorHandler(doctorHandler);
        }

        else
        {
            Console.WriteLine("doktor kan niet wroden ingesteld.");
            bikeHandlers.Add(bikeHandler);
        }
        bikeHandler.HandleBike();

        bikeClients.Remove(BikeID);
    }

    public static void HandleDoctor(Socket socket)
    {
        doctorHandler = new DoctorHandler(socket,DataStorage);

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