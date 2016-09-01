using HubManPractices.Models;
using HubManPractices.Repository.Infastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubManPractices.Repository.Repositories
{
    public class ResellerRepository : RepositoryBase<Reseller>, IResellerRepository
    {
        public ResellerRepository(IDbFactory dbFactory) : base(dbFactory){}

        public IEnumerable<Reseller> GetUserReseller(string UserId)
        {
            return new List<Reseller>() { DbContext.Users.Find(UserId).Reseller }.AsEnumerable();
        }

    }
}
