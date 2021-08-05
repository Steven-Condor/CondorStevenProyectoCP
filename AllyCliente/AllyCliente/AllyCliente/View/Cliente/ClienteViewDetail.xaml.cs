using AllyCliente.ViewModel.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyCliente.View.Cliente
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClienteViewDetail : TabbedPage
    {
        public ClienteViewDetail()
        {
            InitializeComponent();
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            int index = Children.IndexOf(CurrentPage);

            if (index == 1)
            {
                OrdenesPage.BindingContext = new ListaOrdenesServicioViewModel();
            }else if(index == 2)
            {
                agendaPage.BindingContext = new AgendaViewModel();
            }
        }
    }
}