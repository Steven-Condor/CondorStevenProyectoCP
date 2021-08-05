using AllyCliente.Model;
using AllyCliente.View.Cliente.Editar;
using AllyCliente.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AllyCliente.View.Editar;

namespace AllyCliente.View.Cliente
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClienteViewFlyout : ContentPage
    {
        public ListView ListView;

        public ClienteViewFlyout()
        {
            InitializeComponent();

            BindingContext = new ClienteViewFlyoutViewModel();
            ListView = MenuItemsListView;
        }

        class ClienteViewFlyoutViewModel
        {
            public ObservableCollection<ClienteViewFlyoutMenuItem> MenuItems { get; set; }

            public ClienteViewFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<ClienteViewFlyoutMenuItem>(new[]
                {
                    new ClienteViewFlyoutMenuItem { icon="perfil.png", Id = 0, Title = "Perfil de Usuario" , TargetType= typeof(PerfilUsuarioView)},
                    new ClienteViewFlyoutMenuItem { icon ="reporte.png", Id = 1, Title = "Reportes de servcios" , TargetType = typeof(ReportesView)},
                    new ClienteViewFlyoutMenuItem { icon="qr.png", Id = 2, Title = "Lector QR" , TargetType = typeof(LectorQRView)},
                    new ClienteViewFlyoutMenuItem { icon="logout.png", Id = 3, Title = "Cerrar Sesion" },
                });
            }
        }
    }
}