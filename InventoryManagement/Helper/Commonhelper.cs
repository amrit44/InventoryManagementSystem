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
using System.Data.SqlClient;

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
            vm.Menumaster = _menulist.OrderBy(x => x.DisplayOrder).ToList();
           
            return vm;
        }

        //public static Permissionviewmodel Getmenu(string UserId)
        //{
        
        //    Permissionviewmodel vm = new Permissionviewmodel();
        //    PermissionMaster _pm = new PermissionMaster();
        //    var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);

        //    using (var db = new ApplicationDbContext())
        //    {
        //        _pm = db.PermissionMaster.Where(x => x.UserId == UserId).Include(x => x._Permission).Include(x => x._ModulePermission).FirstOrDefault();
        //    }
        //    vm.PermissionMaster = _pm;
        //    return vm;
        //}
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
        public static Menumaster GetmenuById(Guid Id)
        {

            Menumaster _master = new Menumaster();
            using (var db = new ApplicationDbContext())
            {
                _master = db.Menumaster.Where(x => x.MenuId == Id).FirstOrDefault();

            }

            return _master;
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
        public static Actionviewmodel GetActionName(Guid Id)
        {

            SubMenumaster _master = new SubMenumaster();
            Actionviewmodel _vm = new Actionviewmodel();
            using (var db = new ApplicationDbContext())
            {
                _master = db.SubMenumaster.Where(x => x.SubMenumasterId == Id).FirstOrDefault();
                if(_master!=null)
                {
                    _vm.Action = _master.Action;
                    _vm.Controller = _master.Controller;
                    _vm.Order = _master.order;
                    _vm.Displayclass = _master.Displayclass;
                    _vm.DisplayName = _master.DisplayName;
                    _vm.DisplayLink = _master.DisplayLink;
                }
            }

            return _vm;
        }
        public static List<SubMenumaster> GetSubmenuById(Guid Id)
        {

            List<SubMenumaster> _master = new List<SubMenumaster>();
            using (var db = new ApplicationDbContext())
            {
                _master = db.SubMenumaster.Where(x => x.SubMenumasterId == Id).ToList();
               
            }

            return _master;
        }
        public static List<PermissionMaster> GetpermissionList()
        {

            List<PermissionMaster> _Permission = new List<PermissionMaster>();
            using (var db = new ApplicationDbContext())
            {
                _Permission = db.PermissionMaster.Include(x => x._User).Include(x => x._ModulePermission).ToList();

            }

            return _Permission;
        }
        public static string _GetModuleslist(string UserId)
        {
            string DisplayMenu = string.Empty;
            using (var db = new ApplicationDbContext())
            {
                var Uservalue = new SqlParameter
                {
                    ParameterName = "USERID",
                    Value = UserId
                };
                DisplayMenu = db.Database.SqlQuery<string>("exec _GetModuleslist @USERID ", Uservalue).FirstOrDefault();
            }

            return DisplayMenu;
        }
        public static Permissionviewmodel GetpermissionById(Guid Id)
        {

            PermissionMaster _Permission = new PermissionMaster();
            List<Menumaster> _menulist = new List<Menumaster>();
            List<Menumaster> _menulst = new List<Menumaster>();
            List<SubMenumaster> _SubMenumasterlist = new List<SubMenumaster>();

            List<ModulePermission> _pm = new List<ModulePermission>();
            Permissionviewmodel vm = new Permissionviewmodel();
            Permissionviewmodel vmlst = new Permissionviewmodel();

            using (var db = new ApplicationDbContext())
            {
                _menulist = db.Menumaster.Include(x => x._SubMenumaster).ToList();
            }
            vm.Menumaster = _menulist.OrderBy(x => x.DisplayOrder).ToList();
            vm._pm = _pm;
            using (var db = new ApplicationDbContext())
            {
                _Permission = db.PermissionMaster.Where(x => x.Id == Id).Include(x=>x._ModulePermission).FirstOrDefault();
               
            }
            if(_menulist.Count>0)
            {
                foreach(var menu in _menulist.ToList())
                {
                    
                    Menumaster ms = new Menumaster();
                    ms.MenuId = menu.MenuId;
                    ms.DisplayOrder = menu.DisplayOrder;
                    ms.Name = menu.Name;
                    ms.DisplayName = menu.DisplayName;
                    ms.Displayclass = menu.Displayclass;
                    _SubMenumasterlist = new List<SubMenumaster>();
                        foreach (var sub in menu._SubMenumaster.Where(x=>x.ParentId==menu.MenuId))
                        {
                           
                            SubMenumaster submenumaster = new SubMenumaster();
                            submenumaster.SubMenumasterId = sub.SubMenumasterId;
                            submenumaster.ParentId = sub.ParentId;
                            submenumaster.Controller = sub.Controller;
                            submenumaster.Action = sub.Action;
                            submenumaster.DisplayName = sub.DisplayName;
                            submenumaster.Displayclass = sub.Displayclass;
                            submenumaster.DisplayLink = sub.DisplayLink;
                            submenumaster.order = sub.order;
                            bool checkadd = _Permission._ModulePermission.Where(x=>x.MenuId==menu.MenuId && x.SubMenuId==sub.SubMenumasterId).Select(x => x.IsAdd).FirstOrDefault();
                            if(checkadd==true)
                            {
                                submenumaster.IsAdd = true;
                                ms.IsSelect = true;
                            }
                            bool checkedit = _Permission._ModulePermission.Where(x => x.MenuId == menu.MenuId && x.SubMenuId == sub.SubMenumasterId).Select(x => x.IsEdit).FirstOrDefault();
                            if (checkedit == true)
                            {
                                submenumaster.IsEdit = true;
                                ms.IsSelect = true;
                            }
                            bool checkview= _Permission._ModulePermission.Where(x => x.MenuId == menu.MenuId && x.SubMenuId == sub.SubMenumasterId).Select(x => x.Isview).FirstOrDefault();
                            if (checkview == true)
                            {
                                submenumaster.Isview = true;
                                ms.IsSelect = true;
                            }
                            bool checkdelete = _Permission._ModulePermission.Where(x => x.MenuId == menu.MenuId && x.SubMenuId == sub.SubMenumasterId).Select(x => x.Isdelete).FirstOrDefault();
                            if (checkdelete == true)
                            {
                                submenumaster.Isdelete = true;
                                ms.IsSelect = true;
                            }
                            _SubMenumasterlist.Add(submenumaster);
                        }
                         ms._SubMenumaster = _SubMenumasterlist;
                       
                       _menulst.Add(ms);
                  
                }
            }
            vmlst.Id = _Permission.Id;
            vmlst.UserId = _Permission.UserId;
            vmlst.Menumaster = _menulst.OrderBy(x => x.DisplayOrder).ToList();
            return vmlst;
        }
        public static void UpdatePermission(PermissionMaster pm)
        {
            PermissionMaster _PermissionMaster = new PermissionMaster();

            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    _PermissionMaster = ctx.PermissionMaster.Where(x => x.Id == pm.Id).Include(x => x._ModulePermission).FirstOrDefault();
                    if (_PermissionMaster != null)
                    {
                        _PermissionMaster.ModifiedBy = pm.ModifiedBy;
                        _PermissionMaster.Datemodified = DateTime.Now;
                       
                    
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

      

        public static bool checkpermission(string userid,string action,string permission)
        {
            bool check = true;
            using (var ctx = new ApplicationDbContext())
            {
                SubMenumaster m = ctx.SubMenumaster.Where(x => x.Action == action).FirstOrDefault();
                PermissionMaster p = ctx.PermissionMaster.Where(x => x.UserId == userid).FirstOrDefault();
                ModulePermission mp = ctx.ModulePermission.Where(x => x.PermissionMasterId == p.Id && x.MenuId == m.ParentId && x.SubMenuId == m.SubMenumasterId).FirstOrDefault();
                if (mp != null)
                {
                    if (permission == "IsAdd")
                    {
                        check = mp.IsAdd;
                    }
                    else if (permission == "IsEdit")
                    {
                        check = mp.IsEdit;
                    }
                    else if (permission == "Isdelete")
                    {
                        check = mp.Isdelete;
                    }
                    else if (permission == "Isview")
                    {
                        check = mp.Isview;
                    }
                }
            }
            return check;
        }


        public static Permissionviewmodel GetUserpermission()
        {
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);
            PermissionMaster _Permission = new PermissionMaster();
            List<Menumaster> _menulist = new List<Menumaster>();
            List<Menumaster> _menulst = new List<Menumaster>();
            List<SubMenumaster> _SubMenumasterlist = new List<SubMenumaster>();

            List<ModulePermission> _pm = new List<ModulePermission>();
            Permissionviewmodel vm = new Permissionviewmodel();
            Permissionviewmodel vmlst = new Permissionviewmodel();
            InventoryManagement.Models.Permissionviewmodel _PermissionMaster = new InventoryManagement.Models.Permissionviewmodel();

        
            using (var db = new ApplicationDbContext())
            {
                _Permission = db.PermissionMaster.Where(x => x.UserId == currentuser.Id).Include(x => x._ModulePermission).FirstOrDefault();
            }
      
               Menumaster _Menumaster = new Menumaster();
               // _Menumaster.DisplayName=GetmenuById(itemmenu)
                if (_Permission._ModulePermission.Count() > 0)
                {
                    foreach (var item in _Permission._ModulePermission.ToList())
                    {


                        _Menumaster = GetmenuById(item.MenuId);
                        _PermissionMaster.Menumaster.Add(_Menumaster);
                    }
                    //foreach (var item in _Permission._ModulePermission.ToList())
                    //{


                    //    _SubMenumasterlist = GetSubmenuById(item.SubMenuId);
                    //    _Menumaster._SubMenumaster = _SubMenumasterlist;


                    //}
                }
            
            
           
            return _PermissionMaster;
        }
    }
}