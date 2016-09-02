using HubManPractices.Models;
using HubManPractices.Service.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubManPractices.Service
{
    public interface IClientService
    {
        IEnumerable<ClientViewModel> MapToViewModel(IEnumerable<Client> Clients);
        ClientViewModel MapToViewModel(Client client);
        Client GetById(Guid ClientID);
        void CreateClient(Client client);
    }
}
