using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace QRTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        FrameRenderer _frameRenderer;
        int frameSkip = 0;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();
            var selectedGroupObjectsList = frameSourceGroups.Select(group =>
               new
               {
                   sourceGroup = group,
                   colorSourceInfo = group.SourceInfos.FirstOrDefault((sourceInfo) =>
                   {
                       // On XBox/Kinect, omit the MediaStreamType and EnclosureLocation tests
                       return sourceInfo.SourceKind == MediaFrameSourceKind.Color;

                   })

               }).Where(t => t.colorSourceInfo != null)
               .ToList();

            var selectedGroupObjects = selectedGroupObjectsList.Skip(1).FirstOrDefault();

            MediaFrameSourceGroup selectedGroup = selectedGroupObjects?.sourceGroup;
            MediaFrameSourceInfo colorSourceInfo = selectedGroupObjects?.colorSourceInfo;

            if (selectedGroup == null)
            {
                return;
            }

            var mediaCapture = new MediaCapture();

            var settings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = selectedGroup,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video
            };
            try
            {
                await mediaCapture.InitializeAsync(settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed: " + ex.Message);
                return;
            }

            var colorFrameSource = mediaCapture.FrameSources[colorSourceInfo.Id];

            BitmapSize size = new BitmapSize() // Choose a lower resolution to make the image processing more performant
            {
                Height = 480,
                Width = 640
            };

            var mediaFrameReader = await mediaCapture.CreateFrameReaderAsync(colorFrameSource, MediaEncodingSubtypes.Argb32, size);
            mediaFrameReader.FrameArrived += MediaFrameReader_FrameArrived;

            imageElement.Source = new SoftwareBitmapSource();
            _frameRenderer = new FrameRenderer(imageElement);

            await mediaFrameReader.StartAsync();
        }

        private async Task<byte[]> EncodedBytes(SoftwareBitmap soft, Guid encoderId)
        {
            byte[] array = null;

            // First: Use an encoder to copy from SoftwareBitmap to an in-mem stream (FlushAsync)
            // Next:  Use ReadAsync on the in-mem stream to get byte[] array

            using (var ms = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, ms);
                encoder.SetSoftwareBitmap(soft);

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception ex) { return new byte[0]; }

                array = new byte[ms.Size];
                await ms.ReadAsync(array.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
            }
            return array;
        }

        private async void MediaFrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {

            var mediaFrameReference = sender.TryAcquireLatestFrame();
            if (mediaFrameReference != null)
            {

                SoftwareBitmap openCVInputBitmap = null;
                var inputBitmap = mediaFrameReference.VideoMediaFrame?.SoftwareBitmap;
                if (inputBitmap != null)
                {
                    //The XAML Image control can only display images in BRGA8 format with premultiplied or no alpha
                    if (inputBitmap.BitmapPixelFormat == BitmapPixelFormat.Bgra8
                        && inputBitmap.BitmapAlphaMode == BitmapAlphaMode.Premultiplied)
                    {
                        openCVInputBitmap = SoftwareBitmap.Copy(inputBitmap);
                    }
                    else
                    {
                        openCVInputBitmap = SoftwareBitmap.Convert(inputBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    }

                    SoftwareBitmap openCVOutputBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, openCVInputBitmap.PixelWidth, openCVInputBitmap.PixelHeight, BitmapAlphaMode.Premultiplied);

                    if (frameSkip == 0)
                    {
                        frameSkip++;

                        using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                        {
                            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                            // Set the software bitmap
                            encoder.SetSoftwareBitmap(openCVOutputBitmap);

                            await encoder.FlushAsync();

                            var client = new HttpClient();
                            var queryString = HttpUtility.ParseQueryString(string.Empty);

                            // Request headers
                            client.DefaultRequestHeaders.Add("Prediction-Key", "");
                            client.DefaultRequestHeaders.Add("Prediction-key", "3ac99d01a5ed4ea7a155b8fdc688a5fa");


                            var uri = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/0c1a773e-bb3d-4c84-9749-99bee73cbe1e/image?" + queryString;

                            HttpResponseMessage response;

                            using (MultipartFormDataContent _multiPartContent = new MultipartFormDataContent())
                            {
                                    StreamContent _imageData = new StreamContent(stream.AsStream());

                                    _imageData.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                                    _multiPartContent.Add(_imageData, "file", "filename.png");

                                    response = await client.PostAsync(uri, _multiPartContent);
                                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                                    {
                                        textElement.Text = await response.Content.ReadAsStringAsync();
                                    });

                            }
                        }
                    }
                    else
                    {
                        frameSkip++;
                        if (frameSkip > 6)
                        {
                            frameSkip = 0;
                        }
                    }

                    // operate on the image and render it
                    //openCVHelper.Blur(openCVInputBitmap, openCVOutputBitmap);
                    _frameRenderer.PresentSoftwareBitmap(openCVInputBitmap);

                }
            }
        }
    }
}