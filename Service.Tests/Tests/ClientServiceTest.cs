using HubManPractices.Models;
using HubManPractices.Repository;
using HubManPractices.Repository.Infastructure;
using HubManPractices.Repository.Repositories;
using HubManPractices.Service;
using HubManPractices.Service.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace Tests.ClientTest
{
    [TestClass]
    public class ClientTest
    {
        private readonly ClientService Cservice;
        List<Reseller> resellers;
        List<Client> clist;
        public ClientTest()
        {
            resellers = CreateList();
            clist = CreateClientsList();
            var mockClientRepository = new Mock<IClientRepository>();

            //Setup for GetAll in Repo.
            mockClientRepository.Setup(p => p.GetAll()).Returns(clist);

            //Setup for GetById in Repo.
            mockClientRepository.Setup(p => p.GetById(It.IsAny<Guid>())).Returns((Guid i) => clist.Where(c => c.ClientID == i).FirstOrDefault());


            //Setup for GetClientSubscription in Repo.
            mockClientRepository.Setup(p => p.GetClientSubscription(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((Guid CID, Guid SubID) => clist.Where(c => c.ClientID == CID).FirstOrDefault().ClientSubscriptions.Where(s=>s.SubscriptionID==SubID).FirstOrDefault());


            //Setup for CheckIfExists in Repo.
            mockClientRepository.Setup(p => p.CheckIfExists(It.IsAny<Client>())).Returns((Client i) => clist.Where(c => c.ClientID == i.ClientID).FirstOrDefault()!=null);
            
            //Setup for NeedsRenewal in Repo.
            mockClientRepository.Setup(p => p.NeedsRenewal(It.IsAny<Client>())).Returns((Client i) => clist.Where(c => c.ClientID == i.ClientID).FirstOrDefault().Expiry <= DateTime.Now);


            //Setup for GetDeletedClient in Repo.
            mockClientRepository.Setup(p => p.GetDeletedClient(It.IsAny<Client>())).Returns((Client i) => clist.Where(c => c.ClientID == i.ClientID && c.IsDeleted==true).FirstOrDefault());


            //Setup for CheckIfExistsAndDeleted in Repo.
            mockClientRepository.Setup(p => p.CheckIfExistsAndDeleted(It.IsAny<Client>())).Returns((Client i) => clist.Where(c => c.ClientID == i.ClientID && c.IsDeleted==true).FirstOrDefault() != null);

            //Setup for ClientNameAndMailExists in Repo.
            mockClientRepository.Setup(p => p.ClientNameAndMailExists(It.IsAny<string>(), It.IsAny<string>())).Returns((string ClientName, string ClientMail) => clist.Where(c => c.ClientName == ClientName && c.ContactMail==ClientMail).FirstOrDefault()!=null);

            //Setup for GetClientByNameAndMail in Repo.
            mockClientRepository.Setup(p => p.GetByNameAndMail(It.IsAny<string>(), It.IsAny<string>())).Returns((string ClientName, string ClientMail) => clist.Where(c => c.ClientName == ClientName && c.ContactMail == ClientMail).FirstOrDefault());


            //Setup for ClientNameAndMailExistsAndDeleted in Repo.
            mockClientRepository.Setup(p => p.ClientNameAndMailExistsAndDeleted(It.IsAny<string>(), It.IsAny<string>())).Returns((string ClientName, string ClientMail) => clist.Where(c => c.ClientName == ClientName && c.ContactMail == ClientMail && c.IsDeleted==true).FirstOrDefault() != null);


            //Setup for GetMany in Repo.
            mockClientRepository.Setup(q => q.GetMany(It.IsAny<Expression<Func<Client, bool>>>()))
             .Returns<Expression<Func<Client, bool>>>(q =>
             {
                 var query = q.Compile();
                 return clist.Where(query);
             });


            //Setup for Add in Repo.
            mockClientRepository.Setup(p => p.Add(It.IsAny<Client>())).Callback((Client r) =>
            {
                clist.Add(r);
                mockClientRepository.Verify(m => m.Add(It.IsAny<Client>()), Times.Once());
            });

            //Setup for Update in Repo.
            mockClientRepository.Setup(p => p.Update(It.IsAny<Client>())).Callback((Client r) =>
            {
                clist.First(re => re.ClientID == r.ClientID).Status = r.Status;
                mockClientRepository.Verify(m => m.Update(It.IsAny<Client>()), Times.Once());
            });

            //Setup For Delete in Repo.
            mockClientRepository.Setup(p => p.Delete(It.IsAny<Client>())).Callback((Client r) =>
            {
                clist.Find(re => re.ClientID == r.ClientID).IsDeleted = true;
                mockClientRepository.Verify(m => m.Delete(It.IsAny<Client>()), Times.Once());
            });

            //Setup For AddOfficeSubscription in Repo.
            mockClientRepository.Setup(p => p.AddOfficeSubscription(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>())).Callback((Guid CID,Guid SID,int Usub) =>
            {
                clist.Find(re => re.ClientID == CID).ClientSubscriptions.Add(new ClientSubscriptions() { Client = clist.Find(re => re.ClientID == CID), ClientID = CID, SubscriptionID = SID, UsersPerSubscription = Usub });
            });


            //Assigning the service to fake repo.
            Cservice = new ClientService(null, mockClientRepository.Object);
        }

        [TestMethod]
        public void TestGetByID()
        {
            //Arrange
            Client ToGet = Cservice.GetClients().FirstOrDefault();
            string Name = ToGet.ClientName;
            Guid Id = ToGet.ClientID;
            //Act
            Client Got = Cservice.GetById(ToGet.ClientID);

            //Assert
            Assert.IsNotNull(Got);
            Assert.AreEqual(Name, Got.ClientName);
            Assert.AreEqual(Id, Got.ClientID);
        }

        [TestMethod]
        public void TestGetClients()
        {
            //No Arrange...

            //Act
            List<Client> clients = Cservice.GetClients().ToList();

            //Assert
            Assert.AreEqual(2, clients.Count());
            Assert.AreEqual("Microsoft", clients[0].ClientName);
            Assert.AreEqual("Doha Bank", clients[1].ClientName);
        }

        [TestMethod]
        public void TestCreateClient()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            Client ToCreate = new Client() { ClientName = "Itworx", ClientID = id, ContactNumber = 0100449136 };

            //Act
            Cservice.CreateClient(ToCreate);

            //Assert
            //Verifies the Reseller has been inserted
            Assert.AreEqual(id, Cservice.GetById(id).ClientID);
            Assert.IsTrue(Cservice.GetClients().Count() == 3);
        }

        [TestMethod]
        public void TestDeleteClient()
        {
            //Arrange
            Client ToDelete = Cservice.GetClients().FirstOrDefault();

            //Act
            Cservice.DeleteClient(ToDelete);

            //Assert
            Assert.AreEqual(true, Cservice.GetById(ToDelete.ClientID).IsDeleted);
        }

        [TestMethod]
        public void TestEditClient()
        {
            //Arrange
            Client NewClient = Cservice.GetClients().FirstOrDefault();
            Client c = new Client() { ClientID = NewClient.ClientID, ClientName = "ItWorx", Status = "On Hold" };

            //Act
            Cservice.EditClient(c);

            //Assert
            Assert.AreEqual("On Hold", Cservice.GetById(c.ClientID).Status);
        }

        [TestMethod]
        public void TestAddOfficeSubscription()
        {
            //Arrange
            Guid cID = clist.FirstOrDefault().ClientID;
            Guid SubID= Guid.NewGuid();
            
            //Act
            Cservice.AddOfficeSubscription(cID, SubID, 10);

            //Assert
            Assert.AreEqual(SubID, clist.Find(c => c.ClientID == cID).ClientSubscriptions.FirstOrDefault().SubscriptionID);
            Assert.AreEqual(10, clist.Find(c => c.ClientID == cID).ClientSubscriptions.FirstOrDefault().UsersPerSubscription);
        }

        [TestMethod]
        public void TestClientNameAndMailExists()
        {
            //Arrange
            string clientname = "Microsoft";
            string contactmail = "david@outlook.com";

            //Act
            bool found = Cservice.ClientNameAndMailExists(clientname, contactmail);

            //Assert
            Assert.AreEqual(true, found);
        }

        [TestMethod]
        public void TestClientNameAndMailExistsAndDeleted()
        {
            //Arrange
            clist.FirstOrDefault().IsDeleted = true;

            //Act
            bool found = Cservice.ClientNameAndMailExistsAndDeleted(clist.FirstOrDefault().ClientName, clist.FirstOrDefault().ContactMail);

            //Assert
            Assert.AreEqual(true, found);
        }

        [TestMethod]
        public void  TestExists()
        {
            //Arrange 
            //Client not in the system , so won't be found..
            Client check = new Client() { ClientID = Guid.NewGuid() };

            //Act
            bool exists = Cservice.Exists(check);

            //Assert
            Assert.AreEqual(false, exists);
        }


        [TestMethod]
        public void TestExistsAndDeleted()
        {
            //Arrange 
            //Client not in the system , so won't be found..
            Client check = new Client() { ClientID = Guid.NewGuid() , IsDeleted=true };

            //Act
            bool exists = Cservice.ExistsAndDeleted(check);

            //Assert
            Assert.AreEqual(false, exists);
        }

        [TestMethod]
        public void TestGetDeletedClient()
        {
            //Arrange
            Client client = new Client() { ClientID = Guid.NewGuid(), IsDeleted = true, ClientName = "Zain" };
            Cservice.CreateClient(client);

            //Act
            Client cl= Cservice.GetDeletedClient(client);

            //Assert
            Assert.AreEqual(cl.ClientID, client.ClientID);
        }

        [TestMethod]
        public void TestNeedsRenewal()
        {
            //Arrange
            Client client = new Client() { ClientID = Guid.NewGuid(), Status = "Activated", Expiry = DateTime.Now };
            Cservice.CreateClient(client);

            //Act
            bool needs = Cservice.NeedsRenewal(client);

            //Assert
            Assert.AreEqual(true, needs);
        }

        [TestMethod]
        public void TestGetClientByNameAndMail()
        {
            //Arrange
            string clientname = "Microsoft";
            string mail = "david@outlook.com";

            //Act
            Client Got =Cservice.GetClientByNameAndMail(clientname, mail);

            //Assert
            Assert.IsNotNull(Got);
            Assert.AreEqual(clientname, Got.ClientName);
            Assert.AreEqual(mail, Got.ContactMail);
        }

        [TestMethod]
        public void TestGetClientSubscription()
        {
            //Arrange
            Guid CID= Guid.NewGuid();
            Guid SubID= Guid.NewGuid();
            Client client = new Client() { ClientID=CID,ClientSubscriptions = new List<ClientSubscriptions>() { new ClientSubscriptions() { ClientID = CID, SubscriptionID = SubID } }, ClientName = "Barclays" };
            Cservice.CreateClient(client);

            //Act
            ClientSubscriptions Got = Cservice.GetClientSubscription(CID, SubID);

            //Assert
            Assert.IsNotNull(Got);
            Assert.AreEqual(SubID, Got.SubscriptionID);
        }

        //Creating Fake List of Resellers ....
        private static List<Reseller> CreateList()
        {
            var list = new List<Reseller>()
            {
                new Reseller{Name="FranceReseller",ResellerID=Guid.NewGuid(),ClientsQuota=5},
                new Reseller{Name="GermanyReseller",ResellerID=Guid.NewGuid(),ClientsQuota=10},
            };
            return list;
        }

        //Creating Fake List of Clients ....
        private  List<Client> CreateClientsList()
        {
            var list = new List<Client>()
            {
                new Client{ClientID=Guid.NewGuid(),ClientName="Microsoft",ContactMail="david@outlook.com",ContactTitle="ProjectManager",ResellerID=resellers.FirstOrDefault().ResellerID},
                new Client{ClientID=Guid.NewGuid(),ClientName="Doha Bank",ContactMail="david2@outlook.com",ContactTitle="Marketing Manager",ResellerID=resellers.FirstOrDefault().ResellerID},
            };
            return list;
        }

    }
}
