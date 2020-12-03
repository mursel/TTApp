using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Olap.UWP.Common;
using Syncfusion.Pdf;
using Syncfusion.ReportWriter;
using TrackAndTrace.ViewModels;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Printing;
using ZXing;
using ZXing.Common;
using ZXing.Datamatrix;
using ZXing.Mobile;


namespace TrackAndTrace.Helpers
{
    public class PrintManager
    {
        private BarcodeWriter writer = null;
        private PrintDocument printDocument = null;
        public GlobalViewModel AppViewModel => App.ViewModel;

        private static PrintManager printManager;
        public static PrintManager Instance
        {
            get
            {
                if (printManager == null)
                    return new PrintManager();
                return printManager;
            }
        }

        public PrintManager() => writer = new BarcodeWriter();

        public async void DirectPrintItem(OmotnaViewModel omotna)
        {
            //writer.Format = BarcodeFormat.DATA_MATRIX;
            //writer.Options = new DatamatrixEncodingOptions
            //{
            //    SymbolShape = ZXing.Datamatrix.Encoder.SymbolShapeHint.FORCE_SQUARE,
            //    Height = 96,    // 41px cca 1,1 cm
            //    Width = 96,
            //    MaxSize = new Dimension(64, 64),
            //    MinSize = new Dimension(32, 32),
            //    GS1Format = true
            //};

            //Assembly assembly = typeof(MainPage).GetTypeInfo().Assembly;

            //Stream reportStream = assembly.GetManifestResourceStream("TrackAndTrace.Report5.rdlc");
            //ReportWriter reportWriter = new ReportWriter();

            //reportWriter.ReportProcessingMode = ProcessingMode.Local;
            //reportWriter.ExportMode = ExportMode.Local;
            ////reportWriter.PaperOrientation = PaperOrientation.Landscape;
            //reportWriter.LoadReport(reportStream);

            //reportWriter.DataSources.Clear();

            //MemoryStream stream = new MemoryStream();
            //reportWriter.Save(stream, WriterFormat.PDF);


            //}

        }

    }
}
