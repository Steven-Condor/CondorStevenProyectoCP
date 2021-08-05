using AllyCliente.Interface;
using AllyCliente.Model;
using AllyCliente.Service;
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
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyCliente.ViewModel.Cliente
{
    public class ListaOrdenesServicioViewModel: OrdenesServicioModel
    {
        #region Instancias
        readonly OrdenesServicioService ordenServcice = new OrdenesServicioService();
        readonly IMessageService messageService;
        #endregion

        #region Atributos

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { isRefreshing = value; OnPropertyChanged(); }
        }


        private ObservableCollection<OrdenesServicioModel> listViewSource;

        public ObservableCollection<OrdenesServicioModel> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged();  }
        }

        public ObservableCollection<OrdenesServicioModel> tempListViewSource;

        public Command<OrdenesServicioModel> SeleccionarOrdenCommand { get; set; }
        public Command RecargarDatosCommand { get; set; }
        public Command BuscarCommand { get; set; }
        public Command FiltrarCommand { get; set; }

        #endregion

        #region Constructor

        public ListaOrdenesServicioViewModel()
        {
            messageService = DependencyService.Get<IMessageService>();
            Task.Run(async () => await this.CargarDatos());
            SeleccionarOrdenCommand = new Command<OrdenesServicioModel>(async (orden) => await SeleccionarOrden(orden));
            RecargarDatosCommand = new Command(async () => await RecargarDatos());
            BuscarCommand = new Command<string>(async (cadena) => await Buscar(cadena));
            FiltrarCommand = new Command(async () => await Filtar());
        }

        #endregion

        #region Ejecutar Comandos
        //Metodo para realizar la busqueda a traves del nombre del servicio o Identificador de la orden
        private async Task Buscar(string cadena)
        {
            try
            {
                var tempLista = tempListViewSource.Where(item => item.NombreServicio.StartsWith(cadena) || item.IdOrden.StartsWith(cadena)).ToList();
                string convertir = JsonConvert.SerializeObject(tempLista);
                var lista = JsonConvert.DeserializeObject<ObservableCollection<OrdenesServicioModel>>(convertir);
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
                messageService.ShortAlert("Orden no encontrada");
            }
        }

        //Filtrar por orden de creacion de orden
        private async Task Filtar()
        {
            try
            {
                string[] menu = { "Ascendente", "Descendente" };
                var verificar = await App.Current.MainPage.DisplayActionSheet("Ordene de acuerdo al a la fecha de creación", "Cancelar", null, menu);
                if (verificar == "Ascendente")
                {
                    var tempLista = tempListViewSource.OrderBy(item => Convert.ToDateTime(item.Fechas.FechaCreacion)).ToList();
                    string convertir = JsonConvert.SerializeObject(tempLista);
                    var lista = JsonConvert.DeserializeObject<ObservableCollection<OrdenesServicioModel>>(convertir);
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
                    var tempLista = tempListViewSource.OrderByDescending(item => Convert.ToDateTime(item.Fechas.FechaCreacion)).ToList();
                    string convertir = JsonConvert.SerializeObject(tempLista);
                    var lista = JsonConvert.DeserializeObject<ObservableCollection<OrdenesServicioModel>>(convertir);
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
                messageService.ShortAlert("No se han encontado datos");
            }
        }

        //Seleccionar una orden y pasar datos a la ventana Infromacion de Ordene de servicio
        public async Task SeleccionarOrden(OrdenesServicioModel orden)
        {
            try
            {
                await App.Current.MainPage.Navigation.PushModalAsync(new InformacionOrdenServicioView(orden));
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", ordenServcice.MensajeError, "Ok");
            }
        }

        #endregion

        #region Cargar Datos 
        //Cargar La colleccion de datos con la lista de ordenes de servicio
        public async Task CargarDatos()
        {
            IsRefreshing = true;
            try
            {
                var idCliente = Preferences.Get("Uid", null);
                var lista = await ordenServcice.ConsultarOrdenesPorCliente(idCliente);
                if(lista==null)
                {
                    messageService.ShortAlert("No se encontraron datos");
                }
                else
                {
                    string convertir = JsonConvert.SerializeObject(lista);
                    ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<OrdenesServicioModel>>(convertir);
                    tempListViewSource = ListViewSource;
                }
                await Task.Delay(1000);
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Ok", ordenServcice.MensajeError,"Ok");
            }
            IsRefreshing = false;
        }

        #endregion

        #region Recargar datos
        //Realizar un refresh a la Collección de datos
        public async Task RecargarDatos()
        {
            try
            {
                await Task.Run(() => this.CargarDatos());
            }
            catch (Exception)
            {
                messageService.ShortAlert(ordenServcice.MensajeError);
            }
        }

        #endregion
    }
}
