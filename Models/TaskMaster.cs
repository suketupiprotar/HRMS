//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRMSWithTheme.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaskMaster
    {
        public int TaskId { get; set; }
        public Nullable<System.DateTime> TaskDate { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public Nullable<int> ApproverId { get; set; }
        public Nullable<int> ApprovedorRejectedBy { get; set; }
        public Nullable<System.DateTime> ApprovedorRejectedOn { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        public virtual EmployeeMaster EmployeeMaster { get; set; }
        public virtual EmployeeMaster EmployeeMaster1 { get; set; }
        public virtual EmployeeMaster EmployeeMaster2 { get; set; }
    }
}