using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PrintTest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Printhtml();
        }


        private readonly IPrintUWPService _printUWPService = DependencyService.Get<IPrintUWPService>();

        public void Printhtml()
        {

            var meta = "<meta name='viewport' content='width=device-width, initial-scale=5.0, maximum-scale=5.0, minimum-scale=1.0'>";
            var style = "<style>svg{height:140px; width:140px;} p{font-size:20px;} </style>";

            var _htmlSource = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                      {meta}
                       {style}
                    </head>
                    <body>
                   <table border = '1px solid'; style='border-collapse:collapse;'>
                    <tbody>
                    <tr>
                    <td>qr</td>
                    <td>
                    <p><b>Heat Number:number</b></p>
                    <p><b> Cylinder Type Name: cylindername</b></p>
                   <p><b> Color: colorname</b></p>
                    </td>
                    </tr>
                    </tbody>
                    </table>
                    </body>
                    </html>
                    ";
            Print(_htmlSource);

        }


        public void Print(string _htmlSource)
        {
            _printUWPService.Print(_htmlSource);
        }
    }
}
