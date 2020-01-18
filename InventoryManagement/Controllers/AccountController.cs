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

        public ActionResult RegisteredAccounts()
        {
            List<ApplicationUser> _user = null;
            using (var ctx = new ApplicationDbContext())
            {
                _user = ctx.Users.Include(x => x._StoreMaster).Include(x => x._CompanyMaster).Include(x => x.Roles).ToList();
            }

            return View(_user);
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
                
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Password = model.Password, FirstName = model.FirstName, LastName = model.LastName, MobileNo = model.MobileNo, createdby = User.Identity.GetUserId(), Datecreated = DateTime.Now, Status = true,StoreId=model.StoreId,CompanyId= CompanyId };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    var Usermanager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                    Usermanager.AddToRole(user.Id, model.UserRole);
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
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
        public ActionResult UserLevelPermission()
        {
            List<PermissionMaster> _Permission  = new List<PermissionMaster>();
            _Permission = Commonhelper.GetpermissionList();

            return View(_Permission);
        }
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
                vm = Commonhelper.Getmenu(UserId);
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
            if (Save!=null)
            {
                Guid CompanyId = new Guid();
                if (Request.Cookies["CompanyId"] != null)
                {
                    CompanyId = new Guid(Commonhelper.GetCookie("CompanyId"));
                }
                if (vm.PermissionMaster!=null)
                {
                    _master.Id = Guid.NewGuid();
                    _master.CompanyId = CompanyId;
                    _master.UserId = vm.PermissionMaster.UserId;
                    _master.CreatedBy= User.Identity.GetUserId();
                    _master.DateCreated = DateTime.Now;
                    _master.Workstation = Commonhelper.GetStation();
                   

                }
                if (vm.Menumaster.Count() > 0)
                {
                    foreach (var item in vm.Menumaster)
                    {
                        Permission permission = new Permission();
                        permission.Id = Guid.NewGuid();
                        permission.MenuId = item.MenuId;
                        permission.DisplayOrder = item.order;
                        if (item.IsSelect==true)
                        {
                            permission.Isactive = true;
                        }
                        else
                        {
                            permission.Isactive = false;
                        }

                        foreach (var child in item._SubMenumaster)
                        {
                           
                            ModulePermission mp = new ModulePermission();
                            mp.Id = Guid.NewGuid();
                            mp.MenuId = item.MenuId;
                            mp.SubMenuId = child.SubMenumasterId;
                            mp.IsAdd = child.IsAdd;
                            mp.IsEdit = child.IsEdit;
                            mp.Isview = child.Isview;
                            mp.Isdelete = child.Isdelete;
                            mp.DisplayOrder = child.order;
                           
                            _master._ModulePermission.Add(mp);
                        }
                        _master._Permission.Add(permission);
                    }
                    try
                    {
                        Commonhelper.SavePermission(_master);

                        result.Add("Permission save successfully...");
                       

                    }
                    catch (Exception ex)
                    {

                    }
                }
               
            }
            else if (Search != null)
            {

                searchmodel= Commonhelper.Getmenu(vm.PermissionMaster.UserId);
                return PartialView("_Permission", searchmodel);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
            ///return View();
        }
        //
        // POST: /Account/LogOff


        public ActionResult EditPermission(Guid Id)
        {
           PermissionMaster _Permission = new PermissionMaster();
            _Permission = Commonhelper.GetpermissionById(Id);
            return View(_Permission);
        }

        [HttpPost]
        public ActionResult Updatepermission(PermissionMaster pm)
        {
            List<string> result = new List<string>();
            List<Permission> permissions = new List<Permission>();
            List<ModulePermission> _MPermission = new List<ModulePermission>();
            try
            {
                if(pm!=null)
                {
                    pm.Datemodified = DateTime.Now;
                    pm.ModifiedBy = User.Identity.GetUserId();
                    if (pm._Permission.Count() > 0)
                    {
                        foreach(var item in pm._Permission)
                        {
                            Permission p = new Permission();
                            p.DisplayOrder = item.DisplayOrder;
                            p.Id = Guid.NewGuid();
                            p.Isactive = item.Isactive;
                            p.MenuId = item.MenuId;
                            p.PermissionMasterId = pm.Id;
                            permissions.Add(p);

                        }
                        pm._Permission.Clear();
                        pm._Permission = permissions;
                        foreach(var child in pm._ModulePermission)
                        {
                            ModulePermission mp = new ModulePermission();
                            mp.Id = Guid.NewGuid();
                            mp.PermissionMasterId = pm.Id;
                            mp.MenuId = child.MenuId;
                            mp.SubMenuId = child.SubMenuId;
                            mp.IsAdd = child.IsAdd;
                            mp.IsEdit = child.IsEdit;
                            mp.Isview = child.Isview;
                            mp.Isdelete = child.Isdelete;
                            mp.DisplayOrder = child.DisplayOrder;
                            _MPermission.Add(mp);
                        }
                        pm._ModulePermission.Clear();
                        pm._ModulePermission.AddRange(_MPermission);
                        try
                        {
                            Commonhelper.UpdatePermission(pm);
                        }
                        catch(Exception ex)
                        {
                            result.Add("Internal server Issue try again.");
                        }
                    }

                }
                result.Add("permission update successfully.");
                ModelState.Clear();
                return Json(new { url = Url.Action("UserLevelPermission") });
                
            }
            catch(Exception ex)
            {
                result.Add("Internal server Issue try again.");

            }


            return Json(result);

        }
        [HttpGet]

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            HttpCookie nameCookie = Request.Cookies["CompanyId"];

            //Set the Expiry date to past date.
            nameCookie.Expires = DateTime.Now.AddDays(-1);

            //Update the Cookie in Browser.
            Response.Cookies.Add(nameCookie);
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