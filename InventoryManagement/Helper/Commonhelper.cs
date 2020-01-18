using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Net;

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
        public static List<SelectListItem> GetRoles()
        {
            List<SelectListItem> _roleselectlist = new List<SelectListItem>();
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);

            using (var db = new ApplicationDbContext())
            {
                var result = db.Roles.Select(x => new { x.Id, x.Name }).ToList();
                if (result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        _roleselectlist.Add(new SelectListItem { Text = item.Name, Value = item.Name.ToString() });
                    }
                }
            }

            return _roleselectlist;
        }

        public static void GetUserRole()
        {
            string rolename = string.Empty;
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);


            ApplicationUserManager UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = UserManager.GetRoles(HttpContext.Current.User.Identity.GetUserId());
            rolename= roles[0];


        }


        public static List<SelectListItem> GetUsers()
        {
            List<SelectListItem> _userslist = new List<SelectListItem>();
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);

            using (var db = new ApplicationDbContext())
            {
                _userslist = db.Database.SqlQuery<SelectListItem>("EXEC sp_GetUserList").ToList();
                //var result = db.Users.Select(x => new { x.Id, x.UserName }).ToList();
                //if (result.Count() > 0)
                //{
                //    foreach (var item in result)
                //    {
                //        _userslist.Add(new SelectListItem { Text = item.UserName, Value = item.Id.ToString() });
                //    }
                //}
            }

            return _userslist;
        }
        public static List<SelectListItem> GetUser()
        {
            List<SelectListItem> _userslist = new List<SelectListItem>();
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);

            using (var db = new ApplicationDbContext())
            {
                var result = db.Users.Select(x => new { x.Id, x.UserName }).ToList();
                if (result.Count() > 0)
                {
                    foreach (var item in result)
                    {
                        _userslist.Add(new SelectListItem { Text = item.UserName, Value = item.Id.ToString() });
                    }
                }
            }

            return _userslist;
        }
        public static Permissionviewmodel Getmenu()
        {
            List<Menumaster> _menulist = new List<Menumaster>();
            List<ModulePermission> _pm = new List<ModulePermission>();
            Permissionviewmodel vm = new Permissionviewmodel();
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);

            using (var db = new ApplicationDbContext())
            {
                _menulist = db.Menumaster.Include(x => x._SubMenumaster).ToList();
            }
            vm.Menumaster = _menulist.OrderBy(x => x.order).ToList();
            vm._pm = _pm;
            return vm;
        }

        public static Permissionviewmodel Getmenu(string UserId)
        {
        
            Permissionviewmodel vm = new Permissionviewmodel();
            PermissionMaster _pm = new PermissionMaster();
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);

            using (var db = new ApplicationDbContext())
            {
                _pm = db.PermissionMaster.Where(x => x.UserId == UserId).Include(x => x._Permission).Include(x => x._ModulePermission).FirstOrDefault();
            }
            vm.PermissionMaster = _pm;
            return vm;
        }
        public static string GetStation()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
          
            string CurrentSystem = Dns.GetHostByName(hostName).AddressList[0].ToString();
            return CurrentSystem;


        }
        public static void SavePermission(PermissionMaster pm)
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    db.PermissionMaster.Add(pm);
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    throw ex;

                }
                

            }


        }
        public static string Getmenuname(Guid Id)
        {
           
            Menumaster _master = new Menumaster();
            using (var db = new ApplicationDbContext())
            {
                _master = db.Menumaster.Where(x => x.MenuId == Id).FirstOrDefault();
               
            }

            return _master.Name;
        }
        public static string GetSubmenuname(Guid Id)
        {

            SubMenumaster _master = new SubMenumaster();
            using (var db = new ApplicationDbContext())
            {
                _master = db.SubMenumaster.Where(x => x.SubMenumasterId == Id).FirstOrDefault();

            }

            return _master.Action;
        }
        public static List<PermissionMaster> GetpermissionList()
        {

            List<PermissionMaster> _Permission = new List<PermissionMaster>();
            using (var db = new ApplicationDbContext())
            {
                _Permission = db.PermissionMaster.Include(x => x._User).Include(x => x._Permission).ToList();

            }

            return _Permission;
        }
        public static PermissionMaster GetpermissionById(Guid Id)
        {

            PermissionMaster _Permission = new PermissionMaster();
            using (var db = new ApplicationDbContext())
            {
                _Permission = db.PermissionMaster.Where(x => x.Id == Id).Include(x=>x._Permission).Include(x=>x._ModulePermission).FirstOrDefault();

            }

            return _Permission;
        }
        public static void UpdatePermission(PermissionMaster pm)
        {
            PermissionMaster _PermissionMaster = new PermissionMaster();

            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    _PermissionMaster = ctx.PermissionMaster.Where(x => x.Id == pm.Id).Include(x=>x._ModulePermission).Include(x=>x._Permission).FirstOrDefault();
                    if(_PermissionMaster!=null)
                    {
                        if(_PermissionMaster._Permission.Count>0)
                        {
                            ctx.Permission.RemoveRange(_PermissionMaster._Permission);
                        }
                        _PermissionMaster._Permission = pm._Permission;
                        if (_PermissionMaster._ModulePermission.Count > 0)
                        {
                            ctx.ModulePermission.RemoveRange(_PermissionMaster._ModulePermission);
                        }
                        _PermissionMaster._ModulePermission = pm._ModulePermission;
                        ctx.Entry(_PermissionMaster).State = EntityState.Modified;
                        ctx.SaveChanges();

                    }
                }

            }
            catch(Exception ex)
            {

            }
        }
    }
}