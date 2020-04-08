using InventoryManagement.Helper;
using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
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

        public ActionResult Hierarchy()
        {
            //var db = new ApplicationDbContext();
            //IEnumerable<Hierarchy> h = db.Hierarchy.ToList();
            List<Treelevel> lst = new List<Treelevel>();
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                lst = context.Treelevel.Include(x => x.Parent).Include(x => x._subParent).ToList();
              
            }
            return View(lst);

            
        }
      
        public ActionResult CreareHierarchy()
        {

            return View();
        }
        [HttpPost]
        public ActionResult CreareHierarchy(Treelevel hierarchy)
        {
            Treelevel l = new Treelevel();
            l.ParentId = hierarchy.ParentId;
            l.SubParentId = hierarchy.SubParentId;
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                context.Treelevel.Add(l);
                context.SaveChanges();
            }
            return View();
        }
        public ActionResult EditHierarchy(int Id)
        {
            Treelevel l = new Treelevel();
          
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
              l=  context.Treelevel.Where(x => x.Id == Id).FirstOrDefault();
                
            }
            return View(l);
        }
        public JsonResult Get(string query)
        {
            List<Hierarchy> locations;
            List<Hierarchyviewmodel> records;
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                locations = context.Hierarchy.ToList();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    locations = locations.Where(q => q.Name.Contains(query)).ToList();
                }

                records = locations.Where(l => l.ParentId == null)
                    .Select(l => new Models.Hierarchyviewmodel
                    {
                        id = l.Id,
                        text = l.Name,
                      
                        children = GetChildren(locations, l.Id)
                    }).ToList();
            }

            return this.Json(records, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LazyGet(int? parentId)
        {
            List<Hierarchy> locations;
            List<Hierarchyviewmodel> records;
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                locations = context.Hierarchy.ToList();

                records = locations.Where(l => l.ParentId == parentId)
                    .Select(l => new Models.Hierarchyviewmodel
                    {
                        id = l.Id,
                        text = l.Name,
                        @checked = true,
                        hasChildren = locations.Any(l2 => l2.ParentId == l.Id)
                    }).ToList();
            }

            return this.Json(records, JsonRequestBehavior.AllowGet);
        }

        private List<Hierarchyviewmodel> GetChildren(List<Hierarchy> locations, int parentId)
        {
            return locations.Where(l => l.ParentId == parentId)
               .Select(l => new Models.Hierarchyviewmodel
               {
                   id = l.Id,
                   text = l.Name,
                 
                   children = GetChildren(locations, l.Id)
               }).ToList();
           
        }
        public ActionResult CreateHierarchy()
        {


            return View();
        }
    }
}