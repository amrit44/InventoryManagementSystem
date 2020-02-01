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
        public static ApplicationUser GetCurrentUserDetails()
        {
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);

            return currentuser;
        }

        public static List<SelectListItem> GetUsers()
        {
            List<SelectListItem> _userslist = new List<SelectListItem>();
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);

            using (var db = new ApplicationDbContext())
            {
                _userslist = db.Database.SqlQuery<SelectListItem>("EXEC sp_GetUserList").ToList();
            
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
        public static SubMenumaster GetSubmenuById(Guid Id)
        {

          SubMenumaster _master = new SubMenumaster();
            using (var db = new ApplicationDbContext())
            {
                _master = db.SubMenumaster.Where(x => x.SubMenumasterId == Id).FirstOrDefault();
               
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
                _Permission = db.PermissionMaster.Where(x => x.Id == Id.ToString()).Include(x=>x._ModulePermission).FirstOrDefault();
               
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
            vmlst.Id = _Permission.Id.ToString();
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
            bool check = false;
            using (var ctx = new ApplicationDbContext())
            {
                SubMenumaster m = ctx.SubMenumaster.Where(x => x.Action == action).FirstOrDefault();
                PermissionMaster p = ctx.PermissionMaster.Where(x => x.UserId == userid).FirstOrDefault();
                ModulePermission mp = ctx.ModulePermission.Where(x => x.PermissionMasterId == p.Id.ToString() && x.MenuId == m.ParentId && x.SubMenuId == m.SubMenumasterId).FirstOrDefault();
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
            SubMenumaster sm = new SubMenumaster();
            List<ModulePermission> _pm = new List<ModulePermission>();
            Permissionviewmodel vm = new Permissionviewmodel();
            Permissionviewmodel vmlst = new Permissionviewmodel();
            InventoryManagement.Models.Permissionviewmodel _PermissionMaster = new InventoryManagement.Models.Permissionviewmodel();
            List<MenuGroup> lst = new List<MenuGroup>();
        
            using (var db = new ApplicationDbContext())
            {
                _Permission = db.PermissionMaster.Where(x => x.UserId == currentuser.Id).Include(x => x._ModulePermission).FirstOrDefault();
            }
            if(_Permission!=null)
            {
                foreach (var _m in _Permission._ModulePermission.ToList())
                {
                    MenuGroup mg = new MenuGroup();
                    mg.Id = _m.MenuId;
                    lst.Add(mg);
                }
                var resultlambaorderbyelement = lst.GroupBy(stu => stu.Id).Select(g => new { Key = g.Key });

                foreach (var _menu in resultlambaorderbyelement)
                {
                    Menumaster _Menumaster = new Menumaster();
                    _Menumaster = GetmenuById(_menu.Key);
                    foreach (var item in _Permission._ModulePermission.Where(x => x.MenuId == _menu.Key).ToList())
                    {
                        sm = GetSubmenuById(item.SubMenuId);
                        _Menumaster._SubMenumaster.Add(sm);
                    }

                    _PermissionMaster.Menumaster.Add(_Menumaster);
                }
            }
           
          
              
           
            return _PermissionMaster;
        }
        public static List<ItemOptionalDetails> GetOptionalFieldsList()
        {
            List<OptionalFields> lst = new List<OptionalFields>();
            List<ItemOptionalDetails> _lst = new List<ItemOptionalDetails>();

            using (var db = new ApplicationDbContext())
            {
                lst = db.OptionalFields.Where(x => x.Status == true).ToList();
                if(lst.Count>0)
                {
                    foreach(var item in lst)
                    {
                        ItemOptionalDetails vm = new ItemOptionalDetails();
                        vm.OptionalId = item.Id;
                        vm.Description = item.option1;
                        _lst.Add(vm);
                    }
                }
            }

            return _lst;
        }
        public static void SaveItem(ItemMaster master)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    db.ItemMaster.Add(master);
                    db.SaveChanges();
                }
                catch(Exception ex)
                {

                }

            }

            
        }

        public static void UpdateItem(ItemMaster master)
        {
            ItemMaster _master = new ItemMaster();
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    _master = db.ItemMaster.Include(x=>x.ItemOptionalDetails).Where(x => x.Id == master.Id).FirstOrDefault();
                    if(_master!=null)
                    {
                        _master.Id = master.Id;
                        _master.StoreId = master.StoreId;
                        _master.CompanyId = master.CompanyId;
                        _master.ProductCode = master.ProductCode;
                        _master.BarCode = master.BarCode;
                        _master.SkuCode = master.SkuCode;
                        _master.SapCode = master.SapCode;
                        _master.Category = master.Category;
                        _master.SubCategory = master.SubCategory;
                        _master.ProductName = master.ProductName;
                        _master.Brand = master.Brand;
                        _master.Size = master.Size;
                        _master.Quality = master.Quality;
                        _master.Gst = master.Gst;
                        _master.Reorderlevel = master.Reorderlevel;
                        _master.Mrp = master.Mrp;
                        _master.Costprice = master.Costprice;
                        _master.Sellprice = master.Sellprice;
                        _master.offer = master.offer;
                        _master.FinancialYear = master.FinancialYear;
                        _master.workstation = GetStation();
                        _master.HsnCode = master.HsnCode;
                        _master.MaximumQuantity = master.MaximumQuantity;
                        _master.MinimumQuantity = master.MinimumQuantity;
                        _master.BoxQuantity = master.BoxQuantity;
                        _master.IsUnique = master.IsUnique;
                        _master.Mou = master.Mou;
                        _master.SubMou = master.SubMou;
                        _master.Color = master.Color;
                        _master.ItemOrder = master.ItemOrder;
                        _master.ModifiedDate = DateTime.Now;
                        _master.ModifiedBy = master.ModifiedBy;

                        if(master.ItemOptionalDetails.Count>0)
                        {
                            db.Entry(_master).State = EntityState.Modified;
                           
                            db.ItemOptionalDetails.RemoveRange(_master.ItemOptionalDetails);
                            _master.ItemOptionalDetails.AddRange(master.ItemOptionalDetails);
                        }
                    }
                    db.Entry(_master).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }


        }


        public static void SaveCategory(CategoryMaster master)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {
                   
                    db.CategoryMaster.Add(master);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                }

            }


        }
        public static CategoryMaster GetCategoryById(string Id)
        {
            CategoryMaster master = new CategoryMaster();
            using (var db = new ApplicationDbContext())
            {
                try
                {

                    master = db.CategoryMaster.Where(x => x.Id == Id).FirstOrDefault();
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                }

            }

            return master;
        }
        public static void UpdateCategory(CategoryMaster master)
        {
            CategoryMaster _master = new CategoryMaster();
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    _master = db.CategoryMaster.Where(x => x.Id == master.Id).FirstOrDefault();
                    if(_master!=null)
                    {
                        _master.Name = master.Name;
                        _master.workstation = GetStation();
                        _master.ModifiedBy = master.ModifiedBy;
                        _master.ModifiedDate = DateTime.Now;
                        _master.Description = master.Description;
                        _master.FinancialYear = master.FinancialYear;

                        db.Entry(_master).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                   
                }
                catch (Exception ex)
                {

                }

            }


        }
        public static void UpdateSubCategory(SubCategoryMaster master)
        {
            SubCategoryMaster _master = new SubCategoryMaster();
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    _master = db.SubCategoryMaster.Where(x => x.Id == master.Id).FirstOrDefault();
                    if (_master != null)
                    {
                        _master.Name = master.Name;
                        _master.CategoryId = master.CategoryId;
                        _master.workstation = GetStation();
                        _master.ModifiedBy = master.ModifiedBy;
                        _master.ModifiedDate = DateTime.Now;
                        _master.Description = master.Description;
                        _master.FinancialYear = master.FinancialYear;

                        db.Entry(_master).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                catch (Exception ex)
                {

                }

            }


        }
        public static List<SubCategoryMaster> GetSubCategory()
        {

            List<SubCategoryMaster> _master = new List<SubCategoryMaster>();
            using (var db = new ApplicationDbContext())
            {
                _master = db.SubCategoryMaster.Include(x=>x._CategoryMaster).ToList();

            }

            return _master;
        }
        public static void SaveSubCategory(SubCategoryMaster master)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {

                    db.SubCategoryMaster.Add(master);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                }

            }


        }

        public static List<BrandMaster> GetBrand()
        {

            List<BrandMaster> _master = new List<BrandMaster>();
            using (var db = new ApplicationDbContext())
            {
                _master = db.BrandMaster.ToList();

            }

            return _master;
        }
        public static void SaveBrand(BrandMaster master)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {

                    db.BrandMaster.Add(master);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                }

            }


        }
        public static BrandMaster GetBrandById(string Id)
        {
            BrandMaster _BrandMaster = new BrandMaster();
            using (var db = new ApplicationDbContext())
            {
                try
                {

                    _BrandMaster = db.BrandMaster.Where(x => x.Id == Id).FirstOrDefault();
                    
                }
                catch (Exception ex)
                {

                }

            }
            return _BrandMaster;


        }
        public static void UpdateBrand(BrandMaster master)
        {
            BrandMaster _master = new BrandMaster();
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    _master = db.BrandMaster.Where(x => x.Id == master.Id).FirstOrDefault();
                    if(_master!=null)
                    {
                        _master.Name = master.Name;
                        _master.Description = master.Description;
                        _master.ModifiedBy = master.ModifiedBy;
                        _master.ModifiedDate = master.ModifiedDate;
                        _master.workstation = master.workstation;
                        _master.StoreId = master.StoreId;
                        _master.FinancialYear = master.FinancialYear;
                        db.Entry(_master).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                   
                }
                catch (Exception ex)
                {

                }

            }


        }

        public static List<ColorMaster> GetColor()
        {

            List<ColorMaster> _master = new List<ColorMaster>();
            using (var db = new ApplicationDbContext())
            {
                _master = db.ColorMaster.ToList();

            }

            return _master;
        }
        public static void SaveColor(ColorMaster master)
        {
            using (var db = new ApplicationDbContext())
            {
                try
                {

                    db.ColorMaster.Add(master);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                }

            }


        }
        public static ColorMaster GetColorById(string Id)
        {
            ColorMaster _BrandMaster = new ColorMaster();
            using (var db = new ApplicationDbContext())
            {
                try
                {

                    _BrandMaster = db.ColorMaster.Where(x => x.Id == Id).FirstOrDefault();

                }
                catch (Exception ex)
                {

                }

            }
            return _BrandMaster;


        }
        public static void Updatecolor(ColorMaster master)
        {
            ColorMaster _master = new ColorMaster();
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    _master = db.ColorMaster.Where(x => x.Id == master.Id).FirstOrDefault();
                    if (_master != null)
                    {
                        _master.Name = master.Name;
                        _master.Description = master.Description;
                        _master.ModifiedBy = master.ModifiedBy;
                        _master.ModifiedDate = master.ModifiedDate;
                        _master.workstation = master.workstation;
                        _master.StoreId = master.StoreId;
                        _master.FinancialYear = master.FinancialYear;
                        db.Entry(_master).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                catch (Exception ex)
                {

                }

            }


        }
        public static SubCategoryMaster GetSubCategoryById(string Id)
        {

           SubCategoryMaster _master = new SubCategoryMaster();
            using (var db = new ApplicationDbContext())
            {
                _master = db.SubCategoryMaster.Where(x => x.Id == Id).FirstOrDefault();

            }

            return _master;
        }
        public static bool GetBarcode(string barcode)
        {
            bool check = false;

            using (var db = new ApplicationDbContext())
            {
                check = db.ItemMaster.Any(x => x.BarCode == barcode);
            }
            return check;
        }

        public static bool Checkproductname(string productname)
        {
            bool check = false;

            using (var db = new ApplicationDbContext())
            {
                check = db.ItemMaster.Any(x => x.ProductName == productname);
            }
            return check;
        }
        public static bool CheckHsncode(string Hsncode)
        {
            bool check = false;

            using (var db = new ApplicationDbContext())
            {
                check = db.ItemMaster.Any(x => x.HsnCode == Hsncode);
            }
            return check;
        }
        public static bool CheckProductcode(string productcode)
        {
            bool check = false;

            using (var db = new ApplicationDbContext())
            {
                check = db.ItemMaster.Any(x => x.ProductCode == productcode);
            }
            return check;
        }
        public static bool Checkskucode(string skucode)
        {
            bool check = false;

            using (var db = new ApplicationDbContext())
            {
                check = db.ItemMaster.Any(x => x.SkuCode == skucode);
            }
            return check;
        }
        public static bool Checksapcode(string sapcode)
        {
            bool check = false;

            using (var db = new ApplicationDbContext())
            {
                check = db.ItemMaster.Any(x => x.SapCode == sapcode);
            }
            return check;
        }

        public static bool Checkcategoryname(string Name)
        {
            bool check = false;

            using (var db = new ApplicationDbContext())
            {
                check = db.CategoryMaster.Any(x => x.Name == Name);
            }
            return check;
        }
        public static bool CheckSubcategoryname(string categoryId,string Name)
        {
            bool check = false;

            using (var db = new ApplicationDbContext())
            {
                check = db.SubCategoryMaster.Any(x => x.Name == Name && x.CategoryId==categoryId);
            }
            return check;
        }

        public static bool CheckBrandname(string brandname)
        {
            bool check = false;

            using (var db = new ApplicationDbContext())
            {
                check = db.BrandMaster.Any(x => x.Name == brandname);
            }
            return check;
        }
        public static bool CheckColorname(string Colorname)
        {
            bool check = false;

            using (var db = new ApplicationDbContext())
            {
                check = db.ColorMaster.Any(x => x.Name == Colorname);
            }
            return check;
        }
        public static List<CategoryMaster> GetCategory()
        {

            List<CategoryMaster> _master = new List<CategoryMaster>();
            using (var db = new ApplicationDbContext())
            {
                _master = db.CategoryMaster.ToList(); 

            }

            return _master;
        }
        public static List<DropDownControl> GetSubCategoryByCategory(string CategoryId)
        {

            List<SubCategoryMaster> _master = new List<SubCategoryMaster>();
            List<DropDownControl> dopdownlst = new List<DropDownControl>();
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    _master = db.SubCategoryMaster.Where(x => x.CategoryId == CategoryId).ToList();
                    if(_master.Count()>0)
                    {
                        foreach(var subcat in _master.ToList())
                        {
                            dopdownlst.Add(new DropDownControl { Name=subcat.Name,Id=subcat.Id });
                        }
                    }

                }
                catch (Exception ex)
                {

                }

            }

            return dopdownlst;
        }
        public static List<BrandMaster> GetBrandMaster()
        {

            List<BrandMaster> _master = new List<BrandMaster>();
            using (var db = new ApplicationDbContext())
            {
                _master = db.BrandMaster.ToList();

            }

            return _master;
        }


        public static List<ItemMaster> GetItemMaster()
        {

            List<ItemMaster> _master = new List<ItemMaster>();
            using (var db = new ApplicationDbContext())
            {
                _master = db.ItemMaster.Include(x => x._CategoryMaster).Include(x => x._SubCategoryMaster).Include(x => x._BrandMaster).Include(x => x._StoreMaster).ToList();

            }

            return _master;
        }
        public static ItemMaster GetItemById(string Id)
        {
            List<ItemOptionalDetails> lst = new List<ItemOptionalDetails>();
            ItemMaster _master = new ItemMaster();
            using (var db = new ApplicationDbContext())
            {
                _master = db.ItemMaster.Include(x=>x.ItemOptionalDetails).Where(x => x.Id == Id).FirstOrDefault();
                if(_master.ItemOptionalDetails.Count>0)
                {
                    foreach(var item in _master.ItemOptionalDetails.ToList())
                    {
                        ItemOptionalDetails vm = new ItemOptionalDetails();
                        vm.Id = item.Id;
                        vm.ItemId = item.ItemId;
                        vm.OptionalValue = item.OptionalValue;
                        vm.OptionalId = item.OptionalId;
                        vm.Description = GetOptionname(item.OptionalId);
                        lst.Add(vm);
                    }
                    _master.ItemOptionalDetails.Clear();
                    _master.ItemOptionalDetails = lst;
                }
            }

            return _master;
        }

        public static void DeleteItemById(string Id)
        {
            ItemMaster _master = new ItemMaster();
            using (var db = new ApplicationDbContext())
            {
                try
                {
                    _master = db.ItemMaster.Where(x => x.Id == Id).FirstOrDefault();
                    if (_master != null)
                    {
                        try
                        {
                            _master.Isactive = false;
                            db.Entry(_master).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }


                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                
            }

          
        }
        public static string GetOptionname(string Id)
        {
            string description = string.Empty;
            using (var db = new ApplicationDbContext())
            {
                description = db.OptionalFields.Where(x => x.Id == Id).Select(x => x.option1).FirstOrDefault();
            }
           return description;
        }


        public static List<SelectListItem> getCounrty()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Text = "India", Value = "1" });
            lst.Add(new SelectListItem { Text = "Australia", Value = "2" });
            lst.Add(new SelectListItem { Text = "London", Value = "3" });
            return lst;
        }
        public static List<SelectListItem> getState(string countryId)
        {
            List<Dropdownviewmodel> lst = new List<Dropdownviewmodel>();
            List<SelectListItem> _lst = new List<SelectListItem>();
            lst.Add(new Dropdownviewmodel { Id="1",Name="Uttar Pradesh",CountryId="1" });
            lst.Add(new Dropdownviewmodel { Id = "2", Name = "Delhi", CountryId = "1" });
            lst.Add(new Dropdownviewmodel { Id = "3", Name = "Mumbai", CountryId = "1" });
            lst = lst.Where(x => x.CountryId == countryId).ToList();
            if(lst.Count>0)
            {
                foreach(var item in lst)
                {
                    _lst.Add(new SelectListItem { Text =item.Name, Value = item.Id.ToString() });
                }
            }
            return _lst;
        
        }
        public static List<SelectListItem> getCity(string stateId)
        {
            List<Dropdownviewmodel> lst = new List<Dropdownviewmodel>();
            List<SelectListItem> _lst = new List<SelectListItem>();
            lst.Add(new Dropdownviewmodel { Id = "1", Name = "Lucknow", StateId = "1" });
            lst.Add(new Dropdownviewmodel { Id = "2", Name = "Noida", StateId = "1" });
            lst.Add(new Dropdownviewmodel { Id = "3", Name = "Kanpur", StateId = "1" });
            lst = lst.Where(x => x.StateId == stateId).ToList();
            if (lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    _lst.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            return _lst;

        }

        public static void SaveOptionalFields(List<OptionalFields>lst)
        {

            using (var db = new ApplicationDbContext())
            {
                db.OptionalFields.AddRange(lst);
                db.SaveChanges();
            }
        }
        public static List<OptionalFields> GetOptionalList()
        {
            List<OptionalFields> lst = new List<OptionalFields>();
            using (var db = new ApplicationDbContext())
            {
                lst = db.OptionalFields.ToList();
                return lst;
            }
        }
        public static OptionalFields GetOptionalFieldbyId(string Id)
        {
         OptionalFields field = new OptionalFields();
            using (var db = new ApplicationDbContext())
            {
                field = db.OptionalFields.Where(x => x.Id == Id).FirstOrDefault();
                return field;
            }
        }
        public static OptionalFields UpdateFieldbyId(OptionalFields _OptionalFields)
        {
            OptionalFields field = new OptionalFields();
            using (var db = new ApplicationDbContext())
            {
                field = db.OptionalFields.Where(x => x.Id == _OptionalFields.Id).FirstOrDefault();
                if(field!=null)
                {
                    field.option1 = _OptionalFields.option1;
                    field.Status = _OptionalFields.Status;
                    db.Entry(field).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch(Exception ex)
                    {

                    }
                    
                }
                return field;
            }
        }
        public static List<SelectListItem> GetHierarchy()
        {
            List<SelectListItem> field = new List<SelectListItem>();
            List<Hierarchy> _field = new List<Hierarchy>();
            using (var db = new ApplicationDbContext())
            {
                _field = db.Hierarchy.ToList();
               if(_field.Count>0)
                {
                    foreach(var item in _field)
                    {
                        field.Add(new SelectListItem { Text=item.Name,Value=item.Id.ToString() });
                    }
                }
                return field;
            }
        }
    }
}