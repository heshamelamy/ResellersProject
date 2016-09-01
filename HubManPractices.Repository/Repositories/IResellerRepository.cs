using HubManPractices.Models;
using HubManPractices.Repository.Infastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubManPractices.Repository.Repositories
{
    public interface IResellerRepository : IRepository<Reseller>
    {
        IEnumerable<Reseller> GetUserReseller(string UserId);
    }
}
