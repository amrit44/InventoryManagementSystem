using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InventoryManagement.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [ForeignKey("_CompanyMaster")]

        public string CompanyId { get; set; }
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
       
        public string Code { get; set; }
  
        public string Url { get; set; }
        public string PanNumber { get; set; }
        public string TinNumber { get; set; }
        public string GstinNumber { get; set; }
        public string ServiceTax { get; set; }
        public bool ? RegisteredDeailer { get; set; }
        public bool? TaxExempted { get; set; }
        public bool? IsVendor { get; set; }
        public bool? IsRetailCustomer { get; set; }
        public bool? IsWholeCustomer { get; set; }
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
        public string CompanyId { get; set; }
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
        public string CompanyId { get; set; }
        public CompanyMaster _CompanyMaster { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public bool status { get; set; }

        public List<SubMenumaster> _SubMenumaster { get; set; }
        [NotMapped]
        public bool IsSelect{ get; set; }
        public string DisplayName { get; set; }
        public string Displayclass { get; set; }

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
        public string DisplayName { get; set; }
        public string Displayclass { get; set; }
        public string DisplayLink { get; set; }

    }
    [Table("PermissionMaster")]
    public class PermissionMaster
    {
        public PermissionMaster()
        {
     
            _ModulePermission = new List<ModulePermission>();
        }
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser _User { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? Datemodified { get; set; }
        public string ModifiedBy { get; set; }
        public string Workstation { get; set; }
        [ForeignKey("_CompanyMaster")]
        public string CompanyId { get; set; }
        public CompanyMaster _CompanyMaster { get; set; }
        public List<ModulePermission> _ModulePermission { get; set; }
   
    }
 
    [Table("ModulePermission")]
    public class ModulePermission
    {
        [Key]
        public string Id { get; set; }
        public string PermissionMasterId { get; set; }
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
        public string DisplayName { get; set; }
        public string Displayclass { get; set; }
    }
    [Table("StoreMaster")]
    public class StoreMaster
    {
        [Key]
        public string Id { get; set; }
        public string CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public CompanyMaster _CompanyMaster { get; set; }
        public string StoreName { get; set; }
        public int FinancialYear { get; set; }
        public string workstation { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
    [Table("CategoryMaster")]
    public class CategoryMaster
    {
        [Key]
        public string Id { get; set; }

        public string StoreId { get; set; }
        [ForeignKey("StoreId")]
        public StoreMaster _StoreMaster { get; set; }
        public string CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public CompanyMaster _CompanyMaster { get; set; }
        [Remote("Checkcategoryname", "Master", ErrorMessage = "Name in use.", AdditionalFields = "Previousname")]
        public string Name { get; set; }
        public string Description { get; set; }
        public int FinancialYear { get; set; }
        public string workstation { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
    [Table("SubCategoryMaster")]
    public class SubCategoryMaster
    {
        [Key]
        public string Id { get; set; }
        public string CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public CompanyMaster _CompanyMaster { get; set; }

        public string CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public CategoryMaster _CategoryMaster { get; set; }
        public string StoreId { get; set; }
        [ForeignKey("StoreId")]
        public StoreMaster _StoreMaster { get; set; }
        public string Name { get; set; }
        public int FinancialYear { get; set; }
        public string workstation { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
    [Table("BrandMaster")]
    public class BrandMaster
    {
        [Key]
        public string Id { get; set; }

        public string StoreId { get; set; }
        [ForeignKey("StoreId")]
        public StoreMaster _StoreMaster { get; set; }
        public string CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public CompanyMaster _CompanyMaster { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        [Remote("CheckProductname", "Master", ErrorMessage = "Name in use.", AdditionalFields = "Previousname")]
        public string Name { get; set; }
    
        public int FinancialYear { get; set; }
        public string workstation { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
    [Table("ItemMaster")]
    public class ItemMaster
    {
        public ItemMaster()
        {
            ItemOptionalDetails = new List<ItemOptionalDetails>();
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }
        public string StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster _StoreMaster { get; set; }
        public string CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual CompanyMaster _CompanyMaster { get; set; }
        [Required(ErrorMessage = "Product Code Required")]
        [Remote("CheckProductcode", "Master", ErrorMessage = "Product Code in use.", AdditionalFields = "Previousproductcode")]
        public string ProductCode { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Bar Code Required")]
        [Remote("CheckBarcode", "Master", ErrorMessage = "Bar Code in use.", AdditionalFields = "Previousbarcode")]
        public string BarCode { get; set; }
        [Required(ErrorMessage = "Sku Code Required")]
        [Remote("CheckSkucode", "Master", ErrorMessage = "Sku Code in use.", AdditionalFields = "Previousskucode")]
        public string SkuCode { get; set; }
        [Required(ErrorMessage = "Sap Code Required")]
        [Remote("Checksapcode", "Master", ErrorMessage = "Sap Code in use.", AdditionalFields = "Previoussapcode")]

        public string SapCode { get; set; }
        [Required(ErrorMessage = "Category Required")]

        public string Category { get; set; }
        [ForeignKey("Category")]
        public virtual CategoryMaster _CategoryMaster { get; set; }
        [Required(ErrorMessage = "Sub Category Required")]

        public string SubCategory { get; set; }
        [ForeignKey("SubCategory")]
        public virtual SubCategoryMaster _SubCategoryMaster { get; set; }
        [Required(ErrorMessage = "Product Name Required")]

        public string ProductName { get; set; }
        [Required(ErrorMessage = "Product  Brand Required")]

        public string Brand { get; set; }
        [ForeignKey("Brand")]
        public virtual BrandMaster _BrandMaster { get; set; }
        [Required(ErrorMessage = "Product  Size Required")]
        public string Size { get; set; }
        [Required(ErrorMessage = "Quality  Size Required")]
        public string Quality { get; set; }
        [Required(ErrorMessage = "Gst Required")]
        public decimal Gst { get; set; }

        public string Reorderlevel { get; set; }
        [Required(ErrorMessage = "Max-Retail price Required")]
        public decimal Mrp { get; set; }
        [Required(ErrorMessage = "Cost price Required")]
        public decimal Costprice { get; set; }
        [Required(ErrorMessage = "Sell price Required")]
        public decimal Sellprice { get; set; }
        public decimal offer { get; set; }
        public int FinancialYear { get; set; }
        public string workstation { get; set; }
        [Required(ErrorMessage = "Hsn Code Required")]
        [Remote("CheckHsncode", "Master", ErrorMessage = "Hsn Code in use.", AdditionalFields = "Previoushsncode")]
        public string HsnCode { get; set; }
        [Required(ErrorMessage = "Maximum Quantity Required")]
        public int MaximumQuantity { get; set; }
        [Required(ErrorMessage = "Minimum Quantity Required")]
        public int MinimumQuantity { get; set; }
        [Required(ErrorMessage = "Box Quantity Required")]
        public int BoxQuantity { get; set; }
        [Required(ErrorMessage = "Transaction Type Required")]

        public bool IsUnique { get; set; }
        [Required(ErrorMessage = "Measurement Unit Required")]
        public string Mou { get; set; }
        public string SubMou { get; set; }
        [Required(ErrorMessage = "Order Required")]
        public int ItemOrder { get; set; }
        public bool Isactive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public List<ItemOptionalDetails> ItemOptionalDetails { get; set; }

    }
    [Table("OptionalFields")]
    public class OptionalFields
    {
        public OptionalFields()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        public string Id { get; set; }
        public string option1 { get; set; }
      
        public string Description { get; set; }
     
        public bool Status { get; set; }
    }
    [Table("InvItemOptionalDetails")]
    public class ItemOptionalDetails
    {
        public ItemOptionalDetails()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key]
        public string Id { get; set; }
        public string ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual ItemMaster _ItemMaster { get; set; }
        public string OptionalId { get; set; }
        [ForeignKey("OptionalId")]
        public virtual OptionalFields _OptionalFields { get; set; }
        public string OptionalValue { get; set; }
        [NotMapped]
        public string Description { get; set; }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("Inventory", throwIfV1Schema: false)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }
        public DbSet<CompanyMaster> CompanyMaster { get; set; }
        public DbSet<Menumaster> Menumaster { get; set; }
        public DbSet<SubMenumaster> SubMenumaster { get; set; }
        public DbSet<StoreMaster> StoreMaster { get; set; }
        public DbSet<PermissionMaster> PermissionMaster { get; set; }
      
        public DbSet<ModulePermission> ModulePermission { get; set; }
        public DbSet<ItemMaster> ItemMaster { get; set; }
        public DbSet<OptionalFields> OptionalFields { get; set; }
        public DbSet<CategoryMaster> CategoryMaster { get; set; }
        public DbSet<SubCategoryMaster> SubCategoryMaster { get; set; }
        public DbSet<BrandMaster> BrandMaster { get; set; }
        public DbSet<ItemOptionalDetails> ItemOptionalDetails { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}