﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrainShedule_HubVersion.DataModel;

namespace TrainShedule_HubVersion.Infrastructure
{
    internal class TrainGrabber
    {
        private const string Pattern = "(?<startTime><div class=\"list_start\">([^<]*)<\\/?)|" +
                                       "(?<endTime><div class=\"list_end\">(.+?)<\\/?)|" +
                                       "(?<city><div class=\"list_text\">(.+?)<\\/?)|" +
                                       "(?<trainDescription><span class=\"list_text_small\">(.+?)<\\/?)|" +
                                       "<div class=\"train_type\">.+?>(?<type>[^<>]+)<\\/div>";

        private const int SearchCountParameter = 5;
        public static async Task<IEnumerable<Train>> GetTrainSchedule(string from, string to, string date, string searchParameter, bool isEconom = false)
        {
            string data = await Task.Run(() => Parser.GetHtmlCode(GetUrl(from, to, date)));
            var addiditionalInformation = GetPlaces(data).ToList();
            var trains = await Task.Run(() => GetSomeTrainsInformation(Parser.ParseTrainData(data, Pattern), addiditionalInformation, searchParameter, isEconom,
                date));
            var schedule = isEconom ? trains.Where(x => x.Type.Contains("эконом")).Select(x => x) : trains;
            var someTrainsInformation = schedule as IList<Train> ?? schedule.ToList();
            return searchParameter == "Национальный аэропорт «Минск»" || searchParameter == "Ближайшие"
                ? someTrainsInformation
                : someTrainsInformation.Where(x => x.Type.Contains(searchParameter));
        }

        private static string GetUrl(string fromName, string toName, string date)
        {
            return "http://rasp.rw.by/m/ru/route/?from=" +
                   fromName + "&to=" + toName + "&date=" + date;
        }

        private static IEnumerable<Train> GetSomeTrainsInformation(IEnumerable<Match> match, IEnumerable<AdditionalInformation[]> placesAndPrices, string searchParameter, bool isEconom, string date)
        {
            var dateOfDeparture = DateTime.Parse(date);
            var parameters = match as IList<Match> ?? match.ToList();
            var imagePath = new List<string>(GetImagePath(parameters));
            var trainList = new List<Train>(parameters.Count / SearchCountParameter);
            var step = parameters.Count - parameters.Count / SearchCountParameter;

            for (var i = 0; i < step; i += 4)
            {
                trainList.Add(new Train
                {
                    StartTime = parameters[i].Groups[1].Value,
                    EndTime = parameters[i + 1].Groups[2].Value,
                    City = ReduceTrainName(parameters[i + 2].Groups[3].Value),
                    Description = parameters[i + 3].Groups[4].Value.Replace("&nbsp;", " "),
                    BeforeDepartureTime =
                        GetBeforeDepartureTime(DateTime.Parse(parameters[i].Groups[1].Value), dateOfDeparture),
                    Type = parameters[i / 4 + step].Value,
                    ImagePath = imagePath[i / 4]
                });
            }
            return GetFinallyResult(placesAndPrices, trainList);
        }

        private static IEnumerable<string> GetImagePath(IEnumerable<Match> match)
        {
            return match.Select(x => x.Groups["type"].Value)
                .Where(x => !string.IsNullOrEmpty(x)).Select(type =>
                {
                    if (type.Contains("Международ"))
                        return "Assets/Inteneshnl.png";
                    if (type.Contains("Межрегион"))
                        return type.Contains("бизнес")
                            ? "Assets/Interregional_biznes.png"
                            : "Assets/Interregional_econom.png";
                    if (type.Contains("Регион"))
                        return type.Contains("бизнес") ? "Assets/Regional_biznes.png" : "Assets/Regional_econom.png";
                    return "Assets/Cityes.png";
                });
        }

        private static string GetBeforeDepartureTime(DateTime time, DateTime dateToDeparture)
        {
            if (dateToDeparture >= DateTime.Now) return dateToDeparture.ToString("D", new CultureInfo("ru-ru"));
            var timeSpan = (time.TimeOfDay - DateTime.Now.TimeOfDay);
            return "через " + timeSpan.Hours + "ч. " + timeSpan.Minutes + "мин.";
        }

        private static string ReduceTrainName(string trainName)
        {
            var shortTrainName = trainName
                .Remove(0, trainName.IndexOf(' '))
                .Split(new[] { "&nbsp;&mdash;" }, StringSplitOptions.None)
                .Aggregate("",
                    (current, cityPoint) =>
                        current + (cityPoint.Length <= 9 ? cityPoint + "-" : cityPoint.Remove(9) + ".-"));
            return shortTrainName.Remove(shortTrainName.Length - 1);
        }

        private const string AddiditionParameterPattern = "div class=\"b-places\">(.*?)</div>";

        private const string ParsePlacesAndPrices =
            "(?<Name><td class=\"places_name\">([^<]+)</td>)|" +
            "(?<quantity><td class=\"places_qty\">([^<]*)<)|" +
            "(?<Price><td class=\"places_price\">([^<]*))";

        private static IEnumerable<AdditionalInformation[]> GetPlaces(string data)
        {
            var addiditionalParameter = Parser.ParseTrainData(data, AddiditionParameterPattern).ToList();
            var addiditionInformationses = new List<AdditionalInformation[]>();
            for (var i = 0; i < addiditionalParameter.Count; i++)
            {
                if (!addiditionalParameter[i].Groups[1].Value.Contains("href")) continue;
                if (i + 1 >= addiditionalParameter.Count || addiditionalParameter[i + 1].Groups[1].Value.Contains("href"))
                    addiditionInformationses.Add(new[]
                    {
                        new AdditionalInformation {Name = "Нет мест"} 
                    });
                else
                {
                    var temp = Parser.ParseTrainData(addiditionalParameter[i + 1].Groups[1].Value, ParsePlacesAndPrices).ToList();
                    var additionalInformations = new AdditionalInformation[temp.Count / 3];
                    for (var j = 0; j < temp.Count; j += 3)
                    {
                        additionalInformations[j / 3] = new AdditionalInformation()
                        {
                            Name =
                                temp[j].Groups[1].Value.Length > 18
                                    ? temp[j].Groups[1].Value.Substring(0, 18)
                                    : temp[j].Groups[1].Value,
                            Place =
                                temp[j + 1].Groups[2].Value == "&nbsp;"
                                    ? "Мест:неограничено"
                                    : temp[j + 1].Groups[2].Value.Replace("&nbsp;", ""),
                            Price = "цена:" + temp[j + 2].Groups[3].Value.Replace("&nbsp;", " ")
                        };
                    }
                    addiditionInformationses.Add(additionalInformations);
                    ++i;
                }
            }
            return addiditionInformationses;
        }

        private static IEnumerable<Train> GetFinallyResult(IEnumerable<AdditionalInformation[]> addInformations, IEnumerable<Train> trains)
        {
            var addInf = addInformations.ToList();
            var trainsList = trains.ToList();
            for (var i = 0; i < addInf.Count; i++)
                trainsList[i].AdditionalInformation = addInf[i];
            return trainsList;
        }
    }
}
