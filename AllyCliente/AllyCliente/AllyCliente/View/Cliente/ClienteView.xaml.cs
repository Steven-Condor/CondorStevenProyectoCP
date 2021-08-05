using AllyCliente.Model;
using AllyCliente.Service;
using AllyCliente.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyCliente.View.Cliente
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClienteView : FlyoutPage
    {
        readonly LoginService servicio = new LoginService();
        public ClienteView()
        {
            InitializeComponent();
            Detail = new NavigationPage(new ClienteViewDetail());
            FlyoutPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as ClienteViewFlyoutMenuItem;
            if(item == null)
            {
                return;
            }
            else    if (item.Id == 2)
            {
                var lector = await CargarLectorQR();
                if(lector != null)
                {
                    Preferences.Set("IdOrdenTemp", lector.ToString());
                    await Detail.Navigation.PushAsync((Page)Activator.CreateInstance(item.TargetType));
                    FlyoutPage.ListView.SelectedItem = null;
                    IsPresented = false;
                }
            }
            else if(item.Id == 3)
            {
                bool sesion = servicio.CerrarSesion();
                if(sesion)
                    App.Current.MainPage = new NavigationPage(new LoginView());
            }
            else
            {
                await Detail .Navigation.PushAsync((Page)Activator.CreateInstance(item.TargetType));
                FlyoutPage.ListView.SelectedItem = null;
                IsPresented = false;
            }
        }

        private async Task<string> CargarLectorQR()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            scanner.TopText = "Ally Servicios te da la bienvenida";
            scanner.BottomText = "Apunta hacia un objetivo para ver su infromación";
            var result = await scanner.Scan();
            if (result != null)
            {
                return result.Text;
            }
            else
            {
                return null;
            }
        }
    }
}