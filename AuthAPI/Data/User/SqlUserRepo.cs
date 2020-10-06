using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthAPI.Hashing;
using AuthAPI.Models;
using Microsoft.Extensions.Configuration;

namespace AuthAPI.Data {
    public class SqlUserRepo : IUserRepo
    {
        private readonly Context _context;
        private readonly IConfiguration _configuration;

        public SqlUserRepo(Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #region GET
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await Task.Run(() => {
                return _context.Users.ToList();
            });
        }

        public async Task<User> GetUserWithEmailAndPasswordAsync(string email, string password)
        {
            User user = null;
            SHA256Hasher hasher = new SHA256Hasher();

            await Task.Run(() => {
                user = _context.Users?.Where(user => 
                    user.Email == email
                ).FirstOrDefault();
            });

            if(
                user != null && 
                user.Password == await hasher.EncryptStringSHA256Async(password, user.Salt)
            ){
                return user;      
            }
            return null;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await Task.Run(() => {
                return _context.Users.FirstOrDefault(p => p.UserId == id);
            });
        }

        #endregion
        #region Create
        public async Task CreateUserAsync(User user)
        {
            if(user == null) {
                throw new ArgumentNullException(nameof(user));
            }

            SHA256Hasher hasher = new SHA256Hasher();
            string salt = await hasher.GenerateSaltAsync(10);

            user.Salt = salt;
            user.Password = await hasher.EncryptStringSHA256Async(user.Password, salt);

            await _context.AddAsync(user);
        }
        #endregion
        #region Update
        public void UpdateUser(User user) {

        }
        #endregion
        #region Delete
        public async Task DeleteUserAsync(User user) {
            await Task.Run(() => {
                if(user == null) {
                    throw new ArgumentNullException(nameof(user));
                }

                _context.Users.Remove(user);
            });
        }
        #endregion
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}