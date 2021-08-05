using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Service;
using AllyContratista.View.Contratista.Editar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AllyContratista.ViewModel.Contratista
{
    public class ContratistaViewModel : ServicioModel
    {

        #region Instancias
        readonly ServicioService servicioService = new ServicioService();
        readonly IMessageService messageService;
        #endregion

        #region Listas
        private ObservableCollection<ServicioModel> listViewSource;
        public ObservableCollection<ServicioModel> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ServicioModel> TempListViewSource;

        #endregion

        #region Parametros
        private bool itemVisible;
        public bool ItemVisible
        {
            get { return itemVisible; }
            set { itemVisible = value; OnPropertyChanged(); }
        }

        private bool isRefresh;
        public bool IsRefresh
        {
            get { return isRefresh; }
            set { isRefresh = value; OnPropertyChanged(); }
        }

        #endregion

        #region Comandos
        public Command FrmNuevoServicioCommand { get; set; }
        public Command FrmActualizarServicioCommand { get; set; }
        public Command EliminarServicioCommand { get; set; }
        public Command DesactivarServicioCommand { get; set; }
        public Command RefreshCommand { get; set; }
        public Command ActivarServicioCommand { get; set; }
        public Command BuscarCommand { get; set; }
        public Command FiltrarCommand { get; set; }
        #endregion

        #region Constructor
        public ContratistaViewModel()
        {
            Task.Run(() => this.CargarDatos());
            InicializarComandos();
            messageService = DependencyService.Get<IMessageService>();
        }
        #endregion

        
        private void InicializarComandos()
        {
            FrmNuevoServicioCommand = new Command(async () => await FrmNuevoServicio());
            FrmActualizarServicioCommand = new Command<ServicioModel>(async (s) => await FrmActualizarServicio(s));
            RefreshCommand = new Command(async () => await RecargarDatos());
            DesactivarServicioCommand = new Command<ServicioModel>(async (m) => await DesactivarServicio(m));
            EliminarServicioCommand = new Command<ServicioModel>(async (m) => await EliminarServicio(m));
            ActivarServicioCommand = new Command<ServicioModel>(async (m) => await ActivarServicio(m));
            BuscarCommand = new Command<string>(async (cadena) => await Buscar(cadena));
            FiltrarCommand = new Command(async () => await Filtar());
        }

        #region Ejecutar Comandos
        //Comando Ordenar 
        private async Task Filtar()
        {
            try
            {
                string [] menu = { "Ascendente", "Descendente" };
                var verificar = await App.Current.MainPage.DisplayActionSheet("Ordene de acuerdo al servicio mejor calificado", "Cancelar", null, menu);
                if(verificar == "Ascendente")
                {
                    //Ordenar por promedio de menor a mayor
                    var tempLista = TempListViewSource.OrderBy(item => Convert.ToDecimal(item.Apoyo)).ToList();
                    string convertir = JsonConvert.SerializeObject(tempLista);
                    var lista = JsonConvert.DeserializeObject<ObservableCollection<ServicioModel>>(convertir);
                    if (lista.Count == 0)
                    {
                        messageService.ShortAlert("No se ha encontrado datos");
                        ListViewSource = lista;
                    }
                    else
                        ListViewSource = lista;
                }
                else
                {
                    //Ordenar por promedio de mayor a menor
                    var tempLista = TempListViewSource.OrderByDescending(item => Convert.ToDouble(item.Apoyo)).ToList();
                    string convertir = JsonConvert.SerializeObject(tempLista);
                    var lista = JsonConvert.DeserializeObject<ObservableCollection<ServicioModel>>(convertir);
                    if (lista.Count == 0)
                    {
                        messageService.ShortAlert("No se ha encontrado datos");
                        ListViewSource = lista;
                    }
                    else
                        ListViewSource = lista;
                }
                

                await Task.Delay(1000);
            }
            catch (Exception)
            {
                messageService.ShortAlert(servicioService.MensajeError);
            }
        }

        //Buscar en la lista sServicios por el nombre
        private async Task Buscar(string cadena)
        {
            try
            {
                var tempLista = TempListViewSource.Where(item => item.TituloServicio.StartsWith(cadena)).ToList();
                string convertir = JsonConvert.SerializeObject(tempLista);
                var lista = JsonConvert.DeserializeObject<ObservableCollection<ServicioModel>>(convertir);
                if (lista.Count == 0)
                {
                    messageService.ShortAlert("No se ha encontrado datos");
                    ListViewSource = lista;
                }
                else
                    ListViewSource = lista;

                await Task.Delay(1000);
            }
            catch (Exception)
            {
                messageService.ShortAlert(servicioService.MensajeError);
            }
        }

        private async Task RecargarDatos()
        {
            await Task.Run(() => CargarDatos());
        }

        private async Task CargarDatos()
        {
            IsRefresh = true;
            try
            {
                //Obtenere datos en una variable
                var lista = await servicioService.ObtenerServicios();
                //Serializar lista en formato json
                string convertir = JsonConvert.SerializeObject(lista);
                //Deserealizar JSON y guardar en la colección de datos
                ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<ServicioModel>>(convertir);
                TempListViewSource = ListViewSource;
                await Task.Delay(1000);
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", servicioService.MensajeError, "Ok");
            }
            IsRefresh = false;
        }

        //Ejecutar el formulario Nuevo Servicio 
        private async Task FrmNuevoServicio()
        {
            ServicioModel modelo = new ServicioModel();
            await App.Current.MainPage.Navigation.PushAsync(new EditarServicioView(modelo));
        }

        //Actualizar el estado del servicio a Activo
        private async Task ActivarServicio(ServicioModel servicioModel)
        {
            try
            {
                if (servicioModel.Estado == false)
                {
                    servicioModel.Apoyo = null;
                    servicioModel.Estado = true;
                    await servicioService.ActualizarServicio(servicioModel);
                    await RecargarDatos();
                    messageService.ShortAlert("Servicio Activado");
                }
                else
                {
                    messageService.ShortAlert("El serivicio ya esta Activado");
                }
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", servicioService.MensajeError, "Aceptar");
                throw;
            }
        }

        //Actualizar el campo bloqueo del serivicio a Bloqueado
        private async Task EliminarServicio(ServicioModel servicioModel)
        {
            var respuesta = await App.Current.MainPage.DisplayAlert("Alerta", "¿Esta seguro que desea Eliminar este servicio?", "Si", "No");
            if (respuesta)
            {
                servicioModel.Apoyo = null;
                servicioModel.Bloquear = true;
                await servicioService.ActualizarServicio(servicioModel);
                await RecargarDatos();
                messageService.ShortAlert("Servicio eliminado con exito");
            }
        }

        //Actualizar el estado del serivicio a Desactivado
        private async Task DesactivarServicio(ServicioModel servicioModel)
        {
            try
            {
                if (servicioModel.Estado == true)
                {
                    var respuesta = await App.Current.MainPage.DisplayAlert("Alerta", "¿Esta seguro que desea Desactivar este servicio", "Si", "No");
                    if (respuesta)
                    {
                        servicioModel.Apoyo = null;
                        servicioModel.Estado = false;
                        await servicioService.ActualizarServicio(servicioModel);
                        await this.RecargarDatos();
                        messageService.ShortAlert("Servicio Desactivado");
                    }
                }
                else
                {
                    messageService.ShortAlert("El serivicio ya esta desactivado");
                }
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", servicioService.MensajeError, "Aceptar");
            }
        }

        //Abrr el formulario nuevo Servicio con datos
        private async Task FrmActualizarServicio(ServicioModel s)
        {
            await App.Current.MainPage.Navigation.PushAsync(new EditarServicioView(s));
        }
        #endregion
    }
}
