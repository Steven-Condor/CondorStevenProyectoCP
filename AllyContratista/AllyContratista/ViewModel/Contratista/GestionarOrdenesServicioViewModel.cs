using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Model.Consultas;
using AllyContratista.Model.SubModel;
using AllyContratista.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.OpenWhatsApp;

namespace AllyContratista.ViewModel.Contratista
{
    class GestionarOrdenesServicioViewModel :  VisibleModel
    {
        #region Instancias

        readonly UsuarioService usuarioService = new UsuarioService();
        readonly OrdenServicioService servicio = new OrdenServicioService();
        readonly ServicioService servicioService = new ServicioService();
        readonly IMessageService messageSerive;
        #endregion

        #region parametros


        //Llenar los datos del nuevo modelo orden de servicio
        private OrdenServicioModel ordenServicio;
        public OrdenServicioModel OrdenServicio
        {
            get { return ordenServicio; }
            set { ordenServicio = value; OnPropertyChanged(); }
        }

        private UsuarioModel informacionUsuario;
        public UsuarioModel InformacionUsuario
        {
            get { return informacionUsuario; }
            set { informacionUsuario = value; OnPropertyChanged(); }
        }

        private ServicioModel informacionServicio;
        public ServicioModel InformacionServicio
        {
            get { return informacionServicio; }
            set { informacionServicio = value; OnPropertyChanged(); }
        }

        public string MinDate { get; set; }
        public string MaxDate { get; set; }

        #region Definir Comandos
        public Command VerPickerCommand { get; set; }
        public Command AcpetarOrdenCommand { get; set; }
        public Command AgendarOrdenCommand { get; set; }
        public Command FinalzarOrdenCommand { get; set; }
        public Command CancelarOrdenCommand { get; set; }
        public Command HabilitarOrdenCommand { get; set; }
        public Command AbrirWhatsappCommand { get; set; }
        #endregion

        #endregion

        #region Constructor
        public GestionarOrdenesServicioViewModel(string _ordenServicio)
        {
            messageSerive = DependencyService.Get<IMessageService>();
            CargarDatos(_ordenServicio);
            Task.Run(() => CargarUsuario());
            Task.Run(()=>CargarInformacioServicio());
            InicializarComandos();
            OcultarBotonoes();
            ValidarFechas();
        }
        #endregion

        #region Ejecutar Commandos

        #region Iniciarlizar comandos

        private void InicializarComandos()
        {
            VerPickerCommand = new Command(async () => await VerPicker());
            AcpetarOrdenCommand = new Command(async () => await AcpetarOrden());
            AgendarOrdenCommand = new Command(async () => await AgendarOrden());
            FinalzarOrdenCommand = new Command(async () => await FinalzarOrden());
            CancelarOrdenCommand = new Command(async () => await CancelarOrden());
            HabilitarOrdenCommand = new Command(async () => await HabilitarOrden());
            AbrirWhatsappCommand = new Command(async () => await AbrirWhatsapp());
        }

        #endregion

        #region Abrir Whathsapp

        public async Task AbrirWhatsapp()
        {
            try
            {
                Chat.Open("593" + InformacionUsuario.Telefono, "Estimad@, reciba un cordial saludo.");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", "No se pudo abrir Whatsapp: " + ex.Message, "Ok");
            }
        }

        #endregion

        #region Commando Aceptar Orden
        //Aceptar las ordenes solicitadas por los clientes
        private async Task AcpetarOrden()
        {
            IsBusy = true;
            try
            {
                var pregunta = await App.Current.MainPage.DisplayAlert("Alerta", "¿Seguro desea Aceptar el servicio?", "Si", "No");
                if (pregunta)
                {
                    //Actualizar atributos del modelo de datos
                    var ordenServicio = OrdenServicio;
                    ordenServicio.Fechas.FechaAceptacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    ordenServicio.Fechas.FechaAgendada = null;
                    ordenServicio.Estado = "Aceptado";
                    //Llamar al método actualizar
                    await servicio.ActualizarEstado(ordenServicio);
                    //Recargar el mdoelo de datos y validar los botones 
                    this.RecargarDatos();
                    FechaAceptacionVisible = true;
                    FechaAgendadaVisible = true;
                    BtnAceptar = "false";
                    PickerVisible = "true";
                    BtnAgendar = "true";
                    //Notificacion Toast
                    messageSerive.ShortAlert("Orden Aceptada");
                }
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Ok", servicio.MensajeError, "Ok");
            }
            IsBusy = false;
        }

        #endregion

        #region Agendar Orden
        //Gandar el servicio, despues de seleccionar una fecha en el DatePicker
        public async Task AgendarOrden()
        {
            IsBusy = true;
            try
            {
                //Validar las fechas
                CultureInfo provider = new CultureInfo("en-US");
                string fechaTemp = Fecha.ToString("dd/MM/yyyy");

                if(OrdenServicio.Fechas.FechaAgendada != fechaTemp && OrdenServicio.Fechas.FechaAgendada != null)
                    Fecha = Convert.ToDateTime(OrdenServicio.Fechas.FechaAgendada, provider);
                else
                    Fecha = Convert.ToDateTime(fechaTemp);

                //Actualizar la infromacion del modelo    
                string fechaAgendada = Fecha.ToString("dd/MM/yyyy") + " " + Hora;
                var ordenServicio = OrdenServicio;
                ordenServicio.Estado = "Agendado";
                ordenServicio.Fechas.FechaAgendada = Convert.ToDateTime(fechaAgendada).ToString("dd/MM/yyyy HH:mm");

                //Llamar al metodo de actualización y enviar los nuevos atributos
                await servicio.ActualizarEstado(ordenServicio);

                //Validar los botones y recaragar el modelo de datos
                PickerVisible = "false";
                LblFechaAgendada = "true";
                this.RecargarDatos();
                BtnFinalizar = "true";
                BtnAgendar = "false";

                //Notificacion
                messageSerive.ShortAlert("Orden Agendada");
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Ok", servicio.MensajeError, "Ok");
            }
            IsBusy = false;
        }

        #endregion

        #region Finalizar Orden
        //Finalizar la orden de servicio, despues de una validación
        private async Task FinalzarOrden()
        {
            IsBusy = true;
            try
            {
                DateTime fechaTemp = Convert.ToDateTime(OrdenServicio.Fechas.FechaAgendada);
                DateTime fechaHoy = DateTime.Now;
                //Validar si la fecha agendada se ha cumplido 
                if (fechaTemp < fechaHoy)
                {
                    var pregunta = await App.Current.MainPage.DisplayAlert("Alerta", "¿Seguro desea Finalizar el servicio?", "Si", "No");
                    if (pregunta)
                    {
                        string pago = await App.Current.MainPage.DisplayPromptAsync("Aviso", "Ingrese La cantidad a cobrar por su servicio: ");
                        //Validar si se ha ingresado el pago total del servicio
                        if(pago != null)
                        {
                            var ordenServicio = OrdenServicio;
                            ordenServicio.Estado = "Finalizado";
                            ordenServicio.Fechas.FechaFinalizacion = fechaHoy.ToString("dd/MM/yyyy HH:mm");
                            ordenServicio.Pago = Convert.ToDouble(pago);

                            await servicio.ActualizarEstado(ordenServicio);

                            this.RecargarDatos();
                            messageSerive.ShortAlert("Orden Finalizada");
                            FechaFinalizacionVisible = true;
                            FechaAceptacionVisible = true;
                            FechaAgendadaVisible = true;
                            BtnFinalizar = "false";
                            BtnCancelar = "false";
                        }                        
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Aviso", "Aun no se ha cumplido la tarea", "Ok");
                }
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Ok", servicio.MensajeError, "Ok");
            }
            IsBusy = false;
        }

        #endregion

        #region Cancelar Orden
        //Cancelar la orden del servicio unilateralmente, despues de una validación
        public async Task CancelarOrden()
        {
            IsBusy = true;
            try
            {
                var pregunta = await App.Current.MainPage.DisplayAlert("Alerta", "¿Seguro que desea Cancelar el servicio?", "Si", "No");
                if (pregunta)
                {
                    var ordenServicio = OrdenServicio;
                    ordenServicio.Estado = "Cancelado";
                    ordenServicio.Fechas.FechaCancelacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    ordenServicio.Fechas.FechaAceptacion = null;
                    ordenServicio.Fechas.FechaAgendada = null;

                    await servicio.ActualizarEstado(ordenServicio);

                    FechaCancelacionVisible = false;
                    this.RecargarDatos();
                    messageSerive.ShortAlert("Orden Cancelada");
                    BtnAceptar = "false";
                    BtnHabilitar = "true";
                    BtnFinalizar = "false";
                    BtnCancelar = "false";
                    BtnAgendar = "false";
                }
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Ok", servicio.MensajeError, "Ok");
            }
            IsBusy = false;
        }

        #endregion

        #region Habilitar servicio
        //Hbilitar los servicios cacelados
        public async Task HabilitarOrden()
        {
            IsBusy = true;
            try
            {
                var pregunta = await App.Current.MainPage.DisplayAlert("Alerta", "¿Seguro que desea Habilitar el servicio?", "Si", "No");
                if (pregunta)
                {
                    OrdenServicioModel ordenServicio = OrdenServicio;
                    ordenServicio.Estado = "Solicitado";
                    ordenServicio.Fechas = new FechasModel
                    {
                        FechaAceptacion = null,
                        FechaAgendada = null,
                        FechaCancelacion = null,
                        FechaCreacion = OrdenServicio.Fechas.FechaCreacion,
                        FechaFinalizacion = null,
                    };

                    await Task.Run(() => servicio.ActualizarEstado(ordenServicio));

                    FechaCancelacionVisible = false;
                    this.RecargarDatos();
                    messageSerive.ShortAlert("Orden Habilitada");
                    BtnAceptar = "true";
                    BtnFinalizar = "false";
                    BtnCancelar = "true";
                    BtnAgendar = "false";
                    BtnHabilitar = "false";
                }
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Ok", servicio.MensajeError, "Ok");
            }
            IsBusy = false;
        }

        private async Task VerPicker()
        {
            IsBusy = true;
            if (PickerVisible == "true")
            {
                PickerVisible = "false";
                LblFechaAgendada = "true";
            }
            else
            {
                PickerVisible = "true";
                LblFechaAgendada = "false";
            }
            
            await Task.Delay(1000);
            IsBusy = false;
        }

        #endregion

        #endregion

        #region Cargar datos 
        //Cargar los datos de la orden de servicio
        public void CargarDatos(string idOrdenServicio)
        {
            var orden = Task.Run(() => servicio.BuscarOrden(idOrdenServicio));
            OrdenServicio = orden.Result;

        }

        //Realizar un refresh al modelo Orden de Servicio
        public void RecargarDatos()
        {
            var ordenServicio = Task.Run(()=>servicio.BuscarOrden(OrdenServicio.IdOrden));
            OrdenServicio = ordenServicio.Result;
        }

        //Cargar el modelo Usuario
        public async Task CargarUsuario()
        {
            var usuario = await usuarioService.BuscarUsuario(OrdenServicio.IdCliente);
            if (usuario != null)
                InformacionUsuario = usuario;
            else
                messageSerive.ShortAlert(usuarioService.MensajeError);
        }

        //Caragr el modelo de servicio
        public async Task CargarInformacioServicio()
        {
            var servicio = await servicioService.BuscarServicioId(OrdenServicio.IdServicio);
            if (servicio != null)
                InformacionServicio = servicio;
            else
                messageSerive.ShortAlert(servicioService.MensajeError);
        }

        #endregion

        #region Validaciones

        //Validar fechas minimas y maximas del Date Picker
        public void ValidarFechas()
        {
            MinDate = DateTime.Now.ToString("MM/dd/yyyy");
            MaxDate = DateTime.Now.AddYears(1).ToString("MM/dd/yyyy");
        }

        //Validar botones activos
        private void OcultarBotonoes()
        {
            FechaAceptacionVisible = false;
            FechaFinalizacionVisible = false;
            FechaAgendadaVisible = false;
            FechaCancelacionVisible = false;
            BtnCancelar = "true";
            PickerVisible = "false";
            LblFechaAgendada = "false";

            ValidacionFechaAgendada();

            if (OrdenServicio.Estado == "Solicitado")
            {
                BtnAceptar = "true";
                BtnAgendar = "false";
                BtnFinalizar = "false";
                BtnHabilitar = "false";
            }
            else if (OrdenServicio.Estado == "Aceptado")
            {
                BtnAceptar = "false";
                BtnAgendar = "true";
                BtnFinalizar = "false";
                BtnHabilitar = "false";

                FechaAceptacionVisible = true;
                FechaAgendadaVisible = true;
                FechaFinalizacionVisible = false;
                FechaCancelacionVisible = false;
                PickerVisible = "true";
                LblFechaAgendada = "false";

            }
            else if(OrdenServicio.Estado == "Agendado")
            {
                BtnAceptar = "false";
                BtnAgendar = "false";
                BtnFinalizar = "true";
                BtnHabilitar = "false";
                FechaCancelacionVisible = false;
                FechaAceptacionVisible = true;
                FechaAgendadaVisible = true;
                FechaFinalizacionVisible = false;
                LblFechaAgendada = "true";
            }
            else if(OrdenServicio.Estado == "Finalizado")
            {
                BtnAceptar = "false";
                BtnAgendar = "false";
                BtnFinalizar = "false";
                BtnCancelar = "false";
                BtnHabilitar = "false";
                FechaCancelacionVisible = false;
                FechaAceptacionVisible = true;
                FechaAgendadaVisible = true;
                FechaFinalizacionVisible = true;
                LblFechaAgendada = "true";
            }
            else if (OrdenServicio.Estado == "Cancelado")
            {
                BtnAceptar = "false";
                BtnAgendar = "false";
                BtnFinalizar = "false";
                BtnCancelar = "false";
                BtnHabilitar = "true";
                FechaCancelacionVisible = true;
                FechaAceptacionVisible = false;
                FechaAgendadaVisible = false;
                FechaFinalizacionVisible = false;
            }
        }

        //Llenar en campo fecha Agendada con alores por defecto
        public void ValidacionFechaAgendada()
        {
            if (OrdenServicio.Fechas.FechaAgendada == null)
            {
                OrdenServicio.Fechas.FechaAgendada = DateTime.Now.ToString("dd/MM/yyyy");
                var horaTexto = DateTime.Now;
                Fecha = Convert.ToDateTime(OrdenServicio.Fechas.FechaAgendada);
                Hora = new TimeSpan(horaTexto.Hour, horaTexto.Minute, horaTexto.Second);
            }
            else
            {
                var fechaTemp = Convert.ToDateTime(OrdenServicio.Fechas.FechaAgendada);
                Fecha = Convert.ToDateTime(OrdenServicio.Fechas.FechaAgendada);
                Hora = new TimeSpan(fechaTemp.Hour, fechaTemp.Minute, fechaTemp.Second);
            }
        }

        #endregion
    }
}
