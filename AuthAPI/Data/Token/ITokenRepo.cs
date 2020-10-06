using System.Collections.Generic;
using System.Threading.Tasks;
using AuthAPI.Models;

namespace AuthAPI.Data {
    public interface ITokenRepo {
        Task<bool> SaveChangesAsync();
        Task<Token> GetTokenByIdAsync(int id);
        Task<Token> GetTokenByUserIdAsync(int userId);
        Task<Token> CreateTokenAsync(User trustedUser, Token trustedToken);
        Task DeleteUsersTokensAsync(Token trustedToken);
    }
}