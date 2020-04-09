using InventoryManagement.Helper;
using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryManagement.Controllers
{
    public class TransactionController : Controller
    {
        // GET: Transaction
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Purchaseorder()
        {
            return View();
        }
 
        public ActionResult CreatePurachaseorder()
        {

            return View();

          
        }
      
        public async System.Threading.Tasks.Task<ActionResult> GetItemDetailById(string Id)
        {
            ItemMaster _items = new ItemMaster();

            _items = Commonhelper.GetItemById(Id);
            return Json(_items, JsonRequestBehavior.AllowGet);
        }
    }
}