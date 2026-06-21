using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Models;

namespace BuyMore.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void InitializeDefaultAdmin();
        void AddUser(User user);
        User? GetUserByEmail(string email);
        User? GetUserById(int id);
        List<User> GetAllUsers();
        bool UpdateUser(int id,User user);
        bool DeleteUser(int id);   
        bool UpdateWalletBalance(int id, decimal newBalance);
    }
}