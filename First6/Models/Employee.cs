using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace First6.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        //[Required]
        //[Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        //[Required]
        //[Display(Name = "Department Id")]
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
    }
}