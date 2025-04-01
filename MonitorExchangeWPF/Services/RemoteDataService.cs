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

        public async Task<LoadDataResponse<T>> LoadDataAsync<T>(string endPoint, RequestLoadData request)
        {
            var loadData = new LoadDataResponse<T>();

            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                
                HttpResponseMessage response = await _httpClient.PostAsync($"api/wpf/{endPoint}", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseJSD = JsonSerializer.Deserialize<ResponseWrapper<T>>(jsonResponse, new JsonSerializerOptions
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
