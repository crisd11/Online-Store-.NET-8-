using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {

        public ProductRepository(OnlineStoreDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IQueryable<Product>> ActiveProducts()
        {
            return _entities.AsNoTracking().Where(p => p.IsActive);
        }
    }
}
