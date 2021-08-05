using AllyCliente.Interface;
using AllyCliente.Model;
using AllyCliente.Model.SubModel;
using AllyCliente.Service;
using AllyCliente.View;
using LaavorRatingConception;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyCliente.ViewModel
{   
    public class EvaluacionViewModel:EvaluacionModel
    {
        #region Instancias
        readonly OrdenesServicioService ordenService= new OrdenesServicioService();
        readonly EvaluacionService servicio = new EvaluacionService();
        readonly IMessageService messageService;
        #endregion

        #region Atributos
        private int nota;
        public int Nota
        {
            get { return nota; }
            set { nota = value; }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        private string btnBack;
        public string BtnBack
        {
            get { return btnBack; }
            set { btnBack = value; OnPropertyChanged(); }
        }


        #endregion

        #region Comandos
        public Command CalificarServicioCommand { get; set; }
        #endregion

        #region Constructor
        public EvaluacionViewModel(OrdenesServicioModel ordenServicio)
        {
            BtnBack = "true";
            messageService = DependencyService.Get<IMessageService>();
            this.IdServicio = ordenServicio.IdServicio;
            CalificarServicioCommand = new Command(async () => await CalificarServicio(ordenServicio));
            Task.Run(()=>CargarDatos());
        }
        #endregion

        #region Ejecutar Comandos
        //Cargar identificador de la evaluacion
        private async Task CargarDatos()
        {
            try
            {
                this.IdEvaluacion = await servicio.ConsultarEvaluacion(IdServicio);
            }
            catch (Exception)
            {
                messageService.ShortAlert(servicio.MensajeError);
            }
        }

        //Ejectar instruccion para calificar el servicio
        private async Task CalificarServicio(OrdenesServicioModel ordenServicio)
        {
            IsBusy = true;
            try
            {
                NotasModel notaModel = new NotasModel
                {
                    IdUsuario = Preferences.Get("Uid", null),
                    Nota = this.Nota
                };
                await servicio.CalificarServicio(this.IdEvaluacion, notaModel);
                ordenServicio.Calificado = "true";
                await ordenService.ActualizarEstado(ordenServicio);
                messageService.ShortAlert("Servcio calificado");
                await Task.Delay(1000);
                BtnBack = "false";

            }
            catch (Exception)
            {
                messageService.ShortAlert(servicio.MensajeError);
            }
            IsBusy = false;
        }
        #endregion
    }
}
