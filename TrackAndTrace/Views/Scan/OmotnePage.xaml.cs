using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Syncfusion.ReportWriter;
using Syncfusion.UI.Xaml.Reports;
using TrackAndTrace.ViewModels;
using Windows.Data.Pdf;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TrackAndTrace.Views.Scan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OmotnePage : Page
    {
        public SerijskiBrojeviViewModel ViewModel { get; set; }
        public GlobalViewModel AppViewModel => App.ViewModel;
        public IzvozPodatakaViewModel IzvozViewModel { get; set; }

        //private PrintDocument printDocument = null;
        //private IPrintDocumentSource source = null;
        //private List<UIElement> uIElements = new List<UIElement>();
        //private Page page = null;

        public OmotnePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var aktivniPlanovi = App.ViewModel.ListaPlanova.Where(p => p.StatusPlana == 1);
            cbPlanovi.ItemsSource = aktivniPlanovi.ToList();

            //printDocument = new PrintDocument();
            //source = printDocument.DocumentSource;
            //printDocument.Paginate += PrintDocument_Paginate;
            //printDocument.AddPages += PrintDocument_AddPages;
            //printDocument.GetPreviewPage += PrintDocument_GetPreviewPage;
            //PrintManager printManager = PrintManager.GetForCurrentView();
            //printManager.PrintTaskRequested += PrintManager_PrintTaskRequested;



        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            //printDocument.Paginate -= PrintDocument_Paginate;
            //printDocument.AddPages -= PrintDocument_AddPages;
            //printDocument.GetPreviewPage -= PrintDocument_GetPreviewPage;
            //PrintManager printManager = PrintManager.GetForCurrentView();
            //printManager.PrintTaskRequested -= PrintManager_PrintTaskRequested;
        }

        //private void PrintManager_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        //{
        //    PrintTask printTask = null;

        //    printTask = args.Request.CreatePrintTask("Printanje etikete", srh =>
        //    {
        //        // ovdje se podesava custom paper size
        //        //PrintTaskOptions printTaskOptions = printTask.Options;
        //        //printTaskOptions.MediaSize = PrintMediaSize.PrinterCustom;

        //        printTask.Completed += async (pt, ptc) =>
        //        {
        //            if (ptc.Completion == PrintTaskCompletion.Failed)
        //            {
        //                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        //                {
        //                    // prikazi gresku!!!
        //                });
        //            }

                    

        //        };
                
        //        srh.SetSource(source);
        //    });
        //}

        //private void PrintDocument_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        //{
        //    PrintDocument printDocument = (PrintDocument)sender;
        //    printDocument.SetPreviewPage(e.PageNumber, uIElements[e.PageNumber - 1]);
        //}

        //private void PrintDocument_AddPages(object sender, AddPagesEventArgs e)
        //{
        //    PrintDocument printDocument = (PrintDocument)sender;

        //    for (int i = 0; i < uIElements.Count; i++)
        //    {
        //        printDocument.AddPage(uIElements[i]);
        //    }

        //    printDocument.AddPagesComplete();
        //}

        //private void PrintDocument_Paginate(object sender, PaginateEventArgs e)
        //{
        //    PrintDocument printDocument = (PrintDocument)sender;
        //    printDocument.SetPreviewPageCount(uIElements.Count, PreviewPageCountType.Intermediate);
        //}

        private void cbPlanovi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel = new SerijskiBrojeviViewModel(App.ViewModel.OdabraniPlan);
            ViewModel.UcitajIdentifikatore();

            RezervisiOmotneZaSkeniranje();
                        
        }

        private void RezervisiOmotneZaSkeniranje()
        {
            var task = Task.Run(() => ViewModel.UcitajOmotne(false, true));
            task.ContinueWith(async t1 =>
            {
                if (t1.Status == TaskStatus.RanToCompletion)
                {
                    await txtSerial.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        txtSerial.IsEnabled = true;
                    });
                }
            });
        }

        private async void txtSerial_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var omotna = ViewModel.ListaSerijskih.Where(     
                    j => j.SerijskiBroj.Replace("(", "").Replace(")", "") == txtSerial.Text &&        // ako se serijski broj podudara
                    j.Model.JeRezervisana == 1 &&                   // ako je rezervisana
                    j.Model.JeSkenirana == 0 &&                     // ako nije skenirana
                    j.Model.JeStampana == 1)                        // ako je isprintana
                    .FirstOrDefault();                              // vrati omotnu sa gore navedenim uslovima

                if (omotna != null)
                {
                    omotna.Model.JeRezervisana = 0;
                    omotna.Model.JeSkenirana = 1;
                    lvSerials.Items.Add(omotna);
                    txtSerial.Text = string.Empty;
                    omotna.Model.UposlenikId = App.ViewModel.AutorizovaniKorisnik.Model.Id;

                    await App.SbRepository.UpdateOmotnaAsync(omotna.Model);

                    ViewModel.ListaSerijskih.Remove(omotna);


                }

                if (ViewModel.ListaSerijskih.Count == 0)
                {
                    
                    // kreiraj novu paketnu i vezi preskenirane omotne za paketnu
                    var novaPaketna = await ViewModel.AddNewOuterPackageByItemsList();
                    

                    await PrepareAndPrint(novaPaketna);

                    // reinicijaliziraj podatke
                    lvSerials.Items.Clear();
                    RezervisiOmotneZaSkeniranje();
                }
            }
        }



        private async Task PrepareAndPrint(PaketnaViewModel novaPaketna)
        {


            IzvozViewModel = new IzvozPodatakaViewModel(App.ViewModel.OdabraniPlan);
            await IzvozViewModel.UcitajPlanPakovanja();

            var dataSource = from j in IzvozViewModel.Paketne
                             join pr in App.ViewModel.ListaProizvoda on j.ProizvodId equals pr.Id
                             join pl in App.ViewModel.ListaPlanova on j.PlanPakovanjaId equals pl.Id
                             where j.PlanPakovanjaId == App.ViewModel.OdabraniPlan.Id && j.PaketnaId == novaPaketna.Model.PaketnaId
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

            Assembly assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream reportStream = assembly.GetManifestResourceStream("TrackAndTrace.Report5.rdlc");
            //PageSettings pageSettings = new PageSettings();
            //pageSettings.PageWidth = 80;
            //pageSettings.PageHeight = 30;
            //reportWriter.PageSettings = pageSettings;
            sfReport.ProcessingMode = Syncfusion.UI.Xaml.Reports.ProcessingMode.Local;
            sfReport.ExportMode = Syncfusion.UI.Xaml.Reports.ExportMode.Local;
            sfReport.LoadReport(reportStream);

            sfReport.DataSources.Clear();
            sfReport.DataSources.Add(new ReportDataSource("DataSet1", dataSource));

            sfReport.RefreshReport();

            //MemoryStream stream = new MemoryStream();
            //reportWriter.Save(stream, WriterFormat.PDF);

            //StorageFile storageFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("jed_etiketa.pdf", CreationCollisionOption.GenerateUniqueName);
            //if (storageFile!=null)
            //{
            //    IRandomAccessStream randomAccessStream = await storageFile.OpenAsync(FileAccessMode.ReadWrite);
            //    await randomAccessStream.WriteAsync(stream.ToArray().AsBuffer());

            //    randomAccessStream.Seek(0);

            //    PdfDocument pdfDocument = await PdfDocument.LoadFromStreamAsync(randomAccessStream);
            //    uIElements.Clear();

            //    var bmp = new BitmapImage();

            //    MemoryStream memoryStream = new MemoryStream();
            //    var outStream = memoryStream.AsRandomAccessStream();

            //    PdfPageRenderOptions renderOptions = new PdfPageRenderOptions();

            //    PdfPage pdfPage = pdfDocument.GetPage(0);

            //    await pdfDocument.GetPage(0).RenderToStreamAsync(outStream, new PdfPageRenderOptions());

            //    bmp.SetSource(outStream);

            //    Image image = new Image();
            //    image.Source = bmp;

            //    page = new Page();
            //    page.Content = image;

            //    uIElements.Add(page);

            //}


            //await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();

        }


    }
}
