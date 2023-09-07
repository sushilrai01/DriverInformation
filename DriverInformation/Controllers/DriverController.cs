using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
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

        //GET: Driver/Active
        public ActionResult Active()
        {
            int Active = 1;
            var DriverInfo = (from drivertbl in db.DriverTables
                              join gendertbl in db.GenderTables on drivertbl.GenderId equals gendertbl.GenderId
                              join activitytbl in db.ActivityTables on drivertbl.IsActive equals activitytbl.IsActive
                              where drivertbl.IsActive == Active
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

        //GET: Driver/ShowAll
        public ActionResult ShowAll()
        {

            return RedirectToAction("Index");
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
                                .Select(x => new HobbyModel { HobbyId = x.HobbyId, Hobby = x.Hobby, IsActive = x.IsActive == null ? false : x.IsActive.Value }).ToList();
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

            if(model.HobList.Count(x => x.IsActive ) == 0)
            {
                return View(model.AddModelError());
            }
            else
            {
               StringBuilder sb = new StringBuilder();
              
               foreach(var hob in model.HobList)
                {
                    if (hob.IsActive)
                    {
                        sb.Append(hob.Hobby + ", ");
                    }
                }
                sb.Remove(sb.ToString().LastIndexOf(","), 1);
                model.Hobby = sb.ToString();

                drivertbl.Hobby = model.Hobby; //Mapping to DB table  
            }

            db.DriverTables.Add(drivertbl);
            db.SaveChanges();

            return  RedirectToAction("Index");
        }

        //GET: Driver/Edit/id
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            DriverInfoModel model = new DriverInfoModel();
            var driverInfo = from driver in db.DriverTables
                             join gender in db.GenderTables on driver.GenderId equals gender.GenderId
                             join active in db.ActivityTables on driver.IsActive equals active.IsActive
                             where driver.DriverId == id
                             select new DriverInfoModel
                             {
                                 DriverId = driver.DriverId,
                                 DriverName = driver.Name,
                                 ContactNo = driver.ContactNo,  
                                 Category = gender.Category,
                                 Available = active.Available,
                                 Hobby = driver.Hobby,
                             };

            model = driverInfo.FirstOrDefault();    
            model.GenList = db.GenderTables
                               .Select(x => new DropdownModel { ID = x.GenderId, TEXT = x.Category }).ToList();
            model.ActList = db.ActivityTables
                              .Select(x => new DropdownModel { ID = x.IsActive, TEXT = x.Available }).ToList();
            model.HobList = db.HobbyTables
                                 .Select(x => new HobbyModel { HobbyId = x.HobbyId, Hobby = x.Hobby, IsActive = x.IsActive == null ? false : x.IsActive.Value }).ToList();

            return View(model);
        }

        //POST: Driver/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DriverInfoModel model)
        {
            if (!ModelState.IsValid)
            {
                model.GenList = db.GenderTables
                             .Select(x => new DropdownModel { ID = x.GenderId, TEXT = x.Category }).ToList();
                model.ActList = db.ActivityTables
                                  .Select(x => new DropdownModel { ID = x.IsActive, TEXT = x.Available }).ToList();
                model.HobList = db.HobbyTables
                                     .Select(x => new HobbyModel { HobbyId = x.HobbyId, Hobby = x.Hobby, IsActive = x.IsActive == null ? false : x.IsActive.Value }).ToList();
                return View(model);
            }

            DriverTable driver = new DriverTable();

            driver.DriverId = model.DriverId;
            driver.Name = model.DriverName;
            driver.ContactNo = model.ContactNo; 
            driver.GenderId =  model.GenderId;
            driver.IsActive = model.ActiveId;

            if(model.HobList.Count(x => x.IsActive) == 0)
            {
                return View(model.AddModelError());
            }
            else
            {
                StringBuilder sb = new StringBuilder(); 
                foreach(var hob in model.HobList)
                {
                    if (hob.IsActive)
                    {
                        sb.Append(hob.Hobby + ", ");
                    }
                }
                sb.Remove(sb.ToString().LastIndexOf(","), 1);
                model.Hobby = sb.ToString();    
                driver.Hobby = model.Hobby;
            }

            db.Entry(driver).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}