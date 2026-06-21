using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyMore.Enums;

namespace BuyMore.Models
{
    public class User: BaseModel
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = default!;
        public string EncryptedPassword { get; set; } = default!;
        public string? Address { get; set; }
        public Role Role { get; set; } = default!;
        public double WalletBalance { get; private set; }

        public User(int id, string firstName, string lastName, string? phoneNumber, string email, string encryptedPassword, string? address, Role role, string createdBy, DateTime createdDate)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            EncryptedPassword = encryptedPassword;
            Address = address;
            Role = role;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }

        public User(string firstName, string lastName, string? phoneNumber, string email, string encryptedPassword, string? address, Role role, string createdBy)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            EncryptedPassword = encryptedPassword;
            Address = address;
            Role = role;
            CreatedBy = createdBy;
            CreatedDate = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"ID: {Id}\tName: {FirstName} {LastName}\tPhoneNo: {PhoneNumber}\tEmail: {Email}\tAddress: {Address}\tUserType: {Role}\tWallet: {WalletBalance:C}";
        }

        public void CreditWallet(double amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
            }

            WalletBalance += amount;
        }

        public bool TryDebitWallet(double amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            if (WalletBalance < amount)
            {
                return false;
            }

            WalletBalance -= amount;
            return true;
        }
    }
}
