using InventoryManagement.Filter;
using InventoryManagement.Helper;
using InventoryManagement.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryManagement.Controllers
{
    public class MasterController : Controller
    {
        // GET: Master
        public ActionResult Index()
        {
            return View();
        }
        [PermissionsAttribute(Action = "Item", Permission = "Isview")]

        public ActionResult Item()
        {
            return View();
        }
        [PermissionsAttribute(Action = "Item", Permission = "IsAdd")]

        public ActionResult Item_Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(GetItemMaster().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        private static IEnumerable<ItemMaster> GetItemMaster()
        {
            List<ItemMaster> itemlst = new List<ItemMaster>();
            itemlst = Commonhelper.GetItemMaster();

            return itemlst;
        }

        public ActionResult CreateItem()
        {
            ItemMaster _ItemMaster = new ItemMaster();
            _ItemMaster.ItemOptionalDetails = Commonhelper.GetOptionalFieldsList();
            return View(_ItemMaster);
        }
        [HttpPost]
        public ActionResult CreateItem(ItemMaster item)
        {
            List<string> res = new List<string>();
            ItemMaster _ItemMaster = new ItemMaster();
            try
            {
                if(ModelState.IsValid)
                {
                    var currentuser = Commonhelper.GetCurrentUserDetails();
                    _ItemMaster.StoreId = currentuser.StoreId;
                    _ItemMaster.CompanyId = currentuser.CompanyId;
                    _ItemMaster.Description = item.Description;
                    _ItemMaster.ProductCode = item.ProductCode;
                    _ItemMaster.BarCode = item.BarCode;
                    _ItemMaster.SkuCode = item.SkuCode;
                    _ItemMaster.SapCode = item.SapCode;
                    _ItemMaster.Category = item.Category;
                    _ItemMaster.Color = item.Color;
                    _ItemMaster.SubCategory = item.SubCategory;

                    _ItemMaster.ProductName = item.ProductName;
                    _ItemMaster.Brand = item.Brand;
                    _ItemMaster.Size = item.Size;
                    _ItemMaster.Quality = item.Quality;
                    _ItemMaster.Gst = item.Gst;
                    _ItemMaster.Reorderlevel = item.Reorderlevel;
                    _ItemMaster.Mrp = item.Mrp;

                    _ItemMaster.Costprice = item.Costprice;
                    _ItemMaster.Sellprice = item.Sellprice;
                    _ItemMaster.offer = item.offer;
                    _ItemMaster.FinancialYear = item.FinancialYear;
                    _ItemMaster.workstation = Commonhelper.GetStation();
                    _ItemMaster.HsnCode = item.HsnCode;
                    _ItemMaster.MaximumQuantity = item.MaximumQuantity;


                    _ItemMaster.MinimumQuantity = item.MinimumQuantity;
                    _ItemMaster.BoxQuantity = item.BoxQuantity;
                    _ItemMaster.IsUnique = item.IsUnique;
                    _ItemMaster.Mou = item.Mou;
                    _ItemMaster.SubMou = item.SubMou;
                    _ItemMaster.ItemOrder = item.ItemOrder;
                    _ItemMaster.CreatedDate = DateTime.Now;
                    _ItemMaster.CreatedBy = currentuser.Id;
                    _ItemMaster.Isactive = true;
                    if(item.ItemOptionalDetails.Count()>0)
                    {
                        foreach(var _item in item.ItemOptionalDetails)
                        {
                            ItemOptionalDetails option = new ItemOptionalDetails();
                            option.Id = Guid.NewGuid().ToString();
                            option.ItemId = _ItemMaster.Id;
                            option.OptionalId = _item.OptionalId;
                            option.OptionalValue = _item.OptionalValue;
                            _ItemMaster.ItemOptionalDetails.Add(option);
                        }
                    }
                    Commonhelper.SaveItem(_ItemMaster);
                    return Content("<script language='javascript' type='text/javascript'>alert('Item Created successfully!');</script>");

                }
                else
                {

                    return View();

                }


            }
            catch (Exception ex)
            {

            }
            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> EditItem(string Id)
        {
            ItemMaster itemMaster = Commonhelper.GetItemById(Id);
            return View(itemMaster);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> UpdateItem(ItemMaster item)
        {
            ItemMaster _ItemMaster = new ItemMaster();
            bool status = false;
            string msg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var currentuser = Commonhelper.GetCurrentUserDetails();
                    _ItemMaster.Id = item.Id;
                    _ItemMaster.StoreId = currentuser.StoreId;
                    _ItemMaster.CompanyId = currentuser.CompanyId;
                    _ItemMaster.Description = item.Description;
                    _ItemMaster.ProductCode = item.ProductCode;
                    _ItemMaster.BarCode = item.BarCode;
                    _ItemMaster.SkuCode = item.SkuCode;
                    _ItemMaster.SapCode = item.SapCode;
                    _ItemMaster.Category = item.Category;
                    _ItemMaster.Color = item.Color;
                    _ItemMaster.SubCategory = item.SubCategory;

                    _ItemMaster.ProductName = item.ProductName;
                    _ItemMaster.Brand = item.Brand;
                    _ItemMaster.Size = item.Size;
                    _ItemMaster.Quality = item.Quality;
                    _ItemMaster.Gst = item.Gst;
                    _ItemMaster.Reorderlevel = item.Reorderlevel;
                    _ItemMaster.Mrp = item.Mrp;

                    _ItemMaster.Costprice = item.Costprice;
                    _ItemMaster.Sellprice = item.Sellprice;
                    _ItemMaster.offer = item.offer;
                    _ItemMaster.FinancialYear = item.FinancialYear;
                    _ItemMaster.workstation = Commonhelper.GetStation();
                    _ItemMaster.HsnCode = item.HsnCode;
                    _ItemMaster.MaximumQuantity = item.MaximumQuantity;


                    _ItemMaster.MinimumQuantity = item.MinimumQuantity;
                    _ItemMaster.BoxQuantity = item.BoxQuantity;
                    _ItemMaster.IsUnique = item.IsUnique;
                    _ItemMaster.Mou = item.Mou;
                    _ItemMaster.SubMou = item.SubMou;
                    _ItemMaster.ItemOrder = item.ItemOrder;
                    _ItemMaster.ModifiedDate = DateTime.Now;
                    _ItemMaster.ModifiedBy = currentuser.Id;
                    if (item.ItemOptionalDetails.Count() > 0)
                    {
                        foreach (var _item in item.ItemOptionalDetails)
                        {
                            ItemOptionalDetails option = new ItemOptionalDetails();
                            option.Id = Guid.NewGuid().ToString();
                            option.ItemId = _ItemMaster.Id;
                            option.OptionalId = _item.OptionalId;
                            option.OptionalValue = _item.OptionalValue;
                            _ItemMaster.ItemOptionalDetails.Add(option);
                        }
                    }
                    try
                    {
                        Commonhelper.UpdateItem(_ItemMaster);
                        status = true;
                        msg = "Item Updated successfully!";
                        return Json(new{ status,msg},JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {
                        msg = "Error in processing";
                        status = false;
                        return Json(new { status, msg }, JsonRequestBehavior.AllowGet);


                    }

                }
                else
                {

                    return View();

                }


            }
            catch (Exception ex)
            {

            }
            return View();
        }


        public async System.Threading.Tasks.Task<ActionResult> DeleteItem(string Id)
        {
            bool status = false;
            string msg = string.Empty;
            try
            {
                try
                {
                    Commonhelper.DeleteItemById(Id);
                    msg = "Item delete successfully!";
                    status = true;
                }
                catch(Exception ex)
                {
                    msg = "Error in processing";
                    status = false;
                }
              
            }
            catch(Exception ex)
            {
                msg = "Error in processing";
                status = false;
            }
            return Json(new { status, msg }, JsonRequestBehavior.AllowGet);
        }

        public async System.Threading.Tasks.Task<ActionResult> GetSubcategory(string category)
        {
            List<DropDownControl> dopdownlst = new List<DropDownControl>();
            dopdownlst = Commonhelper.GetSubCategoryByCategory(category);
            return Json(dopdownlst,JsonRequestBehavior.AllowGet);
        }
        [PermissionsAttribute(Action = "CategoryMaster", Permission = "Isview")]
        public ActionResult Category()
        {
            List<CategoryMaster> lst = new List<CategoryMaster>();
            lst = Commonhelper.GetCategory();
            return View(lst);
        }
        public ActionResult Category_Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(GetCategory().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        private static IEnumerable<CategoryMaster> GetCategory()
        {
            List<CategoryMaster> lst = new List<CategoryMaster>();

            using (var ctx = new ApplicationDbContext())
            {
                lst = Commonhelper.GetCategory();
            }
            
            return lst;
        }
        [PermissionsAttribute(Action = "CategoryMaster", Permission = "IsAdd")]

        public ActionResult CreateCategory()
        {
          
            return View();

        }
        [HttpPost]
        [PermissionsAttribute(Action = "CategoryMaster", Permission = "IsAdd")]
        public ActionResult CreateCategory(CategoryMaster category)
        {
            List<string> res = new List<string>();
            var currentuser = Commonhelper.GetCurrentUserDetails();
            if (ModelState.IsValid)
            {
                try
                {
                    CategoryMaster master = new CategoryMaster();
                    master.Id = Guid.NewGuid().ToString();
                    master.CreatedBy = currentuser.Id;
                    master.CreatedDate = DateTime.Now;
                    master.Name = category.Name;
                    master.Discount = category.Discount;
                    master.StoreId = currentuser.StoreId;
                    master.workstation = Commonhelper.GetStation();
                    master.FinancialYear = DateTime.Now.Year;
                    master.CompanyId = currentuser.CompanyId;
                    master.Isactive = true;
                    try
                    {
                        Commonhelper.SaveCategory(master);
                        return RedirectToAction("Category");

                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Category");

                    }
                }
                catch(Exception ex)
                {
                    return RedirectToAction("Category");

                }


            }
            else
            {

              
            }

            return RedirectToAction("Category");
        }
        [PermissionsAttribute(Action = "CategoryMaster", Permission = "IsEdit")]
        public ActionResult EditCategory(string Id)
        {
            CategoryMaster c = new CategoryMaster();
            c = Commonhelper.GetCategoryById(Id);
            return View(c);
        }

        [HttpPost]
        [PermissionsAttribute(Action = "CategoryMaster", Permission = "IsAdd")]
        public ActionResult UpdateCategory(CategoryMaster category)
        {
            List<string> res = new List<string>();
            var currentuser = Commonhelper.GetCurrentUserDetails();
            try
                {
                    CategoryMaster master = new CategoryMaster();
                    master.Id = category.Id;
                    master.ModifiedBy = currentuser.Id;
                    master.ModifiedDate = DateTime.Now;
                    master.Name = category.Name;
                    master.Discount = category.Discount;
                    master.Description = category.Description;
                    master.StoreId = currentuser.StoreId;
                    master.workstation = Commonhelper.GetStation();
                    master.FinancialYear = DateTime.Now.Year;
                    master.CompanyId = currentuser.CompanyId;
                    try
                    {
                        Commonhelper.UpdateCategory(master);
                        return Content("<script language='javascript' type='text/javascript'>alert('Category Created successfully!');</script>");

                    }
                    catch (Exception ex)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Error in category creation!');</script>");

                    }
                }
                catch (Exception ex)
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('Error in category creation!');</script>");

                }


           
            

            return Json(res);
        }
        [PermissionsAttribute(Action = "SubCategoryMaster", Permission = "Isview")]
        public async System.Threading.Tasks.Task<ActionResult>Subcategory()
        {
            List<SubCategoryMaster> _master = new List<SubCategoryMaster>();
            _master = Commonhelper.GetSubCategory();

            return View(_master);
        }

        public ActionResult SubCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(GetSubCategory().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        private static IEnumerable<SubCategoryMaster> GetSubCategory()
        {
            List<SubCategoryMaster> lst = new List<SubCategoryMaster>();

            using (var ctx = new ApplicationDbContext())
            {
                lst = Commonhelper.GetSubCategory();
            }

            return lst;
        }

        [PermissionsAttribute(Action = "SubCategoryMaster", Permission = "IsAdd")]
        public async System.Threading.Tasks.Task<ActionResult> CreateSubcategory()
        {

            return View();
        }
        [HttpPost]
        [PermissionsAttribute(Action = "SubCategoryMaster", Permission = "IsAdd")]
        public async System.Threading.Tasks.Task<ActionResult> CreateSubcategory(SubCategoryMaster sub)
        {
            if(ModelState.IsValid)
            {
                List<string> res = new List<string>();
                var currentuser = Commonhelper.GetCurrentUserDetails();
                if (ModelState.IsValid)
                {
                    try
                    {
                        SubCategoryMaster master = new SubCategoryMaster();
                        master.Id = Guid.NewGuid().ToString();
                        master.CreatedBy = currentuser.Id;
                        master.CreatedDate = DateTime.Now;
                        master.Name = sub.Name;
                        master.CategoryId = sub.CategoryId;
                        master.Description = sub.Description;
                        master.StoreId = currentuser.StoreId;
                        master.workstation = Commonhelper.GetStation();
                        master.FinancialYear = DateTime.Now.Year;
                        master.CompanyId = currentuser.CompanyId;
                        master.Isactive = true;
                        try
                        {
                            Commonhelper.SaveSubCategory(master);
                            return RedirectToAction("Subcategory");
                        }
                        catch (Exception ex)
                        {
                            return RedirectToAction("Subcategory");
                        }
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Subcategory");
                    }


                }
                else
                {


                }


            }
            return View();
        }


        public async System.Threading.Tasks.Task<ActionResult> EditSubcategory(string Id)
        {
            SubCategoryMaster _master = new SubCategoryMaster();
            _master = Commonhelper.GetSubCategoryById(Id);
            return View(_master);
        }
        [HttpPost]
        [PermissionsAttribute(Action = "SubCategoryMaster", Permission = "IsAdd")]
        public ActionResult UpdateSubcategory([DataSourceRequest] DataSourceRequest request, SubCategoryMaster subcategory)
        {
            List<string> res = new List<string>();
            var currentuser = Commonhelper.GetCurrentUserDetails();
            SubCategoryMaster master = new SubCategoryMaster();
            try
                {
                   
                    master.Id = subcategory.Id;
                    master.ModifiedBy = currentuser.Id;
                    master.ModifiedDate = DateTime.Now;
                    master.Name = subcategory.Name;
                    master.CheckName = subcategory.Name;
                    master.CategoryName = Commonhelper.GetCategoryName(subcategory.CategoryId);
                    master.CategoryId = subcategory.CategoryId;
                    master.Description = subcategory.Description;
                    master.StoreId = currentuser.StoreId;
                    master.workstation = Commonhelper.GetStation();
                    master.FinancialYear = DateTime.Now.Year;
                    master.CompanyId = currentuser.CompanyId;
                    try
                    {
                        Commonhelper.UpdateSubCategory(master);
                       return Json(new[] { master }.ToDataSourceResult(request));
                   /// return RedirectToActionPermanent("SubCategory_Read");
                  ///  return RedirectToAction("Subcategory");
                    }
                    catch (Exception ex)
                    {
                    return Json(new[] { master }.ToDataSourceResult(request, ModelState));
                }
            }
                catch (Exception ex)
                {
                return Json(new[] { master }.ToDataSourceResult(request, ModelState));
              }


        }

        public ActionResult Deletecategory(CategoryMaster cat)
        {
            CategoryMaster cm = new CategoryMaster();
            try
            {
                using(var ctx=new ApplicationDbContext())
                {
                    cm = ctx.CategoryMaster.Where(x => x.Id == cat.Id).FirstOrDefault();
                    if(cm!=null)
                    {
                        cm.Isactive = false;
                        ctx.Entry(cm).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        return RedirectToAction("Category");

                    }

                }
            }
            catch(Exception ex)
            {

            }
            return RedirectToAction("Category");
        }

        public ActionResult DeleteSubcategory(CategoryMaster cat)
        {
            SubCategoryMaster cm = new SubCategoryMaster();
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    cm = ctx.SubCategoryMaster.Where(x => x.Id == cat.Id).FirstOrDefault();
                    if (cm != null)
                    {
                        cm.Isactive = false;
                        ctx.Entry(cm).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        return RedirectToAction("SubCategory");

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("SubCategory");
        }


        [PermissionsAttribute(Action = "BrandMaster", Permission = "Isview")]
        public async System.Threading.Tasks.Task<ActionResult> BrandMaster()
        {
            List<BrandMaster> _master = new List<BrandMaster>();
            _master = Commonhelper.GetBrand();

            return View(_master);
        }

        public ActionResult BrandMaster_Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(GetBrandMaster().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        private static IEnumerable<BrandMaster> GetBrandMaster()
        {
            List<BrandMaster> _master = new List<BrandMaster>();

            using (var ctx = new ApplicationDbContext())
            {
               
                _master = Commonhelper.GetBrand();
            }

            return _master;
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> UpdateBrandMaster([DataSourceRequest] DataSourceRequest request,BrandMaster _brand)
        {
            List<string> res = new List<string>();
            var currentuser = Commonhelper.GetCurrentUserDetails();
            BrandMaster master = new BrandMaster();
            try
                {
                    
                    master.Id = _brand.Id;
                    master.ModifiedBy = currentuser.Id;
                    master.ModifiedDate = DateTime.Now;
                    master.Name = _brand.Name;
                    master.CheckName = _brand.Name;
                    master.Description = _brand.Description;
                    master.StoreId = currentuser.StoreId;
                    master.workstation = Commonhelper.GetStation();
                    master.FinancialYear = DateTime.Now.Year;
                    master.CompanyId = currentuser.CompanyId;
                    try
                    {
                        Commonhelper.UpdateBrand(master);
                     return Json(new[] { master }.ToDataSourceResult(request, ModelState));
                    }
                    catch (Exception ex)
                    {

                    return Json(new[] { master }.ToDataSourceResult(request, ModelState));

                }
            }
                catch (Exception ex)
                {
                return Json(new[] { master }.ToDataSourceResult(request, ModelState));

                }

        }
        [HttpGet]
        [PermissionsAttribute(Action = "BrandMaster", Permission = "IsAdd")]
        public async System.Threading.Tasks.Task<ActionResult> CreateBrandMaster()
        {

            return View();
        }
        public ActionResult Deletebrand(BrandMaster cat)
        {
            BrandMaster cm = new BrandMaster();
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    cm = ctx.BrandMaster.Where(x => x.Id == cat.Id).FirstOrDefault();
                    if (cm != null)
                    {
                        cm.Isactive = false;
                        ctx.Entry(cm).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        return RedirectToAction("BrandMaster");

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Category");
        }

        public ActionResult ColorMaster_Read([DataSourceRequest]DataSourceRequest request)
        {
            return Json(GetColorMaster().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        private static IEnumerable<ColorMaster> GetColorMaster()
        {
            List<ColorMaster> _master = new List<ColorMaster>();

            using (var ctx = new ApplicationDbContext())
            {

                _master = Commonhelper.GetColor();
            }

            return _master;
        }

        public ActionResult DeleteColor(ColorMaster cat)
        {
            ColorMaster cm = new ColorMaster();
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    cm = ctx.ColorMaster.Where(x => x.Id == cat.Id).FirstOrDefault();
                    if (cm != null)
                    {
                        cm.Isactive = false;
                        ctx.Entry(cm).State = System.Data.Entity.EntityState.Modified;
                        ctx.SaveChanges();
                        return RedirectToAction("ColorMaster");

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("ColorMaster");
        }

        [HttpPost]
        [PermissionsAttribute(Action = "ColorMaster", Permission = "IsAdd")]
        public async System.Threading.Tasks.Task<ActionResult> CreateBrandMaster(BrandMaster _brand)
        {
            List<string> res = new List<string>();
            var currentuser = Commonhelper.GetCurrentUserDetails();
            if (ModelState.IsValid)
            {
                try
                {
                    BrandMaster master = new BrandMaster();
                    master.Id = Guid.NewGuid().ToString();
                    master.CreatedBy = currentuser.Id;
                    master.CreatedDate = DateTime.Now;
                    master.Name = _brand.Name;
                    master.Description = _brand.Description;
                    master.StoreId = currentuser.StoreId;
                    master.workstation = Commonhelper.GetStation();
                    master.FinancialYear = DateTime.Now.Year;
                    master.CompanyId = currentuser.CompanyId;
                    master.Isactive = true;
                    try
                    {
                        Commonhelper.SaveBrand(master);
                        return RedirectToAction("BrandMaster");
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("BrandMaster");
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("BrandMaster");
                }


            }
            else
            {


            }

            return Json(res);
            return View();
        }
       
        [PermissionsAttribute(Action = "ColorMaster", Permission = "IsEdit")]
        public async System.Threading.Tasks.Task<ActionResult> EditBrand(string Id)
        {
            BrandMaster _master = new BrandMaster();
            _master = Commonhelper.GetBrandById(Id);
            return View(_master);
        }

        [PermissionsAttribute(Action = "ColorMaster", Permission = "Isview")]
        public async System.Threading.Tasks.Task<ActionResult> ColorMaster()
        {
            List<ColorMaster> _master = new List<ColorMaster>();
            _master = Commonhelper.GetColor();

            return View(_master);
        }
        [HttpGet]
        [PermissionsAttribute(Action = "ColorMaster", Permission = "IsAdd")]
        public async System.Threading.Tasks.Task<ActionResult> CreateColorMaster()
        {

            return View();
        }
        [HttpPost]
        [PermissionsAttribute(Action = "ColorMaster", Permission = "IsAdd")]
        public async System.Threading.Tasks.Task<ActionResult> CreateColorMaster(ColorMaster _master)
        {

            List<string> res = new List<string>();
            var currentuser = Commonhelper.GetCurrentUserDetails();
            if (ModelState.IsValid)
            {
                try
                {
                    ColorMaster master = new ColorMaster();
                    master.Id = Guid.NewGuid().ToString();
                    master.CreatedBy = currentuser.Id;
                    master.CreatedDate = DateTime.Now;
                    master.Name = _master.Name;
                    master.Description = _master.Description;
                    master.StoreId = currentuser.StoreId;
                    master.workstation = Commonhelper.GetStation();
                    master.FinancialYear = DateTime.Now.Year;
                    master.CompanyId = currentuser.CompanyId;
                    master.Isactive = true;
                    try
                    {
                        Commonhelper.SaveColor(master);
                        return RedirectToAction("ColorMaster");
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("ColorMaster");
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("ColorMaster");
                }


            }
            else
            {


            }

            return Json(res);
        }


        [PermissionsAttribute(Action = "ColorMaster", Permission = "IsEdit")]
        public async System.Threading.Tasks.Task<ActionResult> EditColor(string Id)
        {
            ColorMaster _master = new ColorMaster();
            _master = Commonhelper.GetColorById(Id);
            return View(_master);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> UpdateColorMaster(ColorMaster _color)
        {
            List<string> res = new List<string>();
            var currentuser = Commonhelper.GetCurrentUserDetails();
           
                try
                {
                    ColorMaster master = new ColorMaster();
                    master.Id = _color.Id;
                    master.ModifiedBy = currentuser.Id;
                    master.ModifiedDate = DateTime.Now;
                    master.Name = _color.Name;
                    master.Description = _color.Description;
                    master.StoreId = currentuser.StoreId;
                    master.workstation = Commonhelper.GetStation();
                    master.FinancialYear = DateTime.Now.Year;
                    master.CompanyId = currentuser.CompanyId;
                    try
                    {
                        Commonhelper.Updatecolor(master);
                    return RedirectToAction("ColorMaster");

                    }
                    catch (Exception ex)
                    {
                    return RedirectToAction("ColorMaster");
                   }
                }
                catch (Exception ex)
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('Error in category creation!');</script>");

                }


           
        }

        #region duplicate check for all

        public async System.Threading.Tasks.Task<ActionResult> Checkproductname(string productname, string Previousproductname)
        {
            if (productname == Previousproductname)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.Checkproductname(productname);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public async System.Threading.Tasks.Task<ActionResult>CheckBarcode(string Barcode,string Previousbarcode)
        {
            if(Barcode==Previousbarcode)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.GetBarcode(Barcode);
            if(check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public async System.Threading.Tasks.Task<ActionResult> CheckProductcode(string ProductCode, string Previousproductcode)
        {
            if(ProductCode == Previousproductcode)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.CheckProductcode(ProductCode);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public async System.Threading.Tasks.Task<ActionResult> CheckHsncode(string Hsncode, string Previoushsncode)
        {
            if (Hsncode == Previoushsncode)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.CheckHsncode(Hsncode);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public async System.Threading.Tasks.Task<ActionResult> CheckSkucode(string skucode, string Previousskucode)
        {
            if (skucode == Previousskucode)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.Checkskucode(skucode);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public async System.Threading.Tasks.Task<ActionResult> Checksapcode(string sapcode, string Previoussapcode)
        {
            if (sapcode == Previoussapcode)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.Checksapcode(sapcode);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Checkcategoryname(string Name, string Previousname)
        {
            if (Name == Previousname)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.Checkcategoryname(Name);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        public async System.Threading.Tasks.Task<ActionResult> Checksubcategoryname(string Name, string Previousname)
        {
            if (Name == Previousname)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.CheckSubcategoryname(Name);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public async System.Threading.Tasks.Task<ActionResult> CheckBrandname(string Name, string PreviousBrandname)
        {
            if (Name == PreviousBrandname)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.CheckBrandname(Name);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public async System.Threading.Tasks.Task<ActionResult> CheckColorname(string Name, string Previouscolorname)
        {
            if (Name == Previouscolorname)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.CheckColorname(Name);
            if (check)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}