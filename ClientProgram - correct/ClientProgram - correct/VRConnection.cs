using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ClientProgram___correct {
    internal class VRConnection {
        private static TcpClient client = new TcpClient();
        private static string _address = "85.145.62.130";
        private static string encoded = "";
        private static string send;
        private static string dataString;
        private static string id;
        private int _port;

        private static NetworkStream networkStream;
        public static byte[] prepend;
        public static byte[] data;

        public static async Task Start() {
            client.Connect(_address, 6666);
            networkStream = client.GetStream();

            string jsonPacket = "{\"id\" : \"session/list\"}";
            data = Encoding.ASCII.GetBytes(jsonPacket);
            prepend = new byte[] { (byte)jsonPacket.Length, 0x00, 0x00, 0x00 };
            SendPacket(prepend, data);

            await readLength();
            id = getID(dataString);

            Console.WriteLine(id);

            createTunnel(id);
            await readLength();
            
        }



        private static async Task readLength() {
            byte[] length = new byte[4];
            int PrependLenght = 0;
            
            while (PrependLenght < 4) { 
                int dataPrependRead = await networkStream.ReadAsync(length, 0, length.Length);
                PrependLenght += dataPrependRead;
            }
          
            int lengthInt = BitConverter.ToInt32(length,0);
            Console.WriteLine(lengthInt);
            byte[] dataBuffer =  new byte[lengthInt];

            int readTotal = 0;
            do
            {
                int read = await networkStream.ReadAsync(dataBuffer, readTotal, dataBuffer.Length - readTotal);
                readTotal += read;
                //Console.WriteLine(readTotal);
            } while (readTotal < dataBuffer.Length);

            dataString = Encoding.UTF8.GetString(dataBuffer,0,readTotal);
            Console.WriteLine(Encoding.UTF8.GetString(dataBuffer,0,readTotal));

            

            PrependLenght = 0;

            /*while (PrependLenght < lengthInt)
            {
                Console.WriteLine(lengthInt);
                int bytesread = await networkStream.ReadAsync(dataBuffer, 0, dataBuffer.Length);
                Console.WriteLine(PrependLenght);
                PrependLenght += bytesread;
            }*/
            
            //collins trying
            //while ((readTotal = await networkStream.ReadAsync(dataBuffer, 0, dataBuffer.Length)) != 0) {
            //    dataString = Encoding.UTF8.GetString(dataBuffer, 0, dataBuffer.Length);
            //}

            //string dataString = Encoding.UTF8.GetString(dataBuffer);
            //Console.WriteLine(dataString);

            //byte[] payload = new byte[lengthInt];
            //int data = await networkStream.ReadAsync(payload, 0, payload.Length);

            //getID(dataString);
            Console.WriteLine("vrconnection done");
        }


        public static void SendPacket(byte[] prepend, byte[] data) {
            byte[] combinedArray = new byte[prepend.Length + data.Length];
            Array.Copy(prepend, 0, combinedArray, 0, prepend.Length);
            Array.Copy(data, 0, combinedArray, prepend.Length, data.Length);
            networkStream.Write(combinedArray, 0, combinedArray.Length);
        }

        public static void createData() {
            string jsonPacket = "{\"id\" : \"session/list\"}";
            data = Encoding.ASCII.GetBytes(jsonPacket);
            prepend = new byte[] {(byte)jsonPacket.Length, 0x00, 0x00, 0x00};
            SendPacket(prepend, data);
        }
        public static void createTunnel(string id)
        {
            string jsonPacket = "{\"id\" : \"tunnel/create\", \"data\" : {\"session\" : \"" + id + "\", \"key\" : \"\"}}";
            Console.WriteLine(jsonPacket);
            data = Encoding.ASCII.GetBytes(jsonPacket);
            prepend = new byte[] { (byte)jsonPacket.Length, 0x00, 0x00, 0x00 };
            
            SendPacket(prepend, data);
           
        }

        
        //Outdated
        /*public static string recieveData() {
            byte[] buffer = new byte[1500];
            Console.WriteLine(networkStream.Read(buffer, 0, buffer.Length));
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, networkStream.Read(buffer, 0, buffer.Length)));
            return Encoding.ASCII.GetString(buffer, 0, networkStream.Read(buffer, 0, buffer.Length));
        }*/

        public static string getID(string data) {
            string idHost = "";
       
            JsonData dataList = JsonConvert.DeserializeObject<JsonData>(data);
            List<Data> dataObject = dataList.data;

            foreach (Data data1 in dataObject)
            {
                if (data1.features.Contains("tunnel")) {
                    ClientInfo info = data1.clientinfo;
                    if (info.host.ToLower() == System.Net.Dns.GetHostName().ToLower())
                    {
                        //Console.WriteLine(data1.id);
                        idHost = data1.id;
                    }
                    else
                    {
                        //Console.WriteLine("Niet de juiste host");
                    }
                }
                
            }
            return idHost;
            //var jsonDocument = JObject.Parse(data);
            //Console.WriteLine(jsonDocument.ToString());

            //JArray dataList = JArray.Parse(jsonDocument["data"].ToString());
            //Console.WriteLine(dataList.ToString());
            //for(int i = 0; i < dataList.Count; i++)
            //{
            //    Console.WriteLine(dataList[i].ToString());
            //}


            //Console.WriteLine(jsonDocument["data"].ToString());
            //Console.WriteLine(jsonDocument["id"].ToString());

            /*if (jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataElement) && 
                dataElement.ValueKind == JsonValueKind.Array && 
                dataElement.GetArrayLength() >0) {
                return dataElement[0].GetProperty("id").GetString();
            }

            if(jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataObject) && dataObject.ValueKind == JsonValueKind.Object) {
                JsonNode jsonnode = System.Text.Json.JsonSerializer.SerializeToNode(dataObject);
                return jsonnode["id"].GetValue<string>();
            }*/

        }
    }

    internal class JsonData {
        public List<Data> data { get; set; }
        public string id { get; set; }
        public JsonData(string id, List<Data> data) {
            this.data = data;
            this.id = id;
        }
    }

    internal class Data {
        public string id { get; set; }
        public  string beginTime { get; set; }
        public  string lastPing { get; set; }
        public  List<Fps> fps { get; set; }
        public  List<string> features { get; set; }
        public  ClientInfo clientinfo{ get; set; }

        public Data(string id, string beginTime, string lastPing, List<Fps> fps, List<string> features, ClientInfo client) {
            this.id = id;
            this.beginTime = beginTime;
            this.lastPing = lastPing;
            this.fps = fps;
            this.features = features;
            this.clientinfo = client;
        }
    }

    internal class ClientInfo {
        public string host { get; set; }
        public string user { get; set; }
        public string file { get; set; }
        public string renderer { get; set; }

        public ClientInfo(string host, string user, string file, string renderer) {
            this.host = host;
            this.user = user;
            this.file = file;
            this.renderer = renderer;
        }
    }

    internal class Fps {
        public long time { get; set; }
        public double fps { get; set; }

        public Fps(long time, double fps) {
            this.fps = fps;
            this.time = time;
        }    
    }
}