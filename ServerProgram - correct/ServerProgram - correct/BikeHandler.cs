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
        private DoctorHandler doctorHandler { get; set; } 
        private Socket socket;
        private int bikeIdb;
        private DataStorage dataStorage;
        private string clientKey;
        private string serverKey;

        public BikeHandler(Socket socket, int bikeId, DataStorage dataStorage, string clientKey, string serverKey)
        {
            this.socket = socket;
            bikeIdb = bikeId;
            Console.WriteLine(bikeId);
            this.dataStorage = dataStorage;
            Console.WriteLine(this.dataStorage.getHighestID());
            this.clientKey = clientKey;
            this.serverKey = serverKey;
        }

        public void SetDoctorHandler(DoctorHandler doctorHandler)
        {
            this.doctorHandler = doctorHandler;
        }


        public void HandleBike()
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            socket.Send(encryption.Encrypt(clientKey, bikeIdb.ToString()));
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[2048];
                    int bytesRead = socket.Receive(buffer);
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

                    dataStorage.AddData(bikeIdb, data);
                    dataStorage.SaveDataToFile();

                    if (doctorHandler != null)
                    {
                        doctorHandler.SendHealthDataToDoctor(jsonData);
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
            socket.Close();
        }        
    }
}