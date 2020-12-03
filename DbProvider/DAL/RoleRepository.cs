using DbProvider.Model;
using DbProvider.Pomocna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbProvider.DAL
{
    public class RoleRepository
    {
        protected Repository<Rola> repository;

        private static RoleRepository instance = null;

        public static RoleRepository Instance
        {
            get
            {
                if (instance == null)
                    return new RoleRepository();
                return instance;
            }
        }

        public RoleRepository()
        {
            repository = new Repository<Rola>();
        }

        public async Task<Rola> Add(Rola entity)
        {
            var value = await repository.AddAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<Rola> GetById(int Id)
        {
            var value = await repository.GetByIdAsync(Id);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<IEnumerable<Rola>> GetList()
        {
            var value = await repository.GetListAsync();

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<IEnumerable<Rola>> GetList(Func<Rola, bool> expression)
        {
            var value = await repository.GetListAsync(expression);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<bool> Remove(Rola entity)
        {
            var value = await repository.RemoveAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> Update(Rola entity)
        {
            var value = await repository.UpdateAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }
    }
}
