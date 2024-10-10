using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;



public class server
{
    static int i = 0;
    public static void Main()
    {
        try
        {
            Data data = new Data(10, 1.1, 2.2, 10, 3);
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
        socket.Send(asen.GetBytes(i + ""));

        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int receivedBytes = socket.Receive(buffer);

                if (receivedBytes == 0)
                {
                    Console.WriteLine("De client is gedisconnect.");
                    break;
                }

                string jsonData = Encoding.ASCII.GetString(buffer, 0, receivedBytes).Trim();
                Console.WriteLine("Ontvangen Data: " + jsonData);

                Data dataObject = JsonConvert.DeserializeObject<Data>(jsonData);

                if (dataObject != null)
                {
                    Console.WriteLine("Ontvangen dataobject:");
                    Console.WriteLine(dataObject.ToString());
                }
                else
                {
                    Console.WriteLine("Error met deserializen.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error met ontvangen van data: " + e.Message);
                break;
            }
        }
        socket.Close();



    }

    public static void HandleDoctor(Socket socket)
    {

        ASCIIEncoding asen = new ASCIIEncoding();
        socket.Send(asen.GetBytes(i + ""));

        while (true)
        {
            try
            {

                byte[] b = new byte[100];
                int k = socket.Receive(b);
                Console.WriteLine("Recieved...");

                String s = System.Text.Encoding.ASCII.GetString(b);
                Console.WriteLine(s);


                if (s.StartsWith("WW"))
                {
                    socket.Send(asen.GetBytes("true"));


                }

    

                else
                {
                    socket.Send(asen.GetBytes("Je bent geïntialiseerd als niks"));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error met ontvangen van data: " + e.Message);
                break;
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
    private double time;
    private int heartBeat;

    public Data(int id, double speed, double distance, int time, int heartBeat)
    {
        this.ID = id;
        this.Speed = speed;
        this.Distance = distance;
        this.Time = time;
        this.HeartBeat = heartBeat;
    }

    public int ID { get; set; }
    public double Speed { get; set; }
    public double Distance { get; set; }
    public int Time { get; set; }
    public int HeartBeat { get; set; }

    public override string ToString()
    {
        return $"ID: {ID}, Speed: {Speed}, Distance: {Distance}, Time: {Time}, HeartBeat: {HeartBeat}";
    }
}
