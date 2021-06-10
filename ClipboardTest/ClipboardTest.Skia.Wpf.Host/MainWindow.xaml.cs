using ClipboardTest.Skia.Wpf.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClipboardTest.WPF.Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //WpfFunctions.GetText = System.Windows.Clipboard.GetText;
            //WpfFunctions.SetText = System.Windows.Clipboard.SetText;

            root.Content = new CustomWpfHost(Dispatcher, () => new ClipboardTest.App());

            var host = root.Content as Uno.UI.Skia.Platform.WpfHost;
            host.Loaded += (sender, args) =>
            {
                var win = Window.GetWindow(root);
                var fromDependencyObject = PresentationSource.FromDependencyObject(win);
                var hwndSource = fromDependencyObject as HwndSource;
                //WpfFunctions.GetHwnd = () =>
                //{
                //    return hwndSource.Handle;
                //};
                //WpfFunctions.HwndAddHook = (f) =>
                //{
                //    hwndSource.AddHook(DelegateUtility.Cast<HwndSourceHook>(f));
                //};
                //WpfFunctions.HwndRemoveHook = (f) =>
                //{
                //    hwndSource.RemoveHook(DelegateUtility.Cast<HwndSourceHook>(f));
                //};
            };
        }
    }
}
