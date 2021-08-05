using AllyContratista.Model;
using AllyContratista.ViewModel;
using AllyContratista.ViewModel.Contratista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyContratista.View.Contratista.Editar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditarServicioView : ContentPage
    {
        public EditarServicioView(ServicioModel servicio)
        {
            InitializeComponent();
            validarComandos(servicio.IdServicio);
            BindingContext = new EditarServicioViewModel(servicio); 
        }

        public void validarComandos(string idServicio)
        {
            if(idServicio == null)
            {
                btnActualizar.IsVisible = false;
                btnNuevo.IsVisible = true;
            }
            else
            {
                btnActualizar.IsVisible = true;
                btnNuevo.IsVisible = false;
            }
        }
    }
}