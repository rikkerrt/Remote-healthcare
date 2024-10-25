using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ClientProgram___correct {
    internal class VRConnection {
        private static TcpClient client = new TcpClient();
        private static string _address = "85.145.62.130";
        private static string encoded = "";
        private static string send;
        private static string dataString;

        private static string id;
        private static string tunnelId;
        private static string uuIDstring;
        private static string routeID;

        private int _port;

        private static NetworkStream networkStream;
        public static byte[] prepend;
        public static byte[] data;

        public static async Task Start() {
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

            addNodeToTerrain();
            await ReadResponse();

            uuIDstring = getUUIDstring(dataString);
            Console.WriteLine("De string van de textures " + uuIDstring);
            addLayerToNode(uuIDstring);
            await ReadResponse();

            addRouteToMap();
            await ReadResponse();

            routeID = getRouteID(dataString);
            Console.WriteLine("Route ID " + routeID);


            showRouteOnMap();
            await ReadResponse();

            addBikeNodeToMap();
            await ReadResponse();

            followRouteWithNode();
            await ReadResponse();


        }

        private static async Task ReadResponse() {
            byte[] length = new byte[4];
            int PrependLenght = 0;

            while (PrependLenght < 4) {
                int dataPrependRead = await networkStream.ReadAsync(length, 0, length.Length);
                PrependLenght += dataPrependRead;
            }

            int lengthInt = BitConverter.ToInt32(length, 0);
            //Console.WriteLine(lengthInt);
            byte[] dataBuffer = new byte[lengthInt];
            
            int readTotal = 0;
            do {
                int read = await networkStream.ReadAsync(dataBuffer, readTotal, dataBuffer.Length - readTotal);
                readTotal += read;
                //Console.WriteLine(readTotal);
            } while (readTotal < dataBuffer.Length);

            dataString = Encoding.UTF8.GetString(dataBuffer, 0, readTotal);
            Console.WriteLine("Response: " + Encoding.UTF8.GetString(dataBuffer, 0, readTotal));

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

        public static void SendPacket(byte[] prepend, byte[] data) {
            byte[] combinedArray = new byte[prepend.Length + data.Length];
            Array.Copy(prepend, 0, combinedArray, 0, prepend.Length);
            Array.Copy(data, 0, combinedArray, prepend.Length, data.Length);
            networkStream.Write(combinedArray, 0, combinedArray.Length);
            Console.WriteLine("Command send: " + Encoding.UTF8.GetString(combinedArray));
        }

        public static void createData() {
            string jsonPacket = "{\"id\" : \"session/list\"}";
            data = Encoding.ASCII.GetBytes(jsonPacket);
            prepend = new byte[] { (byte)jsonPacket.Length, 0x00, 0x00, 0x00 };
            SendPacket(prepend, data);
        }

        public static void createTunnel(string id) {
            var tunnelCommand = new {
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

        public static void clearScene() {
            var clearData = new {
                
            };

            SendTunnelCommand("scene/reset", clearData);
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

        private static void generateTerrain() {
            int width = 256;
            int height = 256;
            float[,] heights = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heights[x, y] = 0;
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

            var nodeData = new {
                name = "terrain",
                components = new {
                    transform = new {
                        position = new[] { -128, 1, -128 },
                        scale = 1,
                        rotation = new[] { 0, 0, 0 },
                    },
                    terrain = new {
                        smooth = true,
                    },
                    panel = new {
                        size = new[] { 64, 64 },
                        resolution = new[] { 512, 512 },
                        background = new[] { 0, 0, 0, 1 },
                        castShadow = true,
                    }
                }
            };

            SendTunnelCommand("scene/node/add", nodeData);
        }
        private static void addBikeNodeToMap()
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
        }
        private static void followRouteWithNode()
        {
            var followData = new
            {
                route = routeID,
                node = "entity",
                speed = 1.0,
                rotate = "NONE",
                smoothing = 1.0,
                followHeight = false,
                rotateOffset = new byte[] { 0, 0,0 },
                positionOffset = new byte[] { 0,0,0 },
            };
            SendTunnelCommand("route/follow", followData);
        }
        private static void addLayerToNode(string uuid)
        {
            var layerData = new
            {
                id = uuid,
                diffuse = "grass.jpg",
                normal =  "grass.jpg",
                minHeight = 1,
                maxHeight = 10,
                fadeDist = 1
            };
            SendTunnelCommand("scene/node/addlayer", layerData);
                
        }
        private static void addRouteToMap()
        {
            var routeData = new
            {
                nodes = new[] { 
                    new { pos = new[] {0,0,0},dir = new[] { 5,0,-5}  },
                    new { pos = new[] {50,0,0 },dir = new[] {5,0,5} },
                    new { pos = new[] {50,0,50},dir = new[] {-5,0,5} },
                    new { pos = new[] {0,0,50 }, dir = new [] {-5,0,-5} }
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
        
        public static string getTunnelId(string tunnelDataString) {
            string tunnelId = "";
            JsonTunnelData tunnelDataObj = JsonConvert.DeserializeObject<JsonTunnelData>(tunnelDataString);

            TunnelData tunnelData = tunnelDataObj.data;
            tunnelId = tunnelData.id;


            return tunnelId;
        }
        public static string getUUIDstring(string uuiddataString)
        {
            string uuid = "";
            JsonTextureData jsonTextureData = JsonConvert.DeserializeObject<JsonTextureData>(uuiddataString);

            TextureScope textureScope = jsonTextureData.data;
            TextureData textureData = textureScope.data;
            TextureTools textureTools = textureData.data;

            uuid = textureTools.uuid;

            return uuid;
        }
        public static string getRouteID(string routeDataString)
        {
            string routeID = "";
            JsonRouteData jsonRouteData = JsonConvert.DeserializeObject<JsonRouteData>(routeDataString);

            RouteScope routeScope = jsonRouteData.data;
            RouteData routeData = routeScope.data;
            RouteTools routeTools = routeData.data;

            routeID = routeTools.uuid;

            return routeID;
        }

        public static string getID(string data) {
            string idHost = "";

            JsonData dataList = JsonConvert.DeserializeObject<JsonData>(data);
            List<Data> dataObject = dataList.data;

            foreach (Data data1 in dataObject) {
                if (data1.features.Contains("tunnel")) {
                    ClientInfo info = data1.clientinfo;
                    if (info.host.ToLower() == System.Net.Dns.GetHostName().ToLower()) {
                        Console.WriteLine(data1.id);
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
    

    internal class JsonTunnelData {
        public string id { get; set; }
        public TunnelData data { get; set; }
        public JsonTunnelData(string id, TunnelData data) {
            this.data = data;
            this.id = id;
        }
    }

    internal class JsonData {
        public List<Data> data { get; set; }
        public string id { get; set; }
        public JsonData(string id, List<Data> data) {
            this.data = data;
            this.id = id;
        }
    }
    internal class TunnelData {
        public string status { get; set; }
        public string id { get; set; }
       
        public TunnelData(string status, string id)
        {
            this.status = status;
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

    internal class JsonTextureData
    {
        public string id { get; set; }
        public TextureScope data { get; set; }
        public JsonTextureData(string id, TextureScope data)
        {
            this.id = id;
            this.data = data;
        }
    }
    internal class TextureScope 
    {
        public string id { get; set;}
        public TextureData data {  get; set; } 
        public TextureScope(string id, TextureData data)
        {
            this.id = id;
            this.data = data;
        }
    }
    internal class TextureData
    {
        public TextureTools data { get; set; }
        public TextureData(TextureTools data)
        {
            this.data = data;
        }
    }
    internal class TextureTools
    {
        public string name { get; set; }
        public string uuid { get; set; }
        public TextureTools(string name, string uuid)
        {
            this.name = name;
            this.uuid = uuid;
        }
    }
    internal class JsonRouteData
    {
        public string id { get; set; }
        public RouteScope data { get; set; }
        public JsonRouteData(string id, RouteScope data)
        {
            this.id = id;
            this.data = data;
        }   
    }
    internal class RouteScope
    {
        public string id { get; set; }
        public RouteData data { get; set; }
        public RouteScope(string id, RouteData data)
        {
            this.id = id;
            this.data = data;
        }
    }
    internal class RouteData
    {
        public RouteTools data { get; set; }
        public RouteData(RouteTools data)
        {
            this.data = data;
        }
    }
    internal class RouteTools
    {
        public string uuid { get; set; }
        public RouteTools(string uuid)
        {
            this.uuid = uuid;
        }
    }
}
