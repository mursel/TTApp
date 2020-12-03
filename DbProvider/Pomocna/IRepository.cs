using DbProvider.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DbProvider.Pomocna
{
    public interface IRepository<T> where T : class
    {
        Task<RepositoryResult<T>> GetByIdAsync(int Id);
        Task<RepositoryResult<T>> AddAsync(T entity);
        Task<RepositoryResult<T>> AddBulkAsync(DataTable dataTable);
        Task<RepositoryResult<T>> RemoveAsync(T entity);
        Task<RepositoryResult<T>> UpdateAsync(T entity);
        Task<RepositoryResult<T>> GetListAsync();
        Task<RepositoryResult<T>> GetListAsync(Func<T, bool> expression);        
    }
}
