using AutoMapper;
using HubManPractices.Models;
using HubManPractices.Service;
using HubManPractices.Service.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebApp.Filters;

namespace WebApp.Controllers
{
    public class ResellerController : Controller
    {
        private readonly IRoleService RoleService;
        private readonly IResellerService ResellerService;


        public ResellerController(IResellerService resellerservice,IRoleService Rs)
        {
            this.ResellerService = resellerservice;
            this.RoleService = Rs;
        }
        // GET: Reseller
        [MyAuthFilter(Roles= "Global Admin , Reseller Admin")]
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
                    try
                    {
                        ResellerViewModels = ResellerService.MapToViewModel(ResellerService.GetUserReseller(User.Identity.GetUserId()));
                        return View(ResellerViewModels);
                    }
                    catch(NullReferenceException ex)
                    {
                        TempData["NoReseller"] = "This admin has no reseller";
                        return RedirectToAction("Index", "Home");
                    }
                }
            
        }

        [MyAuthFilter(Roles = "Global Admin")]
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
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];

                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = reseller.Name + ".PNG";
                            var path = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);
                            file.SaveAs(path);
                        }
                    }
                }
                catch (DbUpdateException ex)
                {
                    TempData["Exists"] = "Reseller already exists";
                    return RedirectToAction("Create");
                }
                catch (Exception ex)
                {
                    TempData["Error Adding Reseller"] = "Error while creating the Reseller";
                    return RedirectToAction("Create");
                }

                TempData["CreateSuccess"] = "Client created successfully";
                return RedirectToAction("Index");
            }
            return View("~/Views/Home/UnAuthorized.cshtml");
        }

        [MyAuthFilter(Roles = "Global Admin, Reseller Admin")]
        public ActionResult Edit(Guid ResellerID)
        {
            if(HasPermission("Edit Reseller"))
            {
                Reseller ToBeEdited=ResellerService.GetById(ResellerID);
                return View(ResellerService.MapToViewModel(ToBeEdited));
            }
            return View("~/Views/Home/UnAuthorized.cshtml");
        }

        [MyAuthFilter(Roles = "Global Admin, Reseller Admin")]
        [HttpPost]
        public ActionResult Edit(Reseller reseller)
        {
            if (HasPermission("Edit Reseller"))
            {
                try
                {
                    ResellerService.EditReseller(reseller);
                }
                catch (DbUpdateException ex)
                {
                    TempData["EditFail"] = "Reseller already exists";
                    return RedirectToAction("Edit", new { id = reseller.ResellerID });
                }
                catch(Exception ex)
                {
                    TempData["EditError"] = "Error in Editing the Reseller";
                    return RedirectToAction("Edit", new { id = reseller.ResellerID });
                }
                TempData["EditSuccess"] = "Edit successful";
                return RedirectToAction("Index");
            }
            return View("~/Views/Home/UnAuthorized.cshtml");
        }

        [MyAuthFilter(Roles= "Global Admin")]
        [HttpPost]
        public ActionResult Delete(Guid ResellerID)
        {
            if(HasPermission("Delete Reseller"))
            {
                
                Reseller ToBeDeleted =ResellerService.GetById(ResellerID);
                try
                {
                    ResellerService.DeleteReseller(ToBeDeleted);
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    TempData["DeleteError"]="Error in Deleting the Reseller";
                    return RedirectToAction("Index");
                }
                
            }
            return View("~/Views/Home/UnAuthorized.cshtml");
        }

        public ActionResult Search(string Query)
        {
            return View("Index", ResellerService.MapToViewModel(ResellerService.SearchForResellers(Query)));
        }

        public bool HasPermission(string PName)
        {
            if (RoleService.HasPermission(User.Identity.GetUserId(), PName) == null)
                return false;
            return true;
        }
    }
}