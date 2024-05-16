using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMSWithTheme.Models
{
    [MetadataType(typeof(USER_Metadata))]
    public partial class EmployeeMaster
    {
        public class USER_Metadata
        {
            public int EmployeeId { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [RegularExpression(@"^[a-zA-Z0-9._]+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$", ErrorMessage = "*Email is not valid*")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; }

            [Required(ErrorMessage = "First Name is required")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name is required")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Birth Date is required")]
            [DataType(DataType.Date)]
            public DateTime BirthDate { get; set; }

            [Required(ErrorMessage = "Gender is required")]
            public string Gender { get; set; }

            public int? DepartmentId { get; set; }

            public int? ReportingPerson { get; set; }

            public virtual DepartmentMaster DepartmentMaster { get; set; }
        }
    }
}