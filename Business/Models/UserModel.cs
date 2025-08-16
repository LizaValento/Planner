using System;
using System.Collections.Generic;
using System.Linq;
using Data.Entities;


namespace Business.Models
{
    public class UserModel
    {
        public UserModel() { }
        public UserModel(User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Nickname = user.Nickname;
            Password = user.Password;
            Role = user.Role;
            Books = user.Books?.Select(b => new BookModel(b)).ToList() ?? new List<BookModel>();
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public List<BookModel> Books { get; set; }
    }
}
