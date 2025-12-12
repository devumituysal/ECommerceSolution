using App.Data.Contexts;
using App.Data.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Repositories.Implementations
{
    internal class DataRepository : IDataRepository
    {
        private readonly AppDbContext _dbContext;

        public DataRepository(AppDbContext dbContext)
        {
            _dbContext=dbContext;
        }

        public async Task<T> Add<T>(T entity) where T : class
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Delete<T>(T entity) where T : class
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public  IQueryable<T> GetAll<T>() where T : class
        {
            return _dbContext.Set<T>();
        }

        public async Task<T?> GetById<T>(int id) where T : class
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> Update<T>(T entity) where T : class
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();    
            return entity;
        }
    }
}
