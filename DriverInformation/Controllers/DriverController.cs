using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using DriverInformation.Models;
using DriverInformation.ViewModel;
using System.Text;
using System.Collections;

namespace DriverInformation.Controllers
{

    public class DriverController : Controller
    {
        private DriverManagementEntities db = new DriverManagementEntities();
        // GET: Driver
        public ActionResult Index()
        {
            var DriverInfo = (from drivertbl in db.DriverTables
                              join gendertbl in db.GenderTables on drivertbl.GenderId equals gendertbl.GenderId
                              join activitytbl in db.ActivityTables on drivertbl.IsActive equals activitytbl.IsActive
                              select new DriverInfoModel
                              {
                                  DriverId = drivertbl.DriverId,
                                  DriverName = drivertbl.Name,
                                  ContactNo = drivertbl.ContactNo,
                                  Category = gendertbl.Category,
                                  Hobby = drivertbl.Hobby,
                                  Available = activitytbl.Available,
                              }).ToList();

            return View(DriverInfo);
        }

        //GET: Driver/Create
        public ActionResult Create()
        {
           DriverInfoModel model = new DriverInfoModel();
           model.GenList = db.GenderTables
                                .Select(x => new DropdownModel { ID = x.GenderId, TEXT = x.Category }).ToList();
           model.ActList = db.ActivityTables
                             .Select(x => new DropdownModel { ID = x.IsActive, TEXT = x.Available }).ToList();
           model.HobList = db.HobbyTables
                                .Select(x => new HobbyModel { HobbyId = x.HobbyId, Hobby = x.Hobby, IsActive = x.IsActive == null?false:x.IsActive.Value }).ToList();
            return View(model);
        }

        //POST: Driver/Create
        [HttpPost]
        [ValidateAntiForgeryToken ]
        public  ActionResult Create(DriverInfoModel model)
        {
            if(!ModelState.IsValid)
            {
                model.GenList = db.GenderTables
                               .Select(x => new DropdownModel { ID = x.GenderId, TEXT = x.Category }).ToList();
                model.ActList = db.ActivityTables
                                  .Select(x => new DropdownModel { ID = x.IsActive, TEXT = x.Available }).ToList();
                model.HobList = db.HobbyTables
                                     .Select(x => new HobbyModel { HobbyId = x.HobbyId, Hobby = x.Hobby, IsActive = x.IsActive == null ? false : x.IsActive.Value }).ToList();
                return View(model); 
            }
             
            DriverTable drivertbl = new DriverTable();
            drivertbl.DriverId = model.DriverId;
            drivertbl.Name = model.DriverName;
            drivertbl.ContactNo = model.ContactNo;  
            drivertbl.GenderId = model.GenderId;
            drivertbl.IsActive = model.ActiveId;

            if(model.HobList.Count(x => x.IsActive) == 0)
            {
                return View(model.AddModelError());
            }
            else
            {
                StringBuilder sb = new StringBuilder();
               //IList list = model.HobList;
               // for (int i = 0; i < list.Count; i++)
               // {
               //     DriverInfoModel h = (DriverInfoModel)list[i];
               // }
               foreach(var hob in model.HobList)
                {
                    if (hob.IsActive)
                    {
                        sb.Append(hob.Hobby + ",");
                    }
                }
                sb.Remove(sb.ToString().LastIndexOf(","), 1);
                //if (model.HobList.Where(x => x.IsActive)
                model.Hobby = sb.ToString();
                drivertbl.Hobby = model.Hobby; //Hereeeeeeeeeeeee  
            }
            

            db.DriverTables.Add(drivertbl);
            db.SaveChanges();

            return  RedirectToAction("Index");
        }
    }
}