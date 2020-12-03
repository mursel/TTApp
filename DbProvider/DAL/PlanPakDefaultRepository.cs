using DbProvider.Model;
using DbProvider.Pomocna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbProvider.DAL
{
    public class PlanPakDefaultRepository
    {
        protected Repository<PlanPakProizvod> repository;

        private static PlanPakDefaultRepository instance = null;

        public static PlanPakDefaultRepository Instance
        {
            get
            {
                if (instance == null)
                    return new PlanPakDefaultRepository();
                return instance;
            }
        }

        public PlanPakDefaultRepository()
        {
            repository = new Repository<PlanPakProizvod>();
        }

        public async Task<PlanPakProizvod> Add(PlanPakProizvod entity)
        {
            var value = await repository.AddAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<PlanPakProizvod> GetById(int Id)
        {
            var value = await repository.GetByIdAsync(Id);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<string> GetProductByPlanId(int planPakId)
        {
            var value = await repository.GetByIdAsync(planPakId);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
            {
                var proizvodId = await ProizvodiRepository.Instance.GetById(value.Entity.ProizvodId);
                if (proizvodId != null)
                    return proizvodId.Naziv;
            }
            return null;
        }

        public async Task<PlanPakProizvod> GetByProductId(int proizvodId)
        {
            var value = await repository.GetListAsync(p => p.ProizvodId == proizvodId);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection.First();

            return null;
        }

        public async Task<IEnumerable<PlanPakProizvod>> GetList()
        {
            var value = await repository.GetListAsync();

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<IEnumerable<PlanPakProizvod>> GetList(Func<PlanPakProizvod, bool> expression)
        {
            var value = await repository.GetListAsync(expression);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<bool> Remove(PlanPakProizvod entity)
        {
            var value = await repository.RemoveAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> Update(PlanPakProizvod entity)
        {
            var value = await repository.UpdateAsync(entity);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }
    }
}
