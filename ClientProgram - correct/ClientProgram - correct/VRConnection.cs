using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;

namespace ClientProgram___correct {
    internal class VRConnection {
        private static TcpClient client = new TcpClient();
        private string _address = "85.145.62.130";
        private static string encoded = "";
        private static string send;
        private int _port;

        private static NetworkStream networkStream;
        public static byte[] prepend;
        public static byte[] data;

        public VRConnection() {
            client.Connect(_address, 6666);
            networkStream = client.GetStream();
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

        public static string recieveData() {
            byte[] buffer = new byte[1500];
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, networkStream.Read(buffer, 0, buffer.Length)));
            return Encoding.ASCII.GetString(buffer, 0, networkStream.Read(buffer, 0, buffer.Length));
        }
    }
}