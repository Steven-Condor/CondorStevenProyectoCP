using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Service;
using AllyContratista.View.Administrador;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AllyContratista.ViewModel.Administrador
{
    public class TipoServicioViewModel:TipoServicioModel
    {

        #region Instancias
        readonly TipoServicioService tipoServicioService = new TipoServicioService();
        readonly IMessageService messageService;
        #endregion

        #region Parametros

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        private ObservableCollection<TipoServicioModel> listViewSource;
        public ObservableCollection<TipoServicioModel> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TipoServicioModel> tempListViewSource;

        #endregion

        #region Comandos
        public Command EliminarTipoServicioCommand { get; set; }
        public Command RefreshCommand { get; set; }
        public Command VerTipoServicioCommand { get; set; }
        public Command ActivarTipoServicioCommand { get; set; }
        public Command BuscarCommand { get; set; }
        #endregion

        #region Constructor
        public TipoServicioViewModel()
        {
            messageService = DependencyService.Get<IMessageService>();
            Task.Run(() => CargarDatos());
            RefreshCommand = new Command(async () => await Refresh());
            EliminarTipoServicioCommand = new Command<TipoServicioModel>(async (ts) => await EliminarTipoServicio(ts));
            ActivarTipoServicioCommand = new Command<TipoServicioModel>(async (ts) => await ActivarTipoServicio(ts));
            VerTipoServicioCommand = new Command<TipoServicioModel>(async (ts) => await VerTipoServicio(ts));
            BuscarCommand = new Command<string>(async (cadena) => await Buscar(cadena));
        }
        #endregion

        #region Ejecutar Comandos
        private async Task Buscar(string cadena)
        {
            try
            {
                var tempLista = tempListViewSource.Where(item => item.TipoServicio.StartsWith(cadena)).ToList();
                string convertir = JsonConvert.SerializeObject(tempLista);
                var lista = JsonConvert.DeserializeObject<ObservableCollection<TipoServicioModel>>(convertir);
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
                messageService.ShortAlert(tipoServicioService.MensajeError);
            }
        }

        private async Task ActivarTipoServicio(TipoServicioModel tipoServicio)
        {
            IsBusy = true;
            try
            {
                var verificacion = await App.Current.MainPage.DisplayAlert("Verificacion", "¿Desea Activar este Tipo de tarea?", "Aceptar", "Cancelar");
                if (verificacion)
                {
                    tipoServicio.Estado = true;
                    await tipoServicioService.ActualizarTipoServicios(tipoServicio);
                    await Refresh();
                    messageService.ShortAlert("Tarea Habilitada");
                    await Task.Delay(1000);
                }
            }
            catch (Exception)
            {
                messageService.ShortAlert(tipoServicioService.MensajeError);
            }
            IsBusy = false;
        }

        private async Task Refresh()
        {
            IsBusy = true;
            await Task.Run(() => CargarDatos());
            await Task.Delay(2000);
            IsBusy = false;
        }

        private async Task VerTipoServicio(TipoServicioModel tipoServicio)
        {
            await App.Current.MainPage.Navigation.PushAsync(new EditarTipoServicioView(tipoServicio));
        }

        public async Task CargarDatos()
        {
            try
            {
                var lista = await tipoServicioService.ConsultarTiposServicios();
                var convertir = JsonConvert.SerializeObject(lista);
                ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<TipoServicioModel>>(convertir);
                tempListViewSource = ListViewSource;
            }
            catch (Exception)
            {
                messageService.ShortAlert(tipoServicioService.MensajeError);
            }
        }

        private async Task EliminarTipoServicio(TipoServicioModel tipoServicio)
        {
            IsBusy = true;
            try
            {
                var verificacion = await App.Current.MainPage.DisplayAlert("Verificacion", "¿Desea Eliminar este Tipo de tarea?", "Aceptar", "Cancelar");
                if (verificacion)
                {
                    tipoServicio.Estado = false;
                    await tipoServicioService.ActualizarTipoServicios(tipoServicio);
                    await Refresh();
                    messageService.ShortAlert("Tarea Deshabilitada");
                    await Task.Delay(1000);
                }                    
            }
            catch (Exception)
            {
                messageService.ShortAlert(tipoServicioService.MensajeError);
            }
            IsBusy = false;
        }
        #endregion
    }
}
