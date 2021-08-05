using AllyCliente.ViewModel.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyCliente.View.Editar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportesView : ContentPage
    {
        public ReportesView()
        {
            InitializeComponent();
            BindingContext = new ReportesViewModel();
        }
    }
}