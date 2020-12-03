using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Syncfusion.UI.Xaml.Reports;
using TrackAndTrace.ViewModels;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ZXing;
using ZXing.Datamatrix;
using ZXing.Mobile;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TrackAndTrace.Views.Planer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PrintPage : Page
    {
        public GlobalViewModel AppViewModel => App.ViewModel;
        public IzvozPodatakaViewModel ViewModel { get; set; }
        private BarcodeWriter writer = new BarcodeWriter();

        public PrintPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            writer.Format = BarcodeFormat.DATA_MATRIX;
            writer.Options = new DatamatrixEncodingOptions
            {
                SymbolShape     = ZXing.Datamatrix.Encoder.SymbolShapeHint.FORCE_SQUARE,
                Height = 96,    // 41px cca 1,1 cm
                Width = 96,
                MaxSize = new Dimension(64, 64),
                MinSize = new Dimension(32, 32),
                GS1Format = true
            };

            Assembly assembly = typeof(MainPage).GetTypeInfo().Assembly;
            
            Stream reportStream = assembly.GetManifestResourceStream("TrackAndTrace.Report5.rdlc");
            sfReport.ProcessingMode = Syncfusion.UI.Xaml.Reports.ProcessingMode.Local;
            sfReport.ExportMode = Syncfusion.UI.Xaml.Reports.ExportMode.Local;
            //sfReport.PaperOrientation = PaperOrientation.Landscape;
            sfReport.LoadReport(reportStream);

            sfReport.DataSources.Clear();

            ViewModel = new IzvozPodatakaViewModel(App.ViewModel.OdabraniPlan);

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AppViewModel.UcitavaSe = true;
            });

            await ViewModel.UcitajPlanPakovanja();

            Task task = null;

            if (e.Parameter != null)
            {
                var param = (SerijskiBrojeviViewModel.TipSerijskogBroja)e.Parameter;

                if (param == SerijskiBrojeviViewModel.TipSerijskogBroja.Item)
                {
                    foreach (var j in ViewModel.Jedinacne)
                    {
                        j.Code = await GetDMCode(j.SerijskiBroj.Replace("(", "").Replace(")", ""));
                        j.JeStampana = 1;
                        j.DatumPrintanja = DateTime.Now;
                    }

                    task = Task.Run(() => App.SbRepository.UpdateJedinacneAsync(ViewModel.Jedinacne));
                    
                }

                if (param == SerijskiBrojeviViewModel.TipSerijskogBroja.InnerPackaging)
                {
                    foreach (var j in ViewModel.Omotne)
                    {
                        j.Code = await GetDMCode(j.SerijskiBroj.Replace("(", "").Replace(")", ""));
                        j.JeStampana = 1;
                        j.DatumPrintanja = DateTime.Now;
                    }

                    task = Task.Run(() => App.SbRepository.UpdateOmotneAsync(ViewModel.Omotne));
                }

                if (param == SerijskiBrojeviViewModel.TipSerijskogBroja.OuterPackaging)
                {
                    foreach (var j in ViewModel.Paketne)
                    {
                        j.Code = await GetDMCode(j.SerijskiBroj.Replace("(", "").Replace(")", ""));
                        j.JeStampana = 1;
                        j.DatumPrintanja = DateTime.Now;
                    }

                    task = Task.Run(() => App.SbRepository.UpdatePaketneAsync(ViewModel.Paketne));
                }

                if (param == SerijskiBrojeviViewModel.TipSerijskogBroja.Pallet)
                {
                    foreach (var j in ViewModel.Paletne)
                    {
                        j.Code = await GetDMCode(j.SerijskiBroj.Replace("(", "").Replace(")", ""));
                        j.JeStampana = 1;
                        j.DatumPrintanja = DateTime.Now;
                    }

                    task = Task.Run(() => App.SbRepository.UpdatePaletneAsync(ViewModel.Paletne));
                }



                await task.ContinueWith(async t1 =>
                {
                    if (t1.Status == TaskStatus.RanToCompletion)
                    {
                        await sfReport.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            if (param == SerijskiBrojeviViewModel.TipSerijskogBroja.Item)
                            {
                                var dataSource = from j in ViewModel.Jedinacne
                                                 join pr in App.ViewModel.ListaProizvoda on j.ProizvodId equals pr.Id
                                                 join pl in App.ViewModel.ListaPlanova on j.PlanPakovanjaId equals pl.Id
                                                 where j.PlanPakovanjaId == App.ViewModel.OdabraniPlan.Id
                                                 select new
                                                 {
                                                     Code = j.Code,
                                                     SerijskiBroj = j.SerijskiBroj,
                                                     NazivProizvoda = pr.Naziv,
                                                     JedinacnihUOmot = pl.JedinacnihUOmot,
                                                     OmotnihUPaket = pl.OmotnihUPaket,
                                                     PaketaNaPaletu = pl.PaketaNaPaletu,
                                                     UkupnoPaleta = pl.UkupnoPaleta
                                                 };

                                sfReport.DataSources.Add(new ReportDataSource("DataSet1", dataSource));
                            }
                            if (param == SerijskiBrojeviViewModel.TipSerijskogBroja.InnerPackaging)
                            {
                                var dataSource = from j in ViewModel.Omotne
                                                 join pr in App.ViewModel.ListaProizvoda on j.ProizvodId equals pr.Id
                                                 join pl in App.ViewModel.ListaPlanova on j.PlanPakovanjaId equals pl.Id
                                                 where j.PlanPakovanjaId == App.ViewModel.OdabraniPlan.Id
                                                 select new
                                                 {
                                                     Code = j.Code,
                                                     SerijskiBroj = j.SerijskiBroj,
                                                     NazivProizvoda = pr.Naziv,
                                                     JedinacnihUOmot = pl.JedinacnihUOmot,
                                                     OmotnihUPaket = pl.OmotnihUPaket,
                                                     PaketaNaPaletu = pl.PaketaNaPaletu,
                                                     UkupnoPaleta = pl.UkupnoPaleta
                                                 };

                                sfReport.DataSources.Add(new ReportDataSource("DataSet1", dataSource));
                            }

                            if (param == SerijskiBrojeviViewModel.TipSerijskogBroja.OuterPackaging)
                            {
                                var dataSource = from j in ViewModel.Paketne
                                                 join pr in App.ViewModel.ListaProizvoda on j.ProizvodId equals pr.Id
                                                 join pl in App.ViewModel.ListaPlanova on j.PlanPakovanjaId equals pl.Id
                                                 where j.PlanPakovanjaId == App.ViewModel.OdabraniPlan.Id
                                                 select new
                                                 {
                                                     Code = j.Code,
                                                     SerijskiBroj = j.SerijskiBroj,
                                                     NazivProizvoda = pr.Naziv,
                                                     JedinacnihUOmot = pl.JedinacnihUOmot,
                                                     OmotnihUPaket = pl.OmotnihUPaket,
                                                     PaketaNaPaletu = pl.PaketaNaPaletu,
                                                     UkupnoPaleta = pl.UkupnoPaleta
                                                 };

                                sfReport.DataSources.Add(new ReportDataSource("DataSet1", dataSource));
                            }

                            if (param == SerijskiBrojeviViewModel.TipSerijskogBroja.Pallet)
                            {
                                var dataSource = from j in ViewModel.Paletne
                                                 join pr in App.ViewModel.ListaProizvoda on j.ProizvodId equals pr.Id
                                                 join pl in App.ViewModel.ListaPlanova on j.PlanPakovanjaId equals pl.Id
                                                 where j.PlanPakovanjaId == App.ViewModel.OdabraniPlan.Id
                                                 select new
                                                 {
                                                     Code = j.Code,
                                                     SerijskiBroj = j.SerijskiBroj,
                                                     NazivProizvoda = pr.Naziv,
                                                     JedinacnihUOmot = pl.JedinacnihUOmot,
                                                     OmotnihUPaket = pl.OmotnihUPaket,
                                                     PaketaNaPaletu = pl.PaketaNaPaletu,
                                                     UkupnoPaleta = pl.UkupnoPaleta
                                                 };

                                sfReport.DataSources.Add(new ReportDataSource("DataSet1", dataSource));
                            }

                            //var listaPlan = App.ViewModel.ListaPlanova.Where(p => p.Id == App.ViewModel.OdabraniPlan.Id);
                            //sfReport.DataSources.Add(new ReportDataSource("DataSet2", listaPlan));

                            sfReport.RefreshReport();
                        });
                    }
                });

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    AppViewModel.UcitavaSe = false;
                });
            }

        }
        
        private async Task<byte[]> GetDMCode(string serBroj)
        {
            WriteableBitmap bitmap = writer.Write(serBroj);
            byte[] bytes;

            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                BitmapEncoder bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms);
                bitmapEncoder.SetSoftwareBitmap(SoftwareBitmap.CreateCopyFromBuffer(
                    bitmap.PixelBuffer, BitmapPixelFormat.Bgra8, bitmap.PixelWidth, bitmap.PixelHeight));
                await bitmapEncoder.FlushAsync();
                bitmap = null;
                bytes = new byte[ms.Size];
                await ms.AsStream().ReadAsync(bytes, 0, bytes.Length);
            }
            return bytes;
        }
    }
}
