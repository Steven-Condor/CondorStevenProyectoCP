using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Service;
using AllyContratista.View.Contratista;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyContratista.ViewModel.Contratista
{
    public class EditarServicioViewModel:ServicioModel
    {
        #region Instancias
        readonly ServicioService servicio = new ServicioService();
        readonly EvaluacionService evaluacion = new EvaluacionService();
        readonly TipoServicioService tipoServicioService = new TipoServicioService();
        readonly IMessageService messageService;
        #endregion

        #region Parametros
        public Command NuevoServicioCommand { get; set; }
        public Command ActualizarServicioCommand { get; set; }
        public Command EliminarServicioCommand { get; set; }

        private List<TipoServicioModel> listaTipoServicios;

        public List<TipoServicioModel> ListaTipoServicios
        {
            get { return listaTipoServicios; }
            set { listaTipoServicios = value; OnPropertyChanged(); }
        }

        private TipoServicioModel seleccionarTipo;
        public TipoServicioModel SeleccionarTipo
        {
            get { return seleccionarTipo; }
            set
            {
                if (seleccionarTipo != value)
                {
                    seleccionarTipo = value;
                    OnPropertyChanged();
                }
                else
                {
                    seleccionarTipo = null;
                }
            }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }


        #endregion

        #region Constructor
        public EditarServicioViewModel(ServicioModel servicio)
        {
            Task.Run(()=>LlenarDatos(servicio));
            messageService = DependencyService.Get<IMessageService>();
            NuevoServicioCommand = new Command(async () => await NuevoServicio());
            ActualizarServicioCommand = new Command(async () => await ActualizarServicio());
            Task.Run(() => this.CargarTipoServicio());
        }
        #endregion

        #region Ejecutar Comandos
        public async Task NuevoServicio()
        {
            IsBusy = true;
            var respuesta = await App.Current.MainPage.DisplayAlert("Alerta", "Esta seguro que desea ingresar este servicio", "Aceptar", "Cancelar");
            if (respuesta)
            {
                var idContratista = Preferences.Get("Uid", null);
                //Nuevo modelo de datos del servicio
                ServicioModel servicioModel = new ServicioModel()
                {
                    Precio = this.Precio,
                    TituloServicio = this.TituloServicio,
                    Descripcion = this.Descripcion,
                    TipoServicio = seleccionarTipo.TipoServicio,
                    IdServicio = servicio.GenerarId(),
                    Estado = true,
                    IdContratista = idContratista,
                    FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    Apoyo = null
                };
                //Modelo de dtaos de Evaluación
                EvaluacionModel evaluacionModel = new EvaluacionModel()
                {
                    IdEvaluacion = evaluacion.GenerarId(),
                    IdServicio = servicioModel.IdServicio,
                    FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                };

                messageService.LongAlert("Servicio Insertado");
                //llamar al método de regirtros de servicios y enviar el modelo de serivicio y evaluación
                await servicio.NuevoServicio(servicioModel, evaluacionModel);
                await Task.Delay(4000);
            }
            else
            {
                await Task.Delay(4000);
            }
            IsBusy = false;
        }

        //Método de actualizacion de servicio
        public async Task ActualizarServicio()
        {
            IsBusy = true;
            if (SeleccionarTipo == null)
            {
                ServicioModel modelo = new ServicioModel()
                {
                    Precio = this.Precio,
                    TituloServicio = this.TituloServicio,
                    Descripcion = this.Descripcion,
                    TipoServicio = this.TipoServicio,
                    IdServicio = this.IdServicio,
                    Estado = this.Estado,
                    IdContratista = this.IdContratista,
                    FechaCreacion = this.FechaCreacion
                };
                //llamara al metodo de actualizacion enivando el modelo con los nuevos datos 
                await servicio.ActualizarServicio(modelo);
                await Task.Delay(1000);
            }
            else
            {
                ServicioModel modelo = new ServicioModel()
                {
                    Precio = this.Precio,
                    TituloServicio = this.TituloServicio,
                    Descripcion = this.Descripcion,
                    TipoServicio = SeleccionarTipo.TipoServicio,
                    IdServicio = this.IdServicio,
                    Estado = this.Estado,
                    IdContratista = this.IdContratista,
                    FechaCreacion = this.FechaCreacion
                };
                await servicio.ActualizarServicio(modelo);
                await Task.Delay(1000);
            }
            messageService.ShortAlert("Servicio Actualizado");
            IsBusy = false;
        }
        //Cragar el modelo de dtaos heredado con los datos extraidos de la vista anterior
        public async Task LlenarDatos(ServicioModel servicio)
        {
            if (servicio.IdServicio != null)
            {
                this.Precio = servicio.Precio;
                SeleccionarTipo = await tipoServicioService.BuscarTipoServicio(servicio.TipoServicio);
                this.TituloServicio = servicio.TituloServicio;
                this.IdServicio = servicio.IdServicio;
                this.Descripcion = servicio.Descripcion;
                this.TipoServicio = servicio.TipoServicio;
                this.Estado = servicio.Estado;
                this.IdContratista = servicio.IdContratista;
                this.FechaCreacion = servicio.FechaCreacion;
            }

        }

        public async Task CargarTipoServicio()
        {
            try
            {
                //Obtener la lista de las categorias
                var lista = await tipoServicioService.ConsultarTiposServicios();
                ListaTipoServicios = lista;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", tipoServicioService.MensajeError + ": " + ex.Message, "Ok");
            }
        }
        #endregion
    }
}
