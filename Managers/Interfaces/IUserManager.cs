using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Models;

namespace BuyMore.Managers.Interfaces
{
    public interface IUserManager
    {
        User? Login();
        void CreateAdmin(string loginUser);
        User Register();
        void UpdateUser(string loginUser);
        void UpdateUserWallet(int id, double newBalance);
        void GetUserByEmail(string email);
        void GetUserById(int id);
        void GetAll();
    }
}