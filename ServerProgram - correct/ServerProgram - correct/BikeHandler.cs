using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerProgram___correct
{
   
    public class BikeHandler
    {
        private DoctorHandler DoctorHandler { get; set; } 
        private Socket Socket;
        private int BikeId;
        private DataStorage DataStorage;
        private string clientKey;
        private string serverKey;


        public BikeHandler(Socket socket, int bikeId, DataStorage dataStorage, string clientKey, string serverKey)
        {
            Socket = socket;
            BikeId = bikeId;
            Console.WriteLine(bikeId);
            DataStorage = dataStorage;
            Console.WriteLine(DataStorage.getHighestID());
            this.clientKey = clientKey;
            this.serverKey = serverKey;
        }

        public void SetDoctorHandler(DoctorHandler doctorHandler)
        {
            DoctorHandler = doctorHandler;
        }


        public void HandleBike()
        {

            ASCIIEncoding asen = new ASCIIEncoding();
            Socket.Send(encryption.Encrypt(clientKey, BikeId.ToString()));
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[2048];
                    int bytesRead = Socket.Receive(buffer);
                    byte[] result = new byte[bytesRead];
                    Array.Copy(buffer, result, bytesRead);

                    if (bytesRead == 0)
                    {
                        Console.WriteLine("De client is gedisconnect.");
                        break;
                    }


                    string jsonData = encryption.Decrypt(serverKey, result);
                    Console.WriteLine(jsonData);

                    Data data = JsonConvert.DeserializeObject<Data>(jsonData);

                    DataStorage.AddData(BikeId, data);
                    DataStorage.SaveDataToFile();

                    if (DoctorHandler != null)
                    {
                        DoctorHandler.SendHealthDataToDoctor(jsonData);
                    }
                    else
                    {
                        Console.WriteLine("Error: DoctorHandler is niet ingesteld.");
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