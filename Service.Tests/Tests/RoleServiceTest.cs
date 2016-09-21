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
namespace Tests.RoleTest
{
    [TestClass]
    public class RoleTest
    {
        private readonly RoleService roleService;
        List<Role> roles;
        List<ApplicationUser> userslist;
        public RoleTest()
        {
            roles = CreateRoles();
            userslist = CreateUsersList();
            var mockPermissionRepository = new Mock<IPermissionRepository>();
            var mockRoleRepository = new Mock<IRoleRepository>();

            //Setup for GetAll in Repo.
            mockRoleRepository.Setup(p => p.GetAll()).Returns(roles);

            //Setup for GetById in Repo.
            mockRoleRepository.Setup(p => p.GetById(It.IsAny<string>())).Returns((string i) => roles.Where(c => c.Id == i).FirstOrDefault());

            //Setup for GetById in Repo.
            mockRoleRepository.Setup(p => p.GetRoleByName(It.IsAny<string>())).Returns((string i) => roles.Where(c => c.Name == i).FirstOrDefault());

            //Setup for GetUserInRole in Repo.
            mockRoleRepository.Setup(p => p.GetUserInRole(It.IsAny<string>())).Returns((string i) => userslist.Where(c => c.Roles.FirstOrDefault().Role.Name == i));

            //Setup for GetUserInRole in Repo.
            mockRoleRepository.Setup(p => p.GetUserRoles(It.IsAny<string>())).Returns((string i) => new ApplicationUserRole() { Role=userslist.Where(U=>U.Id==i).FirstOrDefault().Roles.FirstOrDefault().Role,RoleId=userslist.Where(U=>U.Id==i).FirstOrDefault().Roles.FirstOrDefault().RoleId,User=userslist.Where(U=>U.Id==i).FirstOrDefault(),UserId=userslist.Where(U=>U.Id==i).FirstOrDefault().Id});


            //Setup for Add in Repo.
            mockRoleRepository.Setup(p => p.Add(It.IsAny<Role>())).Callback((Role r) =>
            {
                    roles.Add(r);
                    mockRoleRepository.Verify(m => m.Add(It.IsAny<Role>()), Times.Once());
            });

            mockRoleRepository.Setup(p => p.RemovePermission(It.IsAny<string>(), It.IsAny<Guid>())).Callback((string r, Guid pID) =>
            {
                Permission pitem =roles.Where(ro=>ro.Name==r).FirstOrDefault().Permissions.Where(p=>p.PermissionId==pID).FirstOrDefault();
                roles.Where(ro => ro.Name == r).FirstOrDefault().Permissions.Remove(pitem);
            });

            //Setup for Delete in Repo.
            mockRoleRepository.Setup(p => p.Delete(It.IsAny<Role>())).Callback((Role r) =>
            {
                roles.Remove(r);
                mockRoleRepository.Verify(m => m.Delete(It.IsAny<Role>()), Times.Once());
            });

            //Assigning the service to fake repo.
            roleService = new RoleService(null,mockRoleRepository.Object);
        }


        [TestMethod]
        public void TestCreateRole()
        {
            //Arrange
            string id = Guid.NewGuid().ToString();
            Role ToCreate = new Role() { Id =id , Name = "Global Admin More"};

            //Act
            roleService.CreateRole(ToCreate);

            //Assert
            //Verifies the Permission has been inserted
            Assert.AreEqual(id, roleService.GetById(id).Id);
            Assert.IsTrue(roleService.GetRoles().Count() == 3);
        }

        [TestMethod]
        public void TestDeleteRole()
        {
            //Arrange
            Role ToDelete = roleService.GetRoles().FirstOrDefault();

            //Act
            roleService.DeleteRole(ToDelete);

            //Assert
            Assert.IsNull(roleService.GetById(ToDelete.Id));
        }


        [TestMethod]
        public void TestGetById()
        {
            //Arrange
            Role ToGet = roleService.GetRoles().FirstOrDefault();
            string Name = ToGet.Name;
            string Id = ToGet.Id;
            //Act
            Role Got = roleService.GetById(ToGet.Id);

            //Assert
            Assert.IsNotNull(Got);
            Assert.AreEqual(Name, Got.Name);
            Assert.AreEqual(Id, Got.Id);
        }

        [TestMethod]
        public void TestGetRoles()
        {
            //No Arrange...

            //Act
            List<Role> roles = roleService.GetRoles().ToList();

            //Assert
            Assert.AreEqual(2, roles.Count());
            Assert.AreEqual("Global Admin", roles[0].Name);
            Assert.AreEqual("Reseller Admin", roles[1].Name);
        }
        [TestMethod]
        public void TestGetUserInRole()
        {
            //No Arrange..
            
            //Act
            IEnumerable<ApplicationUser> GlobalAdmins = roleService.GetUserInRole("Global Admin");

            //Assert
            Assert.AreEqual(1, GlobalAdmins.Count());
            Assert.AreEqual("Global Admin", GlobalAdmins.FirstOrDefault().Roles.FirstOrDefault().Role.Name);
        }

        [TestMethod]
        public void TestRemovePermission()
        {
            //Arrange
            Role GlobalAdmin = roles.FirstOrDefault();

            //Act
            roleService.RemovePermission(GlobalAdmin.Name, GlobalAdmin.Permissions.FirstOrDefault().PermissionId);

            //Assert
            Assert.AreEqual(0, GlobalAdmin.Permissions.Count());
        }

        [TestMethod]
        public void TestHasPermission()
        {
            //Arrange
            ApplicationUser user =userslist.FirstOrDefault();

            //Act
            Permission has = roleService.HasPermission(user.Id, "Add Reseller");
            //Assert
            Assert.IsNotNull(has);
        }
        private static List<Role> CreateRoles()
        {
            Permission perm = new Permission() { PermissionId = Guid.NewGuid(), PermissionName = "Add Reseller" };
            var list = new List<Role>()
            {
                new Role{Id=Guid.NewGuid().ToString(),Name="Global Admin",Permissions=new List<Permission>(){perm}},
                new Role{Id=Guid.NewGuid().ToString(),Name="Reseller Admin"},
            };
            return list;
        }

        private List<ApplicationUser> CreateUsersList()
        {
            string id = Guid.NewGuid().ToString();
            ApplicationUserRole UR = new ApplicationUserRole() { Role = roles.FirstOrDefault(), RoleId = roles.FirstOrDefault().Id, UserId = id };
            var list = new List<ApplicationUser>()
            {
                 new ApplicationUser() {Id=id,UserName="Omar",Email="omarelsakka@hotmail.com",Roles=new List<ApplicationUserRole>(){UR}},
            };
            return list;
        }


    }
}
