using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace First6.Models
{
    public class Department
    {
        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }
        //[Required]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}