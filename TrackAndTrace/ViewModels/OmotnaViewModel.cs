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
    public class OmotnaViewModel : INotifyPropertyChanged
    {

        public OmotnaViewModel(Omotna omotna = null) => Model = omotna ?? new Omotna();

        private Omotna model;
        public Omotna Model
        {
            get { return model; }
            set {
                if (model != value)
                {
                    model = value;
                    Set(string.Empty);
                }
            }
        }

        #region Atributi

        public string SerijskiBroj { get { return Model.SerijskiBroj; }
            set
            {
                if (Model.SerijskiBroj != value)
                {
                    Model.SerijskiBroj = value;
                    Set();
                    Set(nameof(SerijskiBroj));
                }
            }
        }
                
        public DateTime DatumPrintanja { get { return Model.DatumPrintanja; }
            set
            {
                if (Model.DatumPrintanja != value)
                {
                    Model.DatumPrintanja = value;
                    Set();
                    Set(nameof(DatumPrintanja));
                }
            }
        }

        public DateTime DatumSkeniranja
        {
            get { return Model.DatumSkeniranja; }
            set
            {
                if (Model.DatumSkeniranja != value)
                {
                    Model.DatumSkeniranja = value;
                    Set();
                    Set(nameof(DatumSkeniranja));
                }
            }
        }

        #endregion
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void Set([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
