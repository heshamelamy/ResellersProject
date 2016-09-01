using AutoMapper;
using HubManPractices.Models;
using HubManPractices.Service;
using HubManPractices.Service.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Filters;

namespace WebApp.Controllers
{
    public class ResellerController : Controller
    {
        private readonly IClientService ClientService;
        private readonly IRoleService RoleService;
        private readonly IResellerService ResellerService;


        public ResellerController(IClientService clientservice, IResellerService resellerservice,IRoleService Rs)
        {
            this.ClientService = clientservice;
            this.ResellerService = resellerservice;
            this.RoleService = Rs;
        }
        // GET: Reseller
        public ActionResult Index()
        { 
            IEnumerable<ResellerViewModel> ResellerViewModels;
            if (User.IsInRole("Global Admin"))
            {
                ResellerViewModels = ResellerService.MapToViewModel(ResellerService.GetResellers());

                return View(ResellerViewModels);
            }

            else
            {
                ResellerViewModels = ResellerService.MapToViewModel(ResellerService.GetUserReseller(User.Identity.GetUserId()));
                return View(ResellerViewModels);
            }
        }
        public ActionResult Create()
        {
            if (HasPermission("Add Reseller"))
                return View();
            return View("~/Views/Home/UnAuthorized.cshtml");
        }
        [HttpPost]
        [MyAuthFilter(Roles = "Global Admin")]
        public ActionResult Create(Reseller reseller)
        {
            if (HasPermission("Add Reseller"))
            {
                try
                {
                    reseller.ResellerID = Guid.NewGuid();
                    ResellerService.CreateReseller(reseller);
                }
                catch (DbUpdateException ex)
                {
                    TempData["Exists"] = "Client already exists";
                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    TempData["Error Adding Client"] = "Error while creating the client";
                    return RedirectToAction("Create");
                }

                TempData["CreateSuccess"] = "Client created successfully";
                return RedirectToAction("Index");
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