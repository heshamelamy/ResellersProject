using HubManPractices.Models;
using HubManPractices.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubManPractices.Service
{
    public interface IResellerService
    {
        IEnumerable<Reseller> GetResellers();
        IEnumerable<ResellerViewModel> MapToViewModel(IEnumerable<Reseller> resellers);

        IEnumerable<Reseller> GetUserReseller(string UserID);
        void CreateReseller(Reseller r);
    }
}
