using DbProvider.Model;
using DbProvider.Pomocna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbProvider.DAL
{
    public class DrzaveRepository
    {
        protected Repository<Drzava> repository;

        private static DrzaveRepository instance = null;

        public static DrzaveRepository Instance
        {
            get
            {
                if (instance == null)
                    return new DrzaveRepository();
                return instance;
            }
        }

        public DrzaveRepository()
        {
            repository = new Repository<Drzava>();
        }

        public async Task<Drzava> Add(Drzava entity)
        {
            var value = await repository.AddAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<Drzava> GetById(int Id)
        {
            var value = await repository.GetByIdAsync(Id);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<IEnumerable<Drzava>> GetList()
        {
            var value = await repository.GetListAsync();

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<IEnumerable<Drzava>> GetList(Func<Drzava, bool> expression)
        {
            var value = await repository.GetListAsync(expression);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<bool> Remove(Drzava entity)
        {
            var value = await repository.RemoveAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> Update(Drzava entity)
        {
            var value = await repository.UpdateAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }
    }
}
