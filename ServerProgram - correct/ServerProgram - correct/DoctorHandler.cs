using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerProgram___correct
{
    internal class DoctorHandler
    {

        private Socket Socket;

        public DoctorHandler(Socket socket)
        {
            Socket = socket;
        }

        public void HandleDoctor()
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            Socket.Send(asen.GetBytes(Socket.ToString()));

            while (true)
            {
                try
                {

                    byte[] b = new byte[1024];
                    int k = Socket.Receive(b);
                    Console.WriteLine("Recieved...");

                    String s = System.Text.Encoding.ASCII.GetString(b,0,k);
                    Console.WriteLine(s);


                    if (s.StartsWith("WW"))
                    {
                        Socket.Send(asen.GetBytes("true"));


                    }

                    if (s.StartsWith("getBikes"))
                    {
                        SendBikeClientList();
                    }

                    if (s.StartsWith("SendMessageToClient"))
                    {
                        string trimmedMessage = s.Substring("SendMessageToClient".Length).Trim();

                        int firstSpaceIndex = trimmedMessage.IndexOf(' ');

                        if (firstSpaceIndex > -1)
                        {
                            string idPart = trimmedMessage.Substring(0, firstSpaceIndex);
                            int id;
                            if (int.TryParse(idPart, out id)) 
                            {
                                string message = trimmedMessage.Substring(firstSpaceIndex + 1);

                                SendMessageToClient(id, message);
                                Console.WriteLine(message + "is gestuurd naar" + id);
                            }
                            else
                            {
                                Console.WriteLine("Ongeldig ID-formaat.");
                            }
                        }
                    }




                    else
                    {
                        Socket.Send(asen.GetBytes("Je bent geïntialiseerd als niks"));

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

        private void SendBikeClientList()
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            List<Data> bikeData = new List<Data>();

            foreach (var bikeClient in server.bikeClients)
            {
                int bikeId = bikeClient.Key;
                bikeData.Add(new Data(bikeId, 0, 0, 0, 0, 0));
       
            }

            string bikeDataString = JsonConvert.SerializeObject(bikeData);

            byte[] messageBytes = asen.GetBytes(bikeDataString);
            Console.WriteLine(messageBytes);
            Socket.Send(messageBytes);
            Console.WriteLine("Bytes Sent: " + BitConverter.ToString(messageBytes));

        }

        private void SendMessageToClient(int ID, string Message)
        {
            foreach (var bikeClient in server.bikeClients)
            {
                int bikeId = bikeClient.Key;
                if (ID.Equals(bikeId)) 
                {
                    Socket clientSocket = bikeClient.Value;

                    if (clientSocket.Connected)
                    {
                        byte[] messageBytes = Encoding.ASCII.GetBytes(Message);

                        clientSocket.Send(messageBytes);
                    }
                    else
                    {
                        Console.WriteLine("Client met ID " + ID + " is niet verbonden.");
                    }

                    break; 
                }
            }
        }

    }
}
