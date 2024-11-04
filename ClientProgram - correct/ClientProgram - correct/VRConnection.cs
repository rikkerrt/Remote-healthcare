using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ClientProgram___correct 
{
    internal class VRConnection 
    {
        private static TcpClient client = new TcpClient();
        private static string _address = "85.145.62.130";
        private static string encoded = "";
        private static string send;
        private static string dataString;

        private static string id;
        private static string tunnelId;
        private static string uuIDstring;
        private static string routeID;
        private static string bikeIDstring;
        private static string cameraIDstring;
        private static string hudID;


        private int _port;
        private static double bikeSpeed = 0;
        private static bool emergenceStop= false;

        private static NetworkStream networkStream;
        public static byte[] prepend;
        public static byte[] data;


        public static async Task Start() 
        {
            client.Connect(_address, 6666);
            networkStream = client.GetStream();

            createData();
            await ReadResponse();

            id = getID(dataString);

            createTunnel(id);
            await ReadResponse();

            tunnelId = getTunnelId(dataString);
            Console.WriteLine(tunnelId);

            clearScene();
            await ReadResponse();

            generateTerrain();
            await ReadResponse();
            Console.WriteLine(dataString);

            addNodeToTerrain();
            await ReadResponse();

            uuIDstring = getUUIDstring(dataString);
            //Console.WriteLine("De string van de textures " + uuIDstring);

            addLayerToNode(uuIDstring);
            await ReadResponse();

            addRouteToMap();
            await ReadResponse();

            routeID = getRouteID(dataString);
            //Console.WriteLine("Route ID " + routeID);

            showRouteOnMap();
            await ReadResponse();

            addRoadTexture(routeID);
            await ReadResponse();

            //addBikeNodeToMap();
            //await ReadResponse();
            //bikeIDstring = getUUIDstring(dataString);

            findCameraNode();
            await ReadResponse();

            cameraIDstring = getUtillID(dataString);
            //Console.WriteLine("Camera ID " + cameraIDstring);

            addBikeNode();
            await ReadResponse();

            addHudNode();
            await ReadResponse();

            hudID = getUUIDstring(dataString);
            //Console.WriteLine("Hud ID: " + hudID);

            clearPanel();
            await ReadResponse();

            speedToHud();
            await ReadResponse();

            swapBuffers();
            await ReadResponse();

            SetupRouteWithNode();
            await ReadResponse();
        }

        private static async Task ReadResponse() 
        {
            byte[] length = new byte[4];
            int PrependLenght = 0;

            while (PrependLenght < 4) 
            {
                int dataPrependRead = await networkStream.ReadAsync(length, 0, length.Length);
                PrependLenght += dataPrependRead;
            }

            int lengthInt = BitConverter.ToInt32(length, 0);
            //Console.WriteLine(lengthInt);
            byte[] dataBuffer = new byte[lengthInt];

            int readTotal = 0;
            do
            {
                int read = await networkStream.ReadAsync(dataBuffer, readTotal, dataBuffer.Length - readTotal);
                readTotal += read;
                //Console.WriteLine(readTotal);
            } while (readTotal < dataBuffer.Length);

            dataString = Encoding.UTF8.GetString(dataBuffer, 0, readTotal);
            //Console.WriteLine("Response: " + Encoding.UTF8.GetString(dataBuffer, 0, readTotal) + "\n");

            /*while (PrependLenght < lengthInt)
            {
                Console.WriteLine(lengthInt);
                int bytesread = await networkStream.ReadAsync(dataBuffer, 0, dataBuffer.Length);
                Console.WriteLine(PrependLenght);
                PrependLenght += bytesread;
            }*/

            //collins trying
            //while ((readTotal = await networkStream.ReadAsync(dataBuffer, 0, dataBuffer.Length)) != 0) {
            //    dataString = Encoding.UTF8.GetString(dataBuffer, 0, dataBuffer.Length);
            //}

            //string dataString = Encoding.UTF8.GetString(dataBuffer);
            //Console.WriteLine(dataString);

            //byte[] payload = new byte[lengthInt];
            //int data = await networkStream.ReadAsync(payload, 0, payload.Length);

            //getID(dataString);
            //Console.WriteLine("vrconnection done");
        }

        public static void SendPacket(byte[] prepend, byte[] data) 
        {
            byte[] combinedArray = new byte[prepend.Length + data.Length];
            Array.Copy(prepend, 0, combinedArray, 0, prepend.Length);
            Array.Copy(data, 0, combinedArray, prepend.Length, data.Length);
            networkStream.Write(combinedArray, 0, combinedArray.Length);
            //Console.WriteLine("Command send: " + Encoding.UTF8.GetString(combinedArray));
        }

        public static void createData() 
        {
            string jsonPacket = "{\"id\" : \"session/list\"}";
            data = Encoding.ASCII.GetBytes(jsonPacket);
            prepend = new byte[] { (byte)jsonPacket.Length, 0x00, 0x00, 0x00 };
            SendPacket(prepend, data);
        }

        public static void createTunnel(string id) 
        {
            var tunnelCommand = new 
            {
                id = "tunnel/create",
                data = new {
                    session = id,
                    key = ""
                }
            };

            string jsonPacket = JsonConvert.SerializeObject(tunnelCommand);
            byte[] data = Encoding.ASCII.GetBytes(jsonPacket);
            byte[] prepend = BitConverter.GetBytes(data.Length);
            SendPacket(prepend, data);
        }

        public static void clearScene() 
        {
            var clearData = new 
            {

            };

            SendTunnelCommand("scene/reset", clearData);
        }
        private static void findCameraNode()
        {
            var cameraNodeCommand = new
            {
                name = "Camera"
            };
            SendTunnelCommand("scene/node/find", cameraNodeCommand);
        }

        public static void SendTunnelCommand(string command, object jsonCommandData)
        {
            var alJsonData = new
            {
                id = "tunnel/send",
                data = new
                {
                    dest = tunnelId,
                    data = new
                    {
                        id = command,
                        data = jsonCommandData
                    }
                }
            };

            string jsonPacket = JsonConvert.SerializeObject(alJsonData);

            byte[] data = Encoding.ASCII.GetBytes(jsonPacket);
            byte[] prepend = BitConverter.GetBytes(data.Length);
            SendPacket(prepend, data);
        }

        private static void generateTerrain() 
        {
            int width = 256;
            int height = 256;
            float[,] heights = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heights[x, y] = 2 + (float)(Math.Sin(x / 10.0) + Math.Cos(y / 10.0));
                }
            }

            var terrainData = new
            {
                size = new[] { width, height },
                heights = heights.Cast<float>().ToArray()
            };
            SendTunnelCommand("scene/terrain/add", terrainData);
        }

        private static void addNodeToTerrain()
        {

            var nodeData = new 
            {
                name = "terrain",
                components = new 
                {
                    transform = new 
                    {
                        position = new[] { -128, 0, -128 },
                        scale = 1,
                        rotation = new[] { 0, 0, 0 },
                    },
                    terrain = new 
                    {
                        smooth = true,
                    }
                }
            };

            SendTunnelCommand("scene/node/add", nodeData);
        }

        private static void addHudNode()
        {
            var nodeData = new
            {
                name = "hudNode",
                parent = cameraIDstring,
                components = new
                {
                    transform = new
                    {
                        position = new[] { 1.5, 0.75, 1 },
                        scale = 1,
                        rotation = new[] { 0, 0, 0 },
                    },
                    panel = new
                    {
                        size = new[] { 1, 1 },
                        resolution = new[] { 512, 512 },
                        background = new[] { 0, 0, 0, 1 },
                        castShadow = true,
                    },
                }
            };

            SendTunnelCommand("scene/node/add", nodeData);
        }
        private static void addBikeNode()
        {
            var bikeData = new
            {
                name = "BikeNode",
                parent = cameraIDstring,
                components = new
                {
                    transform = new
                    {
                        position = new[] { 0, 0, 2.75 },
                        scale = 1,
                        rotation = new[] { 0, -90, 0 }
                    },
                    model = new
                    {
                        file = "data/NetworkEngine/models/bike/bike.fbx",
                        cullbackfaces = true,
                    }
                }
            };
            SendTunnelCommand("scene/node/add", bikeData);
        }
        private static void clearPanel()
        {
            var clearPanel = new
            {
                id = hudID
            };
            SendTunnelCommand("scene/panel/clear", clearPanel);
        }
        private static void swapBuffers()
        {
            var bufferSwap = new
            {
                id = hudID
            };
            SendTunnelCommand("scene/panel/swap", bufferSwap);
        }
        private static void speedToHud()
        {
            var speedToHud = new
            {
                id = hudID,
                text = bikeSpeed.ToString() + " km/h",
                position = new[] {75.0,75.0},
                size = 50.0,
                color = new[] { 1, 1, 1, 1 },
                font = "segoeui"
            };
            SendTunnelCommand("scene/panel/drawtext", speedToHud);
        }

        private async static void updateBikeSpeed() 
        {
            var speedUpdate = new 
            {
                node = cameraIDstring,
                speed = bikeSpeed,
            };

            SendTunnelCommand("route/follow/speed", speedUpdate);
            await ReadResponse();
        }   
        
        /*private static void addBikeNodeToMap()
        {
            var nodeData = new
            {
                name = "entity",
                components = new
                {
                    transform = new
                    {
                        position = new[] { 0, 0, 0 },
                        scale = 1,
                        rotation = new[] { 0, 0, 0 },
                    },
                    panel = new
                    {
                        size = new[] { 4, 4 },
                        resolution = new[] { 512, 512, },
                        background = new[] { 0, 0, 0, 1 },
                        castShadow = true,
                    }
                } 
            };

            SendTunnelCommand("scene/node/add",nodeData);
        }*/
        private static void SetupRouteWithNode()
        {
            var followData = new
            {
                route = routeID,
                node = cameraIDstring,
                speed = bikeSpeed,
                offset = 0.0,
                rotate = "XYZ",
                smoothing = 1.0,
                followHeight = true,
                rotateOffset = new[] { 0, 0, 0 },
                positionOffset = new[] { 0, 0, 0 }
            };
            SendTunnelCommand("route/follow", followData);
        }
        private static void addLayerToNode(string uuid)
        {

            var layerData = new
            {
                id = uuid,
                diffuse = "data/NetworkEngine/textures/grass/grass_green_d.jpg",
                normal = "data/NetworkEngine/textures/grass/grass_green_d.jpg",
                minHeight = -1,
                maxHeight = 10,
                fadeDist = 1000
            };
            SendTunnelCommand("scene/node/addlayer", layerData);
        }
        private static void addRouteToMap()
        {
            var routeData = new
            {
                nodes = new[] {
                        new { pos = new[] { 0, 0, 0 }, dir = new[] { 90, 0, -20 } },
                        new { pos = new[] { -50, 0, -50 }, dir = new[] { -100, 0, -50 } },
                        new { pos = new[] { 100, 0, -90 }, dir = new[] { 90, 0, 75 } },
                        new { pos = new[] { -80, 0, 100 }, dir = new[] { -100, 0, -100 } },
                        
                }
            };
            SendTunnelCommand("route/add",routeData);
        }
        private static void showRouteOnMap()
        {
            var showRouteData = new
            {
                show = true,
            };
            SendTunnelCommand("route/show",showRouteData);
        }
        private static void addRoadTexture(string routeID)
        {
            var addRoadData = new
            {
                route = routeID,
                diffuse = "data/NetworkEngine/textures/tarmac_diffuse.png",
                normal = "data/NetworkEngine/textures/tarmac_normal.png",
                specular = "data/NetworkEngine/textures/tarmac_specular.png",
                heightoffset = 0.01
            };
            SendTunnelCommand("scene/road/add",addRoadData);
        }
        
        public static string getTunnelId(string tunnelDataString) 
        {
            string tunnelId = "";
            JsonNode jsonNode = JsonNode.Parse(tunnelDataString);
            JsonObject jsonObject = jsonNode.AsObject();

            tunnelId = jsonObject["data"]?["id"]?.ToString();

            return tunnelId;
        }
        public static string getUUIDstring(string uuidDataString)
        {
            string uuid = "";
            JsonNode jsonNode = JsonNode.Parse(uuidDataString);
            JsonObject jsonObject = jsonNode.AsObject();

            uuid = jsonObject["data"]?["data"]?["data"]?["uuid"]?.ToString();

            return uuid;
        }
        public static string getRouteID(string routeDataString)
        {
            string routeID = "";
            JsonNode jsonNode = JsonNode.Parse(routeDataString);
            JsonObject jsonObject = jsonNode.AsObject();

            routeID = jsonObject["data"]?["data"]?["data"]?["uuid"]?.ToString();
            return routeID;
        }
        public static string getUtillID(string cameraDataString)
        {
            string utillID = "";
            JsonNode jsonNode = JsonNode.Parse(cameraDataString);
            JsonObject jsonObject = jsonNode.AsObject();

            utillID = jsonObject["data"]?["data"]?["data"]?[0]?["uuid"]?.ToString();

            return utillID;
        }

        public static string getID(string data) 
        {
            string idHost = "";

            JsonData dataList = JsonConvert.DeserializeObject<JsonData>(data);
            List<Data> dataObject = dataList.data;

            foreach (Data data1 in dataObject) {
                if (data1.features.Contains("tunnel")) {
                    ClientInfo info = data1.clientinfo;
                    if (info.host.ToLower() == Dns.GetHostName().ToLower()) 
                    {
                        //Console.WriteLine(data1.id);
                        idHost = data1.id;
                    }
                }
            }

            return idHost;

            //var jsonDocument = JObject.Parse(data);
            //Console.WriteLine(jsonDocument.ToString());

            //JArray dataList = JArray.Parse(jsonDocument["data"].ToString());
            //Console.WriteLine(dataList.ToString());
            //for(int i = 0; i < dataList.Count; i++)
            //{
            //    Console.WriteLine(dataList[i].ToString());
            //}


            //Console.WriteLine(jsonDocument["data"].ToString());
            //Console.WriteLine(jsonDocument["id"].ToString());

            /*if (jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataElement) && 
                dataElement.ValueKind == JsonValueKind.Array && 
                dataElement.GetArrayLength() >0) {
                return dataElement[0].GetProperty("id").GetString();
            }

            if(jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataObject) && dataObject.ValueKind == JsonValueKind.Object) {
                JsonNode jsonnode = System.Text.Json.JsonSerializer.SerializeToNode(dataObject);
                return jsonnode["id"].GetValue<string>();
            }*/

        }

        public static void setSpeed(double speed) 
        {
            if (!emergenceStop) {
                bikeSpeed = speed * 0.01;
                updateBikeSpeed();
            }
        }

        public static void setEmergencyStop(bool stop) 
        {
            bikeSpeed = 0;
            updateBikeSpeed();
            emergenceStop = stop;
        }
        
        //public static void sendTunnel(string command) {
        //    string jsonPacket = "{\"id\" : \"tunnel/send\", \"data\" : {\"dest\" : \"" + tunnelId + "\", \"data\" : " + command + "}}";
        //    data = Encoding.ASCII.GetBytes(jsonPacket);
        //    Console.WriteLine(jsonPacket);
        //    prepend = new byte[] { (byte)jsonPacket.Length, 0x00, 0x00, 0x00 };
        //    SendPacket(prepend, data);
        //}
        
        //Outdated
        /*public static string recieveData() {
            byte[] buffer = new byte[1500];
            Console.WriteLine(networkStream.Read(buffer, 0, buffer.Length));
            Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, networkStream.Read(buffer, 0, buffer.Length)));
            return Encoding.ASCII.GetString(buffer, 0, networkStream.Read(buffer, 0, buffer.Length));
        }*/

        /*private static void getTerrain() {
            string jsonPacket = "{\"id\" : \"scene/load\", \"data\" : {\"filename\" : \"cookie.json\"}}";
            sendTunnel(jsonPacket);
        }
            
        private static void saveFile() {
            string jsonPacket = "{\"id\" : \"scene/save\", \"data\" : {\"filename\" : \"cookie.json\", \"overwrite\" : \"false\"} }";
            sendTunnel(jsonPacket);
        }
        private static void terrainHeight()
        {
            string jsonPacket = "{\"id\" : \"scene/terrain/getheight\", \"data\" : {\"position\" : [10.2,4.4], \"positions\" : [[10.2,4.4],[11.2,4.4],[12.2,4.4]]}}";
            sendTunnel(jsonPacket);
        }*/


    }

    internal class JsonData 
    {
        public List<Data> data { get; set; }
        public string id { get; set; }
        public JsonData(string id, List<Data> data)
        {
            this.data = data;
            this.id = id;
        }
    }

    internal class Data {
        public string id { get; set; }
        public string beginTime { get; set; }
        public string lastPing { get; set; }
        public List<Fps> fps { get; set; }
        public List<string> features { get; set; }
        public ClientInfo clientinfo { get; set; }

        public Data(string id, string beginTime, string lastPing, List<Fps> fps, List<string> features, ClientInfo client) {
            this.id = id;
            this.beginTime = beginTime;
            this.lastPing = lastPing;
            this.fps = fps;
            this.features = features;
            this.clientinfo = client;
        }
    }

    internal class ClientInfo {
        public string host { get; set; }
        public string user { get; set; }
        public string file { get; set; }
        public string renderer { get; set; }

        public ClientInfo(string host, string user, string file, string renderer) {
            this.host = host;
            this.user = user;
            this.file = file;
            this.renderer = renderer;
        }
    }

    internal class Fps {
        public long time { get; set; }
        public double fps { get; set; }

        public Fps(long time, double fps) {
            this.fps = fps;
            this.time = time;
        }
    }
}
