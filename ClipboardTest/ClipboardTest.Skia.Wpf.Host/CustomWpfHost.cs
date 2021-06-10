using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using System.Windows.Threading;
using Uno.UI.Skia.Platform;

namespace ClipboardTest.Skia.Wpf.Host
{
    public class CustomWpfHost : WpfHost
    {
        private readonly FieldInfo bitmapField;

        public CustomWpfHost(Dispatcher dispatcher, Func<Windows.UI.Xaml.Application> appBuilder, string[] args = null)
            : base(dispatcher, appBuilder, args)
        {
            bitmapField = typeof(WpfHost).GetField("bitmap", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            bitmapField.SetValue(this, null);
            base.OnRender(drawingContext);
        }
    }
}
