using MonitorExchangeWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MonitorExchangeWPF.Services
{
    internal class RemoteDataService : IRemoteDataService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public RemoteDataService() {

            _httpClient.BaseAddress = new Uri("http://localhost:5253");
        }

        public async Task<LoadDataResponse<T>> LoadDataAsync<T>(string endPoint, int page, int pageSize)
        {
            var loadData = new LoadDataResponse<T>();

            try
            {                
                HttpResponseMessage response = await _httpClient.GetAsync($"api/{endPoint}?page={page}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var responseJSD = JsonSerializer.Deserialize<ResponseWrapper<T>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (responseJSD != null && responseJSD.saccess)
                    {
                        foreach (var imp in responseJSD.data)
                        {
                            loadData.listItems.Add(imp);
                        }

                        loadData.meta = responseJSD.meta;
                    }
                }
            }
            catch (Exception ex)
            {
               
            }

            return loadData;

        }
    }
}
