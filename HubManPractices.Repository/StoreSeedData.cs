using HubManPractices.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubManPractices.Repository
{
    public class StoreSeedData : DropCreateDatabaseIfModelChanges<ApplicationEntities>
    {
        private static Role GlobalRole = new Role { Name = "Global Admin", Permissions = new List<Permission>() };
        private static Role ResellerRole = new Role { Name = "Reseller Admin", Permissions = new List<Permission>() };



        protected override void Seed(ApplicationEntities context)
        {

            AddRolesPermissions(context);
            AddUserAndRoles(context);
            context.Commit();

            //base.Seed(context);
        }


        private static void AddUserAndRoles(ApplicationEntities context)
        {
            var GlobalAdmin = new ApplicationUser() {SecurityStamp=Guid.NewGuid().ToString(), UserName = "admin", PasswordHash = new PasswordHasher().HashPassword("global123") };
            var ResellerAdmin = new ApplicationUser() { SecurityStamp = Guid.NewGuid().ToString(), UserName = "adminreseller", PasswordHash = new PasswordHasher().HashPassword("reseller123") };
           


            //password must be greater than 6 chars
            context.Users.AddOrUpdate(GlobalAdmin);
            context.Users.AddOrUpdate(ResellerAdmin);

            context.ApplicationUserRoles.AddOrUpdate(t => t.UserId, new ApplicationUserRole() { RoleId = GlobalRole.Id, UserId = GlobalAdmin.Id });
            context.ApplicationUserRoles.AddOrUpdate(t => t.UserId, new ApplicationUserRole() { RoleId = ResellerRole.Id, UserId = ResellerAdmin.Id });

        }

        private static void AddRolesPermissions(ApplicationEntities context)
        {
            var AddReseller = new Permission { PermissionId = Guid.NewGuid(), PermissionName = "Add Reseller" };
            var AddClient = new Permission { PermissionId = Guid.NewGuid(), PermissionName = "Add Client" };
            var EditClient = new Permission { PermissionId = Guid.NewGuid(), PermissionName = "Edit Client" };
            var EditReseller = new Permission { PermissionId = Guid.NewGuid(), PermissionName = "Edit Reseller" };
            var DeleteReseller = new Permission { PermissionId = Guid.NewGuid(), PermissionName = "Delete Reseller" };
            var DeleteClient = new Permission { PermissionId = Guid.NewGuid(), PermissionName = "Delete Client" };


            List<Permission> permissions = new List<Permission>() {
                AddReseller,
                AddClient,
                EditReseller,
                EditClient,
                DeleteReseller,
                DeleteClient,
            };


            try
            {

                //Add permissions to db and give them all to the global admin
                foreach (var perm in permissions)
                {
                    GlobalRole.Permissions.Add(perm);
                }


                ResellerRole.Permissions.Add(AddClient);
                ResellerRole.Permissions.Add(EditClient);
                ResellerRole.Permissions.Add(DeleteClient);
                ResellerRole.Permissions.Add(EditReseller);

                



                context.Roles.AddOrUpdate(t => t.Name, GlobalRole);
                context.Roles.AddOrUpdate(t => t.Name, ResellerRole);

            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}
