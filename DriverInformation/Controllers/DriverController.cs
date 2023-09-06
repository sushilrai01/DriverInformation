using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using DriverInformation.Models;
using DriverInformation.ViewModel;

namespace DriverInformation.Controllers
{

    public class DriverController : Controller
    {
        private DriverManagementEntities1 db = new DriverManagementEntities1();
        // GET: Driver
        public ActionResult Index()
        {
            var DriverInfo = (from drivertbl in db.DriverTables
                              join gendertbl in db.GenderTables on drivertbl.GenderId equals gendertbl.GenderId
                              join activitytbl in db.ActivityTables on drivertbl.IsActive equals activitytbl.IsActive
                              join hobbytbl in db.HobbyTables on drivertbl.HobbyId equals hobbytbl.HobbyId
                              select new DriverInfoModel
                              {
                                  DriverId = drivertbl.DriverId,
                                  DriverName = drivertbl.Name,
                                  ContactNo = drivertbl.ContactNo,
                                  Category = gendertbl.Category,
                                  Hobby = hobbytbl.Hobby,
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
                                .Select(x => new DropdownModel { ID = x.HobbyId, TEXT = x.Hobby }).ToList();
            return View(model);
        }

        //POST: Driver/Create
        [HttpPost]
        [ValidateAntiForgeryToken ]
        public  ActionResult Create(DriverInfoModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model); 
            }
             
            DriverTable drivertbl = new DriverTable();
            drivertbl.DriverId = model.DriverId;
            drivertbl.Name = model.DriverName;
            drivertbl.ContactNo = model.ContactNo;  
            drivertbl.GenderId = model.GenderId;
            drivertbl.HobbyId = model.HobbyId;
            drivertbl.IsActive = model.ActiveId;

            db.DriverTables.Add(drivertbl);
            db.SaveChanges();

            return  RedirectToAction("Index");
        }
    }
}