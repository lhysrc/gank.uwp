using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GankIO.Common
{   

    public class WebViewExt
    {
        public static readonly DependencyProperty HtmlStringProperty =
           DependencyProperty.RegisterAttached("HtmlString", typeof(string), typeof(WebViewExt), new PropertyMetadata("", OnHtmlStringChanged));

        public static string GetHtmlString(DependencyObject obj) { return (string)obj.GetValue(HtmlStringProperty); }
        public static void SetHtmlString(DependencyObject obj, string value) { obj.SetValue(HtmlStringProperty, value); }

        private static void OnHtmlStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebView wv = d as WebView;
            if (wv != null)
            {
                wv.NavigateToString((string)e.NewValue);
            }
        }







        public static string GetBindingUri(DependencyObject obj)
        {
            return (string)obj.GetValue(BindingUriProperty);
        }

        public static void SetBindingUri(DependencyObject obj, string value)
        {
            obj.SetValue(BindingUriProperty, value);
        }
        
        public static readonly DependencyProperty BindingUriProperty =
            DependencyProperty.RegisterAttached("BindingUri", typeof(string), typeof(WebViewExt), new PropertyMetadata("", OnUriChanged));

        private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebView wv = d as WebView;
            if (wv != null)
            {
                wv.Navigate(new Uri((string)e.NewValue));
            }
        }
    }

    public static class Exts
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
                    this IEnumerable<TSource> source,
                    Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }
        public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
        {
            return source.OrderBy(_ => Guid.NewGuid());
        }


        public static IEnumerable<TSource> Sample<TSource>(this IEnumerable<TSource> source, int i)
        {
            return source.Shuffle().Take(i);
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return String.IsNullOrEmpty(s);
        }

        public static bool IsNullOrWhiteSpace(this string s)
        {
            return String.IsNullOrWhiteSpace(s);
        }


        public static async Task SaveToFileAsync(this BitmapImage bitmap, StorageFile newFile)
        {
            if (bitmap.UriSource == null) throw new ArgumentNullException(nameof(bitmap.UriSource), "BitmapImage.UriSource is null.");
            using (IRandomAccessStream ras = await RandomAccessStreamReference.CreateFromUri(bitmap.UriSource).OpenReadAsync())
            {
                WriteableBitmap writeableBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
                await writeableBitmap.SetSourceAsync(ras);
                await SaveToFileAsync(writeableBitmap, newFile);
            }
        }

        public static async Task SaveToFileAsync(this WriteableBitmap image, StorageFile newFile)
        {

            if (image == null)
            {
                return;
            }
            Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
            var path = newFile.Path;
            if (path.EndsWith("jpg"))
                BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
            else if (path.EndsWith("png"))
                BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
            else if (path.EndsWith("bmp"))
                BitmapEncoderGuid = BitmapEncoder.BmpEncoderId;
            else if (path.EndsWith("tiff"))
                BitmapEncoderGuid = BitmapEncoder.TiffEncoderId;
            else if (path.EndsWith("gif"))
                BitmapEncoderGuid = BitmapEncoder.GifEncoderId;
            //var folder = await _local_folder.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);
            //var file = await KnownFolders.PicturesLibrary.CreateFileAsync(newFile, CreationCollisionOption.GenerateUniqueName);

            using (IRandomAccessStream stream = await newFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);
                Stream pixelStream = image.PixelBuffer.AsStream();
                byte[] pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                          (uint)image.PixelWidth,
                          (uint)image.PixelHeight,
                          96.0,
                          96.0,
                          pixels);
                await encoder.FlushAsync();
            }

        }
    }




    public static class VisualTreeHelperExtensions
    {
        public static T GetFirstDescendantOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetDescendantsOfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetDescendantsOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetDescendants().OfType<T>();
        }

        public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject start)
        {
            var queue = new Queue<DependencyObject>();
            var count = VisualTreeHelper.GetChildrenCount(start);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(start, i);
                yield return child;
                queue.Enqueue(child);
            }

            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();
                var count2 = VisualTreeHelper.GetChildrenCount(parent);

                for (int i = 0; i < count2; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    yield return child;
                    queue.Enqueue(child);
                }
            }
        }

        public static T GetFirstAncestorOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetAncestorsOfType<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetAncestorsOfType<T>(this DependencyObject start) where T : DependencyObject
        {
            return start.GetAncestors().OfType<T>();
        }

        public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject start)
        {
            var parent = VisualTreeHelper.GetParent(start);

            while (parent != null)
            {
                yield return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        public static bool IsInVisualTree(this DependencyObject dob)
        {
            return Window.Current.Content != null && dob.GetAncestors().Contains(Window.Current.Content);
        }

        public static Rect GetBoundingRect(this FrameworkElement dob, FrameworkElement relativeTo = null)
        {
            if (relativeTo == null)
            {
                relativeTo = Window.Current.Content as FrameworkElement;
            }

            if (relativeTo == null)
            {
                throw new InvalidOperationException("Element not in visual tree.");
            }

            if (dob == relativeTo)
                return new Rect(0, 0, relativeTo.ActualWidth, relativeTo.ActualHeight);

            var ancestors = dob.GetAncestors().ToArray();

            if (!ancestors.Contains(relativeTo))
            {
                throw new InvalidOperationException("Element not in visual tree.");
            }

            var pos =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(new Point());
            var pos2 =
                dob
                    .TransformToVisual(relativeTo)
                    .TransformPoint(
                        new Point(
                            dob.ActualWidth,
                            dob.ActualHeight));

            return new Rect(pos, pos2);
        }
    }
}
