using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerProgram___correct
{
    public class DoctorHandler
    {

        private Socket Socket;
        private Dictionary<int, ClientSession> clientSessions = new Dictionary<int, ClientSession>();


        public DoctorHandler(Socket socket)
        {
            Socket = socket;
        }

        public void HandleDoctor()
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            Socket.Send(asen.GetBytes(Socket.ToString()));
            SendBikeClientList();

            while (true)
            {
                try
                {

                    byte[] b = new byte[1024];
                    int k = Socket.Receive(b);
                    Console.WriteLine("Recieved...");

                    String s = System.Text.Encoding.ASCII.GetString(b,0,k);
                    Console.WriteLine(s);


                    if (s.StartsWith("DATA"))
                    {

                        Console.WriteLine("IK HEB DATA ONTVANGEN");


                    }

                    if (s.StartsWith("getBikes"))
                    {
                        SendBikeClientList();
                        NotifyDoctorAboutNewBikeClient();
                    }
                    
                    if (s.StartsWith("Start"))
                    {

                        int id = int.Parse(s.Split(' ')[1]);
                        Console.WriteLine("Start sessie voor client met ID: " + id);

                        SendMessageToClient(id, "sendData| true");



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

        public void SendBikeClientList()
        {
            List<int> bikeData = new List<int>();

            foreach (var bikeClient in server.bikeClients)
            {
                int bikeId = bikeClient.Key;
                bikeData.Add(bikeId);
            }


            ASCIIEncoding asen = new ASCIIEncoding();

            string bikeDataString = "bikeclients|"+JsonConvert.SerializeObject(bikeData);

            byte[] messageBytes = asen.GetBytes(bikeDataString);
            Socket.Send(messageBytes); 
            Console.WriteLine("Fietsclients naar dokter verzonden: " + BitConverter.ToString(messageBytes));
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

        public void SendHealthDataToDoctor(String data)
        {

            Console.WriteLine(data);
            ASCIIEncoding asen = new ASCIIEncoding();

            string bikeDataString = "DATA|" + data;

            byte[] messageBytes = asen.GetBytes(bikeDataString);
            Socket.Send(messageBytes);
        }

        public void NotifyDoctorAboutNewBikeClient()
        {
            SendBikeClientList();
        }

    }

    public class ClientSession
    {
        public bool IsSessionActive { get; set; }
        public List<string> HealthData { get; set; }
        public System.Timers.Timer DataTimer { get; set; }

        public ClientSession()
        {
            IsSessionActive = false;
            HealthData = new List<string>();
        }
    }
}
