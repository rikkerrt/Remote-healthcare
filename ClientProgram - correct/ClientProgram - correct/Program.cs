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
using System.Runtime.CompilerServices;

namespace FietsDemo
{
    public class Program
    {
        static int ID;

        public static async Task Main()
        {
            await VRConnection.Start();
            

            IBike sim = new Simulation(3);
            while (true)
            {
                string input = sim.getSpeed();
                Console.WriteLine(Calculations.GetSpeed(input.Substring(2), input.Substring(0, 2)));
                Thread.Sleep(500);
                //Console.WriteLine();   
            }


            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");
                tcpclnt.Connect(IPAddress.Loopback, 8001);
                Console.WriteLine("Connected");

                Stream stm = tcpclnt.GetStream();
                ASCIIEncoding asen = new ASCIIEncoding();

                stm.Write(asen.GetBytes("f"), 0, asen.GetBytes("f").Length);
                byte[] buffer = new byte[100];
                int bytesRead = stm.Read(buffer, 0, buffer.Length);
                String Respons = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                int IntResponse = Convert.ToInt32(Respons);
                ID = IntResponse;

                StreamWriter writer = new StreamWriter(stm);
                writer.AutoFlush = true;

                //IBike sim = new Simulation(1);

                while (true)
                {
                    Data data = new Data(ID, 15, 16, 10, 76);

                    string input = sim.getSpeed();
                    data.Speed = Calculations.GetSpeed(input.Substring(2), input.Substring(0, 2));
                    data.Distance = Calculations.GetDistance(sim.getDistance());
                    data.Time = Calculations.GetDuration(sim.getDuration());
                    //data.HeartBeat = Calculations.get

                    string jsonData = JsonConvert.SerializeObject(data);
                    writer.WriteLine(jsonData);
                    Console.WriteLine("Data object in JSON sent.");

                    System.Threading.Thread.Sleep(10000);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
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
    }
}
