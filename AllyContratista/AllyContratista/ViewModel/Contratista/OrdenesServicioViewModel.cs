using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Model.Consultas;
using AllyContratista.Service;
using AllyContratista.View.Contratista;
using AllyContratista.View.Contratista.Editar;
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

namespace AllyContratista.ViewModel.Contratista
{
    public class OrdenesServicioViewModel: INotifyPropertyChanged
    {
        #region Instancias
        readonly OrdenServicioService ordenService = new OrdenServicioService();
        readonly IMessageService messageService;
        #endregion

        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        #endregion

        #region Parametros

        private bool isRefresh;

        public bool IsRefresh
        {
            get { return isRefresh; }
            set { isRefresh = value; OnPropertyChanged(); }
        }

        private ObservableCollection<OrdenServicioQuery> listViewSource;

        public ObservableCollection<OrdenServicioQuery> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged(); }

        }

        private List<OrdenServicioQuery> TempListViewSource { get; set; }

        private string servicioId;
        public string ServicioID
        {
            get { return servicioId; }
            set { servicioId = value; }
        }

        #endregion

        #region Inicializar Comandos
        public Command SeleccionarOrdenServicioCommand { get; set; }
        public Command RefreshCommand { get; set; }
        public Command OrdenarComand { get; set; }
        public Command SearchCommand{ get; set; }
        #endregion

        #region Constructor
        public OrdenesServicioViewModel(string idServicio)
        {
            messageService = DependencyService.Get<IMessageService>();
            ServicioID = idServicio;
            Task.Run(() => this.CargarDatos());
            SeleccionarOrdenServicioCommand = new Command<OrdenServicioQuery>(async (m)=> await SeleccionarOrdenServicio(m.IdOrden));
            RefreshCommand = new Command(async () => await RecargarDatos());
            OrdenarComand = new Command(async () => await Ordenar());
            SearchCommand = new Command<string>(async (s) => await Search(s));
        }
        #endregion

        #region Ejecutar Comandos
        //Ordenar lista de ordenes de serivcio pr fecha de creación de menor a mayor y de mayor a menor 
        private async Task Ordenar()
        {
            try
            {
                string[] orden = { "Ascenddente", "Descendente" };
                
                var ordenar = await App.Current.MainPage.DisplayActionSheet("Seleccione el orden de acuerdo a la fecha de creación", "Cancelar", null, orden);

                if (ordenar == "Ascenddente")
                {
                    var lista = await ordenService.ConsultarOrdenServcioJoin(ServicioID, false);
                    string convertir = JsonConvert.SerializeObject(lista);
                    ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<OrdenServicioQuery>>(convertir);
                }
                else if((ordenar == "Descendente"))
                {
                    var lista = await ordenService.ConsultarOrdenServcioJoin(ServicioID, true);
                    string convertir = JsonConvert.SerializeObject(lista);
                    ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<OrdenServicioQuery>>(convertir);
                }
                                    
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", ordenService.MensajeError, "Ok");
            }
        }

        //Abrir el formulario de gestioanr servicio, enviando el id de orden
        private async Task SeleccionarOrdenServicio(string idOrdenServicio)
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new GestionarOrdenesServicioView(idOrdenServicio));
        }

        private async Task CargarDatos()
        {
            IsRefresh = true;
            try
            {
                var lista = await ordenService.ConsultarOrdenServcioJoin(ServicioID, true);
                string convertir = JsonConvert.SerializeObject(lista);
                ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<OrdenServicioQuery>>(convertir);
                TempListViewSource = lista;
                await Task.Delay(1000);
            }
            catch (Exception)
            {
                messageService.ShortAlert(ordenService.MensajeError);
            }
            IsRefresh = false;
        }

        private async Task RecargarDatos()
        {
            await this.CargarDatos();
        }

        //Buscar una orden de servicio enespecifico a traves de Id de orden y Nombre del cliente
        public async Task Search(string texto)
        {
            await Task.Delay(1000);
            var temLista = TempListViewSource.Where(a => a.NombreCliente.StartsWith(texto) || a.IdOrden.StartsWith(texto));
            string convertir = JsonConvert.SerializeObject(temLista);
            var lista = JsonConvert.DeserializeObject<ObservableCollection<OrdenServicioQuery>>(convertir);
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
        #endregion

    }
}
