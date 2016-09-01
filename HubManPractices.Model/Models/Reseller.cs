using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubManPractices.Models
{
    public class Reseller
    {
        public Reseller()
        {
            Clients = new List<Client>();
        }
        public Guid ResellerID { get; set; }
        public string Name { get; set; }
        public int ClientsQuota { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
        public bool IsDeleted { get; set; }

    }
}
