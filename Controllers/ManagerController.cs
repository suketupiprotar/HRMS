using HRMSWithTheme.CustomModels;
using HRMSWithTheme.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMSWithTheme.Controllers
{
    [Authorize]
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        suketuEntities entities = new suketuEntities();
        // GET: Manager

        public ActionResult DashboardView()
        {
            return View("~/Views/Manager/DashBoard.cshtml");
        }
        public ActionResult DashBoard()
        {
            try
            {

                var sid = Convert.ToInt32(Session["EmployeeId"]);
                string draw = Request.Form.GetValues("draw")[0];
                string search = Request.Form.GetValues("search[value]")[0];
                int start = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
                string orderBy = Request["columns[" + Request["order[0][column]"] + "][data]"];
                string orderDir = Request.Form["order[0][dir]"];


                var data = entities.EmployeeMasters.ToList().Where(e => (e.DepartmentId == null || e.DepartmentId == 2 || e.DepartmentId == 3) && e.ReportingPerson == sid).Select(emp => new
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

                }).ToList();

                // Apply sorting
                if (orderDir == "asc")
                {
                    data = data.OrderBy(x => GetPropertyValue(x, orderBy)).ToList();
                }
                else
                {
                    data = data.OrderByDescending(x => GetPropertyValue(x, orderBy)).ToList();
                }

                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        data = data.Where(x => x.FirstName.ToLower().Contains(search.ToLower())
                                    || x.EmployeeId.ToString().Contains(search.ToLower())
                                    || x.ReportingPerson.ToString().Contains(search.ToLower())
                                    || x.LastName.ToLower().Contains(search.ToLower())
                                    || x.Gender.ToLower().Contains(search.ToLower())).ToList();
                    }
                }
                int totalRecords = data.Count;

                var recFilter = data.Count;
                var displayResult = data.Skip(start).Take(pageSize).ToList();

                return Json(new
                {
                    success = true,
                    draw = Convert.ToInt32(draw),
                    recordsTotal = totalRecords,
                    recordsFiltered = recFilter,
                    data = displayResult
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return View(e);
            }
            return View();
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo.GetValue(obj, null);
        }

        public ActionResult Edit(int id)
        {

            var sid = Convert.ToInt32(Session["EmployeeId"]);

            //ViewBag.EmployeeList = entity.EmployeeMasters.ToList();
            EmployeeMaster emp = entities.EmployeeMasters.Find(id);
            List<EmployeeMaster> employeeList = entities.EmployeeMasters
                                            .Where(e => e.EmployeeId != id)
                                            .ToList();

            if (sid == id)
            {
                return PartialView("_EditOwnProfile", emp);
            }
            else
            {
                ViewBag.EmployeeList = employeeList;
                return PartialView("_EditOtherProfile", emp);
            }


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
                    return Json(new { success = true });
                }

                return Json(new { success = false });

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


                List<TaskMaster> tasksToRemove = entities.TaskMasters.Where(task => task.EmployeeId == id).ToList();


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


        public ActionResult GetTaskListView()
        {
            return View("~/Views/Manager/TaskList.cshtml");
        }

        [HttpPost]
        public ActionResult TaskList()
        {
            try
            {

                var empId = Convert.ToInt32(Session["EmployeeId"]);

                string draw = Request.Form.GetValues("draw")[0];
                string search = Request.Form.GetValues("search[value]")[0];
                int start = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
                string orderBy = Request["columns[" + Request["order[0][column]"] + "][data]"];
                string orderDir = Request.Form["order[0][dir]"];
                var abcd = Request.Form["order[1]"];

                // Get IQueryable to defer execution until later
                var tasks = entities.TaskMasters
    .Join(entities.EmployeeMasters, task => task.EmployeeId, emp => emp.EmployeeId, (task, emp) => new { Task = task, Employee = emp })
    .Join(entities.EmployeeMasters, t => t.Employee.ReportingPerson, rep => rep.EmployeeId, (t, rep) => new { Task = t.Task, Employee = t.Employee, ReportingPerson = rep })
    .Where(x => x.ReportingPerson.EmployeeId == empId)
    .Select(x => x.Task).ToList();


                var data = tasks.Select(task => new
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

                }).ToList();

                // Apply sorting
                if (orderDir == "asc")
                {
                    data = data.OrderBy(x => GetPropertyValue(x, orderBy)).ToList();
                }
                else
                {
                    data = data.OrderByDescending(x => GetPropertyValue(x, orderBy)).ToList();
                }

                // Searching
                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        data = data.Where(x => x.TaskName.ToLower().Contains(search.ToLower())
                                    || x.TaskId.ToString().Contains(search.ToLower())
                                    || x.EmployeeId.ToString().Contains(search.ToLower())
                                    || x.ApprovedorRejectedBy.ToString().Contains(search.ToLower())
                                    || x.ApprovedorRejectedOn.ToString().Contains(search.ToLower())
                                    || x.TaskDescription.ToLower().Contains(search.ToLower())
                                    || x.Status.ToLower().Contains(search.ToLower())).ToList();
                    }
                }


                // Total count before filtering
                var totalRecords = data.Count();
                var recFilter = data.Count;
                //Pagination
                var displayResult = data.Skip(start).Take(pageSize).ToList();


                // Return JSON response
                return Json(new
                {
                    success = true,
                    draw = Convert.ToInt32(draw),
                    recordsTotal = totalRecords,
                    recordsFiltered = recFilter,
                    data = displayResult
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ApproveOrRejectTask(int taskId, string status)
        {
            try
            {

                TaskMaster task = entities.TaskMasters.Find(taskId);
                if (task != null)
                {
                    task.Status = status;
                    task.ApproverId = Convert.ToInt32(Session["EmployeeId"]);
                    task.ApprovedorRejectedBy = Convert.ToInt32(Session["EmployeeId"]);
                    task.ApprovedorRejectedOn = DateTime.Now;
                    task.ModifiedOn = DateTime.Now;
                    entities.Entry(task).State = EntityState.Modified;
                    entities.SaveChanges();
                }


                return Json(new { success = true, taskStatus = status }); ;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
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

                if (!ModelState.IsValid)
                {
                    if (task.Status == null)
                    {
                        task.Status = "Pending";
                    }
                    task.CreatedOn = DateTime.Now;
                    task.ModifiedOn = DateTime.Now;
                    task.EmployeeId = Convert.ToInt32(Session["EmployeeId"]);
                    if (!ModelState.IsValidField("TaskId"))
                    {
                        //ModelState.ClearValidationState("FieldName");
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
                        entities.Entry(task).State = EntityState.Modified;
                        entities.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false });
                }

                

            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
        }
        public ActionResult ManagerTaskView()
        {
            return View("~/Views/Manager/ManagerTask.cshtml");
        }
        public ActionResult ManagerTask()
        {
            try
            {
                var empId = Convert.ToInt32(Session["EmployeeId"]);
                string draw = Request.Form.GetValues("draw")[0];
                string search = Request.Form.GetValues("search[value]")[0];
                int start = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
                string orderBy = Request["columns[" + Request["order[0][column]"] + "][data]"];
                string orderDir = Request.Form["order[0][dir]"];

              

                var data = entities.TaskMasters.ToList().Where(x => x.EmployeeId == empId).Select(task => new
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

                }).ToList();

                // Apply sorting
                if (orderDir == "asc")
                {
                    data = data.OrderBy(x => GetPropertyValue(x, orderBy)).ToList();
                }
                else
                {
                    data = data.OrderByDescending(x => GetPropertyValue(x, orderBy)).ToList();
                }


                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        data = data.Where(x => x.TaskName.ToLower().Contains(search.ToLower())
                                    || x.TaskId.ToString().Contains(search.ToLower())
                                    || x.EmployeeId.ToString().Contains(search.ToLower())
                                    || x.ApprovedorRejectedBy.ToString().Contains(search.ToLower())
                                    || x.ApprovedorRejectedOn.ToString().Contains(search.ToLower())
                                    || x.TaskDescription.ToLower().Contains(search.ToLower())
                                    || x.Status.ToLower().Contains(search.ToLower())).ToList();
                    }
                }

                // Total count before filtering
                var totalRecords = data.Count();
                var recFilter = data.Count;
                //    //Pagination
                var displayResult = data.Skip(start).Take(pageSize).ToList();

                // Return JSON response
                return Json(new
                {
                    success = true,
                    draw = Convert.ToInt32(draw),
                    recordsTotal = totalRecords,
                    recordsFiltered = recFilter,
                    data = displayResult
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ManagerTaskDelete(int id)
        {
            try
            {
                if(id != 0) {
                    TaskMaster task = entities.TaskMasters.Find(id);
                    entities.TaskMasters.Remove(task);
                    entities.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
        }
        public ActionResult childTask(int id)
        {
            try
            {

                List<TaskMaster> tasks = entities.TaskMasters
                                        .Where(x => x.EmployeeId == id)
                                        .ToList();
                return View(tasks);


            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
        }
    }
}