using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace azuread_data_analyzer.Services
{
    public interface IDataStorageService
    {
        Task Insert<T>(string destination, IEnumerable<T> data);
    }
}
