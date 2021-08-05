using AllyContratista.Model;
using AllyContratista.Model.Consultas;
using AllyContratista.Model.SubModel;
using AllyContratista.ViewModel.Contratista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace AllyContratista.View.Contratista.Editar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GestionarOrdenesServicioView : ContentPage
    {
        readonly private string idOrden;
        public GestionarOrdenesServicioView(string idOrdenServicio)
        {
            idOrden = idOrdenServicio;
            InitializeComponent();
            GestionarOrdenesServicioViewModel contexto = new GestionarOrdenesServicioViewModel(idOrdenServicio);
            BindingContext = contexto;
            var stk = CrearQr();
            //Mostrara Codigo QR en un StackLayout
            StackQr.Children.Add(stk);
        }

        //Generar Codigo QR
        public ZXingBarcodeImageView CrearQr()
        {
            var qr = new ZXingBarcodeImageView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            qr.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
            qr.BarcodeOptions.Width = 500;
            qr.BarcodeOptions.Height = 500;
            qr.BarcodeValue = idOrden;

            return qr;
        }
    }
}