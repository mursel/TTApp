using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackAndTrace.Helpers
{
    public static class Ekstenzije
    {
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> obj)
        {
            return new ObservableCollection<T>(obj);
        }
    }
}
