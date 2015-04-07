﻿using System;
using System.Threading.Tasks;
using Trains.Model;
using Trains.Resources;
using Newtonsoft.Json;
using Trains.Services.Interfaces;

namespace Trains.Services
{
    public class WebBackendAppService : IBackendService
    {
        private readonly IHttpService _httpService;

        public WebBackendAppService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ChygunkaSettings> GetAppSettings()
        {
            string response = await _httpService.LoadResponseAsync(new Uri(String.Format("{0}{1}", Constants.BaseAppUri, Constants.SettingsRelativeUri)));
            var result = JsonConvert.DeserializeObject<ChygunkaSettings>(response);
            return result;
        }
    }
}

