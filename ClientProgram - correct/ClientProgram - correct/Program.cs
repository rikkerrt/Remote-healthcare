using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Data;
using ClientProgram___correct;
using ClientProgram;
using ClientProgram___correct;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Web;

namespace FietsDemo
{
    public class Program
    {
        static int ID;
        static IBike sim;
        static StreamWriter writer;
        static Stream stm;
        //static DataProtocol dataProtocol;
        static bool sendData;
        static string UserName;
        static string serverKey;
        static string privateKey;
        static string publicKey;

        public static void Main()
        {

            Console.Write("Wat is je naam? ");
            UserName = Console.ReadLine();
            Console.WriteLine("je username ="+UserName);

            //IBike sim = new Simulation(3);
            //while (true)
            //{
            //    string input = sim.getSpeed();
            //    Console.WriteLine(Calculations.GetSpeed(input.Substring(2), input.Substring(0, 2)));
            //    Thread.Sleep(500);
            //    //Console.WriteLine();   
            //}
            sim = new Simulation(3);
            //dataProtocol = new  DataProtocol(sim); 
            sendData = false;


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

                    string input = sim.getSpeed();
                    data.Speed = Calculations.GetSpeed(input.Substring(2), input.Substring(0, 2));
                    Console.WriteLine(data.Speed);
                    data.Distance = Calculations.GetDistance(sim.getDistance());
                    Console.WriteLine(data.Distance);
                    data.Time = Calculations.GetDuration(sim.getDuration());
                    Console.WriteLine(data.Time);
                    data.HeartBeat = Calculations.getHeartBeat(sim.getHeartBeat());
                    Console.WriteLine(data.HeartBeat);
                    data.Name = UserName;
                    data.Resistance = sim.getResistance();
                    

                    string jsonData = JsonConvert.SerializeObject(data);
                    byte[] bytes = encryption.Encrypt(serverKey, jsonData);
                    stm.Write(bytes, 0, bytes.Length);
                    Console.WriteLine("Data object in JSON sent.");

                    System.Threading.Thread.Sleep(10000);

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
                        }


                        Console.WriteLine("ik ben nu: "+send);
                    }

                    else if (s.StartsWith("MESSAGE"))
                    {
                        string message = (s.Split('|')[1]);
                        Console.WriteLine(message);
                    }

                    else if (s.StartsWith("RESISTANCE"))
                    {
                        int Resistance = int.Parse(s.Split('|')[1]);
                        sim.sendResistance(Resistance);
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
