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
namespace Tests.ResellerTest
{
    [TestClass]
    public class ResellerTest
    {
        private readonly ResellerService resellerService;
        private List<ApplicationUser> users;
        public ResellerTest()
        {
            var list = CreateList();
            users= CreateUsersList();
            var _mockResellerRepository = new Mock<IResellerRepository>();
            
            //Setup for GetAll in Repo.
            _mockResellerRepository.Setup(p => p.GetAll()).Returns(list);
            
            //Setup for GetMany in Repo.

             _mockResellerRepository.Setup(q => q.GetMany(It.IsAny<Expression<Func<Reseller, bool>>>()))
              .Returns<Expression<Func<Reseller, bool>>>(q =>
                  {
                      var query = q.Compile();
                      return list.Where(query);
                  });
            
            //Setup for GetById in Repo.
            _mockResellerRepository.Setup(p => p.GetById(It.IsAny<Guid>())).Returns((Guid i) => list.Where(c => c.ResellerID == i).FirstOrDefault());
            
            //Setup for Add in Repo.
            _mockResellerRepository.Setup(p => p.Add(It.IsAny<Reseller>())).Callback((Reseller r)=>
            {
                list.Add(r);
                _mockResellerRepository.Verify(m => m.Add(It.IsAny<Reseller>()), Times.Once());
            });

            //Setup for Update in Repo.
            _mockResellerRepository.Setup(p => p.Update(It.IsAny<Reseller>())).Callback((Reseller r) =>
            {
                list.First(re => re.ResellerID == r.ResellerID).Name = r.Name;
                list.First(re => re.ResellerID == r.ResellerID).ClientsQuota = r.ClientsQuota;
                _mockResellerRepository.Verify(m => m.Update(It.IsAny<Reseller>()), Times.Once());
            });
            
            //Setup For Delete in Repo.
            _mockResellerRepository.Setup(p => p.Delete(It.IsAny<Reseller>())).Callback((Reseller r) =>
            {
                list.Find(re => re.ResellerID == r.ResellerID).IsDeleted = true;
                _mockResellerRepository.Verify(m => m.Delete(It.IsAny<Reseller>()), Times.Once());

            });
            
            //Setup for GetResellerClients in Repo.
            _mockResellerRepository.Setup(p => p.GetResellerClients(It.IsAny<Guid>())).Returns((Guid i) => list.Find(r=>r.ResellerID==i).Clients.Where(c=>c.IsDeleted==false).ToList());

            //Setup for GetResellerDeletedClients in Repo.
            _mockResellerRepository.Setup(p => p.GetResellerDeletedClients(It.IsAny<Guid>())).Returns((Guid i) => list.Find(r => r.ResellerID == i).Clients.Where(c => c.IsDeleted == true).ToList());


            //Setup for GetUserReseller in Repo.
            _mockResellerRepository.Setup(p => p.GetUserReseller(It.IsAny<string>())).Returns((string i) =>new List<Reseller>(){users.Find(r => r.Id == i).Reseller});

            //Setup for QuotaFull in Repo.
            _mockResellerRepository.Setup(p => p.QuotaFull(It.IsAny<Guid>())).Returns((Guid i) => list.Find(r => r.ResellerID == i).Clients.Count()==list.Find(r => r.ResellerID == i).ClientsQuota);

            //Setup for SearchForResellers in Repo.
            _mockResellerRepository.Setup(p => p.SearchForResellers(It.IsAny<string>())).Returns((string i) => list.Where(r=>r.Name.StartsWith(i) && r.IsDeleted==false));

            //Assigning the service to fake repo.
            resellerService = new ResellerService(null, _mockResellerRepository.Object);
        }

        [TestMethod]
        public void TestCreateReseller()
        {
            //Arrange
            Guid id= Guid.NewGuid();
            Reseller ToCreate = new Reseller() { Name = "AustriaReseller", ResellerID = id, ClientsQuota = 10 };

            //Act
            resellerService.CreateReseller(ToCreate);

            //Assert
            //Verifies the Reseller has been inserted
            Assert.AreEqual(id, resellerService.GetById(id).ResellerID);
            Assert.IsTrue(resellerService.GetResellers().Count()==3);
        }

        [TestMethod]
        public void TestDeleteReseller()
        {
            //Arrange
            Reseller ToDelete = resellerService.GetResellers().FirstOrDefault();
            
            //Act
            resellerService.DeleteReseller(ToDelete);

            //Assert
            Assert.AreEqual(true,resellerService.GetById(ToDelete.ResellerID).IsDeleted);
        }
        [TestMethod]
        public void TestEditReseller()
        {
            //Arrange
            Reseller NewReseller = resellerService.GetResellers().FirstOrDefault();
            Reseller r = new Reseller() { ResellerID=NewReseller.ResellerID,Name="MyFrance",ClientsQuota=5};

            //Act
            resellerService.EditReseller(r);

            //Assert
            Assert.AreEqual("MyFrance", resellerService.GetById(r.ResellerID).Name);
            Assert.AreEqual(5, resellerService.GetById(r.ResellerID).ClientsQuota);
        }
        
        [TestMethod]
        public void TestGetById()
        {
            //Arrange
            Reseller ToGet= resellerService.GetResellers().FirstOrDefault();
            string Name = ToGet.Name;
            Guid Id = ToGet.ResellerID;
            //Act
            Reseller Got =resellerService.GetById(ToGet.ResellerID);

            //Assert
            Assert.IsNotNull(Got);
            Assert.AreEqual(Name, Got.Name);
            Assert.AreEqual(Id, Got.ResellerID);
        }
        [TestMethod]
        public void TestGetResellerClients()
        {
            //Arrange
            Reseller r= resellerService.GetResellers().FirstOrDefault();
            List<Client> clients= new List<Client>();
            clients.Add(new Client(){ClientID=Guid.NewGuid(),ClientName="Microsoft",ResellerID=r.ResellerID});
            clients.Add(new Client(){ClientID=Guid.NewGuid(),ClientName="ITworx",ResellerID=r.ResellerID});
            r.Clients=clients;

            //Act
            List<Client>GotClients= resellerService.GetResellerClients(r.ResellerID).ToList();
            
            //Assert
            Assert.AreEqual(2, GotClients.Count());
            Assert.AreEqual("Microsoft", GotClients[0].ClientName);
            Assert.AreEqual("ITworx", GotClients[1].ClientName);
        }

        [TestMethod]
        public void TestGetResellerDeletedClients()
        {
            //Arrange
            Reseller r = resellerService.GetResellers().FirstOrDefault();
            List<Client> clients = new List<Client>();
            clients.Add(new Client() {IsDeleted=true , ClientID = Guid.NewGuid(), ClientName = "Microsoft", ResellerID = r.ResellerID});
            clients.Add(new Client() {IsDeleted=true ,  ClientID = Guid.NewGuid(), ClientName = "ITworx", ResellerID = r.ResellerID});
            r.Clients = clients;

            //Act
            List<Client> GotClients = resellerService.GetResellerDeletedClients(r.ResellerID).ToList();
            
            //Assert
            Assert.AreEqual(2, GotClients.Count());
            Assert.AreEqual("Microsoft", GotClients[0].ClientName);
            Assert.AreEqual("ITworx", GotClients[1].ClientName);
        }

        [TestMethod]
        public void TestGetResellers()
        {
            //No Arrange...

            //Act
            List<Reseller> resellers = resellerService.GetResellers().ToList();

            //Assert
            Assert.AreEqual(2, resellers.Count());
            Assert.AreEqual("FranceReseller", resellers[0].Name);
            Assert.AreEqual("GermanyReseller", resellers[1].Name);
        }

        [TestMethod]
        public void TestGetUserReseller()
        {
            //NoArrange..


            //Act
            Reseller Get = resellerService.GetUserReseller(users.FirstOrDefault().Id).FirstOrDefault();

            //Assert
            Assert.IsNotNull(Get);
        }

        [TestMethod]
        public void TestQuotaFull()
        {
            //Arrange
            //This Reseller Has only 1 client to be added.. , so this arrange will give Quota Full
            Reseller reseller = resellerService.GetResellers().FirstOrDefault();
            List<Client> clients = new List<Client>();
            clients.Add(new Client() { ClientID = Guid.NewGuid(), ClientName = "Microsoft", ResellerID = reseller.ResellerID });
            reseller.Clients = clients;

            //Act
            bool quota = resellerService.QuotaFull(reseller.ResellerID);
            
            //Assert
            Assert.AreEqual(true, quota);
        }

        [TestMethod]
        public void TestSearchForResellers()
        {
            //Arrange
            string query = "Fran";
            
            //Act
            List<Reseller> resellers= resellerService.SearchForResellers(query).ToList();

            //Assert
            Assert.AreEqual(1, resellers.Count());
            Assert.AreEqual("FranceReseller", resellers[0].Name);
        }

        //Creating Fake List of Resellers ....
        private static List<Reseller> CreateList()
        {
            var list = new List<Reseller>()
            {
                new Reseller{Name="FranceReseller",ResellerID=Guid.NewGuid(),ClientsQuota=1},
                new Reseller{Name="GermanyReseller",ResellerID=Guid.NewGuid(),ClientsQuota=10},
            };
            return list;
        }

        //Creating Fake List of Users ....
        private static List<ApplicationUser> CreateUsersList()
        {
            List<Reseller> resellerslist= CreateList();
            var list = new List<ApplicationUser>()
            {
                 new ApplicationUser() {Id=Guid.NewGuid().ToString(),UserName="Omar",Reseller=resellerslist.FirstOrDefault(), ResellerID=resellerslist.FirstOrDefault().ResellerID,Email="omarelsakka@hotmail.com"},
            };
            return list;
        }

    }
}
