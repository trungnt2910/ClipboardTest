//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Windows.ApplicationModel.DataTransfer;
//using Windows.UI.Core;

//namespace ClipboardTest
//{
//	public static partial class Clipboard
//	{
//		public static void Clear() => SetClipboardText(string.Empty);

//		public static void SetContent(DataPackage/* ? */ content)
//		{
//			CoreDispatcher.Main.RunAsync(
//				CoreDispatcherPriority.High,
//				() => SetContentAsync(content));
//		}

//		internal static async Task SetContentAsync(DataPackage/* ? */ content)
//		{
//			var data = content?.GetView(); // Freezes the DataPackage
//			if (data?.Contains(StandardDataFormats.Text) ?? false)
//			{
//				var text = await data.GetTextAsync();
//				SetClipboardText(text);
//			}
//		}

//		public static DataPackageView GetContent()
//		{
//			var dataPackage = new DataPackage();

//			var setDataProvider = dataPackage.GetType().GetMethod("SetDataProvider", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

//			var funcAsyncType = setDataProvider.GetParameters()[1].ParameterType;

//            setDataProvider.Invoke(dataPackage, new object[] { StandardDataFormats.Text, DelegateUtility.Cast(new Func<CancellationToken, Task<object>>(async ct =>
//				{
//					var text = string.Empty;
//					await CoreDispatcher.Main.RunAsync(
//						CoreDispatcherPriority.High,
//						async () => text = await GetClipboardText());

//					return text;
//				}), funcAsyncType)
//			});

//            return dataPackage.GetView();
//		}

//		private static async Task<string> GetClipboardText()
//		{
//			return WpfFunctions.GetText?.Invoke();
//			//var command = $"{JsType}.getText();";
//			//var text = await WebAssemblyRuntime.InvokeAsync(command);

//			//return text;
//		}

//		private static void SetClipboardText(string text)
//		{
//			WpfFunctions.SetText?.Invoke(text);
//			//var escapedText = WebAssemblyRuntime.EscapeJs(text);
//			//var command = $"{JsType}.setText(\"{escapedText}\");";
//			//WebAssemblyRuntime.InvokeJS(command);
//		}

//		private static void StartContentChanged()
//		{
//			NativeFunctions.AddClipboardFormatListener(WpfFunctions.GetHwnd());
//			WpfFunctions.HwndAddHook(OnWmMessage);
//			//var command = $"{JsType}.startContentChanged()";
//			//WebAssemblyRuntime.InvokeJS(command);
//		}

//		private static void StopContentChanged()
//		{
//			WpfFunctions.HwndRemoveHook(OnWmMessage);
//			NativeFunctions.RemoveClipboardFormatListener(WpfFunctions.GetHwnd());
//			//var command = $"{JsType}.stopContentChanged()";
//			//WebAssemblyRuntime.InvokeJS(command);
//		}

//		private static IntPtr OnWmMessage(IntPtr hwnd, int msg, IntPtr wparamOriginal, IntPtr lparamOriginal, ref bool handled)
//		{
//			switch (msg)
//			{
//				case NativeFunctions.WM_CLIPBOARDUPDATE:
//                {
//					DispatchContentChanged();
//					handled = true;
//					break;
//                }
//			}

//			return IntPtr.Zero;
//		}

//		public static int DispatchContentChanged()
//		{
//			OnContentChanged();
//			return 0;
//		}
//	}

//	public static class WpfFunctions
//    {
//		public delegate IntPtr WmMessageHandler(IntPtr hwnd, int msg, IntPtr wparamOriginal, IntPtr lparamOriginal, ref bool handled);

//		public static Action<string> SetText;
//		public static Func<string> GetText;

//		public static Func<IntPtr> GetHwnd;
//		public static Action<WmMessageHandler> HwndAddHook;
//		public static Action<WmMessageHandler> HwndRemoveHook;
//	}

//	public static class NativeFunctions
//    {
//		[DllImport("user32.dll", SetLastError = true)]
//		[return: MarshalAs(UnmanagedType.Bool)]
//		public static extern bool AddClipboardFormatListener(IntPtr hwnd);

//		[DllImport("user32.dll", SetLastError = true)]
//		[return: MarshalAs(UnmanagedType.Bool)]
//		public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

//		public const int WM_CLIPBOARDUPDATE = 0x031D;
//	}

//    public static class DelegateUtility
//    {
//        public static T Cast<T>(Delegate source) where T : class
//        {
//            return Cast(source, typeof(T)) as T;
//        }

//        public static Delegate Cast(Delegate source, Type type)
//        {
//            if (source == null)
//                return null;

//            Delegate[] delegates = source.GetInvocationList();
//            if (delegates.Length == 1)
//                return Delegate.CreateDelegate(type,
//                    delegates[0].Target, delegates[0].Method);

//            Delegate[] delegatesDest = new Delegate[delegates.Length];
//            for (int nDelegate = 0; nDelegate < delegates.Length; nDelegate++)
//                delegatesDest[nDelegate] = Delegate.CreateDelegate(type,
//                    delegates[nDelegate].Target, delegates[nDelegate].Method);
//            return Delegate.Combine(delegatesDest);
//        }
//    }
//}
