using HRMSWithTheme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HRMSWithTheme.CustomModels;
namespace HRMSWithTheme.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginCustomModel login)
        {
            try
            {
         
                using (var context = new suketuEntities())
                {
                    if (ModelState.IsValid)
                    {
                        var user = context.EmployeeMasters.FirstOrDefault(x => x.Email == login.Email && login.Password == x.Password);
                        if (user != null)
                        {
                            if (user.Password == login.Password)
                            {
                                FormsAuthentication.SetAuthCookie(login.Email, false);
                                Session["FullName"] = user.FirstName + " " + user.LastName;
                                Session["Email"] = user.Email;
                                Session["EmployeeId"] = user.EmployeeId;
                                Session["department"] = user.DepartmentId;
                                //return RedirectToAction("Task", "Home");
                                if (user.DepartmentId == 1)
                                {
                                    return RedirectToAction("DashboardView", "Director");
                                }
                                else if (user.DepartmentId == 2)
                                {
                                    return RedirectToAction("DashboardView", "Manager");
                                }
                                else
                                {
                                    return RedirectToAction("DashboardView", "Employee");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Password is incorrect");
                                return View(login);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid user or Password");
                            return View(login);
                        }

                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
        }

        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(EmployeeMaster user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = new suketuEntities())
                    {
                        // Check if email already exists
                        var existingUser = context.EmployeeMasters.FirstOrDefault(x => x.Email == user.Email);
                        if (existingUser != null)
                        {
                            ModelState.AddModelError("", "Email already exists");
                            return View(user);
                        }

                        context.EmployeeMasters.Add(user);
                        context.SaveChanges();
                        return RedirectToAction("Login");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
            }
            return View(user);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}