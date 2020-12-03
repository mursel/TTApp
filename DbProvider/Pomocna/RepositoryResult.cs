using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbProvider.Pomocna
{
    /// <summary>
    /// Pohranjuje rezultat izvršenja database operacija
    /// </summary>
    /// <typeparam name="T">T entitet</typeparam>
    public class RepositoryResult<T> where T : class
    {
        public HResultStatus Result { get; set; } = new HResultStatus(null, HResultStatus.ResultStatusCodes.SUCCESS);
        public T Entity { get; set; } = null;
        public IEnumerable<T> EntityCollection { get; set; }
        public IEnumerable<T> EntityCollection2 { get; set; }
    }
}
