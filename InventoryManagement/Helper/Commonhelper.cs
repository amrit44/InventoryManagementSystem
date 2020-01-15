using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace InventoryManagement.Helper
{
    public static class Commonhelper
    {

       

        public static List<SelectListItem> GetStores()
        {
            List<SelectListItem> _storeselectlist = new List<SelectListItem>();
            List<StoreMaster> _store = new List<StoreMaster>();
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);
            
            using (var db = new ApplicationDbContext())
            {
                var result = db.StoreMaster.Where(x => x.CompanyId == currentuser.CompanyId).Select(x => new { x.Id, x.StoreName }).ToList();
                if (result.Count()>0)
                {
                    foreach(var item in result)
                    {
                        _storeselectlist.Add(new SelectListItem { Text=item.StoreName,Value=item.Id.ToString() });
                    }
                }
            }
            
            return _storeselectlist;
        }
        public static void SetCookie(string key, string value, TimeSpan expires)
        {
            HttpCookie encodedCookie = new HttpCookie(key, value);

            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[key];
                cookieOld.Expires = DateTime.Now.Add(expires);
                cookieOld.Value = encodedCookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                encodedCookie.Expires = DateTime.Now.Add(expires);
                HttpContext.Current.Response.Cookies.Add(encodedCookie);
            }

        }
        public static string GetCookie(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

            if (cookie != null)
            {
                // For security purpose, we need to encrypt the value.

                value = cookie.Value;
            }
            return value;
        }

    }
}