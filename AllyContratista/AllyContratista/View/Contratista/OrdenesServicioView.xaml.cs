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
    public partial class OrdenesServicioView : ContentPage
    {
        OrdenesServicioViewModel contexto;
        public OrdenesServicioView(string idServicio)
        {
            InitializeComponent();
            contexto = new OrdenesServicioViewModel(idServicio);
            BindingContext = contexto;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}