using AllyContratista.ViewModel.Administrador;
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
    public partial class AdministradorMenuView : ContentPage
    {
        public ListView MenuLista;
        public AdministradorMenuView()
        {
            InitializeComponent();
            BindingContext = new MenuViewModel();
            MenuLista = MenuItemsListView;
        }
    }
}