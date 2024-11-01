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

        public BikeHandler(Socket socket, int bikeId)
        {
            Socket = socket;
            BikeId = bikeId;
        }

        public void SetDoctorHandler(DoctorHandler doctorHandler)
        {
            DoctorHandler = doctorHandler;
        }


        public void HandleBike()
        {

            ASCIIEncoding asen = new ASCIIEncoding();
            Socket.Send(asen.GetBytes(BikeId.ToString()));
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