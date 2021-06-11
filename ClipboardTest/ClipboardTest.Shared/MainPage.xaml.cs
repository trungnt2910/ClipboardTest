using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ClipboardTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public BitmapImage ClipboardInspectedImage { get; private set; }
        public string ClipboardStatusText { get; set; } = "Press a button to test your clipboard!";

        private int _changeCount = 0;

        public MainPage()
        {
            this.InitializeComponent();
            Clipboard.ContentChanged += Clipboard_ContentChanged;
            Log("Hooked.");
        }

        private void Clipboard_ContentChanged(object sender, object args)
        {
            ++_changeCount;
            ClipboardStatusText = $"Clipboard content changed {_changeCount} times";
            StatusText.Text = ClipboardStatusText;
            Log("Content changed.");
        }

        private static void Log(object o)
        {
            Console.WriteLine(o);
            System.Diagnostics.Debug.WriteLine(o);
        }

        private void CopyText_Click(object sender, RoutedEventArgs args)
        {
            var package = new DataPackage();
            package.SetText("Hello World!");
            Clipboard.SetContent(package);
        }

        private async void CopyBitmap_Click(object sender, RoutedEventArgs args)
        {
            var filename = $@"Assets{Path.DirectorySeparatorChar}image.png";
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var sFile = await folder.GetFileAsync(filename);
            System.Diagnostics.Debug.WriteLine(sFile.Path);
            var package = new DataPackage();
            package.SetBitmap(RandomAccessStreamReference.CreateFromFile(sFile));
            Clipboard.SetContent(package);
        }

        private void CopyHtml_Click(object sender, RoutedEventArgs args)
        {
            var package = new DataPackage();
            const string html = @"<!DOCTYPE html><html><body><p>Hello World!</p></body></html>";
            package.SetHtmlFormat(html);
            Clipboard.SetContent(package);
        }

        private void CopyRtf_Click(object sender, RoutedEventArgs args)
        {
            var package = new DataPackage();
            const string rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang1033{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
{\*\generator Riched20 10.0.19041}\viewkind4\uc1 
\pard\sa200\sl276\slmult1\b\f0\fs22\lang9 Hello World!\par
}";
            package.SetRtf(rtf);
            Clipboard.SetContent(package);
        }

        private async void CopyFile_Click(object sender, RoutedEventArgs args)
        {
            var filename = $@"Assets{Path.DirectorySeparatorChar}image.png";
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var sFile = await folder.GetFileAsync(filename);
            var package = new DataPackage();
            package.SetStorageItems(new IStorageItem[] { sFile });
            Clipboard.SetContent(package);
        }

        private void CopyTextHtmlRtf_Click(object sender, RoutedEventArgs args)
        {
            var package = new DataPackage();
            package.SetText("Hello World!");
            const string html = @"<!DOCTYPE html><html><body><p>Hello World!</p></body></html>";
            package.SetHtmlFormat(html);
            const string rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang1033{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
{\*\generator Riched20 10.0.19041}\viewkind4\uc1 
\pard\sa200\sl276\slmult1\b\f0\fs22\lang9 Hello World!\par
}";
            package.SetRtf(rtf);
            Clipboard.SetContent(package);
        }

        private async void PasteText_Click(object sender, RoutedEventArgs args)
        {
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Text))
            {
                var dialog = new ContentDialog()
                {
                    Title = "Pasted text",
                    Content = new TextBlock() { Text = await package.GetTextAsync() },
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new ContentDialog()
                {
                    Title = "No text in clipboard",
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }

        private async void PasteBitmap_Click(object sender, RoutedEventArgs args)
        {
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Bitmap))
            {
                var img = new Image();
                var src = new BitmapImage();
                var streamRef = await package.GetBitmapAsync();
                src.SetSource(await streamRef.OpenReadAsync());
                img.Source = src;
                var dialog = new ContentDialog()
                {
                    Title = "Pasted Image",
                    Content = img,
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new ContentDialog()
                {
                    Title = "No Image in clipboard",
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }

        private async void PasteHtml_Click(object sender, RoutedEventArgs args)
        {
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Html))
            {
                var dialog = new ContentDialog()
                {
                    Title = "Pasted HTML",
                    Content = new TextBlock() { Text = await package.GetHtmlFormatAsync() },
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new ContentDialog()
                {
                    Title = "No HTML in clipboard",
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }

        private async void PasteRtf_Click(object sender, RoutedEventArgs args)
        {
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Rtf))
            {
                var dialog = new ContentDialog()
                {
                    Title = "Pasted RTF",
                    Content = new TextBlock() { Text = await package.GetRtfAsync() },
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new ContentDialog()
                {
                    Title = "No RTF in clipboard",
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }

        private async void PasteFile_Click(object sender, RoutedEventArgs args)
        {
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.StorageItems))
            {
                var files = await package.GetStorageItemsAsync();
                var list = string.Join("\n", files.Select(file => file.Path));
                var dialog = new ContentDialog()
                {
                    Title = "Pasted Files",
                    Content = new TextBlock() { Text = list },
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new ContentDialog()
                {
                    Title = "No StorageItems in clipboard",
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }

        private void Flush_Click(object sender, RoutedEventArgs args)
        {
            Clipboard.Flush();
        }

        private void Clear_Click(object sender, RoutedEventArgs args)
        {
            Clipboard.Clear();
        }
    }
}
