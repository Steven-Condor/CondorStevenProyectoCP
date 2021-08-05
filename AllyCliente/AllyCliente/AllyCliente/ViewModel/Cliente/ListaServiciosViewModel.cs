using AllyCliente.Interface;
using AllyCliente.Model;
using AllyCliente.Model.Query;
using AllyCliente.Service;
using AllyCliente.View.Cliente;
using AllyCliente.View.Cliente.Editar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AllyCliente.ViewModel
{
    public class ListaServiciosViewModel: ServiciosModel
    {
        #region Instancias
        ServiciosService servicioService = new ServiciosService();
        IMessageService messageService;
        #endregion

        #region Parametros

        private bool isRefreh;

        public bool IsRefreh
        {
            get { return isRefreh; }
            set { isRefreh = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ServiciosQuery> listViewSource;

        public ObservableCollection<ServiciosQuery> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ServiciosQuery> tempListViewSource;

        #endregion

        #region Inicializar Comandos
        public Command<ServiciosQuery> SeleccionarServicioCommand { get; set; }
        public Command RecargarDatosCommand { get; set; }
        public Command BuscarCommand { get; set; }
        public Command FiltrarCommand { get; set; }
        public Command FiltrarFechaCommand { get; set; }
        #endregion

        #region Constructor
        public ListaServiciosViewModel(string tipoServicio)
        {
            messageService = DependencyService.Get<IMessageService>();
            this.TipoServicio = tipoServicio;
            Task.Run(() => this.CargarDatos(tipoServicio));
            SeleccionarServicioCommand = new Command<ServiciosQuery>(async (servicio) => await SeleccionarServicio(servicio));
            RecargarDatosCommand = new Command(async () => await RecargarDatos(tipoServicio));
            BuscarCommand = new Command<string>(async (cadena) => await Buscar(cadena));
            FiltrarCommand = new Command(async () => await Filtar());
            FiltrarFechaCommand = new Command(async () => await FiltrarFecha());
        }

        private async Task FiltrarFecha()
        {
            try
            {
                string[] menuFecha = { "Ascendente", "Descendente" };
                var verificar = await App.Current.MainPage.DisplayActionSheet("Ordene de acuerdo a la fecha de creación", "Cancelar", null, menuFecha);
                if (verificar == "Ascendente")
                {
                    var listaTemp = tempListViewSource.OrderBy(item => Convert.ToDateTime(item.FechaCreacion)).ToList();
                    string convertir = JsonConvert.SerializeObject(listaTemp);
                    var lista = JsonConvert.DeserializeObject<ObservableCollection<ServiciosQuery>>(convertir);
                    if (lista.Count == 0)
                    {
                        messageService.ShortAlert("No se ha encontrado datos");
                        ListViewSource = lista;
                    }
                    else
                    {
                        ListViewSource = lista;
                    }                     
                }
                else
                {
                    var listaTemp = tempListViewSource.OrderByDescending(item => Convert.ToDouble(item.Nota)).ToList();
                    string convertir = JsonConvert.SerializeObject(listaTemp);
                    if (listaTemp.Count == 0)
                    {
                        messageService.ShortAlert("No se ha encontrado datos");
                        ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<ServiciosQuery>>(convertir);
                    }
                    else
                    {
                        ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<ServiciosQuery>>(convertir); 
                    }      
                }
            }
            catch (Exception)
            {
                messageService.ShortAlert(servicioService.MensajeError);
            }
        }

        private async Task Filtar()
        {
            try
            {
                string[] menu = { "Ascendente", "Descendente" };
                var verificar = await App.Current.MainPage.DisplayActionSheet("Ordene de acuerdo al servicio mejor calificado", "Cancelar", null, menu);
                if (verificar == "Ascendente")
                {
                    var tempLista = tempListViewSource.OrderBy(item => Convert.ToDecimal(item.Nota)).ToList();
                    string convertir = JsonConvert.SerializeObject(tempLista);
                    var lista = JsonConvert.DeserializeObject<ObservableCollection<ServiciosQuery>>(convertir);
                    if (lista.Count == 0)
                    {
                        messageService.ShortAlert("No se ha encontrado datos");
                        ListViewSource = lista;
                    }
                    else
                    {
                        ListViewSource = lista;
                    }     
                }
                else
                {
                    var tempLista = tempListViewSource.OrderByDescending(item => Convert.ToDouble(item.Nota)).ToList();
                    string convertir = JsonConvert.SerializeObject(tempLista);
                    var lista = JsonConvert.DeserializeObject<ObservableCollection<ServiciosQuery>>(convertir);
                    if (lista.Count == 0)
                    {
                        messageService.ShortAlert("No se ha encontrado datos");
                        ListViewSource = lista;
                    }
                    else
                    {
                        ListViewSource = lista;
                    }
                        
                }
                await Task.Delay(1000);
            }
            catch (Exception)
            {
                messageService.ShortAlert(servicioService.MensajeError);
            }
        }

        private async Task Buscar(string cadena)
        {
            try
            {
                var tempLista = tempListViewSource.Where(item => item.TituloServicio.StartsWith(cadena) || item.NombreContratista.StartsWith(cadena)).ToList();
                string convertir = JsonConvert.SerializeObject(tempLista);
                var lista = JsonConvert.DeserializeObject<ObservableCollection<ServiciosQuery>>(convertir);
                if (lista.Count == 0)
                {
                    messageService.ShortAlert("No se ha encontrado datos");
                    ListViewSource = lista;
                }
                else
                {
                    ListViewSource = lista;
                }
                await Task.Delay(1000);
            }
            catch (Exception)
            {
                messageService.ShortAlert(servicioService.MensajeError);
            }
        }
        #endregion

        #region Seleccionar el servicio

        private async Task SeleccionarServicio(ServiciosQuery modelo)
        {
            try
            {
                await App.Current.MainPage.Navigation.PushModalAsync(new InformacionServicioView(modelo.IdServicio));
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", servicioService.MensajeError+" : "+ex.Message, "Ok");
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        #endregion

        #region Cargar datos
        private async Task CargarDatos(string tipoServicio)
        {
            try
            {
                IsRefreh = true;
                ListViewSource = await servicioService.ConsultarServicios(tipoServicio);
                tempListViewSource = ListViewSource;
                IsRefreh = false;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", servicioService.MensajeError+" : "+ex.Message, "Ok");
            }
        }
        #endregion

        #region Cargar datos
        private async Task RecargarDatos(string tipoServicio)
        {
            try
            {
                await Task.Run(() => CargarDatos(tipoServicio));
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", servicioService.MensajeError + " : " + ex.Message, "Ok");
            }
        }
        #endregion
    }
}
