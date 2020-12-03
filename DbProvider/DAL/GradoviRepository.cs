using DbProvider.Model;
using DbProvider.Pomocna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbProvider.DAL
{
    public class GradoviRepository
    {
        protected Repository<Grad> repository;

        private static GradoviRepository instance = null;

        public static GradoviRepository Instance
        {
            get
            {
                if (instance == null)
                    return new GradoviRepository();
                return instance;
            }
        }

        public GradoviRepository()
        {
            repository = new Repository<Grad>();
        }

        public async Task<Grad> Add(Grad entity)
        {
            var value = await repository.AddAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<Grad> GetById(int Id)
        {
            var value = await repository.GetByIdAsync(Id);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<IEnumerable<Grad>> GetList()
        {
            var value = await repository.GetListAsync();

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<IEnumerable<Grad>> GetList(Func<Grad, bool> expression)
        {
            var value = await repository.GetListAsync(expression);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<bool> Remove(Grad entity)
        {
            var value = await repository.RemoveAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> Update(Grad entity)
        {
            var value = await repository.UpdateAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }
    }
}
