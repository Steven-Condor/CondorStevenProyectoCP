using AllyContratista.ViewModel.Administrador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyContratista.View.Administrador
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdministradorView : TabbedPage
    {
        public AdministradorView()
        {
            InitializeComponent();
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            int index = Children.IndexOf(CurrentPage);

            if (index == 1)
            {
                usuarioPage.BindingContext = new UsuarioViewModel();
            }else if (index == 2)
            {
                serviciosPage.BindingContext = new ServiciosViewModel();
            }
        }
    }
}