using InventoryManagement.Helper;
using InventoryManagement.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
namespace InventoryManagement.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }
        
       
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {

            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            if(user!=null)
            {
                Commonhelper.SetCookie("CompanyId", user.CompanyId.ToString(), TimeSpan.FromHours(30));
                Commonhelper.GetStores();
                Commonhelper.GetUserRole();
            }
            else
            {
                Commonhelper.GetUserRole();
            }
           
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}