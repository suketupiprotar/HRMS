using HRMSWithTheme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace HRMSWithTheme
{
    public class WebRoleProvider : RoleProvider
    {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            using (suketuEntities entities = new suketuEntities())
            {
                //var result = (from emp in entities.EmployeeMasters
                //              join dept in entities.DepartmentMasters on emp.DepartmentId equals dept.DepartmentId
                //              where emp.Email == username
                //              select dept.DepartmentName).ToArray();

                var result = (from emp in entities.EmployeeMasters
                              join dept in entities.DepartmentMasters on emp.DepartmentId equals dept.DepartmentId
                              where emp.Email == username
                              select dept.DepartmentName)
             .Where(name => name == "Director" || name == "Manager")
             .DefaultIfEmpty("null")
             .ToArray();
                //Array.Clear(result, 0, result.Length);
                //if(!result.Contains("Director") && !result.Contains("Manager"))
                //{
                //    result.Append("null");
                //}

                return result;
            }
        }
      
        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}