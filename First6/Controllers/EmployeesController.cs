using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using First6.Models;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;

namespace First6.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.Department);
            return View(employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return PartialView(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            return PartialView();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeId,EmployeeName,Position,DepartmentId,File")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
              
                    employee.File.SaveAs(Server.MapPath("~/ImagesEmployees/") + employee.EmployeeId + ".jpg");

                
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            return PartialView(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeId,EmployeeName,Position,DepartmentId,File")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                if (employee.File != null)
                {
                    employee.File.SaveAs(Server.MapPath("~/ImagesEmployees/") + employee.EmployeeId + ".jpg");

                }
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return PartialView(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
       // [ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            return Json(JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // function ExportToExcel
        public void ExportToExcel()
        {
            var employees = db.Employees.Include(e => e.Department).ToList();
            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
            ws.Cells["A1"].Value = "Communication";
            ws.Cells["B1"].Value = "Com1";

            ws.Cells["A2"].Value = "Report";
            ws.Cells["B2"].Value = "report1";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B2"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);

            // start table 
            ws.Cells["A6"].Value = "EmployeeID";
            ws.Cells["B6"].Value = "FirstName";
            ws.Cells["C6"].Value = "Position";
            ws.Cells["D6"].Value = "DepartmentName";

            int rowStart = 7;
            foreach (var item in employees)
            {
                //if (item.Experience < 5)
                //{
                //    ws.Row(rowStart).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //    ws.Row(rowStart).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("pink")));

                //}

                ws.Cells[string.Format("A{0}", rowStart)].Value = item.EmployeeId;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.EmployeeName;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.Position;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.Department.DepartmentName;
                rowStart++;
            }
            // end table
            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();


        }// end  ExportToExcel
        // import file excel use EPPlus
        public ActionResult ImportExcel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase file)
        {

            try
            {
                if (Path.GetExtension(file.FileName) == ".xlsx" || Path.GetExtension(file.FileName) == ".xls")
            {
                ExcelPackage package = new ExcelPackage(file.InputStream);
                DataTable Dt = ExcelPackageExtensions.ToDataTable(package);


                var employee = new Employee();
               
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                        employee.EmployeeName = Dt.Rows[i][1].ToString();
                    employee.Position = Dt.Rows[i][2].ToString();
                    var x = Dt.Rows[i][3].ToString();
                    employee.DepartmentId = Int32.Parse(x);
                    
                        db.Employees.Add(employee);
                        db.SaveChanges();
                   

                    

                }
               
            }
            }
                    catch { }

            return RedirectToAction("Index");
        }//end import excel file to data base
        // Crystal Report
        public ActionResult Reports(string ReportType)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/EmployeeReport.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "EmployeeDataSet";
            reportDataSource.Value = db.Employees.ToList();
            localReport.DataSources.Add(reportDataSource);
            string mimeType;
            string encoding;
            string fileNameExtension;
            if (ReportType == "Excel")
            {
                fileNameExtension = "xlsx";
            }
            else if (ReportType == "Word")
            {
                fileNameExtension = "docx";
            }
            else if (ReportType == "PDF")
            {
                fileNameExtension = "pdf";
            }
            else if (ReportType == "Image")
            {
                fileNameExtension = "jpg";
            }
            string[] streams;
            Warning[] warnings;
            byte[] renderedByte;
            renderedByte = localReport.Render(ReportType, "", out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            Response.AddHeader("content-disposition", "attachment: filename=" + "EmployeesReports." + fileNameExtension);
            return File(renderedByte, fileNameExtension);
        }// end crystal report

    }
}
