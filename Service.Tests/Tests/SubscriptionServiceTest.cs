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
namespace Tests.SubscriptionTest
{
    [TestClass]
    public class SubscriptionTest
    {
        private readonly SubscriptionsService subservice;
        public SubscriptionTest()
        {
            var list = CreateList();
            var mockSubscriptionRepository = new Mock<ISubscriptionsRepository>();

            //Setup for GetAll in Repo.
            mockSubscriptionRepository.Setup(p => p.GetAll()).Returns(list);

            //Setup for GetById in Repo.
            mockSubscriptionRepository.Setup(p => p.GetById(It.IsAny<Guid>())).Returns((Guid i) => list.Where(c => c.SubscriptionID == i).FirstOrDefault());

            //Assigning the service to fake repo.
            subservice = new SubscriptionsService(null, mockSubscriptionRepository.Object);
        }


        [TestMethod]
        public void TestGetById()
        {
            //Arrange
            OfficeSubscription ToGet = subservice.GetAllSubscriptions().FirstOrDefault();
            string Name = ToGet.SubscriptionName;
            Guid Id = ToGet.SubscriptionID;
            //Act
            OfficeSubscription Got = subservice.GetById(ToGet.SubscriptionID);

            //Assert
            Assert.IsNotNull(Got);
            Assert.AreEqual(Name, Got.SubscriptionName);
            Assert.AreEqual(Id, Got.SubscriptionID);
        }


        [TestMethod]
        public void TestGetRoles()
        {
            //No Arrange...

            //Act
            List<OfficeSubscription> subs = subservice.GetAllSubscriptions().ToList();

            //Assert
            Assert.AreEqual(2, subs.Count());
            Assert.AreEqual("Office E1", subs[0].SubscriptionName);
            Assert.AreEqual("Office E3", subs[1].SubscriptionName);
        }


        //Creating Fake List of Subscriptions ....
        private static List<OfficeSubscription> CreateList()
        {
            var list = new List<OfficeSubscription>()
            {
                new OfficeSubscription{SubscriptionID=Guid.NewGuid(),SubscriptionName="Office E1",MonthlyFee=8},
                new OfficeSubscription{SubscriptionID=Guid.NewGuid(),SubscriptionName="Office E3",MonthlyFee=12},
            };
            return list;
        }

    }
}
