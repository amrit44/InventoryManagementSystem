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

            return View();
        }
        [HttpGet]
        [PermissionsAttribute(Action = "Item", Permission = "IsAdd")]

        public ActionResult CreateItem()
        {
            ItemMaster _ItemMaster = new ItemMaster();
            _ItemMaster.OptionalFields = Commonhelper.GetOptionalFieldsList();
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
                    _ItemMaster.CompanyId = item.BarCode;
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
                    _ItemMaster.workstation = item.workstation;
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
                    if(_ItemMaster.OptionalFields.Count()>0)
                    {
                        foreach(var _item in _ItemMaster.OptionalFields)
                        {
                            ItemOptionalDetails option = new ItemOptionalDetails();
                            option.Id = Guid.NewGuid().ToString();
                            option.ItemId = _ItemMaster.Id;
                            option.OptionalValue = _ItemMaster.Description;
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


        public async System.Threading.Tasks.Task<ActionResult> GetSubcategory(string category)
        {
            List<DropDownControl> dopdownlst = new List<DropDownControl>();
            dopdownlst = Commonhelper.GetSubCategoryByCategory(category);
            return Json(dopdownlst,JsonRequestBehavior.AllowGet);
        }

        public async System.Threading.Tasks.Task<ActionResult>CheckBarcode(string Barcode)
        {
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
        public async System.Threading.Tasks.Task<ActionResult> CheckProductcode(string productcode)
        {
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
        public async System.Threading.Tasks.Task<ActionResult> CheckHsncode(string Hsncode)
        {
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
        public async System.Threading.Tasks.Task<ActionResult> CheckSkucode(string skucode)
        {
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
        public async System.Threading.Tasks.Task<ActionResult> Checksapcode(string sapcode)
        {
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