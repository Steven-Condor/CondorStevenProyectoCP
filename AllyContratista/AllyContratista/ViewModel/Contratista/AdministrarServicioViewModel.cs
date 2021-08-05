using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Service;
using AllyContratista.View.Contratista;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AllyContratista.ViewModel.Contratista
{
    public class AdministrarServicioViewModel:ServicioModel
    {
        #region Instancias
        readonly ServicioService servicioService = new ServicioService();
        readonly IMessageService messageService;
        #endregion

        #region Parametros
        private ObservableCollection<ServicioModel> listViewSource;
        public ObservableCollection<ServicioModel> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ServicioModel> TempListViewSource { get; set; }

        private bool isRefresh;

        public bool IsRefresh
        {
            get { return isRefresh; }
            set { isRefresh = value; OnPropertyChanged(); }
        }

        public Command RefreshCommand { get; set; }
        public Command<ServicioModel> ListaOrdenesServicioCommand { get; set; }
        public Command BuscarCommand { get; set; }
        public Command FiltrarCommand { get; set; }

        #endregion

        #region Constructor
        public AdministrarServicioViewModel()
        {
            messageService = DependencyService.Get<IMessageService>();
            Task.Run(() => this.CargarDatos());
            RefreshCommand = new Command(async () => await RecargarDatos());
            ListaOrdenesServicioCommand = new Command<ServicioModel>(async (s) => await ListaOrdenesServicio(s.IdServicio));
            BuscarCommand = new Command<string>(async (cadena) => await Buscar(cadena));
            FiltrarCommand = new Command(async () => await Filtar());
        }
        #endregion

        #region Ejecutar Comandos
        //Ordenar de aceurdo al numero de ordenes generadas por serivcio
        private async Task Filtar()
        {
            try
            {
                //Ordenar de menor a mayor
                string[] menu = { "Ascendente", "Descendente" };
                var verificar = await App.Current.MainPage.DisplayActionSheet("Ordene de acuerdo a la cantidad de clientes", "Cancelar", null, menu);
                if (verificar == "Ascendente")
                {
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
                    //Ordenar de mayor a menor
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

        //Buscar Servicio por el titulo del servicio
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
        
        //Abrir el formulario lista de Ordenes de servicio
        public async Task ListaOrdenesServicio(string idServicio)
        {
            await App.Current.MainPage.Navigation.PushAsync(new OrdenesServicioView(idServicio));
        }

        public async Task CargarDatos()
        {
            //Activar el indicador de refresh
            IsRefresh = true;
            try
            {
                //caragar variable con la lista de serviio
                var lista = await servicioService.ObtenerServiciosContador();
                string convertir = JsonConvert.SerializeObject(lista);
                ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<ServicioModel>>(convertir);
                //Guardar los datos de la varibale en la coleccion de datos 
                TempListViewSource = ListViewSource;
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", servicioService.MensajeError, "Ok");
            }
            //Desactivar el indicador de refresh
            IsRefresh = false;
        }

        public async Task RecargarDatos()
        {
            IsRefresh = true;
            try
            {
                await Task.Run(() => this.CargarDatos());
                await Task.Delay(2000);
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", servicioService.MensajeError, "Ok");
                throw;
            }
            IsRefresh = false;
        }

        #endregion

    }
}
