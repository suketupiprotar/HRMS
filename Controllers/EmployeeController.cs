using HRMSWithTheme.CustomModels;
using HRMSWithTheme.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace HRMSWithTheme.Controllers
{
    [Authorize]
    [Authorize(Roles = "Employee,null")]
    public class EmployeeController : Controller
    {
        // GET: Employee
        suketuEntities entities = new suketuEntities();
        public ActionResult DashboardView()
        {
            return View("~/Views/Employee/DashBoard.cshtml");
        }
        public ActionResult DashBoard(JqueryDatatableParam param)
        {
            try
            {

                int sid = Convert.ToInt32(Session["EmployeeId"]);
                var emps = entities.EmployeeMasters
                                .Where(e => e.EmployeeId == sid);
                if (param.search != null)
                {
                    if (!string.IsNullOrEmpty(param.search.value))
                    {
                        emps = emps.Where(x => x.FirstName.ToLower().Contains(param.search.value.ToLower())
                                    || x.EmployeeId.ToString().Contains(param.search.value.ToLower())
                                    || x.ReportingPerson.ToString().Contains(param.search.value.ToLower())
                                    || x.LastName.ToLower().Contains(param.search.value.ToLower())
                                    || x.Gender.ToLower().Contains(param.search.value.ToLower()));
                    }
                }
                var totalRecords = emps.Count();
                List<object> result = new List<object>();
                foreach (var emp in emps)
                {
                    EmployeeMaster employeeMaster = new EmployeeMaster
                    {
                        EmployeeId = emp.EmployeeId,
                        FirstName = emp.FirstName,
                        LastName = emp.LastName,
                        Gender = emp.Gender,
                        ReportingPerson = emp.ReportingPerson,
                        Email = emp.Email,
                        Password = emp.Password,
                        BirthDate = emp.BirthDate,
                        DepartmentId = emp.DepartmentId,
                    };
                    result.Add(employeeMaster);
                }
                return Json(new
                {
                    param.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords, // Since we're not filtering yet on the server side
                    data = result,
                    success = true
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return View(e);
            }
            return View();
        }


        public ActionResult Edit(int id)
        {

            EmployeeMaster emp = entities.EmployeeMasters.Find(id);
            return PartialView("_EditProfile",emp);
        }
        [HttpPost]
        public ActionResult Edit(EmployeeMaster emp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    emp.DepartmentId = Convert.ToInt32(emp.DepartmentId);
                    entities.Entry(emp).State = EntityState.Modified;
                    entities.SaveChanges();
                    return Json(new {success = true});
                }
                return Json(new {success = false});

            }
            catch (Exception e)
            {
                return View(e);
            }
        }


        public ActionResult Delete(int id)
        {
            try
            {

                EmployeeMaster emp = entities.EmployeeMasters.Find(id);

                // Find all tasks associated with the employee
                List<TaskMaster> tasksToRemove = entities.TaskMasters.Where(task => task.EmployeeId == id).ToList();

                // Remove tasks associated with the employee
                if (tasksToRemove.Count > 0)
                {
                    foreach (var task in tasksToRemove)
                    {
                        entities.TaskMasters.Remove(task);
                    }
                }



                // Remove the employee
                entities.EmployeeMasters.Remove(emp);

                // Save changes
                entities.SaveChanges();
                return RedirectToAction("DashBoard");

            }
            catch (Exception e)
            {
                return View(e.ToString());
            }
        }

        public ActionResult TaskCreate(int id)
        {
            if (id == 0)
            {
                return PartialView("_TaskCreate");
            }
            else
            {
                var task = entities.TaskMasters.FirstOrDefault(x => x.TaskId == id);
                return PartialView("_TaskCreate", task);
            }
        }
        [HttpPost]
        public ActionResult TaskCreate(TaskMaster task)
        {
            try
            {
                if (!ModelState.IsValid) {

                    if (task.Status == null)
                    {
                        task.Status = "Pending";
                    }
                    task.CreatedOn = DateTime.Now;
                    task.ModifiedOn = DateTime.Now;
                    task.EmployeeId = Convert.ToInt32(Session["EmployeeId"]);
                    TaskMaster tdk = task;
                    if (!ModelState.IsValidField("TaskId"))
                    {
                        entities.TaskMasters.Add(task);
                        entities.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false });
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        task.ModifiedOn = DateTime.Now;
                        entities.Entry(task).State = EntityState.Modified;
                        entities.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false });
                }

                return View();

            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
        }
        public ActionResult EmployeeTaskView()
        {
            return View("~/Views/Employee/EmployeeTask.cshtml");
        }
        public ActionResult EmployeeTask(JqueryDatatableParam param)
        {
            try
            {
                var empId = Convert.ToInt32(Session["EmployeeId"]);

                var tasks = entities.TaskMasters
                                         .Where(x => x.EmployeeId == empId);

                if (param.search != null)
                {
                    if (!string.IsNullOrEmpty(param.search.value))
                    {
                        tasks = tasks.Where(x => x.TaskName.ToLower().Contains(param.search.value.ToLower())
                                    || x.TaskId.ToString().Contains(param.search.value.ToLower())
                                    || x.EmployeeId.ToString().Contains(param.search.value.ToLower())
                                    || x.ApprovedorRejectedBy.ToString().Contains(param.search.value.ToLower())
                                    || x.ApprovedorRejectedOn.ToString().Contains(param.search.value.ToLower())
                                    || x.TaskDescription.ToLower().Contains(param.search.value.ToLower())
                                    || x.Status.ToLower().Contains(param.search.value.ToLower()));
                    }
                }
                // Total count before filtering
                var totalRecords = tasks.Count();

                List<object> result = new List<object>();
                foreach (var task in tasks)
                {
                    TaskMaster AllTask = new TaskMaster
                    {
                        TaskId = task.TaskId,
                        TaskName = task.TaskName,
                        EmployeeId = task.EmployeeId,
                        TaskDescription = task.TaskDescription,
                        TaskDate = task.TaskDate,
                        ApprovedorRejectedBy = task.ApprovedorRejectedBy,
                        ApprovedorRejectedOn = task.ApprovedorRejectedOn,
                        CreatedOn = task.CreatedOn,
                        ModifiedOn = task.ModifiedOn,
                        ApproverId = task.ApproverId,
                        Status = task.Status,
                    };
                    result.Add(AllTask);
                }
                // Return JSON response
                return Json(new
                {
                    param.draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords, // Since we're not filtering yet on the server side
                    data = result,
                    success = true
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return View(e.Message);
            }
        }


        public ActionResult EmployeeTaskDelete(int id)
        {
            try
            {

                TaskMaster task = entities.TaskMasters.Find(id);
                entities.TaskMasters.Remove(task);
                entities.SaveChanges();
                return RedirectToAction("DirectorTask");

            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
        }

    }
}