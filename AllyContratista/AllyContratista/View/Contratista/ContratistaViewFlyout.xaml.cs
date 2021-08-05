using AllyContratista.Model;
using AllyContratista.View.Editar;
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

namespace AllyContratista.View.Contratista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContratistaViewFlyout : ContentPage
    {
        public ListView ListView;

        public ContratistaViewFlyout()
        {
            InitializeComponent();

            BindingContext = new ContratistaViewFlyoutViewModel();
            ListView = MenuItemsListView;
        }

        class ContratistaViewFlyoutViewModel 
        {
            public ObservableCollection<MenuContratistaModel> MenuItems { get; set; }

            public ContratistaViewFlyoutViewModel()
            {
                
                MenuItems = new ObservableCollection<MenuContratistaModel>(new[]
                {
                    new MenuContratistaModel { Id = 0, Title = "Perfil de Usuario", Icon = "perfil.png", TargetType = typeof(PerfilUsuarioView) },
                    new MenuContratistaModel { Id = 1, Title = "Reportes", Icon = "reporte.png", TargetType = typeof(ReportesView) },
                    new MenuContratistaModel { Id = 3, Title = "Cerrar Sesion" , Icon="logout.png"},
                });
            }
        }
    }
}