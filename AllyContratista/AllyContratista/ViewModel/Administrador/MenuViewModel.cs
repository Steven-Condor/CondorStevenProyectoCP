using AllyContratista.Model;
using AllyContratista.View.Administrador;
using AllyContratista.View.Editar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace AllyContratista.ViewModel.Administrador
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {
            CaragrDatos();
        }

        public ObservableCollection<MenuContratistaModel> MenuItems { get; set; }

        private void  CaragrDatos()
        {
            //Cargar la coleccion de datos Menu con las opciones disponibles
            MenuItems = new ObservableCollection<MenuContratistaModel>(new[]
            {
                    new MenuContratistaModel { Id = 1, Title = "Perfil de Usuario", Icon = "perfil.png", TargetType = typeof(PerfilUsuarioView) },
                    new MenuContratistaModel { Id = 2, Title = "Cerrar Sesion" , Icon="logout.png"},
            });
        }
    }
}
