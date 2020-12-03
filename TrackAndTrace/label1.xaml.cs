using DbProvider.Pomocna;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using ZXing;
using ZXing.Datamatrix;
using ZXing.Mobile;
using ZXing.Rendering;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TrackAndTrace
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class label1 : Page
    {
        public label1()
        {
            this.InitializeComponent();
        }

        private async void btngen_Click(object sender, RoutedEventArgs e)
        {
            var writer = new BarcodeWriter();
            writer.Format = ZXing.BarcodeFormat.DATA_MATRIX;
            writer.Options = new DatamatrixEncodingOptions
            {
                //SymbolShape = ZXing.Datamatrix.Encoder.SymbolShapeHint.FORCE_SQUARE,
                Height=128,
                Width=128,
                GS1Format = true
            };
            
            
            
            var bitmap = writer.Write("90SK010"+ (char)29 + "2500000170907001006");

            string b64 = Convert.ToBase64String(bitmap.PixelBuffer.ToArray(), Base64FormattingOptions.None);

            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                var bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ms);
                bitmapEncoder.SetSoftwareBitmap(
                    SoftwareBitmap.CreateCopyFromBuffer(bitmap.PixelBuffer, BitmapPixelFormat.Bgra8, bitmap.PixelWidth, bitmap.PixelHeight));
                await bitmapEncoder.FlushAsync();

                byte[] bs = new byte[ms.Size];
                await ms.AsStream().ReadAsync(bs, 0, bs.Length);

                b64 = Convert.ToBase64String(bs);
            }

        }
    }
}
