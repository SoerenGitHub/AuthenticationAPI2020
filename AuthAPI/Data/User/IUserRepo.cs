using System.Collections.Generic;
using System.Threading.Tasks;
using AuthAPI.Models;

namespace AuthAPI.Data {
    public interface IUserRepo {
        Task<bool> SaveChangesAsync();

        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserWithEmailAndPasswordAsync(string email, string password);

        Task CreateUserAsync(User user);

        void UpdateUser(User user);

        Task DeleteUserAsync(User user);
    }
}