using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using ClientProgram___correct;
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
        static string serverKey;
        static string privateKey;
        static string publicKey;

        public static async Task Main()
        {
            Console.Write("Wat is je naam? ");
            UserName = Console.ReadLine();
            Console.WriteLine("je username ="+UserName);

            connection = new Connection();
            sendData = false;
            await VRConnection.Start();

            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");
                tcpclnt.Connect(IPAddress.Loopback, 8001);
                Console.WriteLine("Connected");
                stm = tcpclnt.GetStream();
                (publicKey, privateKey) = encryption.GenerateKeys();

                // recieve public key
                byte[] buffer = new byte[2048];
                int bytesRead = stm.Read(buffer, 0, buffer.Length);
                byte[] result = new byte[bytesRead];
                Array.Copy(buffer, result, bytesRead);
                serverKey = Encoding.UTF8.GetString(result);

                // send public key
                stm.Write(Encoding.ASCII.GetBytes(publicKey), 0, Encoding.ASCII.GetBytes(publicKey).Length);

                // send client id
                byte[] encryptedMessage = encryption.Encrypt(serverKey, "f|"+ UserName);
                stm.Write(encryptedMessage, 0, encryptedMessage.Length);

                buffer = new byte[2048];    
                bytesRead = stm.Read(buffer, 0, buffer.Length);
                result = new byte[bytesRead];
                Array.Copy(buffer, result, bytesRead);
                ID = Int32.Parse(encryption.Decrypt(privateKey, result));

                writer = new StreamWriter(stm);
                writer.AutoFlush = true;

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
                    Console.WriteLine(UserName);
                  
                    Data data = new Data(ID, 15, 16, 10, 76, 8,UserName);

                    string input = connection.getSpeed();
                    data.Speed = Calculations.GetSpeed(input.Substring(2), input.Substring(0, 2));
                    data.Name = UserName;
                    data.Resistance = connection.getResistance();
                    //Console.WriteLine(data.Speed);
                    data.Distance = Calculations.GetDistance(connection.getDistance());
                    //Console.WriteLine(data.Distance);
                    data.Time = Calculations.GetDuration(connection.getDuration());
                    //Console.WriteLine(data.Time);
                    data.HeartBeat = Calculations.getHeartBeat(connection.getHeartBeat());
                    //Console.WriteLine("Heartbeat: " + data.HeartBeat);

                    string jsonData = JsonConvert.SerializeObject(data);
                    byte[] bytes = encryption.Encrypt(serverKey, jsonData);
                    stm.Write(bytes, 0, bytes.Length);

                    VRConnection.setSpeed(data.Speed);

                    Thread.Sleep(500);
                    }
                }
            } 
            catch (Exception e) 
            {
                Console.WriteLine(e);
            }
        }

        public static void RecieveData()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[2048];
                    int bytesRead = stm.Read(buffer, 0, buffer.Length);
                    byte[] result = new byte[bytesRead];
                    Array.Copy(buffer, result, bytesRead);

                    string s = encryption.Decrypt(privateKey, result);
                    Console.WriteLine(s);

                    if (s.StartsWith("sendData"))
                    {
                        string send = (s.Split('|')[1]);
                        if (s.Contains("true"))
                        {
                            sendData = true;
                        }

                        if (s.Contains("false"))
                        {
                            sendData = false;
                            VRConnection.setEmergencyStop(!sendData);
                        }
                    }
                    else if (s.StartsWith("MESSAGE"))
                    {
                        string message = (s.Split('|')[1]);
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
                Console.WriteLine(e);
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
