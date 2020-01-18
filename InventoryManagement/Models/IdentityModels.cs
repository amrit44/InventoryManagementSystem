using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InventoryManagement.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [ForeignKey("_CompanyMaster")]

        public Guid CompanyId { get; set; }
        public CompanyMaster _CompanyMaster { get; set; }

        public string StoreId { get; set; }
        [ForeignKey("StoreId")]
        public StoreMaster _StoreMaster { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string MobileNo { get; set; }

        public bool Status { get; set; }
        public DateTime? UserExpiry { get; set; }
        public DateTime Datecreated { get; set; }
        public string createdby { get; set; }
        public DateTime? Datemodified { get; set; }
        public string modifiedby { get; set; }
        [NotMapped]
        public string UserRole { get; set; }

        //public virtual IdentityUserRole IdentityUserRole { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    [Table("CompanyMaster")]
    public class CompanyMaster
    {
        [Key]
        public Guid CompanyId { get; set; }
        [Required]
        public string CompanyCode { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailId { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public bool Status { get; set; }
        [Required]
        public int FinancialYear { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string StationName { get; set; }
        public string DocumentNo { get; set; }
    }
    [Table("Menumaster")]
    public class Menumaster
    {
        public Menumaster()
        {

            _SubMenumaster = new List<SubMenumaster>();
      
        }
        [Key]
        public Guid MenuId { get; set; }
        [ForeignKey("_CompanyMaster")]
        public Guid CompanyId { get; set; }
        public CompanyMaster _CompanyMaster { get; set; }
        public string Name { get; set; }
        public int order { get; set; }
        public bool status { get; set; }

        public List<SubMenumaster> _SubMenumaster { get; set; }
        [NotMapped]
        public bool IsSelect{ get; set; }

    }
    [Table("SubMenumaster")]
    public class SubMenumaster
    {
        [Key]
        public Guid SubMenumasterId { get; set; }
        [ForeignKey("_Menumaster")]
        public Guid ParentId { get; set; }
        public Menumaster _Menumaster { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int order { get; set; }
        public bool status { get; set; }
        [NotMapped]
        public bool Isview { get; set; }
        [NotMapped]
        public bool IsEdit { get; set; }
        [NotMapped]
        public bool IsAdd { get; set; }
        [NotMapped]
        public bool Isdelete { get; set; }

    }
    [Table("PermissionMaster")]
    public class PermissionMaster
    {
        public PermissionMaster()
        {
            _Permission = new List<Permission>();
            _ModulePermission = new List<ModulePermission>();
        }
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser _User { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? Datemodified { get; set; }
        public string ModifiedBy { get; set; }
        public string Workstation { get; set; }
        [ForeignKey("_CompanyMaster")]
        public Guid CompanyId { get; set; }
        public CompanyMaster _CompanyMaster { get; set; }
        public List<Permission> _Permission { get; set; }
        public List<ModulePermission> _ModulePermission { get; set; }
    }
    [Table("Permission")]
    public class Permission
    {
       
        [Key]
        public Guid Id { get; set; }

        public Guid MenuId { get; set; }
        [ForeignKey("MenuId")]
        public virtual Menumaster _Menumaster { get; set; }
        public Guid PermissionMasterId { get; set; }
        [ForeignKey("PermissionMasterId")]
        public virtual PermissionMaster PermissionMaster { get; set; }
       
        public bool Isactive { get; set; }
        public int DisplayOrder { get; set; }
    }
    [Table("ModulePermission")]
    public class ModulePermission
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PermissionMasterId { get; set; }
        [ForeignKey("PermissionMasterId")]
        public virtual PermissionMaster PermissionMaster { get; set; }

        public Guid MenuId { get; set; }
        [ForeignKey("MenuId")]
        public virtual Menumaster _Menumaster { get; set; }
        public Guid SubMenuId { get; set; }
        [ForeignKey("SubMenuId")]
        public virtual SubMenumaster _SubMenumaster { get; set; }
        public bool Isview { get; set; }
        public bool IsEdit { get; set; }
        public bool IsAdd { get; set; }
        public bool Isdelete { get; set; }

        public int DisplayOrder { get; set; }
    }
    [Table("StoreMaster")]
    public class StoreMaster
    {
        [Key]
        public string Id { get; set; }
        public Guid CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public CompanyMaster _CompanyMaster { get; set; }
        public string StoreName { get; set; }
        public int FinancialYear { get; set; }
        public string workstation { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
    public class Permissionviewmodel
    {
        public Permissionviewmodel()
        {
            
            Menumaster = new List<Menumaster>();
            _pm = new List<ModulePermission>();
        }
        public PermissionMaster PermissionMaster { get; set; }
        public List<Menumaster> Menumaster { get; set; }
        public List<ModulePermission> _pm { get; set; }
    }
  
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Inventory", throwIfV1Schema: false)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }
        public DbSet<CompanyMaster> CompanyMaster { get; set; }
        public DbSet<Menumaster> Menumaster { get; set; }
        public DbSet<SubMenumaster> SubMenumaster { get; set; }
        public DbSet<StoreMaster> StoreMaster { get; set; }
        public DbSet<PermissionMaster> PermissionMaster { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<ModulePermission> ModulePermission { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}