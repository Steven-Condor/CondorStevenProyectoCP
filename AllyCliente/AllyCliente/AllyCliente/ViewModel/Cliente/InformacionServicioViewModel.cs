using AllyCliente.Interface;
using AllyCliente.Model;
using AllyCliente.Model.SubModel;
using AllyCliente.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.OpenWhatsApp;

namespace AllyCliente.ViewModel.Cliente
{
    public class InformacionServicioViewModel: ServiciosModel
    {
        #region Instancias

        OrdenesServicioService ordenService = new OrdenesServicioService();
        UsuarioService usuarioService = new UsuarioService();
        ServiciosService serviciosService = new ServiciosService();
        readonly IMessageService messageService;

        #endregion

        #region Parametros
        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; OnPropertyChanged(); }
        }


        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        private UsuarioModel contratistaModel;
        public UsuarioModel ContratistaModel
        {
            get { return contratistaModel; }
            set { contratistaModel = value; OnPropertyChanged(); }
        }

        public Command SolicitarServicioCommand { get; set; }
        public Command AbrirWhatsappCommand { get; set; }

        #endregion

        #region Constructor

        public InformacionServicioViewModel(string idServicio)
        {
            messageService = DependencyService.Get<IMessageService>();
            Task.Run(()=>CargarDatos(idServicio));  
            SolicitarServicioCommand = new Command(async () => await SolicitarServicio());
            AbrirWhatsappCommand = new Command(async () => await AbrirWhatsapp());
        }

        #endregion

        #region Ejecutar Comandos

        public async Task AbrirWhatsapp()
        {
            IsBusy = true;
            try
            {
                Chat.Open("593"+contratistaModel.Telefono, "Estimad@, reciba un cordial saludo, le escribo para solicitar ");
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                messageService.ShortAlert("No se pudo abrir Whatsapp");
                Console.WriteLine("Error: "+ex.Message.ToString());
            }
            isBusy = false;
        }

        private async Task SolicitarServicio()
        {
            IsBusy = true;
            try
            {
                var respuesta = await App.Current.MainPage.DisplayAlert("Confirmación", "¿Seguro desea solicitar el servicio?", "Aceptar", "Cancelar");
                if (respuesta)
                {
                    string idUsuario = Preferences.Get("Uid", null);
                    OrdenesServicioModel ordenServicio = new OrdenesServicioModel()
                    {
                        IdCliente = idUsuario,
                        Estado = "Solicitado",
                        IdOrden = ordenService.GenerarIdOrden(),
                        IdServicio = this.IdServicio,
                        Pago = 0,
                        Fechas = new FechasModel
                        {
                            FechaCancelacion =null,
                            FechaAceptacion = null,
                            FechaAgendada = null,
                            FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                            FechaFinalizacion = null
                        }
                    };
                    IsVisible = false;
                    await Task.Run(() => ordenService.CrearOrdenServicio(ordenServicio));
                    messageService.ShortAlert("Servicio Solicitado");
                    await Task.Delay(2000);
                }
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Ok", ordenService.MensajeError, "Ok");
            }
            IsBusy = false;
        }

        #endregion

        #region Cargar Datos

        private async Task CargarDatos(string idServicio)
        {
            try
            {
                var servicio = await serviciosService.BuscarServicioId(idServicio);
                this.Descripcion = servicio.Descripcion;
                this.IdContratista = servicio.IdContratista;
                this.IdServicio = servicio.IdServicio;
                this.FechaCreacion = servicio.FechaCreacion;
                this.Estado = servicio.Estado;
                this.TituloServicio = servicio.TituloServicio;
                this.Precio = servicio.Precio;
                this.TipoServicio = servicio.TipoServicio;
                this.Apoyo = servicio.Apoyo;
                ContratistaModel = await usuarioService.BuscarUsuario(this.IdContratista);
                IsVisible = true;
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Ok", ordenService.MensajeError, "alerta");
            }
        }
        #endregion
    }
}
