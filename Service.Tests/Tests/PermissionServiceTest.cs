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
namespace Tests.PermissionTest
{
    [TestClass]
    public class PermissionTest
    {
        private readonly PermissionService permissionService;
        public PermissionTest()
        {
            var roles = CreateRoles();
            var list = CreateList(roles.FirstOrDefault());
            var mockPermissionRepository = new Mock<IPermissionRepository>();
            var mockRoleRepository = new Mock<IRoleRepository>();

            //Setup for GetAll in Repo.
            mockPermissionRepository.Setup(p => p.GetAll()).Returns(list);

            //Setup for GetById in Repo.
            mockPermissionRepository.Setup(p => p.GetById(It.IsAny<Guid>())).Returns((Guid i) => list.Where(c => c.PermissionId == i).FirstOrDefault());

            //Setup for GetById in Repo.
            mockRoleRepository.Setup(p => p.GetRoleByName(It.IsAny<string>())).Returns((string i) => roles.Where(c => c.Name == i).FirstOrDefault());


            //Setup for Add in Repo.
            mockPermissionRepository.Setup(p => p.Add(It.IsAny<Permission>())).Callback((Permission r) =>
            {
                list.Add(r);
                mockPermissionRepository.Verify(m => m.Add(It.IsAny<Permission>()), Times.Once());
            });

            //Setup for Delete in Repo.
            mockPermissionRepository.Setup(p => p.Delete(It.IsAny<Permission>())).Callback((Permission r) =>
            {
                list.Remove(r);
                mockPermissionRepository.Verify(m => m.Delete(It.IsAny<Permission>()), Times.Once());
            });

            


            //Assigning the service to fake repo.
            permissionService = new PermissionService(null, mockPermissionRepository.Object, mockRoleRepository.Object);
        }


        [TestMethod]
        public void TestCreatePermission()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            Permission ToCreate = new Permission() { PermissionId = id, PermissionName = "Suspend Client",Roles=CreateRoles()};

            //Act
            permissionService.CreatePermission(ToCreate);
            //Assert
            //Verifies the Permission has been inserted
            Assert.AreEqual(id, permissionService.GetById(id).PermissionId);
            Assert.IsTrue(permissionService.GetPermissions().Count() == 3);
        }

        [TestMethod]
        public void TestDeletePermission()
        {
            //Arrange
            Permission ToDelete = permissionService.GetPermissions().FirstOrDefault();

            //Act
            permissionService.DeletePermission(ToDelete);

            //Assert
            Assert.IsNull(permissionService.GetById(ToDelete.PermissionId));
        }


        [TestMethod]
        public void TestGetById()
        {
            //Arrange
            Permission ToGet = permissionService.GetPermissions().FirstOrDefault();
            string Name = ToGet.PermissionName;
            Guid Id = ToGet.PermissionId;
            //Act
            Permission Got = permissionService.GetById(ToGet.PermissionId);

            //Assert
            Assert.IsNotNull(Got);
            Assert.AreEqual(Name, Got.PermissionName);
            Assert.AreEqual(Id, Got.PermissionId);
        }

        [TestMethod]
        public void TestAssignPermissionToRole()
        {
            //Arrange
            Permission permission = permissionService.GetPermissions().FirstOrDefault();

            //Act
            permissionService.AssignPermissionToRole("Reseller Admin", permission.PermissionId);

            //Assert
            Assert.AreEqual("Reseller Admin", permissionService.GetById(permission.PermissionId).Roles.Where(r => r.Name == "Reseller Admin").FirstOrDefault().Name);
            Assert.AreEqual(2, permissionService.GetById(permission.PermissionId).Roles.Count());
        }


        [TestMethod]
        public void TestGetPermissions()
        {
            //No Arrange...

            //Act
            List<Permission> permissions = permissionService.GetPermissions().ToList();

            //Assert
            Assert.AreEqual(2, permissions.Count());
            Assert.AreEqual("Add Client", permissions[0].PermissionName);
            Assert.AreEqual("Remove Client", permissions[1].PermissionName);
        }


        private static List<Role> CreateRoles()
        {
            var list = new List<Role>()
            {
                new Role{Id=Guid.NewGuid().ToString(),Name="Global Admin"},
                new Role{Id=Guid.NewGuid().ToString(),Name="Reseller Admin"},
            };
            return list;
        }

        //Creating Fake List of Permissions ....
        private static List<Permission> CreateList(Role role)
        {
            var list = new List<Permission>()
            {
                new Permission{PermissionId=Guid.NewGuid(),PermissionName="Add Client",Roles=new List<Role>{role}},
                new Permission{PermissionId=Guid.NewGuid(),PermissionName="Remove Client",Roles=new List<Role>{role}},
            };
            return list;
        }

    }
}
