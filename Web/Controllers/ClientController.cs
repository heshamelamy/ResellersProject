using HubManPractices.Models;
using HubManPractices.Service;
using HubManPractices.Service.ViewModels;
using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Filters;
using System.Data.Entity.Infrastructure;

namespace WebApp.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService ClientService;
        private readonly IRoleService RoleService;
        private readonly IResellerService ResellerService;


        public ClientController(IClientService clientservice, IResellerService resellerservice,IRoleService Rs)
        {
            this.ClientService = clientservice;
            this.ResellerService = resellerservice;
            this.RoleService = Rs;
        }
        // GET: Client
        [MyAuthFilter(Roles="Global Admin , Reseller Admin")]
        public ActionResult Index(Guid ResellerID)
        {
            IEnumerable<Client> ResellerClients = ResellerService.GetResellerClients(ResellerID);
            foreach(var client in ResellerClients)
            {
                client.reseller = ResellerService.GetById(ResellerID);
            }
            TempData["ResellerID"] = ResellerID;
            return View(ClientService.MapToViewModel(ResellerClients));
        }
        [MyAuthFilter(Roles="Reseller Admin")]
        public ActionResult Create(Guid ResellerID)
        {
            if(HasPermission("Add Client"))
            {
                if (ResellerService.QuotaFull(ResellerID))
                {
                    TempData["QuotaFull"] = "Your Clients Quota is full";
                    return RedirectToAction("Index", new { ResellerID = ResellerID });
                }
                else
                {
                    TempData["ResellerID"] = ResellerID;
                    return View();
                }  
            }
            return View("~/Views/Home/UnAuthorized.cshtml");
        }

        [MyAuthFilter(Roles = "Reseller Admin")]
        [HttpPost]
        public ActionResult Create(Client client)
        {
            if (HasPermission("Add Client"))
            {
                try
                {
                    client.ClientID = Guid.NewGuid();
                    client.reseller = ResellerService.GetById(client.ResellerID);
                    ClientService.CreateClient(client);
                    TempData["Create Success"] = "Client Created Successfully";
                    return RedirectToAction("Index", new { ResellerID = client.ResellerID });
                }
                catch(DbUpdateException ex)
                {
                    TempData["Exists"] = "Client Name Exists";
                    return RedirectToAction("Create", new { ResellerID = client.ResellerID });
                }
                catch(Exception ex)
                {
                    TempData["ErrorCreate"] = "Error in adding Client";
                    return RedirectToAction("Create", new { ResellerID = client.ResellerID });
                }
                
            }
            return View("~/Views/Home/UnAuthorized.cshtml");
        }

        [MyAuthFilter(Roles="Reseller Admin")]
        public ActionResult Edit(Guid ClientID)
        {
            if(HasPermission("Edit Client"))
            {
                return View(ClientService.MapToViewModel(ClientService.GetById(ClientID)));
            }

            return View("~/Views/Home/UnAuthorized.cshtml");
        }

        [MyAuthFilter(Roles = "Reseller Admin")]
        [HttpPost]
        public ActionResult Edit(Client client)
        {
            if (HasPermission("Edit Client"))
            {
                try
                {
                    ClientService.EditClient(client);
                }
                catch (DbUpdateException ex)
                {
                    TempData["EditFail"] = "Client already exists";
                    return RedirectToAction("Edit", new { ClientID = client.ClientID });
                }
                catch (Exception ex)
                {
                    TempData["EditError"] = "Error in Editing the Client";
                    return RedirectToAction("Edit", new { ClientID = client.ClientID });
                }
                TempData["EditSuccess"] = "Edit successful";
                return RedirectToAction("Index", new { ResellerID=client.ResellerID});
            }
            return View("~/Views/Home/UnAuthorized.cshtml");
        }

        [MyAuthFilter(Roles = "Reseller Admin")]
        [HttpPost]
        public ActionResult Delete(Guid ClientID)
        {
            if (HasPermission("Delete Client"))
            {

                Client ToBeDeleted = ClientService.GetById(ClientID);
                try
                {
                    ClientService.DeleteReseller(ToBeDeleted);
                    return RedirectToAction("Index",new { ResellerID = ToBeDeleted.ResellerID });
                }
                catch (Exception ex)
                {
                    TempData["DeleteError"] = "Error in Deleting the Reseller";
                    return RedirectToAction("Index", new { ResellerID=ToBeDeleted.ResellerID});
                }

            }
            return View("~/Views/Home/UnAuthorized.cshtml");
        }


        public bool HasPermission(string PName)
        {
            if (RoleService.HasPermission(User.Identity.GetUserId(), PName) == null)
                return false;
            return true;
        }
    }
}