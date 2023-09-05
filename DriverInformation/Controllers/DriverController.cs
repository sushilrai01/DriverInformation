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
        private DriverManagementEntities db = new DriverManagementEntities();
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
           model.Dropdownlist = db.GenderTables
                                .Select(x => new DropdownModel { ID = x.GenderId, TEXT = x.Category }).ToList();

            return View(model);
        }
    }
}