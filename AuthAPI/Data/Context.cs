using AuthAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Data {
    public class Context : DbContext {
        public Context(DbContextOptions<Context> opt) : base(opt) {

        }

        public DbSet<Token> Tokens { get; set; }
        public DbSet<User> Users { get; set; }

    }
}