using AllyContratista.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyContratista.View.Registro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistroView : ContentPage
    {
        public RegistroView()
        {
            InitializeComponent();
            BindingContext = new RegistroViewModel();
        }
    }
}