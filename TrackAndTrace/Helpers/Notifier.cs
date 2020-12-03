using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrackAndTrace.Helpers
{
    public class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void Set<T>(ref T member, T vrijednost, [CallerMemberName] string propName = null)
        {
            if (object.Equals(member, vrijednost))
                return;

            member = vrijednost;

        }

        protected virtual void OnPropertyChanged(string propChanged) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propChanged));
    }
}
