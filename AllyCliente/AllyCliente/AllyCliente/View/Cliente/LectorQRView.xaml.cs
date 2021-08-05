using AllyCliente.ViewModel.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyCliente.View.Cliente
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LectorQRView : ContentPage
    {
        public LectorQRView()
        {
            BindingContext = new LectorQRViewModel(Preferences.Get("IdOrdenTemp", null));
            InitializeComponent();
            Preferences.Set("IdOrdenTemp", null);
        }
    }
}