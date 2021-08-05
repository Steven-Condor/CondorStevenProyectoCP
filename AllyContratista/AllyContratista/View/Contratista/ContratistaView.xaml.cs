using AllyContratista.Model;
using AllyContratista.Service;
using AllyContratista.View.Registro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyContratista.View.Contratista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContratistaView : FlyoutPage
    {
        readonly LoginService loginService = new LoginService();
        public ContratistaView()
        {
            
            Detail = new NavigationPage(new ContratistaViewDetail())
            {
                BarBackgroundColor = Color.FromHex("F2CC05"),
                BarTextColor = Color.White
            };
            InitializeComponent();
            FlyoutPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MenuContratistaModel;
            if (item == null)
            {
                return;
            }
            else if(item.Id == 3)
            {
                bool sesion = loginService.CerrarSesion();
                if(sesion == true)
                {
                    App.Current.MainPage = new NavigationPage(new LoginView());
                    FlyoutPage.ListView.SelectedItem = null;
                    IsPresented = false;
                }
                else
                {
                    FlyoutPage.ListView.SelectedItem = null;
                    IsPresented = false;
                }
                
            }
            else
            {
                Detail.Navigation.PushAsync((Page)Activator.CreateInstance(item.TargetType));
                FlyoutPage.ListView.SelectedItem = null;
                IsPresented = false;
            }
        }
    }
}