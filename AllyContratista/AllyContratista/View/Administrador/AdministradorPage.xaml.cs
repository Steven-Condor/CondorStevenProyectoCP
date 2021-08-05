using AllyContratista.Model;
using AllyContratista.Service;
using AllyContratista.View.Registro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyContratista.View.Administrador
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdministradorPage : FlyoutPage
    {
        LoginService loginService = new LoginService();
        public AdministradorPage()
        {
            InitializeComponent();
            Detail = new NavigationPage(new AdministradorView())
            {
                BarBackgroundColor = Color.FromHex("F2CC05"),
                BarTextColor = Color.White
            };
            FlyoutPage.MenuLista.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MenuContratistaModel;
            if (item == null)
            {
                return;
            }
            else if (item.Id == 2)
            {
                bool sesion = loginService.CerrarSesion();
                if (sesion == true)
                {
                    App.Current.MainPage = new NavigationPage(new LoginView());
                    FlyoutPage.MenuLista.SelectedItem = null;
                    IsPresented = false;
                }
                else
                {
                    FlyoutPage.MenuLista.SelectedItem = null;
                    IsPresented = false;
                }

            }
            else
            {
                Detail.Navigation.PushAsync((Page)Activator.CreateInstance(item.TargetType));
                FlyoutPage.MenuLista.SelectedItem = null;
                IsPresented = false;
            }
        }

        }
}