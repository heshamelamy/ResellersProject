using HubManPractices.Models;
using HubManPractices.Repository.Infastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubManPractices.Repository.Repositories
{
    public class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
         public ClientRepository(IDbFactory dbFactory) : base(dbFactory){}

         public override void Add(Client client)
         {
                 Client Found = DbContext.Clients.Where(c => c.ClientName == client.ClientName).Where(c => c.IsDeleted == true).FirstOrDefault();
                 if (Found != null)
                 {
                     Found.IsDeleted = false;
                     Found.reseller = client.reseller;
                     DbContext.Commit();
                 }
                 else
                 {
                     DbContext.Clients.Add(client);
                     DbContext.Commit();
                 }
         }
         public override void Delete(Client client)
         {
             Client ToBeDeleted = Get(c => c.ClientID == client.ClientID);
             ToBeDeleted.IsDeleted = true;
             DbContext.Commit();
         }


         public void AddOfficeSubscription(Guid ClientID, Guid SubID, int UsersPerSub)
         {
             ClientSubscriptions ToAdd= new ClientSubscriptions(){ClientID=ClientID,SubscriptionID=SubID,UsersPerSubscription=UsersPerSub,};
             DbContext.ClientSubscriptions.Add(ToAdd);
         }
    }

}
