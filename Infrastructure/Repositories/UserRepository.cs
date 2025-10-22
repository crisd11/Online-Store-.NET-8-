using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(OnlineStoreDbContext dbContext) : base(dbContext)
        {
            
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _entities.Where(u => u.Email == email).FirstOrDefaultAsync();
        }
    }
}
