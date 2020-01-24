using InventoryManagement.Filter;
using InventoryManagement.Helper;
using InventoryManagement.Models;
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
            List<ItemMaster> itemlst = new List<ItemMaster>();
            itemlst = Commonhelper.GetItemMaster();
            return View(itemlst);
        }
        [HttpGet]
        [PermissionsAttribute(Action = "Item", Permission = "IsAdd")]

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

            return View();
        }
        [PermissionsAttribute(Action = "CategoryMaster", Permission = "IsAdd")]

        public ActionResult CreateCategory()
        {

            return View();

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
        public async System.Threading.Tasks.Task<ActionResult> CheckProductcode(string productcode,string Previousproductcode)
        {
            if(productcode== Previousproductcode)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            bool check = Commonhelper.CheckProductcode(productcode);
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
    }
}