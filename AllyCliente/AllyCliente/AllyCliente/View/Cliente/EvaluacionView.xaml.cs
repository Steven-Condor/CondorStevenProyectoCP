using AllyCliente.Model;
using AllyCliente.ViewModel;
using LaavorRatingConception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyCliente.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EvaluacionView : ContentPage
    {

        public EvaluacionView(OrdenesServicioModel ordenServicio)
        {
            EvaluacionViewModel contexto = new EvaluacionViewModel(ordenServicio);
            InitializeComponent();
            BindingContext = contexto;
        }

        private void RatingConception_Voted(object sender, EventArgs e)
        {
            RatingConception rating = (RatingConception)sender;
            txtNota.Text = rating.Value.ToString();
        }
    }
}