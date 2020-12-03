using DbProvider.Model;
using DbProvider.Pomocna;
using FastMember;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DbProvider.DAL
{
    public class SbRepository
    {
        protected Repository<Jedinacna> jedRepository;
        protected Repository<Omotna> omoRepository;
        protected Repository<Paketna> pakRepository;
        protected Repository<Paletna> palRepository;

        private static SbRepository instance = null;
        public static SbRepository Instance
        {
            get
            {
                if (instance == null)
                    return new SbRepository();
                return instance;
            }
        }

        public SbRepository()
        {
            jedRepository = new Repository<Jedinacna>();
            omoRepository = new Repository<Omotna>();
            pakRepository = new Repository<Paketna>();
            palRepository = new Repository<Paletna>();
        }

        #region metode
        //public async Task<Drzava> Add(Drzava entity)
        //{
        //    var value = await repository.AddAsync(entity);

        //    if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
        //        return value.Entity;

        //    return null;
        //}

        //public async Task<Drzava> GetById(int Id)
        //{
        //    var value = await repository.GetByIdAsync(Id);

        //    if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
        //        return value.Entity;

        //    return null;
        //}

        //public async Task<IEnumerable<Drzava>> GetList()
        //{
        //    var value = await repository.GetListAsync();

        //    if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
        //        return value.EntityCollection;

        //    return null;
        //}

        //public async Task<IEnumerable<Drzava>> GetList(Func<Drzava, bool> expression)
        //{
        //    var value = await repository.GetListAsync(expression);

        //    if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
        //        return value.EntityCollection;

        //    return null;
        //}

        //public async Task<bool> Remove(Drzava entity)
        //{
        //    var value = await repository.RemoveAsync(entity);

        //    if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
        //        return true;

        //    return false;
        //}

        //public async Task<bool> Update(Drzava entity)
        //{
        //    var value = await repository.UpdateAsync(entity);

        //    if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
        //        return true;

        //    return false;
        //}
        #endregion  

        public async Task<bool> AddBulkJedinacneAsync(IEnumerable<Jedinacna> jedinacne)
        {
            var dataTable = new DataTable("dbo.Jedinacne");
            using (var reader = ObjectReader.Create(jedinacne, "Id", "KupacId", "ProizvodId", "UposlenikId", 
                "SerijskiBroj", "SerijskiBrojInt", "UkupnaKolicina", "Code", "DatumPrintanja", "DatumSkeniranja", "OmotnaId", "PlanPakovanjaId", "JeStampana", "JeSkenirana", "JeRezervisana"))
            {
                dataTable.Load(reader);
            }

            var result = await jedRepository.AddBulkAsync(dataTable);
            
            if (result.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }
        
        public async Task<bool> UpdateJedinacnaAsync(Jedinacna jedinacna)
        {
            var value = await jedRepository.UpdateAsync(jedinacna);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> UpdateOmotnaAsync(Omotna omotna)
        {
            var value = await omoRepository.UpdateAsync(omotna);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> AddBulkOmotneAsync(IEnumerable<Omotna> omotne)
        {
            var dataTable = new DataTable("dbo.Omotne");
            using (var reader = ObjectReader.Create(omotne, "Id", "KupacId", "ProizvodId",  "UposlenikId",
                "SerijskiBroj", "SerijskiBrojInt", "UkupnaKolicina", "Code", "DatumPrintanja", "DatumSkeniranja", "PaketnaId", "PlanPakovanjaId", "JeStampana", "JeSkenirana", "JeRezervisana"))
            {
                dataTable.Load(reader);
            }

            var result = await omoRepository.AddBulkAsync(dataTable);

            if (result.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<Paletna> AddNewPallet(Paletna paletna)
        {
            var value = await palRepository.AddAsync(paletna);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;

            return null;
        }

        public async Task<bool> AddBulkPaketneAsync(IEnumerable<Paketna> paketne)
        {
            var dataTable = new DataTable("dbo.Paketne");
            using (var reader = ObjectReader.Create(paketne, "PaketnaId", "KupacId", "ProizvodId", "UposlenikId",
                "SerijskiBroj", "SerijskiBrojInt", "UkupnaKolicina", "Code", "DatumPrintanja", "DatumSkeniranja", "PaletnaId", "PlanPakovanjaId", "JeStampana", "JeSkenirana"))
            {
                dataTable.Load(reader);
            }

            var result = await pakRepository.AddBulkAsync(dataTable);

            if (result.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> RemovePaketnaFromPaletnaAsync(string tempPaketna)
        {
            var value = await pakRepository.GetListAsync(p => p.SerijskiBroj == tempPaketna);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
            {
                var paketna = value.EntityCollection.First();
                paketna.PaletnaId = 0;
                await pakRepository.UpdateAsync(paketna);
                return true;
            }

            return false;
        }

        public async Task<bool> RemovePaketnaByName(string paketnaSerijskiBroj)
        {
            var value = await pakRepository.GetListAsync(p => p.SerijskiBroj == paketnaSerijskiBroj);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
            {
                var paketna = value.EntityCollection.First();
                var rezultat = pakRepository.RemoveAsync(paketna);
                return true;
            }

            return false;
        }

        public async Task<Omotna> GetLastInnerPackageByPlanIdAsync(int planId)
        {
            var value = await omoRepository.GetListAsync(o => o.PlanPakovanjaId == planId);
            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS && value.EntityCollection.Count() > 0)
                return value.EntityCollection.Last();
            return null;
        }

        public async Task<Paketna> GetLastOuterPackageByPlanIdAsync(int planId)
        {
            var value = await pakRepository.GetListAsync(o => o.PlanPakovanjaId == planId);
            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS && value.EntityCollection.Count() > 0)
                return value.EntityCollection.Last();
            return null;
        }

        public async Task<Paletna> GetLastPalletByPlanIdAsync(int planId)
        {
            var value = await palRepository.GetListAsync(o => o.PlanPakovanjaId == planId);
            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS && value.EntityCollection.Count() > 0)
                return value.EntityCollection.Last();
            return null;
        }

        public async Task<Omotna> AddNewInnerPackageAsync(Omotna omot)
        {
            var value = await omoRepository.AddAsync(omot);
            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;
            return null;
        }

        public async Task<Paketna> AddNewOuterPackageAsync(Paketna paketna)
        {
            var value = await pakRepository.AddAsync(paketna);
            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.Entity;
            return null;
        }

        public async Task<Paletna> GetPaletnaByNameAsync(string izabranaPaleta)
        {
            var value = await palRepository.GetListAsync(p => p.SerijskiBroj == izabranaPaleta);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection.First();

            return null;
        }

        public async Task<Paketna> GetPaketnaByNameAsync(string izabraniPaket)
        {
            var value = await pakRepository.GetListAsync(p => p.SerijskiBroj == izabraniPaket);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection.First();

            return null;
        }

        public async Task<IEnumerable<Paletna>> GetPaletneAndPaketneAsync(int planPakovanjaId = -1)
        {

            try
            {
                using (var connection = DatabaseManager.GetConnection())
                {
                    await connection.OpenAsync();

                    var lookup = new Dictionary<int, Paletna>();

                    var rezultat = await connection.QueryAsync<Paletna, Paketna, Paletna>(
                        "select A.*, B.* from Paletne A inner join Paketne B on A.Id = B.PaletnaId and A.PlanPakovanjaId = @pid",
                        (pal, pak) =>
                        {
                            Paletna paletna;

                            if (!lookup.TryGetValue(pal.Id, out paletna))
                            {
                                paletna = pal;
                                paletna.Paketne = new List<Paketna>();
                                lookup.Add(paletna.Id, paletna);
                            }

                            paletna.Paketne.Add(pak);
                            return paletna;

                        },
                        new { pid = planPakovanjaId },
                        splitOn: "PaketnaId"
                        );

                    if (rezultat.Count() > 0)
                        return rezultat;
                }
            }
            catch (Exception e)
            {
                var err = e.Message;
            }

            return null;
        }


        public async Task<bool> AddBulkPaletneAsync(IEnumerable<Paletna> paletne)
        {
            var dataTable = new DataTable("dbo.Paletne");
            using (var reader = ObjectReader.Create(paletne, "Id", "KupacId", "ProizvodId", "UposlenikId",
                "SerijskiBroj", "SerijskiBrojInt", "UkupnaKolicina", "Code", "DatumPrintanja", "DatumSkeniranja", "PlanPakovanjaId", "JeStampana", "JeSkenirana", "JeAdHoc"))
            {
                dataTable.Load(reader);
            }

            var result = await palRepository.AddBulkAsync(dataTable);

            if (result.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<IEnumerable<Jedinacna>> GetJedinacneAsync(int planPakovanjaId = -1)
        {
            RepositoryResult<Jedinacna> value;

            if (planPakovanjaId == -1)
                value = await jedRepository.GetListAsync();
            else
                value = await jedRepository.GetListAsync(j => j.PlanPakovanjaId == planPakovanjaId);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        /// <summary>
        /// Vraca listu jedinacnih koje nisu skenirane, stampane i rezervisane
        /// </summary>
        /// <param name="planPakovanjaId"></param>
        /// <param name="jeSkenirana"></param>
        /// <param name="jeStampana"></param>
        /// <param name="odabranaKolicina"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Jedinacna>> GetJedinacneAsync(bool jeSkenirana, bool jeStampana, int odabranaKolicina = 0, int planPakovanjaId = -1)
        {
            RepositoryResult<Jedinacna> value;

            int jsk = !jeSkenirana ? 0 : 1;
            int jst = !jeStampana ? 0 : 1;

            if (planPakovanjaId == -1)
                value = await jedRepository.GetListAsync();
            else
                value = await jedRepository.GetListAsync(j => j.PlanPakovanjaId == planPakovanjaId && j.JeSkenirana == jsk && j.JeStampana == jst && j.JeRezervisana == 0);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection.Take(odabranaKolicina);

            return null;
        }

        public async Task<IEnumerable<Jedinacna>> GetJedinacneAsync(int odabranaKolicina = 0, int planPakovanjaId = -1)
        {
            RepositoryResult<Jedinacna> value;
            
            if (planPakovanjaId == -1)
                value = await jedRepository.GetListAsync();
            else
                value = await jedRepository.GetListAsync(j => j.PlanPakovanjaId == planPakovanjaId);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection.Take(odabranaKolicina);

            return null;
        }

        public async Task<IEnumerable<Omotna>> GetOmotneAsync(int planPakovanjaId = -1)
        {
            RepositoryResult<Omotna> value;

            if (planPakovanjaId == -1)
                value = await omoRepository.GetListAsync();
            else
                value = await omoRepository.GetListAsync(j => j.PlanPakovanjaId == planPakovanjaId);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<IEnumerable<Omotna>> GetOmotneAsync(bool jeSkenirana, bool jeStampana, int odabranaKolicina = 0, int planPakovanjaId = -1)
        {
            RepositoryResult<Omotna> value;

            int jsk = !jeSkenirana ? 0 : 1;
            int jst = !jeStampana ? 0 : 1;

            if (planPakovanjaId == -1)
                value = await omoRepository.GetListAsync();
            else
                value = await omoRepository.GetListAsync(j => j.PlanPakovanjaId == planPakovanjaId && j.JeSkenirana == jsk && j.JeStampana == jst);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection.Take(odabranaKolicina);

            return null;
        }

        public async Task<IEnumerable<Omotna>> GetOmotneAsync(int odabranaKolicina = 0, int planPakovanjaId = -1)
        {
            RepositoryResult<Omotna> value;
            
            if (planPakovanjaId == -1)
                value = await omoRepository.GetListAsync();
            else
                value = await omoRepository.GetListAsync(j => j.PlanPakovanjaId == planPakovanjaId);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection.Take(odabranaKolicina);

            return null;
        }

        public async Task<IEnumerable<Paketna>> GetPaketneAsync(bool jeSkenirana, bool jeStampana, int odabranaKolicina = 0, int planPakovanjaId = -1)
        {
            RepositoryResult<Paketna> value;

            int jsk = !jeSkenirana ? 0 : 1;
            int jst = !jeStampana ? 0 : 1;

            if (planPakovanjaId == -1)
                value = await pakRepository.GetListAsync();
            else
                value = await pakRepository.GetListAsync(j => j.PlanPakovanjaId == planPakovanjaId && j.JeSkenirana == jsk && j.JeStampana == jst);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection.Take(odabranaKolicina);

            return null;
        }

        public async Task<IEnumerable<Paketna>> GetPaketneAsync(int odabranaKolicina = 0, int planPakovanjaId = -1)
        {
            RepositoryResult<Paketna> value;
            
            if (planPakovanjaId == -1)
                value = await pakRepository.GetListAsync();
            else
                value = await pakRepository.GetListAsync(j => j.PlanPakovanjaId == planPakovanjaId);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection.Take(odabranaKolicina);

            return null;
        }

        public async Task<IEnumerable<Paketna>> GetPaketneAsync(int planPakovanjaId = -1)
        {
            RepositoryResult<Paketna> value;

            if (planPakovanjaId == -1)
                value = await pakRepository.GetListAsync();
            else
                value = await pakRepository.GetListAsync(j => j.PlanPakovanjaId == planPakovanjaId);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return value.EntityCollection;

            return null;
        }

        public async Task<IEnumerable<Paletna>> GetPaletneAsync(int planPakovanjaId = -1, bool jeAdHoc = false)
        {
            RepositoryResult<Paletna> value;

            // vrati sve ako pp = -1
            if (planPakovanjaId == -1)
                value = await palRepository.GetListAsync();
            else
                value = await palRepository.GetListAsync(j => j.PlanPakovanjaId == planPakovanjaId);

            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
            {
                // standardne palete??
                if (!jeAdHoc)
                    return value.EntityCollection;
                else
                    return value.EntityCollection.Where(p => p.JeAdHoc == 1);
            }

            return null;
        }
        
        public async Task<bool> UpdateJedinacneAsync(IEnumerable<Jedinacna> jedinacne)
        {
             var value = await jedRepository.UpdateAllAsync(jedinacne);
            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> UpdateOmotneAsync(IEnumerable<Omotna> omotne)
        {
            var value = await omoRepository.UpdateAllAsync(omotne);
            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> UpdatePaketneAsync(IEnumerable<Paketna> paketne)
        {
            var value = await pakRepository.UpdateAllAsync(paketne);
            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> UpdatePaletneAsync(IEnumerable<Paletna> paletne)
        {
            var value = await palRepository.UpdateAllAsync(paletne);
            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

        public async Task<bool> UpdatePaketnaAsync(Paketna paketna)
        {
            var value = await pakRepository.UpdateAsync(paketna);
            if (value.Result.ResultCode == HResultStatus.ResultStatusCodes.SUCCESS)
                return true;

            return false;
        }

    }
}
