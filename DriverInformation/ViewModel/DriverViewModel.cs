using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DriverInformation.ViewModel
{
    //public class DriverViewModel
    //{
    //}

    public class GenderModel
    {
        public int GenderId{ get; set; }
        public string Category { get; set; }
    }

    public class ActivityModel
    {
        public int ActiveId { get; set; }
        public string Available { get; set; }
    }

    public class HobbyModel
    {
        public int HobbyId { get; set; }
        public string Hobby { get; set; }
        //public Nullable<bool> IsActive { get; set; }
        public bool IsActive { get; set; }  
    }
    public class DropdownModel
    {
        public int ID { get;set; } 
        public string TEXT { get;set; }
    }

    public class DriverInfoModel
    {
        public int DriverId { get; set; }
        public string DriverName { get; set; }
        public string ContactNo { get; set; }
        public int GenderId { get; set; }
        public string Category { get; set; }
        public int ActiveId { get; set; }
        public string Available { get; set; }
        //public int HobbyId { get; set; }

        //[Required(ErrorMessage = "Please Select A Hobby.")]
        public string Hobby { get; set; }

        public List<HobbyModel> HobList { get; set; }
        public List<DropdownModel> GenList { get; set; }
        public List<DropdownModel> ActList { get; set; }

        internal object AddModelError()
        {
            throw new NotImplementedException("!!Please Select a hobby!");
        }
    }

}