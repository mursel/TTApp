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
    public class PaletnaViewModel : INotifyPropertyChanged
    {

        public PaletnaViewModel(Paletna paletna = null, bool jeAdHoc = false)
        {
            Model = paletna ?? new Paletna();
            JeAdHoc = !jeAdHoc ? 0 : 1;
        }

        private Paletna model;
        public Paletna Model
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

        public int JeAdHoc
        {
            get { return Model.JeAdHoc; }
            set
            {
                if (Model.JeAdHoc != value)
                {
                    Model.JeAdHoc = value;
                    Set();
                    Set(nameof(JeAdHoc));
                }
            }
        }

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
