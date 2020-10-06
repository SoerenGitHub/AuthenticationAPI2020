using System;
using AuthAPI.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAPI.Data {
    public class SqlTokenRepo : ITokenRepo
    {
        private readonly Context _context;
        private readonly IConfiguration _configuration;

        public SqlTokenRepo(Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        

        #region GET
        public async Task<Token> GetTokenByIdAsync(int id)
        {
            return await Task.Run(() => {
                return _context.Tokens.FirstOrDefault(p => p.Id == id);
            });
        }

        public async Task<Token> GetTokenByUserIdAsync(int userId)
        {
            return await Task.Run(() => {
                return _context.Tokens
                    .OrderByDescending(p => p.ExpiryDate)
                    .FirstOrDefault(p => p.UserId == userId);
            });
        }
        #endregion
        #region Create
        public async Task<Token> CreateTokenAsync(User trustedUser, Token trustedToken)
        {
            return await Task.Run(() => {
                if(trustedUser == null) {
                    throw new ArgumentNullException(nameof(trustedUser));
                }
                if(trustedToken == null) {
                    throw new ArgumentNullException(nameof(trustedToken));
                }

                using (var dbContextTransaction =_context.Database.BeginTransaction()) {
                    _context.Tokens.RemoveRange(_context.Tokens.Where(t => t.UserId == trustedUser.UserId));    
                    Token token = _context.Add(trustedToken).Entity;

                    dbContextTransaction.Commit();
                    return token;
                }
            });
            
        }
        #endregion
        #region Update
       
        #endregion
        #region Delete
        public async Task DeleteUsersTokensAsync(Token trustedToken)
        {
            await Task.Run(() => {
                _context.Tokens.RemoveRange(_context.Tokens.Where(t => t.UserId == trustedToken.UserId));
            });
        }
        #endregion
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}