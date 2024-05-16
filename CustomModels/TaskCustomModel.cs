using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMSWithTheme.Models
{
    [MetadataType(typeof(USER_Metadata))]
    public partial class TaskMaster
    {
        public class USER_Metadata
        {
            [Required(ErrorMessage = "Task Date is required")]
            public Nullable<System.DateTime> TaskDate { get; set; }


            [Required(ErrorMessage = "Task Name is required")]
            [StringLength(30)]
            public string TaskName { get; set; }


            [Required(ErrorMessage = "Task Description is required")]
            [StringLength(200)]
            public string TaskDescription { get; set; }
        }
    }
}