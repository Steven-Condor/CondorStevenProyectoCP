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
    public partial class ContratistaViewDetail : TabbedPage
    {
        public ContratistaViewDetail()
        {
            InitializeComponent();
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            int index = Children.IndexOf(CurrentPage);

            if (index == 0)
            {
                serviciosPage.BindingContext = new ContratistaViewModel();
            }
            else if (index == 1)
            {
                serviciosContadorPage.BindingContext = new AdministrarServicioViewModel();
            }else if(index == 2)
            {
                serviciosPage.BindingContext = new AgendaViewModel();
            }
        }
    }
}