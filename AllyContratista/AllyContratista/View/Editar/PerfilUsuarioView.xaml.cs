using AllyContratista.ViewModel.Contratista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyContratista.View.Editar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PerfilUsuarioView : ContentPage
    {
        public PerfilUsuarioView()
        {
            InitializeComponent();
            BindingContext = new PerfilViewModel();
        }
    }
}