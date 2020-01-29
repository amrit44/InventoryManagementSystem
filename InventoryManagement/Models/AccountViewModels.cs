using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        //[EmailAddress]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class Modulemenu
    {
        
        public PermissionMaster _PermissionMaster { get; set; }
        public List<Menumaster> _Menumaster { get; set; }
        public List<SubMenumaster> _SubMenumaster { get; set; }
    }
    public class ModuleViewModel
    {
        public ModuleViewModel()
        {
            _Actionviewmodel = new List<Actionviewmodel>();
        }
        public string ModuleName { get; set; }
        public string Displayclass { get; set; }
        public int Order { get; set; }
        public List<Actionviewmodel> _Actionviewmodel { get; set; }
    }
    public class Actionviewmodel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public int Order { get; set; }
        public string DisplayName { get; set; }
        public string Displayclass { get; set; }
        public string DisplayLink { get; set; }
        public bool Isview { get; set; }
        public bool IsEdit { get; set; }
        public bool IsAdd { get; set; }
        public bool Isdelete { get; set; }
    }
    public class Permissionviewmodel
    {
        public Permissionviewmodel()
        {
            PermissionMaster = new PermissionMaster();
            Menumaster = new List<Menumaster>();
            _pm = new List<ModulePermission>();
        }
        public string Id { get; set; }
        public string UserId { get; set; }
        public PermissionMaster PermissionMaster { get; set; }
        public List<Menumaster> Menumaster { get; set; }
        public List<ModulePermission> _pm { get; set; }
    }
   
    public class MenuGroup
    {
        public Guid Id { get; set; }

    }
    public class DropDownControl
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class Dropdownviewmodel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CountryId { get; set; }
        public string StateId { get; set; }
    }
}
