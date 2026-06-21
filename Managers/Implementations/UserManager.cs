using BuyMore.Enums;
using BuyMore.Models;
using BuyMore.Managers.Interfaces;
using BuyMore.Helpers;
using BuyMore.Repositories.Interfaces;
using BuyMore.Repositories.Implementations;

namespace BuyMore.Managers.Implementations
{
    public class UserManager: IUserManager
    {        
        private readonly IUserRepository _userRepository;
        public UserManager()
        {
            _userRepository = new UserRepository();
            _userRepository.InitializeDefaultAdmin();       
        }

        public void CreateAdmin(string loginUser)
        {
            Console.Write("Enter firstName: ");
            var firstName = Console.ReadLine()!;
            Console.Write("Enter last name: ");
            var lastName = Console.ReadLine()!;

            string email;
            do
            {
                Console.Write("Enter email: ");
                email = Console.ReadLine()!;
            }while(string.IsNullOrEmpty(email) || EmailAlreadyExist(email));

            Console.Write("Enter password: ");
            var password = Console.ReadLine()!;
            Console.Write("Enter phone number: ");
            var phone = Console.ReadLine()!;
            Console.Write("Enter address: ");
            var address = Console.ReadLine()!;

            var encryptedPassword = UserHelper.EncryptPassword(password);
            var admin = new User(firstName, lastName, phone, email, encryptedPassword, address, Role.Admin, loginUser);
            _userRepository.AddUser(admin);

            Console.WriteLine("Admin user created successfully...");
            Console.WriteLine($"Name: {firstName} {lastName}    Email: {email}");
        }

        public User? Login()
        {
            Console.Write("Enter your email: ");
            var email = Console.ReadLine()!;
            Console.Write("Enter your password: ");
            var password = Console.ReadLine()!;
            var user = _userRepository.GetUserByEmail(email);
            if (user != null)
            {
                if(UserHelper.IsValidPassword(password, user.EncryptedPassword))
                {
                    return user;
                }
            }
            Console.WriteLine("Invalid user email or password.");
            return null;
        }

        public User Register()
        {
            Console.Write("Enter firstName: ");
            var firstName = Console.ReadLine()!;
            Console.Write("Enter last name: ");
            var lastName = Console.ReadLine()!;
            string email;
            do
            {
                Console.Write("Enter email: ");
                email = Console.ReadLine()!;
            }while(string.IsNullOrEmpty(email) || EmailAlreadyExist(email));
            Console.Write("Enter password: ");
            var password = Console.ReadLine()!;

            var encryptedPassword = UserHelper.EncryptPassword(password);
            var user = new User(firstName, lastName, null, email, encryptedPassword, null, Role.Customer, email);
            _userRepository.AddUser(user);

            Console.WriteLine("Registration completed successfully...");
            Console.WriteLine($"Name: {firstName} {lastName}    Email: {email}");

            return user;
        }

        public void UpdateUser(string loginUser)
        {
            var user = _userRepository.GetUserByEmail(loginUser);

            if(user == null)
            {
                Console.WriteLine("User details not found.");
                return;
            }

            Console.WriteLine(user);
            
            Console.Write("Enter firstName: ");
            var firstName = Console.ReadLine()!;
            Console.Write("Enter last name: ");
            var lastName = Console.ReadLine()!;
            Console.Write("Enter phone number: ");
            var phone = Console.ReadLine()!;
            Console.Write("Enter address: ");
            var address = Console.ReadLine()!;

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Address = address;
            user.PhoneNumber = phone;

            _userRepository.UpdateUser(user.Id, user);
            Console.WriteLine("Profile updated successfully.");
        }

        public void GetUserByEmail(string email)
        {
            var user = _userRepository.GetUserByEmail(email);
            if(user == null)
            {
                Console.WriteLine($"User with Email: {email} not found.");
            }
            Console.WriteLine(user);
        }

        public void GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if(user == null)
            {
                Console.WriteLine($"User with Id: {id} not found.");
            }
            Console.WriteLine(user);
        }

        public void GetAll()
        {
            var users = _userRepository.GetAllUsers();
            if(users.Count == 0)
            {
                Console.WriteLine("No user found.");
            }
            foreach(var user in users)
            {
                Console.WriteLine(user);
            }
        }

        public void UpdateUserWallet(int id, decimal newBalance)
        {
            var user = _userRepository.GetUserById(id);
            if(user == null)
            {
                Console.WriteLine($"User with Id: {id} not found.");
                return;
            }
            _userRepository.UpdateWalletBalance(id, newBalance);
            Console.WriteLine("Wallet balance updated successfully.");
        }

        private bool EmailAlreadyExist(string email)
        {
            var exists = _userRepository.GetUserByEmail(email) != null;
            if (exists)
            {
                Console.WriteLine($"{email} already exist, please use another email.");
                return true;
            }
            return false;
        }
    }
}