using DbProvider.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrackAndTrace.ViewModels
{
    public class IdentifikatorViewModel : INotifyPropertyChanged
    {
        public IdentifikatorViewModel(Identifikator identifikator = null)
        {
            Model = identifikator ?? new Identifikator();
        }

        private Identifikator identifikator;
        public Identifikator Model
        {
            get { return identifikator; }
            set
            {
                if (identifikator != value)
                {
                    identifikator = value;
                    Set();
                }
            }
        }

        #region Atributi

        public int Id { get { return Model.Id; } }

        public int KupacId
        {
            get => Model.KupacId;
            set
            {
                if (Model.KupacId != value)
                {
                    Model.KupacId = value;
                    Set();
                }
            }
        }

        public string Naziv
        {
            get { return Model.Naziv; }
            set
            {
                if (Model.Naziv != value)
                {
                    Model.Naziv = value;
                    Set();
                }
            }
        }                

        #endregion
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void Set([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
