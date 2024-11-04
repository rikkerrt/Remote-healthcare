using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerProgram___correct
{
    public class DoctorHandler
    {

        private Socket Socket;
        private Dictionary<int, ClientSession> clientSessions = new Dictionary<int, ClientSession>();
        private string RightPassword;
        private DataStorage DataStorage;


        public DoctorHandler(Socket socket, DataStorage dataStorage)
        {
            Socket = socket;
            RightPassword = "pieter";
            DataStorage = dataStorage;  
        }

        public void HandleDoctor()
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            Socket.Send(asen.GetBytes(Socket.ToString()));
            SendBikeClientList();
            Socket.Send(asen.GetBytes("WW|true"));


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
                        string PassWord = (s.Split('|')[1]);

                        if (PassWord.Equals(RightPassword))
                        {
                            byte[] messageBytes = asen.GetBytes("WW|true");
                            Socket.Send(messageBytes);
                        }
                        else
                        {
                            byte[] messageBytes = asen.GetBytes("WW|false");
                            Socket.Send(messageBytes);
                        }
                    }

                    if (s.StartsWith("GETHISTORY"))
                    {
                        Dictionary<int, List<Data>> History = DataStorage.getAllData();
                        string bikeDataString = "HISTORY|" + JsonConvert.SerializeObject(History);
                        byte[] messageBytes = asen.GetBytes(bikeDataString);
                        Socket.Send(messageBytes);
                    }


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
                    
                    if (s.StartsWith("STOP"))
                    {
                        int id = int.Parse(s.Split(' ')[1]);
                        Console.WriteLine("Stop sessie voor client met ID: " + id);

                        SendMessageToClient(id, "sendData| false");
                    }
                    
                    if (s.StartsWith("RESISTANCE"))
                    {
                        int id = int.Parse(s.Split(' ')[1]);
                        int Resistance = int.Parse(s.Split('|')[2]);
                        SendMessageToClient(id, "RESISTANCE"+Resistance);
                        Console.WriteLine("Deze resistance" + Resistance + "word ingesteld bij: " + id);

                    }

                    if (s.StartsWith("MESSAGE|"))
                    {
                        //string trimmedMessage = s.Substring("MESSAGE|".Length).Trim();

                        //int firstSpaceIndex = trimmedMessage.IndexOf('|');

                        //if (firstSpaceIndex > -1)
                        //{
                        //    string idPart = trimmedMessage.Substring(0, firstSpaceIndex);
                        //    int id;
                        //    if (int.TryParse(idPart, out id)) 
                        //    {
                        //        string message = trimmedMessage.Substring(firstSpaceIndex + 1);

                        //        SendMessageToClient(id, "MESSAGE|"+message);
                        //        Console.WriteLine(message + "is gestuurd naar" + id);
                        //    }
                        //    else
                        //    {
                        //        Console.WriteLine("Ongeldig ID-formaat.");
                        //    }
                        //}

                        int id = int.Parse(s.Split('|')[1]);
                        string Message ="MESSAGE|"+s.Split('|')[2];
                        SendMessageToClient(id,Message);
                        Console.WriteLine("Deze message wordt verstuurd"+Message+"Naar: "+id);

                    }


                    if (s.StartsWith("SENDMESSAGETO"))
                    {

                        int id = int.Parse(s.Split('|')[1]);
                        string Message = "MESSAGE|" + s.Split('|')[2];
                        SendMessageToClient(id, Message);
                        Console.WriteLine("Deze message wordt verstuurd" + Message + "Naar: " + id);

                    }


                    else
                    {

                        Console.WriteLine("Dit bericht heb ik binnen"+s);
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
            List<string> bikeData1 = new List<string>();

            foreach (var bikeClient in server.bikeClients1)
            {
                string bikeId = bikeClient.Key;
                bikeData1.Add(bikeId);
            }


            ASCIIEncoding asen = new ASCIIEncoding();

            string bikeDataString = "bikeclients|"+JsonConvert.SerializeObject(bikeData1);

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
