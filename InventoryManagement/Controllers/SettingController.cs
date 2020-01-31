using InventoryManagement.Helper;
using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InventoryManagement.Controllers
{
    public class SettingController : AsyncController
    {
        // GET: Setting
        public ActionResult Index()
        {
            List<OptionalFields> lst = new List<OptionalFields>();
            lst = Commonhelper.GetOptionalList();
            return View(lst);
        }
        public ActionResult DynamicControl()
        {

           
            return View();
        }
        [HttpPost]
        public ActionResult DynamicControl(List<OptionalFields> lst)
        {

            Commonhelper.SaveOptionalFields(lst);
            return View();
        }
        public ActionResult EditField(string Id)
        {
            OptionalFields field = Commonhelper.GetOptionalFieldbyId(Id);
            return View(field);
        }
        [HttpPost]
        public ActionResult UpdateField(OptionalFields field )
           {
            List<string> res = new List<string>();
            bool status = false;
            if (ModelState.IsValid)
            {
                try
                {

                    Commonhelper.UpdateFieldbyId(field);
                    res.Add("Update Successfully.");
                    status = true;
                   
                }
                catch (Exception ex)
                {
                    res.Add("Error in Field updation!");
                    status = false;
                }
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        res.Add(modelError.ErrorMessage);
                    }


                }
            }
            return Json(new { res,status },JsonRequestBehavior.AllowGet);
        }
    }
}