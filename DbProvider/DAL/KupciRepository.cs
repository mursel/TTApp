using DbProvider.Model;
using DbProvider.Pomocna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbProvider.DAL
{
    public class KupciRepository
    {
        protected Repository<Kupac> repository;

        private static KupciRepository instance = null;

        public static KupciRepository Instance
        {
            get
            {
                if (instance == null)
                    return new KupciRepository();
                return instance;
            }
        }

        public KupciRepository()
        {
            repository = new Repository<Kupac>();
        }

        public async Task<Kupac> Add(Kupac entity)
        {
            var value = await repository.AddAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<Kupac> GetById(int Id)
        {
            var value = await repository.GetByIdAsync(Id);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<IEnumerable<Kupac>> GetList()
        {
            var value = await repository.GetListAsync();

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<IEnumerable<Kupac>> GetList(Func<Kupac, bool> expression)
        {
            var value = await repository.GetListAsync(expression);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<bool> Remove(Kupac entity)
        {
            var value = await repository.RemoveAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> Update(Kupac entity)
        {
            var value = await repository.UpdateAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }
    }
}
