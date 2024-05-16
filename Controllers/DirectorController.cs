using HRMSWithTheme.CustomModels;
using HRMSWithTheme.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HRMSWithTheme.Controllers
{
    //[Authorize]
    [Authorize(Roles = "Director")]
    public class DirectorController : Controller
    {
        // GET: Director
        suketuEntities entity = new suketuEntities();

        public ActionResult DashboardView()
        {
            return View("~/Views/Director/DashBoard.cshtml");
        }

        public ActionResult DashBoard()
        {
            try
            {
                var sid = Convert.ToInt32(Session["EmployeeId"]);
                //var emps = entity.EmployeeMasters.Where(e => e.EmployeeId != sid);

                string draw = Request.Form.GetValues("draw")[0];
                string search = Request.Form.GetValues("search[value]")[0];
                int start = Convert.ToInt32(Request.Form.GetValues("start")[0]);
                int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
                //string orderColumn = Request.Form["order[0][column]"];
                string orderBy = Request["columns[" + Request["order[0][column]"] + "][data]"];
                string orderDir = Request.Form["order[0][dir]"];

        //        Dictionary<int, string> columnMapping = new Dictionary<int, string>
        //{
        //    {0, "EmployeeId"},
        //    {1, "FirstName"},
        //    {2, "LastName"},
        //    {3, "Email"},
        //    {4, "BirthDate"},
        //    {5, "Gender"},
        //    {6, "DepartmentId"},
        //    {7, "ReportingPerson"}
        //};

        //        // Get the property name for the column being sorted
        //        string orderBy = columnMapping[int.Parse(orderColumn)];

                var data = entity.EmployeeMasters.ToList().Where(e => e.EmployeeId != sid).Select(emp => new
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
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Function to get property value using reflection
        private object GetPropertyValue(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo.GetValue(obj, null);
        }

        public ActionResult Edit(int id)
        {

            //ViewBag.EmployeeList = entity.EmployeeMasters.ToList();
            EmployeeMaster emp = entity.EmployeeMasters.Find(id);
            List<EmployeeMaster> employeeList = entity.EmployeeMasters
                                            .Where(e => e.EmployeeId != id)
                                            .ToList();
            ViewBag.EmployeeList = employeeList;
            return PartialView("_EditOtherProfile", emp);


        }
        [HttpPost]
        public ActionResult Edit(EmployeeMaster emp)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    emp.DepartmentId = Convert.ToInt32(emp.DepartmentId);
                    entity.Entry(emp).State = EntityState.Modified;
                    entity.SaveChanges();
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
                if (id > 0)
                {
                    EmployeeMaster emp = entity.EmployeeMasters.Find(id);

                    List<TaskMaster> tasksToRemove = entity.TaskMasters.Where(task => task.EmployeeId == id).ToList();

                    if (tasksToRemove.Count > 0)
                    {
                        foreach (var task in tasksToRemove)
                        {
                            entity.TaskMasters.Remove(task);
                        }
                    }

                    entity.EmployeeMasters.Remove(emp);
                    entity.SaveChanges();
                    return RedirectToAction("DashBoard");
                }
                else
                {
                    return RedirectToAction("DashBoard");
                }


            }
            catch (Exception e)
            {
                return View(e.ToString());
            }
        }
        public ActionResult GetTaskListView()
        {
            return View("~/Views/Director/TaskList.cshtml");
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
                //string orderColumn = Request.Form["order[0][column]"];
                string orderDir = Request.Form["order[0][dir]"];
                var abcd = Request.Form["order[1]"];

                //var tasks = entity.TaskMasters.Where(x => x.EmployeeId != empId);

        //        Dictionary<int, string> columnMapping = new Dictionary<int, string>
        //{
        //    {0, "TaskId"},
        //    {1, "TaskDate"},
        //    {2, "EmployeeId"},
        //    {3, "TaskName"},
        //    {4, "TaskDescription"},
        //    {5, "Status"},
        //    {6, "CreatedOn"},
        //    {7, "ModifiedOn"},
        //    {8, "ApprovedorRejectedBy"}
        //};

        //        // Get the property name for the column being sorted
        //        string orderBy = columnMapping[int.Parse(orderColumn)];

                var data = entity.TaskMasters.ToList().Where(x=>x.EmployeeId != empId).Select(task => new
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
            catch (Exception e)
            {
                // Handle exceptions gracefully
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult ApproveOrRejectTask(int taskId, string status)
        {
            try
            {

                TaskMaster task = entity.TaskMasters.Find(taskId);
                if (task != null)
                {
                    task.Status = status;
                    task.ApproverId = Convert.ToInt32(Session["EmployeeId"]);
                    task.ApprovedorRejectedBy = Convert.ToInt32(Session["EmployeeId"]);
                    task.ApprovedorRejectedOn = DateTime.Now;
                    task.ModifiedOn = DateTime.Now;
                    entity.Entry(task).State = EntityState.Modified;
                    entity.SaveChanges();
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
                var task = entity.TaskMasters.FirstOrDefault(x => x.TaskId == id);
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
                    task.Status = "Approve";
                    task.ApproverId = Convert.ToInt32(Session["EmployeeId"]);
                    task.ApprovedorRejectedBy = Convert.ToInt32(Session["EmployeeId"]);
                    task.ApprovedorRejectedOn = DateTime.Now;
                    task.CreatedOn = DateTime.Now;
                    task.ModifiedOn = DateTime.Now;
                    task.EmployeeId = Convert.ToInt32(Session["EmployeeId"]);
                    if (!ModelState.IsValidField("TaskId"))
                    {
                        //ModelState.ClearValidationState("FieldName");
                        entity.TaskMasters.Add(task);
                        entity.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false });
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        entity.Entry(task).State = EntityState.Modified;
                        entity.SaveChanges();
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

        public ActionResult DirectorTaskView()
        {
            return View("~/Views/Director/DirectorTask.cshtml");
        }
        public ActionResult DirectorTask()
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

    //            Dictionary<int, string> columnMapping = new Dictionary<int, string>
    //        {
    //        { 0, "TaskId" },
    //    { 1, "TaskDate" },
    //    { 2, "EmployeeId" },
    //    { 3, "TaskName" },
    //    { 4, "TaskDescription" },
    //    { 5, "Status" },
    //    { 6, "CreatedOn" },
    //    { 7, "ModifiedOn" },
    //    { 8, "ApprovedorRejectedBy" }
    //};
    //            // Get the property name for the column being sorted
    //            string orderBy = columnMapping[int.Parse(orderColumn)];

                var data = entity.TaskMasters.ToList().Where(x => x.EmployeeId == empId).Select(task => new
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

                //    // Apply sorting
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

                //    // Total count before filtering
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

        public ActionResult DirectorTaskEdit(int id)
        {
            try
            {
                TaskMaster task = entity.TaskMasters.Find(id);
                return View(task);

            }
            catch (Exception e)
            {
                return View(e.Message);
            }
        }
        [HttpPost]
        public ActionResult DirectorTaskEdit(TaskMaster task)
        {
            try
            {
                task.ModifiedOn = DateTime.Now;
                entity.Entry(task).State = EntityState.Modified;
                entity.SaveChanges();
                return RedirectToAction("DirectorTask");
            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
        }

        public ActionResult DirectorTaskDelete(int id)
        {
            try
            {
                if (id != 0)
                {
                    TaskMaster task = entity.TaskMasters.Find(id);
                    entity.TaskMasters.Remove(task);
                    entity.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
        }
        //public ActionResult childTaskView()
        //{
        //    return PartialView("_childTask");
        //}
        public ActionResult childTask(int id)
        {
            try
            {
                List<TaskMaster> tasks = entity.TaskMasters
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