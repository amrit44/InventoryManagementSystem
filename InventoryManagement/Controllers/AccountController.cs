using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using InventoryManagement.Models;
using System.Collections.Generic;
using InventoryManagement.Helper;
using System.Data.Entity;
using InventoryManagement.Filter;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Web.Routing;

namespace InventoryManagement.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                
                case SignInStatus.Success:
                   
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }
        public ActionResult Customers_Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(GetUsers().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        private static IEnumerable<ApplicationUser> GetUsers()
        {
            List<ApplicationUser> _user = new List<ApplicationUser>();
            List<ApplicationUser> _userlst = new List<ApplicationUser>();

            using (var ctx = new ApplicationDbContext())
            {
                _user = ctx.Users.Include(x => x._CompanyMaster).Include(x => x._StoreMaster).Include(x => x.Roles).ToList();

            }
            if(_user.Count>0)
            {
                _userlst.Clear();
                foreach (var item in _user)
                {
                    ApplicationUser user = new ApplicationUser();
                    user.Id = item.Id;
                    user.UserName = item.UserName;
                    user.FirstName = item.FirstName;
                    user.LastName = item.LastName;
                    user.CompanyName = item._CompanyMaster.CompanyName;
                    if(item.StoreId!=null)
                    {
                        user.StoreId = item.StoreId;
                        user.StoreName = item._StoreMaster.StoreName;

                    }
                    else
                    {
                        user.StoreId = "0";
                        user.StoreName = "N/A";

                    }
                    if (item.Status == true)
                    {
                        user.Status = true;
                        user.StatusName ="Active";

                    }
                    else
                    {
                        user.Status = false;
                        user.StatusName = "In Active";

                    }
                    user.MobileNo = item.MobileNo;

                    user.UserRoleName = Commonhelper.getrolenameById(item.Roles.FirstOrDefault().RoleId);
                    user.UserRole = item.Roles.FirstOrDefault().RoleId;
                    _userlst.Add(user);
                }
                
            }
            return _userlst;
        }
        public ActionResult RegisteredAccounts()
        {
            //List<ApplicationUser> _user = null;
            //using (var ctx = new ApplicationDbContext())
            //{
            //    _user = ctx.Users.Include(x => x._StoreMaster).Include(x => x._CompanyMaster).Include(x => x.Roles).ToList();
            //}

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update(ApplicationUser _user)
        {
           
                //The model is valid - update the product and redisplay the grid.
                using (var ctx = new ApplicationDbContext())
                {
                    ApplicationUser user = new ApplicationUser();
                    user = ctx.Users.Where(x => x.Id == _user.Id).FirstOrDefault();
                    if(user!=null)
                    {
                        user.FirstName = _user.FirstName;
                        user.LastName = _user.LastName;
                        user.Status = _user.Status;
                        user.UserRole= _user.UserRole;
                        user.MobileNo = _user.MobileNo;
                    }
                    var Usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    string Existinguserrole = string.Empty;
                    Existinguserrole = Commonhelper.Rolename(_user.UserRole);
                    Usermanager.RemoveFromRole(user.Id, Existinguserrole);
                    Usermanager.AddToRole(user.Id, _user.UserRoleName);
                    ctx.Entry(user).State = EntityState.Modified;
                    ctx.SaveChanges();
                  }

                    //GridRouteValues() is an extension method which returns the 
                    //route values defining the grid state - current page, sort expression, filter etc.
                    RouteValueDictionary routeValues = this.GridRouteValues();

                return RedirectToAction("Customers_Read", Commonhelper.GetAll());
           

            
        }
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ApplicationUser user = new ApplicationUser();
            return View(user);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                Guid CompanyId = new Guid();
                if(Request.Cookies["CompanyId"] !=null)
                {
                    CompanyId =new Guid(Commonhelper.GetCookie("CompanyId"));
                }
                
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Password = model.Password, FirstName = model.FirstName, LastName = model.LastName, MobileNo = model.MobileNo, createdby = User.Identity.GetUserId(), Datecreated = DateTime.Now, Status = true,StoreId=model.StoreId,CompanyId= CompanyId.ToString(), RegisteredDeailer=false, TaxExempted=false, IsVendor=false, IsRetailCustomer=false, IsWholeCustomer=false };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    /////await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    var Usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    Usermanager.AddToRole(user.Id, model.UserRole);
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("RegisteredAccounts");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        [PermissionsAttribute(Action = "UserLevelPermission", Permission = "Isview")]
        public ActionResult UserLevelPermission()
        {
            List<PermissionMaster> _Permission  = new List<PermissionMaster>();
            _Permission = Commonhelper.GetpermissionList();

            return View(_Permission);
        }
        [PermissionsAttribute(Action = "UserLevelPermission", Permission = "IsAdd")]
        public ActionResult UserPermission()
        {

            return View();
        }
     
        public PartialViewResult GetMenu(string UserId)
        {
            Permissionviewmodel vm = new Permissionviewmodel();
            List<Menumaster> _lst = new List<Menumaster>();
            if(UserId==null)
            {
                vm = Commonhelper.Getmenu();
            }
            else
            {
               // vm = Commonhelper.Getmenu(UserId);
            }
            return PartialView("_Permission", vm);
        }
        [HttpPost]
        public ActionResult UserPermission(Permissionviewmodel vm,string Search,string Save)
        {
            List<string> result = new List<string>();
            PermissionMaster _master = new PermissionMaster();
            List<ModulePermission> p = new List<ModulePermission>();
            Permissionviewmodel searchmodel = new Permissionviewmodel();
           
                Guid CompanyId = new Guid();
                if (Request.Cookies["CompanyId"] != null)
                {
                    CompanyId = new Guid(Commonhelper.GetCookie("CompanyId"));
                }
                if (vm.PermissionMaster!=null)
                {
                    _master.Id = Guid.NewGuid().ToString();
                    _master.CompanyId = CompanyId.ToString();
                    _master.UserId = vm.PermissionMaster.UserId;
                    _master.CreatedBy= User.Identity.GetUserId();
                    _master.DateCreated = DateTime.Now;
                    _master.Workstation = Commonhelper.GetStation();
                   

                }
                if (vm.Menumaster.Count() > 0)
                {
                    foreach (var item in vm.Menumaster)
                    {
                        

                     foreach (var child in item._SubMenumaster)
                      {
                        if(child.IsAdd==true || child.IsEdit==true || child.Isview==true || child.Isdelete==true)
                        {
                            ModulePermission mp = new ModulePermission();
                            mp.Id = Guid.NewGuid().ToString();
                            mp.MenuId = item.MenuId;
                            mp.SubMenuId = child.SubMenumasterId;
                            mp.IsAdd = child.IsAdd;
                            mp.IsEdit = child.IsEdit;
                            mp.Isview = child.Isview;
                            mp.Isdelete = child.Isdelete;
                            mp.DisplayOrder = child.order;
                            mp.Displayclass = child.Displayclass;
                            mp.DisplayName = child.DisplayName;
                            _master._ModulePermission.Add(mp);
                        }
                           
                        }
                       
                    }
                    try
                    {
                        Commonhelper.SavePermission(_master);
                    return Content("<script language='javascript' type='text/javascript'>alert('permission saved successfully!');</script>");

    
                    }
                    catch (Exception ex)
                    {

                    }
                   }
                  return Json(result, JsonRequestBehavior.AllowGet);
        
           
           
            ///return View();
        }
        //
        // POST: /Account/LogOff

        [PermissionsAttribute(Action = "UserLevelPermission", Permission = "IsEdit")]
        public ActionResult EditPermission(Guid Id)
        {
           PermissionMaster _Permission = new PermissionMaster();
            Permissionviewmodel vm = new Permissionviewmodel();
            vm = Commonhelper.GetpermissionById(Id);
            vm.UserId = Id.ToString();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Updatepermission(Permissionviewmodel vm)
        {
            List<string> result = new List<string>();
            PermissionMaster _master = new PermissionMaster();
            List<ModulePermission> p = new List<ModulePermission>();
            Guid CompanyId = new Guid();
            if (Request.Cookies["CompanyId"] != null)
            {
                CompanyId = new Guid(Commonhelper.GetCookie("CompanyId"));
            }
            Permissionviewmodel searchmodel = new Permissionviewmodel();
            if (vm.PermissionMaster != null)
            {
                _master.Id = vm.Id;
                _master.CompanyId = CompanyId.ToString();
                _master.UserId = vm.UserId;
                _master.ModifiedBy = User.Identity.GetUserId();
                _master.Datemodified = DateTime.Now;
                _master.Workstation = Commonhelper.GetStation();


            }
            if (vm.Menumaster.Count() > 0)
            {
                foreach (var item in vm.Menumaster)
                {
                  
                   
                    foreach (var child in item._SubMenumaster)
                    {
                        if (child.IsAdd == true || child.IsEdit == true || child.Isview == true || child.Isdelete == true)
                        {
                            ModulePermission mp = new ModulePermission();
                            mp.Id = Guid.NewGuid().ToString();
                            mp.MenuId = item.MenuId;
                            mp.SubMenuId = child.SubMenumasterId;
                            mp.IsAdd = child.IsAdd;
                            mp.IsEdit = child.IsEdit;
                            mp.Isview = child.Isview;
                            mp.Isdelete = child.Isdelete;
                            mp.DisplayOrder = child.order;
                            mp.Displayclass = child.Displayclass;
                            mp.DisplayName = child.DisplayName;
                            _master._ModulePermission.Add(mp);


                        }
                           
                    }
                   
                }
                try
                {
                    Commonhelper.UpdatePermission(_master);
                    return RedirectToAction("RegisteredAccounts");
                    ////return Content("<script language='javascript' type='text/javascript'>alert('permission update successfully!');</script>");


                }
                catch (Exception ex)
                {

                }
            }
            //List<Permission> permissions = new List<Permission>();
            //List<ModulePermission> _MPermission = new List<ModulePermission>();
            //try
            //{
            //    if(pm!=null)
            //    {
            //        pm.Datemodified = DateTime.Now;
            //        pm.ModifiedBy = User.Identity.GetUserId();
            //        if (pm._Permission.Count() > 0)
            //        {
            //            foreach(var item in pm._Permission)
            //            {
            //                Permission p = new Permission();
            //                p.DisplayOrder = item.DisplayOrder;
            //                p.Id = Guid.NewGuid();
            //                p.Isactive = item.Isactive;
            //                p.MenuId = item.MenuId;
            //                p.PermissionMasterId = pm.Id;
            //                p.Displayclass = item.Displayclass;
            //                p.DisplayName = item.DisplayName;
            //                permissions.Add(p);

            //            }
            //            pm._Permission.Clear();
            //            pm._Permission = permissions;
            //            foreach(var child in pm._ModulePermission)
            //            {
            //                ModulePermission mp = new ModulePermission();
            //                mp.Id = Guid.NewGuid();
            //                mp.PermissionMasterId = pm.Id;
            //                mp.MenuId = child.MenuId;
            //                mp.SubMenuId = child.SubMenuId;
            //                mp.IsAdd = child.IsAdd;
            //                mp.IsEdit = child.IsEdit;
            //                mp.Isview = child.Isview;
            //                mp.Isdelete = child.Isdelete;
            //                mp.DisplayOrder = child.DisplayOrder;
            //                mp.Displayclass = child.Displayclass;
            //                mp.DisplayName = child.DisplayName;
            //                _MPermission.Add(mp);
            //            }
            //            pm._ModulePermission.Clear();
            //            pm._ModulePermission.AddRange(_MPermission);
            //            try
            //            {
            //                Commonhelper.SavePermission(_master);
            //                /// Commonhelper.UpdatePermission(pm);
            //            }
            //            catch(Exception ex)
            //            {
            //                result.Add("Internal server Issue try again.");
            //            }
            //        }

            //    }
            //    return Content("<script language='javascript' type='text/javascript'>alert('permission update successfully!');</script>");
            //    //result.Add("permission update successfully.");
            //    //ModelState.Clear();
            //    //return Json(new { url = Url.Action("UserLevelPermission") });

            //}
            //catch(Exception ex)
            //{
            //    result.Add("Internal server Issue try again.");

            //}


            return Json(result);

        }
        [HttpGet]
        public ActionResult Vendor()
        {
            string fileName = string.Empty;
            DateTime fileCreationDatetime = DateTime.Now;
            fileName = string.Format("{0}.pdf", fileCreationDatetime.ToString(@"yyyyMMdd") + "_" + fileCreationDatetime.ToString(@"HHmmss"));
            string pdfPath = Server.MapPath(@"~\PDFs\") + fileName;
            using (FileStream msReport = new FileStream(pdfPath, FileMode.Create))
            {
                using (Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 140f, 10f))
                {
                    try
                    {
                        // step 2
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, msReport);
                        pdfWriter.PageEvent = new ITextEvents();

                        //open the stream 
                        pdfDoc.Open();
                        //var path = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/credit/paypal.png");
                        //Image tif = Image.GetInstance(path);

                        //tif.ScalePercent(24f);

                        //tif.SetAbsolutePosition(pdfDoc.PageSize.Width - 36f - 72f,
                        //pdfDoc.PageSize.Height - 36f - 216.6f);

                        //pdfDoc.Add(tif);


                        //Paragraph para = new Paragraph("Hello world. Checking Header Footer", new Font(Font.FontFamily.HELVETICA, 22));
                        //para.Alignment = Element.ALIGN_CENTER;
                        //pdfDoc.Add(para);
                        PdfPTable table = new PdfPTable(4);
                        table.DefaultCell.BackgroundColor = BaseColor.BLACK;          
                        table.PaddingTop = 0;
                        table.TotalWidth = 510;
                        table.SpacingBefore = 0;
                        table.SpacingAfter=0;
                        table.LockedWidth = true;

                        PdfPCell header = new PdfPCell(new Phrase("Header"));

                        header.Colspan = 4;

                        table.AddCell(header);

                        table.AddCell("Cell 1");

                        table.AddCell("Cell 2");

                        table.AddCell("Cell 3");

                        table.AddCell("Cell 4");

                        PdfPTable nested = new PdfPTable(1);

                        nested.AddCell("Nested Row 1");

                        nested.AddCell("Nested Row 2");

                        nested.AddCell("Nested Row 3");

                        PdfPCell nesthousing = new PdfPCell(nested);

                        nesthousing.Padding = 0f;

                        table.AddCell(nesthousing);

                        PdfPCell bottom = new PdfPCell(new Phrase("bottom"));

                        bottom.Colspan = 3;

                        table.AddCell(bottom);

                        pdfDoc.Add(table);
                        pdfDoc.NewPage();

                        pdfDoc.Close();
                    }
                    catch (Exception ex)
                    {
                        //handle exception
                    }
                    finally
                    {
                    }
                }
            }
            return View();
        }
        public class ITextEvents : PdfPageEventHelper
        {
            // This is the contentbyte object of the writer
            PdfContentByte cb;

            // we will put the final number of pages in a template
            PdfTemplate headerTemplate, footerTemplate;

            // this is the BaseFont we are going to use for the header / footer
            BaseFont bf = null;

            // This keeps track of the creation time
            DateTime PrintTime = DateTime.Now;

            #region Fields
            private string _header;
            #endregion

            #region Properties
            public string Header
            {
                get { return _header; }
                set { _header = value; }
            }
            #endregion

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                try
                {
                    PrintTime = DateTime.Now;
                    bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb = writer.DirectContent;
                   
                    headerTemplate = cb.CreateTemplate(100, 100);
                    footerTemplate = cb.CreateTemplate(50, 50);
                }
                catch (DocumentException de)
                {
                }
                catch (System.IO.IOException ioe)
                {
                }
            }

            public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnEndPage(writer, document);
                iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                Phrase p1Header = new Phrase("Sample Header Here", baseFontNormal);
                var path = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/credit/paypal.png");


                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(path);
                jpg.ScaleToFit(100f, 100f);
                jpg.SpacingBefore = 0f;
                jpg.SpacingAfter = 1f;

                jpg.Alignment = Element.ALIGN_LEFT;
                jpg.SetAbsolutePosition(100, 100);
                //Create PdfTable object
                PdfPTable pdfTab = new PdfPTable(3);

                //We will have to create separate cells to include image logo and 2 separate strings
                //Row 1
                PdfPCell pdfCell1 = new PdfPCell(jpg);
                PdfPCell pdfCell2 = new PdfPCell(jpg);
                PdfPCell pdfCell3 = new PdfPCell(jpg);
                String text = "Page " + writer.PageNumber + " of ";

                //Add paging to header
                //{
                //    cb.BeginText();
                //    cb.SetFontAndSize(bf, 12);
                //    cb.SetTextMatrix(document.PageSize.GetRight(200), document.PageSize.GetTop(45));
                //    cb.ShowText(text);
                //    cb.EndText();
                //    float len = bf.GetWidthPoint(text, 12);
                //    //Adds "12" in Page 1 of 12
                //    cb.AddTemplate(headerTemplate, document.PageSize.GetRight(200) + len, document.PageSize.GetTop(45));
                //}
                //Add paging to footer
                {
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 12);
                    cb.SetTextMatrix(document.PageSize.GetRight(180), document.PageSize.GetBottom(30));
                    cb.ShowText(text);
                    cb.EndText();
                    float len = bf.GetWidthPoint(text, 12);
                    cb.AddTemplate(footerTemplate, document.PageSize.GetRight(180) + len, document.PageSize.GetBottom(30));
                }
            
                //Row 2
                PdfPCell pdfCell4 = new PdfPCell();
        
                //Row 3 
                PdfPCell pdfCell5 = new PdfPCell();
                PdfPCell pdfCell6 = new PdfPCell();
                PdfPCell pdfCell7 = new PdfPCell();
               

                //set the alignment of all three cells and set border to 0
                pdfCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                pdfCell4.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell5.HorizontalAlignment = Element.ALIGN_LEFT;
                pdfCell6.HorizontalAlignment = Element.ALIGN_CENTER;
                pdfCell7.HorizontalAlignment = Element.ALIGN_CENTER;

                pdfCell2.VerticalAlignment = Element.ALIGN_BOTTOM;
                pdfCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfCell4.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfCell5.VerticalAlignment = Element.ALIGN_TOP;
                pdfCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfCell7.VerticalAlignment = Element.ALIGN_MIDDLE;

                pdfCell4.Colspan = 3;

                pdfCell1.Border = 0;
                pdfCell2.Border = 0;
                pdfCell3.Border = 0;
                pdfCell4.Border = 0;
                pdfCell5.Border = 0;
                pdfCell6.Border = 0;
                pdfCell7.Border = 0;

                //add all three cells into PdfTable
                pdfTab.AddCell(pdfCell1);
                pdfTab.AddCell(pdfCell2);
                pdfTab.AddCell(pdfCell3);
                pdfTab.AddCell(pdfCell4);
                pdfTab.AddCell(pdfCell5);
                pdfTab.AddCell(pdfCell6);
                pdfTab.AddCell(pdfCell7);

                pdfTab.TotalWidth = document.PageSize.Width - 80f;
                pdfTab.WidthPercentage = 70;
                //pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;    

                //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
                //first param is start row. -1 indicates there is no end row and all the rows to be included to write
                //Third and fourth param is x and y position to start writing
                pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
                //set pdfContent value

                //Move the pointer and draw line to separate header section from rest of page
                cb.MoveTo(40, document.PageSize.Height - 100);
                cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 100);
                cb.Stroke();

                //Move the pointer and draw line to separate footer section from rest of page
                cb.MoveTo(40, document.PageSize.GetBottom(50));
                cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(50));
                cb.Stroke();
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);

                headerTemplate.BeginText();
                headerTemplate.SetFontAndSize(bf, 12);
                headerTemplate.SetTextMatrix(0, 0);
                headerTemplate.ShowText((writer.PageNumber - 1).ToString());
                headerTemplate.EndText();

                footerTemplate.BeginText();
                footerTemplate.SetFontAndSize(bf, 12);
                footerTemplate.SetTextMatrix(0, 0);
                footerTemplate.ShowText((writer.PageNumber - 1).ToString());
                footerTemplate.EndText();
            }
        }
        [HttpGet]
        public ActionResult CreateVendor()
        {
            Vendor v = new Vendor();
            return View(v);
        }
        [HttpPost]
        public ActionResult CreateVendor(Vendor v)
        {
           
            return View(v);
        }
        public async System.Threading.Tasks.Task<ActionResult> getStatebyCountry(string countryId)
        {
            List<SelectListItem> _lst = new List<SelectListItem>();
            _lst = Commonhelper.getState(countryId);
            return Json(_lst, JsonRequestBehavior.AllowGet);

        }
        public async System.Threading.Tasks.Task<ActionResult> getCitybyState(string stateId)
        {
            List<SelectListItem> _lst = new List<SelectListItem>();
            _lst = Commonhelper.getCity(stateId);
            return Json(_lst, JsonRequestBehavior.AllowGet);

        }
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //HttpCookie nameCookie = Request.Cookies["CompanyId"];

            ////Set the Expiry date to past date.
            //nameCookie.Expires = DateTime.Now.AddDays(-1);

            ////Update the Cookie in Browser.
            //Response.Cookies.Add(nameCookie);
            return RedirectToAction("Login", "Account");
           
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Unauthorised()
        {


            return View();
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}