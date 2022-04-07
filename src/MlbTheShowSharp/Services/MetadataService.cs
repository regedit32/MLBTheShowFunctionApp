using MLBTheShowSharp.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MLBTheShowSharp.Services
{
    internal class MetadataService
    {
        public List<LeagueMetadata> GetLeagueMetadata()
        {
            return ReadJson<List<LeagueMetadata>>(@"Resources\importLeagueMetadata.json");
        }

        public T ReadJson<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
