//#if HAS_UNO_SKIA && !HAS_UNO_SKIA_WPF && !HAS_UNO_SKIA_GTK
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ClipboardTest
//{
//	public partial class Clipboard
//	{
//		private static object _syncLock = new object();
//		private static EventHandler<object> _contentChanged;

//		public static void Flush()
//		{
//			FlushInternal();
//		}

//		public static event EventHandler<object> ContentChanged
//		{
//			add
//			{
//				lock (_syncLock)
//				{
//					var firstSubscriber = _contentChanged == null;
//					_contentChanged += value;
//					if (firstSubscriber)
//					{
//						StartContentChanged();
//					}
//				}
//			}
//			remove
//			{
//				lock (_syncLock)
//				{
//					_contentChanged -= value;
//					if (_contentChanged == null)
//					{
//						StopContentChanged();
//					}
//				}
//			}
//		}

//		private static void OnContentChanged()
//		{
//			_contentChanged?.Invoke(null, null);
//		}
//	}
//}
//#endif