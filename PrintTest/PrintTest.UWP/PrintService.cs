using PrintTest.UWP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Printing;
using Xamarin.Forms;

[assembly: Dependency(typeof(PrintUWPService))]

namespace PrintTest.UWP
{
    public class PrintUWPService : IPrintUWPService
    {
        PrintManager printmgr = PrintManager.GetForCurrentView();
        PrintDocument PrintDoc;
        PrintDocument printDoc;
        PrintTask Task;
        private Windows.UI.Xaml.Controls.WebView ViewToPrint = new Windows.UI.Xaml.Controls.WebView();
        private DeviceInformationCollection deviceCollection;
        private Windows.UI.Xaml.Shapes.Rectangle rectangle = new Windows.UI.Xaml.Shapes.Rectangle();


        public PrintUWPService()
        {
            printmgr.PrintTaskRequested += Printmgr_PrintTaskRequested;
            ViewToPrint.LoadCompleted += ViewToPrint_LoadCompleted;
        }

        private void ViewToPrint_LoadCompleted(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            MakePage();
        }

        public void Print(string html)
        {

            Window.Current.Content = ViewToPrint;
            ViewToPrint.NavigateToString(html);


            if (PrintDoc != null)
            {
                printDoc.GetPreviewPage -= PrintDoc_GetPreviewPage;
                printDoc.Paginate -= PrintDoc_Paginate;
                printDoc.AddPages -= PrintDoc_AddPages;
            }

            printDoc = new PrintDocument();

            try
            {
                printDoc.GetPreviewPage += PrintDoc_GetPreviewPage;
                printDoc.Paginate += PrintDoc_Paginate;
                printDoc.AddPages += PrintDoc_AddPages;

                var showprint = PrintManager.ShowPrintUIAsync();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            PrintDoc = null;
            GC.Collect();
        }

        private void MakePage()
        {
            var brush = new WebViewBrush
            {
                Stretch = (Windows.UI.Xaml.Media.Stretch)Windows.UI.Xaml.Media.Stretch.Uniform
            };

            brush.SetSource(ViewToPrint);
            brush.Redraw();


            rectangle.Width = 800;
            rectangle.Height = 800;
            rectangle.Fill = brush;
            brush.Stretch = (Windows.UI.Xaml.Media.Stretch)Windows.UI.Xaml.Media.Stretch.UniformToFill;
            brush.AlignmentY = AlignmentY.Top;
            brush.AlignmentX = AlignmentX.Left;
            rectangle.Name = "MyWebViewRectangle";

            rectangle.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void Printmgr_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var deff = args.Request.GetDeferral();
            Task = args.Request.CreatePrintTask($"Card Stock { DateTime.Now}", OnPrintTaskSourceRequested);

            deff.Complete();

        }
        async void OnPrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {

            var def = args.GetDeferral();
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                args.SetSource(printDoc.DocumentSource);
            });
            def.Complete();
        }

        private void PrintDoc_AddPages(object sender, AddPagesEventArgs e)
        {
            printDoc.AddPage(rectangle);
            printDoc.AddPagesComplete();
        }

        private async void PrintDoc_Paginate(object sender, PaginateEventArgs e)
        {
            PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);
            deviceCollection = await DeviceInformation.FindAllAsync("System.Devices.InterfaceClassGuid:=\"{0ecef634-6ef0-472a-8085-5ad023ecbccd}\"");
            var rolloPrinter = deviceCollection.Where(x => x.Name.Contains("Rollo")).SingleOrDefault();

            // Get the page description to deterimine how big the page is
            PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

            PrintTaskOptions opt = Task.Options;

            printDoc.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void PrintDoc_GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            printDoc.SetPreviewPage(e.PageNumber, rectangle);
        }


    }
}
