using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace DoctorClient
{
    public class Connection
    {
        private Socket ConnectionSocket;
        private Stream Stream;
        private ASCIIEncoding asen;
        private bool isListening;
        private bool PasswordCheck;
        private string publicKey;
        private string privateKey;
        private string serverKey;

        public void Connect()
        {
            TcpClient tcpclnt = new TcpClient();
            tcpclnt.Connect(IPAddress.Loopback, 8001);
            PasswordCheck = false;
            Stream = tcpclnt.GetStream();

            (publicKey, privateKey) = encryption.GenerateKeys();

            //recieve serverKey
            byte[] buffer = new byte[2048];
            int bytesRead = Stream.Read(buffer, 0, buffer.Length);
            byte[] result = new byte[bytesRead];
            Array.Copy(buffer, result, bytesRead);
            serverKey = Encoding.UTF8.GetString(result);

            //send public key
            Stream.Write(Encoding.ASCII.GetBytes(publicKey), 0, Encoding.ASCII.GetBytes(publicKey).Length);

            asen = new ASCIIEncoding();


            isListening = true;
            StartListening();
        }

        public void Write(string s)
        {
            byte[] data = encryption.Encrypt(serverKey, s);
            Stream.Write(data, 0, data.Length);
        }

        //public string Read()
        //{
        //    byte[] buffer = new byte[4096];  
        //    StringBuilder responseBuilder = new StringBuilder();  
        //    int bytesRead;

        //    while ((bytesRead = Stream.Read(buffer, 0, buffer.Length)) > 0)
        //    {
        //        responseBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));

        //        if (bytesRead < buffer.Length)
        //        {
        //            break;
        //        }
        //    }

        //    string response = responseBuilder.ToString();
        //    Console.WriteLine("Ontvangen data: " + response);  

        //    return response;
        //}

        //private static string ASCIIToJson(string s)
        //{
        //    string[] hexValues = s.Split('-');
        //    byte[] bytes = new byte[hexValues.Length];

        //    for (int i = 0; i < hexValues.Length; i++)
        //    {
        //        try
        //        {
        //            bytes[i] = Convert.ToByte(hexValues[i], 16);
        //        }
        //        catch (FormatException)
        //        {
        //            Console.WriteLine($"Ongeldige hexwaarde: {hexValues[i]}");
        //        }
        //    }

        //    string jsonString = Encoding.ASCII.GetString(bytes);
        //    return jsonString;
        //}

        public bool CheckPassword()
        {
            return PasswordCheck;
        }

        private async void StartListening()
        {
            byte[] buffer = new byte[10024];
            

            while (isListening)
            {
                try
                {
                    int bytesRead = await Stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        byte[] result = new byte[bytesRead];
                        Array.Copy(buffer, result, bytesRead);
                        string normalMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        string message = encryption.Decrypt(privateKey, result);

                        if (normalMessage.StartsWith("HISTORY"))
                        {
                            string[] parts = message.Split("|");

                            Dictionary<int, List<Data>> History = JsonConvert.DeserializeObject<Dictionary<int, List<Data>>>(parts[1]);
                            MainWindow.UpdateHistory(History);
                        }
                        else if (message.StartsWith("message"))
                        {
                        }
                        else if (message.StartsWith("bikeclients"))
                        {
                            string[] parts = message.Split("|");

                            List<string> bikeClients = JsonConvert.DeserializeObject<List<string>>(parts[1]);

                            MainWindow.UpdateBikeClient(bikeClients);
                        }
                        else if (message.StartsWith("DATA"))
                        {
                            string[] parts = message.Split("|");

                            Write("ik heb data ontvangen...///" + parts[1]);

                            Data data1 = JsonConvert.DeserializeObject<Data>(parts[1]);
                            string message1 = "{\"ID\":1,\"Speed\":32.4,\"Distance\":210.0,\"Time\":1,\"HeartBeat\":8,\"Resistance\":8}";

                            MainWindow.UpdateBikeData(data1);

                        }

                        else if (message.StartsWith("WW"))
                        {

                            string PassWord = message.Split('|')[1];

                            if (message.Contains("true"))
                            {
                                PasswordCheck = true;
                                Write("het is goed");
                            }
                            else
                            {
                                PasswordCheck = false;
                                Write("het is fout");
                            }
                        }

                        else if (message.StartsWith("HISTORY"))
                        {
                            string[] parts = message.Split("|");

                            Dictionary<int, List<Data>> History = JsonConvert.DeserializeObject<Dictionary<int, List<Data>>>(parts[1]);
                            MainWindow.UpdateHistory(History);

                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during listening: {ex.Message}");
                    break;
                }
            }
        }


        //public string Read()
        //    {
        //        byte[] buffer = new byte[4096];  
        //        StringBuilder responseBuilder = new StringBuilder();  
        //        int bytesRead;

        //        while ((bytesRead = Stream.Read(buffer, 0, buffer.Length)) > 0)
        //        {
        //            responseBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));

        //            if (bytesRead < buffer.Length)
        //            {
        //                break;
        //            }
        //        }

        //        string response = responseBuilder.ToString();
        //        Console.WriteLine("Ontvangen data: " + response);  

        //        return response;
        //    }



        //public void RequestBikeClients()
        //{
        //    Write("getBikes");

        //    string response = Read();
        //    string response2 = Read();
        //    //string response2 = ASCIIToJson("5B-30-2C-31-2C-32-5D");

        //    try
        //    {
        //        //List<Data> bikeClients = new List<Data>();
        //        //bikeClients.Add(new Data(1, 0.0, 0.0, 0, 0, 0));
        //        //bikeClients.Add(new Data(2, 0.0, 0.0, 0, 0, 0));
        //        //bikeClients.Add(new Data(3, 0.0, 0.0, 0, 0, 0));
        //        //List<Data> bikeClients = JsonConvert.DeserializeObject<List<Data>>(response);
        //        //MainWindow.UpdateLogOut(response2);

        //        List<int> bikeClients = JsonConvert.DeserializeObject<List<int>>(response2);

        //        MainWindow.UpdateBikeClient(bikeClients);

        //    }
        //    catch (JsonReaderException ex)
        //    {
        //    }
        //}

        //public List<Data> readData()
        //{
        //    Write("getData");
        //    string response = Read();


        //    try
        //    {

        //        List<Data> bikeClients = JsonConvert.DeserializeObject<List<Data>>(response);

        //        return bikeClients;

        //    }
        //    catch (JsonReaderException ex)
        //    {
        //    }
        //}

    }
}
