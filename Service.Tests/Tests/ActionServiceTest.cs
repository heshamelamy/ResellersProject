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
namespace Tests.ActionTest
{
    [TestClass]
    public class ActionTest
    {
        private readonly ActionService actionService;
        public ActionTest()
        {
            var list = CreateList();
            var mockActionRepository = new Mock<IActionRepository>();


            //Setup for GetAll in Repo.
            mockActionRepository.Setup(p => p.GetAll()).Returns(list);

            //Setup for GetById in Repo.
            mockActionRepository.Setup(p => p.GetById(It.IsAny<Guid>())).Returns((Guid i) => list.Where(c => c.ActionID == i).FirstOrDefault());
            

            //Setup for Add in Repo.
            mockActionRepository.Setup(p => p.Add(It.IsAny<HubManPractices.Models.Action>())).Callback((HubManPractices.Models.Action r) =>
            {
                list.Add(r);
                mockActionRepository.Verify(m => m.Add(It.IsAny<HubManPractices.Models.Action>()), Times.Once());
            });

         
            //Assigning the service to fake repo.
            actionService = new ActionService(null, mockActionRepository.Object);
        }


        [TestMethod]
        public void TestCreateAction()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            HubManPractices.Models.Action ToCreate = new HubManPractices.Models.Action() { ActionID = id, ActionName = "Terminate", Client = new Client() { ClientID = Guid.NewGuid(), ClientName = "Doha" }, Date = DateTime.Now };

            //Act
            actionService.CreateAction(ToCreate);

            //Assert
            //Verifies the Action has been inserted
            Assert.AreEqual(id, actionService.GetById(id).ActionID);
            Assert.IsTrue(actionService.GetActions().Count() == 3);
        }
       
        //Creating Fake List of Actions ....
        private static List<HubManPractices.Models.Action> CreateList()
        {
            var list = new List<HubManPractices.Models.Action>()
            {
                new HubManPractices.Models.Action{ActionID=Guid.NewGuid(),ActionName="Upgrade",Date=DateTime.Now,Client= new Client(){ClientID=Guid.NewGuid(),ClientName="Microsoft"}},
                new HubManPractices.Models.Action{ActionID=Guid.NewGuid(),ActionName="Suspend",Date=DateTime.Now,Client= new Client(){ClientID=Guid.NewGuid(),ClientName="ITworx"}},
            };
            return list;
        }

    }
}
