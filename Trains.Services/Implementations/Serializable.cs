﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trains.Infrastructure.Infrastructure;
using Trains.Model.Entities;
using Trains.Services.Interfaces;

namespace Trains.Services.Implementations
{
    public class Serializable : ISerializableService
    {
        public List<LastRequest> SerializeLastRequest(string from, string to, List<LastRequest> lastRequests)
        {
            if (lastRequests == null) lastRequests = new List<LastRequest>();
            if (lastRequests.Any(x => x.From == from && x.To == to)) return lastRequests;
            if (lastRequests.Count < 3)
            {
                lastRequests.Add(new LastRequest
                {
                    From = from,
                    To = to
                });
            }
            else
            {
                lastRequests[2] = lastRequests[1];
                lastRequests[1] = lastRequests[0];
                lastRequests[0] = new LastRequest
                {
                    From = from,
                    To = to
                };
            }
            SerializeObjectToXml(lastRequests, "lastRequests");
            return lastRequests;
        }

        public async void SerializeObjectToXml<T>(T obj, string fileName)
        {
            await Serialize.SaveObjectToXml(obj, fileName);
        }

        public async Task<List<LastRequest>> GetLastRequests(string fileName)
        {
            var obj = await Serialize.ReadObjectFromXmlFileAsync<List<LastRequest>>(fileName);
            return obj == null ? null : obj.ToList();
        }

        public void DeleteFile(string fileName)
        {
            Serialize.DeleteFile("favoriteRequests");
        }

        public Task<T> ReadObjectFromXmlFileAsync<T>(string filename) where T : class
        {
            return Serialize.ReadObjectFromXmlFileAsync<T>(filename);
        }
    }
}
