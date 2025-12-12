using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Repositories.Abstractions
{
    public interface IDataRepository
    {
        IQueryable<T> GetAll<T>() where T : class;
        Task<T?> GetById<T>(int id) where T : class;
        Task<T> Add<T>(T entity) where T : class;
        Task<T> Update<T>(T entity) where T : class;
        Task<T> Delete<T>(T entity) where T : class;
    }
}
