using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Service.Interfaces
{
    public interface IUserService
    {
        User GetById(int id);
        List<User> GetUsers();
        void AddUser(UserModel user);
        void UpdateUser(UserModel user);
        void DeleteUser(int id);
    }
}
