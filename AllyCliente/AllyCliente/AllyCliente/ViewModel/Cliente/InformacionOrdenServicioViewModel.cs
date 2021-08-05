using AllyCliente.Interface;
using AllyCliente.Model;
using AllyCliente.Model.SubModel;
using AllyCliente.Service;
using AllyCliente.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.OpenWhatsApp;

namespace AllyCliente.ViewModel.Cliente
{

    public class InformacionOrdenServicioViewModel:OrdenesServicioModel
    {
        #region Instancias
        readonly OrdenesServicioService ordenService = new OrdenesServicioService();
        readonly ServiciosService servicioService = new ServiciosService();
        readonly UsuarioService usuarioService = new UsuarioService();
        readonly IMessageService messageService;
        #endregion

        #region Comandos
        public Command CancelarServicioCommand { get; set; }
        public Command AbrirWhatsappCommand { get; set; }
        public Command CalificarServicioCommand { get; set; }

        #endregion

        #region Parametros

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        private bool btnCancelar;
        public bool BtnCancelar
        {
            get { return btnCancelar; }
            set { btnCancelar = value; OnPropertyChanged(); }
        }

        private UsuarioModel informacionUsuario;
        public UsuarioModel InformacionUsuario
        {
            get { return informacionUsuario; }
            set { informacionUsuario = value; OnPropertyChanged(); }
        }

        private ServiciosModel informacionServicio;
        public ServiciosModel InformacionServicio
        {
            get { return informacionServicio; }
            set { informacionServicio = value; OnPropertyChanged(); }
        }

        private bool btnCalificar;
        public bool BtnCalificar
        {
            get { return btnCalificar; }
            set { btnCalificar = value; OnPropertyChanged(); }
        }

        private string fechaCancelacionVisible;
        public string FechaCancelacionVisible
        {
            get { return fechaCancelacionVisible; }
            set { fechaCancelacionVisible = value; OnPropertyChanged(); }
        }

        private string fechaAceptacionVisible;
        public string FechaAceptacionVisible
        {
            get { return fechaAceptacionVisible; }
            set { fechaAceptacionVisible = value; OnPropertyChanged(); }
        }

        private string fechaAgendadaVisible;
        public string FechaAgendadaVisible
        {
            get { return fechaAgendadaVisible; }
            set { fechaAgendadaVisible = value; OnPropertyChanged(); }
        }

        private bool fechaFinalizacionVisible;
        public bool FechaFinalizacionVisible
        {
            get { return fechaFinalizacionVisible; }
            set { fechaFinalizacionVisible = value; OnPropertyChanged(); }
        }

        #endregion

        #region Constructor
        public InformacionOrdenServicioViewModel(OrdenesServicioModel ordenServicio)
        {
            messageService = DependencyService.Get<IMessageService>();
            Task.Run(()=>CargarDatos(ordenServicio));
            AbrirWhatsappCommand = new Command( () => AbrirWhatsapp());
            CalificarServicioCommand = new Command(async () => await CalificarServicio());
            CancelarServicioCommand = new Command(async () => await CancelarServicio());
        }

        #endregion

        #region Ejecutar Comandos

        //Abrir el formulario de calificacion
        private async Task CalificarServicio()
        {
            IsBusy = true;
            var modelo = OrdenModelo();
            await App.Current.MainPage.Navigation.PushModalAsync(new EvaluacionView(modelo));
            IsBusy = false;
        }

        //Caragar la infromación de la orden en un nuevo modelo
        private OrdenesServicioModel OrdenModelo()
        {
            OrdenesServicioModel ordenServicio = new OrdenesServicioModel()
            {
                IdServicio = this.IdServicio,
                IdCliente = this.IdCliente,
                IdOrden = this.IdOrden,
                Estado = this.Estado,
                Pago = this.Pago,
                Fechas = new FechasModel
                {
                    FechaCreacion = this.Fechas.FechaCreacion,
                    FechaAceptacion = this.Fechas.FechaAceptacion,
                    FechaAgendada = this.Fechas.FechaAgendada,
                    FechaCancelacion = this.Fechas.FechaCancelacion,
                    FechaFinalizacion = this.Fechas.FechaFinalizacion
                },
            };
            return ordenServicio;
        }

        //Abri la aplicación Whtasapp con el numero del contratista
        private void AbrirWhatsapp()
        {
            IsBusy = true;
            try
            {
                Chat.Open("593"+InformacionUsuario.Telefono, "Estimado, reciba un cordial saludo, le escribo por: ");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                messageService.ShortAlert("No se ha podido ejecutar whatsapp");
            }
            IsBusy = false;
        }


        //Actualizar estado de la orden de servicio a Cancelado y actualizacion ded fechas
        private async Task CancelarServicio()
        {
            IsBusy = true;
            try
            {
                var pregunta = await App.Current.MainPage.DisplayAlert("Alerta", "¿Esta seguro que desea Cancelar la Orden?", "Aceptar", "Cancelar");
                if (pregunta)
                {
                    var ordenServicio = OrdenModelo();
                    ordenServicio.Estado = "Cancelado";
                    ordenServicio.Fechas.FechaCancelacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    ordenServicio.Fechas.FechaAceptacion = null;
                    ordenServicio.Fechas.FechaAgendada = null;
                    await ordenService.ActualizarEstado(ordenServicio);
                    await Task.Run(()=>RecargarDatos());
                    BtnCancelar = false;
                    messageService.ShortAlert("Orden Cancelada");
                    await Task.Delay(2000);
                }
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", ordenService.MensajeError, "Ok");
            }
            IsBusy = false;
        }

        #endregion

        #region Caragar Datos
        //Cargar los datos en el modelo para visulizarlos en la vista
        private async Task CargarDatos(OrdenesServicioModel ordenServicio)
        {
            BtnCancelar = true;
            this.IdServicio = ordenServicio.IdServicio;
            this.IdCliente = ordenServicio.IdCliente;
            this.IdOrden = ordenServicio.IdOrden;
            this.Pago = ordenServicio.Pago;
            this.Estado = ordenServicio.Estado;
            this.Fechas = new FechasModel
            {
                FechaCreacion = ordenServicio.Fechas.FechaCreacion,
                FechaAceptacion = ordenServicio.Fechas.FechaAceptacion,
                FechaAgendada = ordenServicio.Fechas.FechaAgendada,
                FechaFinalizacion = ordenServicio.Fechas.FechaFinalizacion,
                FechaCancelacion = ordenServicio.Fechas.FechaCancelacion
            };
            this.Calificado = ordenServicio.Calificado;
            await CargarInformacioServicio();
            this.OcultarBotonoes();
        }

        //Actuaizar los datos en el modelo para visulizarlos en las vista
        private async Task RecargarDatos()
        {
            OrdenesServicioModel ordenServicioModel = await ordenService.BuscarOrden(this.IdOrden);
            this.IdOrden = ordenServicioModel.IdOrden;
            this.Estado = ordenServicioModel.Estado;
            this.Fechas = new FechasModel
            {
                FechaAceptacion = ordenServicioModel.Fechas.FechaAceptacion,
                FechaAgendada = ordenServicioModel.Fechas.FechaAgendada,
                FechaCancelacion = ordenServicioModel.Fechas.FechaCancelacion,
                FechaCreacion = ordenServicioModel.Fechas.FechaCreacion,
                FechaFinalizacion = ordenServicioModel.Fechas.FechaFinalizacion,
            };
        }

        //Cargar los datos del usuario Contratita
        private async Task CargarUsuario(string idContratista)
        {
            var usuario = await usuarioService.BuscarUsuario(idContratista);
            if (usuario != null)
                InformacionUsuario = usuario;
            else
                messageService.ShortAlert(usuarioService.MensajeError);
        }

        //Cargar los datos del Servicio
        private async Task CargarInformacioServicio()
        {
            var servicio = await servicioService.BuscarServicioId(this.IdServicio);
            if(servicio != null)
            {
                InformacionServicio = servicio;
                await CargarUsuario(InformacionServicio.IdContratista);
            }
            else
            {
                messageService.ShortAlert(servicioService.MensajeError);
            }
        }

        #endregion

        #region Validaciones
        //Validar los botones en función de los estados de la orden
        private void OcultarBotonoes()
        {
            BtnCancelar = true;
            BtnCalificar = false;
            FechaAceptacionVisible = "false";
            FechaAgendadaVisible = "false";
            FechaFinalizacionVisible = false;
            FechaCancelacionVisible = "false";

            if (Estado == "Solicitado")
            {
                BtnCancelar = true;
            }
            else if (Estado == "Aceptado")
            {
                BtnCancelar = true;
                FechaAceptacionVisible = "true";
                FechaAgendadaVisible = "true";
                FechaFinalizacionVisible = false;
                FechaCancelacionVisible = "false";

            }
            else if (Estado == "Agendado")
            {
                BtnCancelar = true;
                FechaCancelacionVisible = "false";
                FechaAceptacionVisible = "true";
                FechaAgendadaVisible = "true";
                FechaFinalizacionVisible = false;

            }
            else if (Estado == "Finalizado")
            {
                BtnCancelar = false;
                FechaCancelacionVisible = "false";
                FechaAceptacionVisible = "true";
                FechaAgendadaVisible = "true";
                FechaFinalizacionVisible = true;
                if (Calificado != null)
                    BtnCalificar = false;
                else
                    BtnCalificar = true;
            }
            else if (Estado == "Cancelado")
            {
                BtnCancelar = false;
                FechaCancelacionVisible = "true";
                FechaAceptacionVisible = "false";
                FechaAgendadaVisible = "false";
                FechaFinalizacionVisible = false;
            }
        }

        #endregion   

    }
}
