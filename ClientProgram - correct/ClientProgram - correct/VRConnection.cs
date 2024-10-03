using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace ClientProgram___correct {
    internal class VRConnection {
        private static TcpClient client = new TcpClient();
        private string _address = "85.145.62.130";
        private int _port;

        private static NetworkStream networkStream;
        public static byte[] prepend;
        public static byte[] data;


        public VRConnection() {
            client.Connect("85.145.62.130", 6666);
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
            string jsonPacket = "{\"id\" : \"tunnel/create\"}";
            data = Encoding.ASCII.GetBytes(jsonPacket);
            prepend = new byte[] { (byte)jsonPacket.Length, 0x00, 0x00, 0x00 };
            SendPacket(prepend, data);
        }

        public static string recieveData() {
            byte[] buffer = new byte[1000];
            int recieved = networkStream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, recieved);
        }
    }
}