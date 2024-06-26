﻿using SV20T1020080.DataLayers;
using SV20T1020080.DataLayers.SQLServer;
using SV20T1020080.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020080.BusinessLayers
{

    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;

            employeeAccountDB = new EmployeeAccountDAL(connectionString);
        }
        public static UserAccount? Authorize(string userName, string password)
        {
            //TODO: Kiểm tra thông tin đăng nhập của Employee
            return employeeAccountDB.Authorize(userName, password);

        }
        public static bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            //TODO: Thay đổi mật khẩu của Employee
            return employeeAccountDB.ChangePassword(userName, oldPassword, newPassword);
        }
    }
}
