using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ServerProgram___correct
{
    public class DataStorage
    {
        private Dictionary<int, List<Data>> dataPerBike = new Dictionary<int, List<Data>>();
        private string filePath;

        public DataStorage(string filePath)
        {
            this.filePath = filePath;
            LoadDataFromFile(); 
        }

        public int getHighestID()
        {
            int highestKey =0;
            if (dataPerBike.Count > 0)
            {
                highestKey = dataPerBike.Keys.Max();
            }
            else
            {
                highestKey = 0;
            }
            return highestKey;
        }

        public void AddData(int bikeId, Data data)
        {
            if (!dataPerBike.ContainsKey(bikeId))
            {
                dataPerBike[bikeId] = new List<Data>();
            }

            dataPerBike[bikeId].Add(data);

            Console.WriteLine("Dit dataobject is toegevoegd: "+data.ToString()+"aan ID: "+bikeId);
        }

        public List<Data> GetData(int bikeId)
        {
            return dataPerBike.ContainsKey(bikeId) ? dataPerBike[bikeId] : new List<Data>();
        }

        public Dictionary<int, List<Data>> getAllData()
        {
            return dataPerBike;
        }

        public void SaveDataToFile()
        {
            string jsonData = JsonConvert.SerializeObject(dataPerBike, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }

        private void LoadDataFromFile()
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                dataPerBike = JsonConvert.DeserializeObject<Dictionary<int, List<Data>>>(jsonData) ?? new Dictionary<int, List<Data>>();
            }
        }
    }
}