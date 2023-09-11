using System ;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
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
                                  GenderId = gendertbl.GenderId,
                                  ContactNo = drivertbl.ContactNo,
                                  Category = gendertbl.Category,
                                  Hobby = drivertbl.Hobby,
                                  Available = activitytbl.Available,
                              }).ToList().OrderBy(x => x.GenderId);

            return View(DriverInfo);
        }

        //GET: Driver/ShowAll
        public ActionResult ShowAll()
        {

            return RedirectToAction("Index");
        }

        //GET: Driver/Details/id
        public ActionResult Details(int? id)
        {
            var DriverInfo = from drivertbl in db.DriverTables
                             join gendertbl in db.GenderTables on drivertbl.GenderId equals gendertbl.GenderId
                             join activitytbl in db.ActivityTables on drivertbl.IsActive equals activitytbl.IsActive
                             join maptbl in db.MapDriverHobs on drivertbl.DriverId equals maptbl.DriverId
                             join hobtbl in db.HobbyTables on maptbl.HobbyId equals hobtbl.HobbyId
                             where drivertbl.DriverId == id 
                             select new DriverInfoModel
                             {
                                 DriverId = drivertbl.DriverId,
                                 DriverName = drivertbl.Name,
                                 ContactNo = drivertbl.ContactNo,
                                 Category = gendertbl.Category,
                                 Hobby = hobtbl.Hobby,
                                 Available = activitytbl.Available,
                                 ImageFilePath = drivertbl.ImageFilePath,
                             };
            //return View(DriverInfo.ToList());
            return View(DriverInfo.FirstOrDefault());
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
        public  ActionResult Create(DriverInfoModel model, List<HttpPostedFileBase> files, HttpPostedFileBase file )
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
            drivertbl.IsActive = model.ActiveId;
            drivertbl.Football = model.Football;
            drivertbl.Basketball = model.Basketball;
            drivertbl.Cricket = model.Cricket;
            drivertbl.Singing = model.Singing;
            drivertbl.Dancing = model.Dancing;
            drivertbl.Reading = model.Reading;
            drivertbl.Travelling = model.Travelling;

            //>_ File Uploading...
            if (files != null && files.Count() > 0)
            {
                
                try
                {
                    string filename = Path.GetFileName(files[0].FileName);
                    string DB_filepath = "~/UploadedImages/" + filename ;
                    string UploadPath = Path.Combine(Server.MapPath("~/UploadedImages"), filename); //Physical File Path (on Folder)
                    files[0].SaveAs(UploadPath); //Saving to physical path(folder)
                    ViewBag.Message = "File uploaded successfully";
                    model.ImageFilePath = DB_filepath; //Saving file path to DataBase
                    drivertbl.ImageFilePath = model.ImageFilePath;
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = " You have not specified a file.";
            }

            //>_ File Uploading End...

            //----Hobby list from HObby table-----
            if (model.HobList.Count(x => x.IsActive ) == 0)
            {
                return View(model.AddModelError()); //View Error on not selecting at least one.
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

                //--( > Start! <)--String concatenation for Hobbies in columns-----
              if(!model.Basketball && !model.Football && !model.Cricket && !model.Singing && !model.Dancing && !model.Reading && !model.Travelling)
                {
                    return View(model.AddModelError());
                }
                else
                {
                    if (model.Football) sb.Append("/Football");
                    if (model.Basketball) sb.Append("/Basketball");
                    if (model.Cricket) sb.Append("/Cricket");
                    if (model.Singing) sb.Append("/Singing");
                    if (model.Dancing) sb.Append("/Dancing");
                    if (model.Reading) sb.Append("/Reading");
                    if (model.Travelling) sb.Append("/Travelling");
                }
                

                //--( > End! <)--String concatenation for Hobbies in columns-----

                model.Hobby = sb.ToString();

                drivertbl.Hobby = model.Hobby; //Mapping to DB table  
            }

            db.DriverTables.Add(drivertbl);
            db.SaveChanges();

            //>_Adding to Mapping Table*******
            MapDriverHob mappingDH = new MapDriverHob();
            mappingDH.MapId = model.MapId;

            foreach (var hob in model.HobList)
            {
                if (hob.IsActive)
                {
                    mappingDH.DriverId = drivertbl.DriverId;
                    mappingDH.HobbyId = hob.HobbyId;
                    db.MapDriverHobs.Add(mappingDH);
                    db.SaveChanges();
                }
            }
            //>_Added to Mapping Table****


            DriverInfoModel model1 = new DriverInfoModel();

            model1.GenList = db.GenderTables
                               .Select(x => new DropdownModel { ID = x.GenderId, TEXT = x.Category }).ToList();
            model1.ActList = db.ActivityTables
                              .Select(x => new DropdownModel { ID = x.IsActive, TEXT = x.Available }).ToList();
            model1.HobList = db.HobbyTables
                                 .Select(x => new HobbyModel { HobbyId = x.HobbyId, Hobby = x.Hobby, IsActive = x.IsActive == null ? false : x.IsActive.Value }).ToList();

            return View(model1);
            //return RedirectToAction("Index");
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

                                 Football = driver.Football == null ? false : driver.Football.Value,
                                 Basketball = driver.Basketball == null ? false : driver.Basketball.Value,
                                 Cricket = driver.Cricket == null ? false : driver.Cricket.Value,
                                 Singing = driver.Singing == null ? false : driver.Singing.Value,
                                 Dancing = driver.Dancing == null ? false : driver.Dancing.Value,
                                 Reading = driver.Reading == null ? false : driver.Reading.Value,
                                 Travelling = driver.Travelling == null ? false : driver.Travelling.Value,

                                 GenderId = gender.GenderId,
                                 ActiveId = active.IsActive,
                                };

            model = driverInfo.FirstOrDefault();    
            model.GenList = db.GenderTables
                               .Select(x => new DropdownModel { ID = x.GenderId, TEXT = x.Category }).ToList();
            model.ActList = db.ActivityTables
                              .Select(x => new DropdownModel { ID = x.IsActive, TEXT = x.Available }).ToList();

            //Updating pre-selected Hobbies for checkbox
            var MapList = db.MapDriverHobs.Where(x => x.DriverId == id).
                Select(x => new MappingModel
                {
                    MapId = x.MapId,
                    DriverId = x.DriverId ==null?0:x.DriverId.Value,
                    HobbyId = x.HobbyId == null ? 0 : x.HobbyId.Value,
                }).ToList();

           var HobbyList = db.HobbyTables
                                 .Select(x => new HobbyModel { HobbyId = x.HobbyId, Hobby = x.Hobby, IsActive = x.IsActive == null ? false : x.IsActive.Value }).ToList();

            foreach(var hob in HobbyList)
            {
                foreach(var map in MapList)
                {
                    if(map.HobbyId == hob.HobbyId) hob.IsActive = true;
                }
            }

            model.HobList = HobbyList;
            return View(model);
        }

        //POST: Driver/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DriverInfoModel model)
        {
            if (!ModelState.IsValid)
            {
               return View(model);
            }

            DriverTable driver = new DriverTable();

            driver.DriverId = model.DriverId;
            driver.Name = model.DriverName;
            driver.ContactNo = model.ContactNo; 
            driver.GenderId =  model.GenderId;
            driver.IsActive = model.ActiveId;
            //Hobbbies List
            driver.Football = model.Football;
            driver.Basketball = model.Basketball;
            driver.Cricket = model.Cricket;
            driver.Singing = model.Singing;
            driver.Dancing = model.Dancing;
            driver.Reading = model.Reading;
            driver.Travelling = model.Travelling;

            if (model.HobList.Count(x => x.IsActive) == 0)
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

                //--( > Start! <)--String concatenation for Hobbies in columns-----

                if (model.Football) sb.Append("/Football");
                if (model.Basketball) sb.Append("/Basketball");
                if (model.Cricket) sb.Append("/Cricket");
                if (model.Singing) sb.Append("/Singing");
                if (model.Dancing) sb.Append("/Dancing");
                if (model.Reading) sb.Append("/Reading");
                if (model.Travelling) sb.Append("/Travelling");

                //--( > End! <)--String concatenation for Hobbies in columns-----

                model.Hobby = sb.ToString();    
                driver.Hobby = model.Hobby;
            }

            db.Entry(driver).State = EntityState.Modified;
            db.SaveChanges();

            //Delete previous hobbies
            var mapList = db.MapDriverHobs.Where(x => x.DriverId == model.DriverId)
                        .Select(x => new MappingModel
                        {
                            MapId = x.MapId,
                            DriverId = x.DriverId == null ? 0 : x.DriverId.Value,
                            HobbyId = x.HobbyId == null ? 0 : x.HobbyId.Value,
                        }).ToList();

            foreach(var map in mapList)
            {
                var delMap = db.MapDriverHobs.Find(map.MapId);
                db.MapDriverHobs.Remove(delMap);
                db.SaveChanges();
            }
           
            //>_Adding to Mapping Table.........
            MapDriverHob mappingDH = new MapDriverHob();
            mappingDH.MapId = model.MapId;

            foreach (var hob in model.HobList)
            {
                if (hob.IsActive)
                {
                    mappingDH.DriverId = driver.DriverId;
                    mappingDH.HobbyId = hob.HobbyId;
                    db.MapDriverHobs.Add(mappingDH);
                    db.SaveChanges();
                }
            }
            //>_Added to Mapping Table.........

            return RedirectToAction("Index");
        }

        //POST: Driver/Delete/id-----------jjjJSsss--------
        public ActionResult Delete(int? id)
        {
            var driver = db.DriverTables.Find(id);
            db.DriverTables.Remove(driver);
            db.SaveChanges();
            //Delete mapped rows(hobbies) of respective IDs
            var mapList = db.MapDriverHobs.Where(x => x.DriverId == id)
                       .Select(x => new MappingModel
                       {
                           MapId = x.MapId,
                           DriverId = x.DriverId == null ? 0 : x.DriverId.Value,
                           HobbyId = x.HobbyId == null ? 0 : x.HobbyId.Value,
                       }).ToList();

            foreach (var map in mapList)
            {
                var delMap = db.MapDriverHobs.Find(map.MapId);
                db.MapDriverHobs.Remove(delMap);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        } 
    }
}