using AllyCliente.Model;
using AllyCliente.ViewModel.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyCliente.View.Cliente.Editar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InformacionOrdenServicioView : ContentPage
    {
        public InformacionOrdenServicioView(OrdenesServicioModel ordenServicio)
        {
            InitializeComponent();
            BindingContext = new InformacionOrdenServicioViewModel(ordenServicio);
        }
    }
}