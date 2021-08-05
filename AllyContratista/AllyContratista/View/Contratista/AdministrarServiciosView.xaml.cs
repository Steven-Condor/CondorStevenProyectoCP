
using AllyContratista.ViewModel;
using AllyContratista.ViewModel.Contratista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyContratista.View.Contratista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdministrarServiciosView : ContentPage
    {
        public AdministrarServiciosView()
        {
            InitializeComponent();
            BindingContext = new AdministrarServicioViewModel();
        }
    }
}