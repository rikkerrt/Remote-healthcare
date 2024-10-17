using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerProgram___correct
{
    internal class BikeHandler
    {
        private Socket Socket;
        private int BikeId;

        public BikeHandler(Socket socket, int bikeId)
        {
            Socket = socket;
            BikeId = bikeId;
        }

        public void HandleBike()
        {

            ASCIIEncoding asen = new ASCIIEncoding();
            Socket.Send(asen.GetBytes(Socket.ToString()));
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int receivedBytes = Socket.Receive(buffer);

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
            
            Socket.Close();
        }        


    }
}