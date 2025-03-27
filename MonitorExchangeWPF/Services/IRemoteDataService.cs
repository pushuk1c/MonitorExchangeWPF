using MonitorExchangeWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorExchangeWPF.Services
{
    internal interface IRemoteDataService
    {
        Task<LoadDataResponse<T>> LoadDataAsync<T>(string endPoint, int page, int pageSize);
    }
}
