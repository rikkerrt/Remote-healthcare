using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientProgram___correct {
    internal class VRConnection {
        private static TcpClient client = new TcpClient();
        private static string _address = "85.145.62.130";
        private static string encoded = "";
        private static string send;
        private int _port;

        private static NetworkStream networkStream;
        public static byte[] prepend;
        public static byte[] data;

        public static async Task Start() {
            client.Connect(_address, 6666);
            networkStream = client.GetStream();

            string jsonPacket = "{\"id\" : \"session/list\"}";
            data = Encoding.ASCII.GetBytes(jsonPacket);
            prepend = new byte[] { (byte)jsonPacket.Length, 0x00, 0x00, 0x00};
            SendPacket(prepend, data);

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
            byte[] dataBuffer =  new byte[lengthInt];
            Console.WriteLine(lengthInt);
            PrependLenght = 0;
            while (PrependLenght < lengthInt) {
                int bytesread = await networkStream.ReadAsync(dataBuffer, 0, lengthInt - PrependLenght);
                PrependLenght += bytesread;
                Console.WriteLine("Shitstuck");
            }

            string dataString = Encoding.UTF8.GetString(dataBuffer);
            Console.WriteLine(dataString);
             
            //byte[] payload = new byte[lengthInt];
            //int data = await networkStream.ReadAsync(payload, 0, payload.Length);


            //getID(dataString);
            Console.WriteLine("vrconnection done");

        }


        public VRConnection() {
           
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
        public static void createTunnel()
        {
            string jsonPacket = "{\"id\" : \"tunnel/create\", \"data\" : {\"session\" : \"0601d4a1-7070-41da-8f2e-3ad1dd4f73fa\", \"key\" : \"\"}}";
            data = Encoding.ASCII.GetBytes(jsonPacket);
            prepend = new byte[] { (byte)jsonPacket.Length, 0x00, 0x00, 0x00 };
            SendPacket(prepend, data);
        }

        
        //Dit is fucking retarted 
        public static string recieveData() {
            byte[] buffer = new byte[1500];
            Console.WriteLine(networkStream.Read(buffer, 0, buffer.Length));
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, networkStream.Read(buffer, 0, buffer.Length)));
            return Encoding.ASCII.GetString(buffer, 0, networkStream.Read(buffer, 0, buffer.Length));
        }

        public static string getID(string data) {
                var jsonDocument = JsonDocument.Parse(data);

                if(jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataElement) && 
                    dataElement.ValueKind == JsonValueKind.Array && 
                    dataElement.GetArrayLength() >0) {
                return dataElement[0].GetProperty("id").GetString();
            }

            return "";
        }
    }
}