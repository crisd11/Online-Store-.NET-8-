using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OnlineStoreDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        public UnitOfWork(OnlineStoreDbContext context)
        {
            _context = context;
        }

        public IUserRepository UserRepository => _userRepository ?? new UserRepository(_context);
        public IProductRepository ProductRepository => _productRepository ?? new ProductRepository(_context);
        public ICartRepository CartRepository => _cartRepository ?? new CartRepository(_context);

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
