using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
using ServerProgram___correct;



public class server
{
    static int i = 0;
    public static Dictionary<int, Socket> bikeClients = new Dictionary<int, Socket>();
    public static void Main()
    {
        try
        {
            Data data = new Data(10, 1.1, 2.2, 10, 3,8);
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
        Console.WriteLine("Recieved...");

        String s = System.Text.Encoding.ASCII.GetString(b);
        Console.WriteLine(s);

        ASCIIEncoding asen = new ASCIIEncoding();

        if (s.StartsWith("f"))
        {
            //Thread.Sleep(5000);
            //Console.WriteLine("i have given id");
            HandleBike(socket);

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

    public static void HandleBike(Socket socket)
    {
        i++;
        ASCIIEncoding asen = new ASCIIEncoding();
        bikeClients.Add(i, socket);
        Console.WriteLine("Fiets-client verbonden met ID: " + i);

        BikeHandler bikeHandler = new BikeHandler(socket, i);
        bikeHandler.HandleBike();
        bikeClients.Remove(i);

    }

    public static void HandleDoctor(Socket socket)
    {

        //ASCIIEncoding asen = new ASCIIEncoding();
        //socket.Send(asen.GetBytes(i + ""));

        DoctorHandler DoctorHandler = new DoctorHandler(socket);
        DoctorHandler.HandleDoctor();


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

    public Data(int id, double speed, double distance, int time, int heartBeat, int resistance)
    {
        this.ID = id;
        this.Speed = speed;
        this.Distance = distance;
        this.Time = time;
        this.HeartBeat = heartBeat;
        this.Resistance = resistance;
    }

    public int ID { get; set; }
    public double Speed { get; set; }
    public double Distance { get; set; }
    public int Time { get; set; }
    public int HeartBeat { get; set; }
    public int Resistance { get; set; }

    public override string ToString()
    {
        return $"ID: {ID}, Speed: {Speed}, Distance: {Distance}, Time: {Time}, HeartBeat: {HeartBeat}";
    }
}