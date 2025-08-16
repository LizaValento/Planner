using Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Business.Service.Interfaces;
using System;
using Data.Data.Context;

namespace Business.Service.Classes
{
    public class UserService : IUserService
    {
        private readonly EventContext _context;
        public User Map(UserModel user)
        {
            var existingUser = _context.Users.Find(user.Id);

            if (existingUser == null)
            {
                existingUser = new User();
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Nickname = user.Nickname;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;
            existingUser.RoleId = user.RoleId;
            existingUser.AvatarImage = user.AvatarImage;

            return existingUser;
        }
        public UserService()
        {
        }
        public UserService(ArtContext artContext)
        {
            _context = artContext;
        }

        public T GetUserById<T>(int? id) where T : class
        {
            var user = _context.Users
                .Include(x => x.Albums)
                .ThenInclude(x => x.AlbumsArts)
                .Include(x => x.Arts)
                .ThenInclude(x => x.Likes)
                .Include(x => x.Arts)
                .ThenInclude(x => x.Comments)
                .FirstOrDefault(c => c.Id == id);

            if (user == null)
            {
                return null;
            }
            else if (typeof(T) == typeof(UserModel))
            {
                return new UserModel(user) as T;
            }
            else if (typeof(T) == typeof(ViewUserModel))
            {
                return new ViewUserModel(user) as T;
            }
            else
            {
                throw new InvalidOperationException("Неизвестный тип модели представления.");
            }
        }

        public List<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public void AddUser(UserModel user)
        {
            _context.Users.Add(Map(user));
            _context.SaveChanges();
        }

        public void UpdateUser(UserModel user)
        {
            _context.Users.Update(Map(user));
            _context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = _context.Users
                        .Include(a => a.Arts)
                        .Include(a => a.Albums)
                        .ThenInclude(a => a.AlbumsArts)
                        .FirstOrDefault(a => a.Id == id);
                    if (user != null)
                    {
                        var likesToDelete = _context.Likes.Where(a => a.UserId == user.Id);
                        _context.Likes.RemoveRange(likesToDelete);

                        var commentsToDelete = _context.Comments.Where(a => a.UserId == user.Id);
                        _context.Comments.RemoveRange(commentsToDelete);

                        foreach (var album in user.Albums)
                        {
                            _context.AlbumsArts.RemoveRange(album.AlbumsArts);
                            _context.Albums.Remove(album);
                        }

                        foreach (var art in user.Arts)
                        {
                            var artToDelete = _context.Arts.Include(a => a.Likes).Include(a => a.Comments).Include(a => a.Keywords).Include(a => a.AlbumsArts).FirstOrDefault(a => a.Id == art.Id);

                            if (artToDelete != null)
                            {
                                _context.Likes.RemoveRange(artToDelete.Likes);

                                _context.Comments.RemoveRange(artToDelete.Comments);

                                _context.Keywords.RemoveRange(artToDelete.Keywords);

                                _context.AlbumsArts.RemoveRange(artToDelete.AlbumsArts);

                                _context.Arts.Remove(artToDelete);
                            }
                        }

                        _context.Users.Remove(user);

                        _context.SaveChanges();

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public async Task<bool> VerifyPasswordAsync(UserModel user, string password)
        {
            var storedPassword = user.Password;

            return await Task.Run(() => string.Equals(password, storedPassword));
        }

        public int GetUserByToken(HttpContext httpContext)
        {
            var token = httpContext.Request.Cookies["jwt"];
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            string id = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            return int.Parse(id);
        }

    }
}

