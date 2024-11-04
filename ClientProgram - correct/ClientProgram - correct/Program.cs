using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using ClientProgram___correct;
using ClientProgram;
using System.Threading.Tasks;

namespace FietsDemo
{
    public class Program
    {
        static int ID;
        static IBike connection;
        static StreamWriter writer;
        static Stream stm;
        //static DataProtocol dataProtocol;
        static bool sendData;
        static string UserName;

        public static async Task Main()
        {

            Console.Write("Wat is je naam? ");
            UserName = Console.ReadLine();

            //IBike sim = new Simulation(3);
            //while (true)
            //{
            //    string input = sim.getSpeed();
            //    Console.WriteLine(Calculations.GetSpeed(input.Substring(2), input.Substring(0, 2)));
            //    Thread.Sleep(500);
            //    //Console.WriteLine();   
            //}
            connection = new Connection();
            //dataProtocol = new  DataProtocol(sim); 
            sendData = true;
            await VRConnection.Start();

            //while (true)
            //{
            //    string input = sim.getDistance();
            //    //Console.WriteLine(Calculations.GetSpeed(input.Substring(2), input.Substring(0, 2)));
            //    //Console.WriteLine(input);
            //    Console.WriteLine(Calculations.GetDistance(input));
            //    Thread.Sleep(500);
            //    //Console.WriteLine();   
            //}


            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");
                tcpclnt.Connect(IPAddress.Loopback, 8001);
                Console.WriteLine("Connected");

                stm = tcpclnt.GetStream();
                ASCIIEncoding asen = new ASCIIEncoding();

                stm.Write(asen.GetBytes("f|"+UserName), 0, asen.GetBytes("f|"+UserName).Length);
                byte[] buffer = new byte[100];
                //int bytesRead = stm.Read(buffer, 0, buffer.Length);
                //String Respons = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                //int IntResponse = Convert.ToInt32(Respons);
                //ID = IntResponse;
                int bytesRead = stm.Read(buffer, 0, buffer.Length);
                String Respons = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                //int IntResponse = Convert.ToInt32(Respons);
                ID = Int32.Parse(Respons);
                //Console.WriteLine("i got id");

                writer = new StreamWriter(stm);
                writer.AutoFlush = true;

                //IBike sim = new Simulation(1);

                Thread dataReciever = new Thread(new ThreadStart(RecieveData));
                dataReciever.Start();
                SendData();
               
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        public static void SendData()
        {
            try
            {
                while (true)
                {
                    if (sendData)
                    {
                  
                    Data data = new Data(ID, 15, 16, 10, 76, 8,UserName);

                    string input = connection.getSpeed();
                    data.Speed = Calculations.GetSpeed(input.Substring(2), input.Substring(0, 2));
                    //Console.WriteLine(data.Speed);
                    data.Distance = Calculations.GetDistance(connection.getDistance());
                    //Console.WriteLine(data.Distance);
                    data.Time = Calculations.GetDuration(connection.getDuration());
                    //Console.WriteLine(data.Time);
                    data.HeartBeat = Calculations.getHeartBeat(connection.getHeartBeat());
                    //Console.WriteLine(data.HeartBeat);

                    string jsonData = JsonConvert.SerializeObject(data);
                    writer.WriteLine(jsonData);
                    //Console.WriteLine("Data object in JSON sent.");

                    VRConnection.setSpeed(data.Speed);

                    System.Threading.Thread.Sleep(500);

                    }

                }
            } 
            catch (Exception e) 
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void RecieveData()
        {
            try
            {
                while (true)
                {
                    byte[] b = new byte[100];
                    int k = stm.Read(b, 0, b.Length);
                    string s = System.Text.Encoding.ASCII.GetString(b);
                    if (s.StartsWith("sendData"))
                    {
                        string send = (s.Split(' ')[1]);
                        if (s.Contains("true"))
                        {
                            sendData = true;
                        }

                        if (s.Contains("false"))
                        {
                            sendData = false;
                        }


                        //Console.WriteLine("ik ben nu: "+send);
                    }

                    else if (s.StartsWith("MESSAGE"))
                    {
                        string message = (s.Split('|')[1]);
                        //Console.WriteLine(message);
                    }

                    else if (s.StartsWith("RESISTANCE"))
                    {
                        int Resistance = int.Parse(s.Split('|')[1]);
                        connection.sendResistance(Resistance);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
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
    }
}
