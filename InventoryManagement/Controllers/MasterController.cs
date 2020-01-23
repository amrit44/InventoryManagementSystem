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

        public ActionResult Item()
        {

            return View();
        }
        [HttpGet]
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
                    Commonhelper.SaveItem(item);
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
    }
}