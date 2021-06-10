using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gdk;
using Gtk;
using Uno.UI.Runtime.Skia;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using GtkClipboard = Gtk.Clipboard;

namespace ClipboardTest
{
	public static partial class Clipboard
	{
		static readonly Atom HtmlContent = Atom.Intern("text/html", false);
		static readonly Atom RtfContent = Atom.Intern("text/rtf", false);
		static readonly Atom GnomeCopiedFilesContent = Atom.Intern("x-special/gnome-copied-files", false);

		private static GtkClipboard _clipboard;

		static Clipboard()
		{
			_clipboard = GtkClipboard.GetDefault(GtkHost.Window.Display);
		}

		public static void Clear() => _clipboard.Clear();
		public static void FlushInternal()
		{
			_clipboard.CanStore = null;
			_clipboard.Store();
		}

		public static DataPackageView GetContent()
		{
			var dataPackage = new DataPackage();

			var setDataProvider = dataPackage.GetType().GetMethod("SetDataProvider", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			var funcAsyncType = setDataProvider.GetParameters()[1].ParameterType;

			if (_clipboard.WaitIsImageAvailable())
			{
				setDataProvider.Invoke(dataPackage, new object[] { StandardDataFormats.Bitmap, DelegateUtility.Cast(new Func<CancellationToken, Task<object>>(async ct =>
				{
					var image = _clipboard.WaitForImage();
					var data = image.SaveToBuffer("bmp");
					var stream = new MemoryStream(data);
					return RandomAccessStreamReference.CreateFromStream(stream.AsRandomAccessStream());
				}), funcAsyncType)});
			}
			if (_clipboard.WaitIsTargetAvailable(HtmlContent))
			{
				setDataProvider.Invoke(dataPackage, new object[] { StandardDataFormats.Html, DelegateUtility.Cast(new Func<CancellationToken, Task<object>>(async ct =>
				{
					var selectionData = _clipboard.WaitForContents(HtmlContent);
					return Encoding.UTF8.GetString(selectionData.Data);
				}), funcAsyncType)});
			}
			if (_clipboard.WaitIsTargetAvailable(RtfContent))
			{
				setDataProvider.Invoke(dataPackage, new object[] { StandardDataFormats.Rtf, DelegateUtility.Cast(new Func<CancellationToken, Task<object>>(async ct =>
				{
					var selectionData = _clipboard.WaitForContents(RtfContent);
					return Encoding.UTF8.GetString(selectionData.Data);
				}), funcAsyncType)});
			}
			if (_clipboard.WaitIsTextAvailable())
			{
				setDataProvider.Invoke(dataPackage, new object[] { StandardDataFormats.Text, DelegateUtility.Cast(new Func<CancellationToken, Task<object>>(async ct =>
				{
					return _clipboard.WaitForText();
				}), funcAsyncType)});
		}
			if (_clipboard.WaitIsUrisAvailable())
			{
				// We have to get the actual Uris before determining
				// the Uri types. Therefore, we will not use SetDataProvider here.

				// TO-DO: Investigate what uris actually is
				// Gtk documentation https://developer.gnome.org/gtk3/stable/gtk3-Clipboards.html#gtk-clipboard-wait-for-uris
				// says that the function returns an **array** of strings,
				// while GTK# just exposes 1 string?
				var uris = _clipboard.WaitForUris();

				global::System.Diagnostics.Debug.WriteLine(uris);

				if (uris != null)
				{
					//DataPackage.SeparateUri(
					//	uris,
					//	out string webLink,
					//	out string applicationLink);

					//var clipWebLink = webLink != null ? new Uri(webLink) : null;
					//var clipApplicationLink = applicationLink != null ? new Uri(applicationLink) : null;
					//var clipUri = new Uri(uris);

					//if (clipWebLink != null)
					//{
					//	dataPackage.SetWebLink(clipWebLink);
					//}
					//if (clipApplicationLink != null)
					//{
					//	dataPackage.SetApplicationLink(clipApplicationLink);
					//}
					//if (clipUri != null)
					//{
					//	dataPackage.SetUri(clipUri);
					//}
				}
			}
			if (_clipboard.WaitIsTargetAvailable(GnomeCopiedFilesContent))
			{
				setDataProvider.Invoke(dataPackage, new object[] { StandardDataFormats.StorageItems, DelegateUtility.Cast(new Func<CancellationToken, Task<object>>(async ct =>
				{
					var data = _clipboard.WaitForContents(GnomeCopiedFilesContent);
					var dataList = Encoding.UTF8.GetString(data.Data);
					global::System.Diagnostics.Debug.WriteLine(dataList);
					return new List<IStorageItem>();
				}), funcAsyncType)});
			}

			return dataPackage.GetView();
		}

		public static void SetContent(DataPackage content) => throw new NotImplementedException();

		public static void StartContentChanged()
		{
			_clipboard.OwnerChange += Clipboard_OwnerChange;
		}

		public static void StopContentChanged()
		{
			_clipboard.OwnerChange -= Clipboard_OwnerChange;
		}

		private static void Clipboard_OwnerChange(object o, OwnerChangeArgs args)
		{
			OnContentChanged();
		}
	}

    public static class DelegateUtility
    {
        public static T Cast<T>(Delegate source) where T : class
        {
            return Cast(source, typeof(T)) as T;
        }

        public static Delegate Cast(Delegate source, Type type)
        {
            if (source == null)
                return null;

            Delegate[] delegates = source.GetInvocationList();
            if (delegates.Length == 1)
                return Delegate.CreateDelegate(type,
                    delegates[0].Target, delegates[0].Method);

            Delegate[] delegatesDest = new Delegate[delegates.Length];
            for (int nDelegate = 0; nDelegate < delegates.Length; nDelegate++)
                delegatesDest[nDelegate] = Delegate.CreateDelegate(type,
                    delegates[nDelegate].Target, delegates[nDelegate].Method);
            return Delegate.Combine(delegatesDest);
        }
    }
}
