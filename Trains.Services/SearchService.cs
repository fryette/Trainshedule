﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trains.Services.Infrastructure;
using Trains.Model.Entities;
using Trains.Services.Interfaces;
using Trains.Entities;
using Trains.Services.Tools;
using Trains.Services.Interfaces;
using System;

namespace Trains.Services.Implementations
{
    public class SearchService : ISearchService
    {
        #region constant

        private const string Pattern = "(?<startTime><div class=\"list_start\">([^<]*)<\\/?)|" +
                                       "(?<endTime><div class=\"list_end\">(.+?)</div>)|" +
                                       "(?<city><div class=\"list_text\">(.+?)<\\/?)|" +
                                       "(?<trainDescription><span class=\"list_text_small\">(.+?)<\\/?)|" +
                                       "<div class=\"train_type\">.+?>(?<type>[^<>]+)<\\/div>";

        private const string AdditionParameterPattern = "div class=\"b-places\">(.*?)</div>";

        private const string PlacesAndPricesPattern =
            "(?<Name><td class=\"places_name\">([^<]+)</td>)|" +
            "(?<quantity><td class=\"places_qty\">([^<]*)<)|" +
            "(?<Price><td class=\"places_price\">([^<]*))";

        #endregion

        public readonly IHttpService _httpService;
        //private readonly IAppSettings _appSettings;

        public SearchService(IHttpService httpService /*,IAppSettings appSettings*/)
        {
            //_appSettings = appSettings;
            _httpService = httpService;
        }

        public async Task<List<Train>> GetTrainSchedule(string from, string to, string date)
        {
			// TODO: refactoring this
			var fromItem = new CountryStopPointItem(); // _appSettings.AutoCompletion.First(x => x.UniqueId == from);
			var toItem = new CountryStopPointItem(); // _appSettings.AutoCompletion.First(x => x.UniqueId == to);

            var data = await _httpService.LoadResponseAsync(GetUrl(fromItem, toItem, date));
            var additionalInformation = TrainGrabber.GetPlaces(data);
            var links = TrainGrabber.GetLink(data);

            IEnumerable<Train> trains;
            if (fromItem.Country != "(Беларусь)" && toItem.Country != "(Беларусь)")
                trains = TrainGrabber.GetTrainsInformationOnForeignStantion(Parser.ParseData(data, Pattern).ToList(), date);
            else
                trains = date == "everyday" ? TrainGrabber.GetTrainsInformationOnAllDays(Parser.ParseData(data, Pattern).ToList())
                    : TrainGrabber.GetTrainsInformation(Parser.ParseData(data, Pattern).ToList(), date);

            return TrainGrabber.GetFinallyResult(additionalInformation, links, trains).ToList();
        }

        private Uri GetUrl(CountryStopPointItem fromItem, CountryStopPointItem toItem, string date)
        {
            return new Uri("http://rasp.rw.by/m/" + "ru" + "/route/?from=" +
                   fromItem.UniqueId.Split('(')[0] + "&from_exp=" + fromItem.Exp + "&to=" + toItem.UniqueId.Split('(')[0] + "&to_exp=" + toItem.Exp + "&date=" + date+"&"+new Random().Next(0,20));
        }

        public async Task<List<Train>> UpdateTrainSchedule()
        {
            //if (_appSettings.UpdatedLastRequest == null) return null;

			// TODO: refactoring this
			// _appSettings.UpdatedLastRequest.From
			// _appSettings.UpdatedLastRequest.To
			// ToolHelper.GetDate(_appSettings.UpdatedLastRequest.Date, _appSettings.UpdatedLastRequest.SelectionMode)
            return await GetTrainSchedule(null, null , null);
        }

    }
}