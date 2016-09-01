using HubManPractices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubManPractices.Service.ViewModels
{
    public class ClientViewModel
    {
        public Guid ClientID { get; set; }
        public string ClientName { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Reseller reseller { get; set; }
    }
}
