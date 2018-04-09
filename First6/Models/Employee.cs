using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace First6.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
    }
}