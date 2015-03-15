﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trains.Infrastructure.Infrastructure;
using Trains.Model.Entities;
using Trains.Services.Interfaces;
using Trains.Entities;

namespace Trains.Services.Implementations
{
    public class Search : ISearchService
    {
        public async Task<IEnumerable<CountryStopPointDataItem>> GetCountryStopPoint()
        {
            return (await CountryStopPointData.GetGroupsAsync()).SelectMany(dataGroup => dataGroup.Items);
        }
        public async Task<IEnumerable<Train>> GetTrainSchedule(params string[] parameters)
        {
            return await TrainGrabber.GetTrainSchedule(parameters[0], parameters[1], parameters[2]);
        }
    }
}
