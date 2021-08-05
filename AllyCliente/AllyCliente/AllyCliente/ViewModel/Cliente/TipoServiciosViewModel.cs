using AllyCliente.Interface;
using AllyCliente.Model;
using AllyCliente.Service;
using AllyCliente.View.Cliente;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AllyCliente.ViewModel.Cliente
{
    public class TipoServiciosViewModel: INotifyPropertyChanged
    {
        #region Instancias
        readonly TipoServiciosService tipoService = new TipoServiciosService();
        readonly IMessageService messageService;
        #endregion

        #region implementacion INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Parametros

        private bool isRefresh;
        public bool IsRefresh
        {
            get { return isRefresh; }
            set { isRefresh = value; OnPropertyChanged(); }
        }

        private ObservableCollection<TipoServicioModel> listViewSource;

        public ObservableCollection<TipoServicioModel> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged(); }
        }

        public Command SeleccionarTipoServicioCommand { get; set; }
        public Command RefreshCommand { get; set; }

        #endregion

        #region Constructor
        public TipoServiciosViewModel()
        {
            messageService = DependencyService.Get<IMessageService>();
            Task.Run(() => this.CargarDatos());
            SeleccionarTipoServicioCommand = new Command<TipoServicioModel>(async (f) => await SeleccionarTipoServicio(f));
            RefreshCommand = new Command(async () => await Refresh());
        }
        #endregion

        #region Ejecutar Comandos
        private async  Task Refresh()
        {
            IsRefresh = true;
            await Task.Run(() => CargarDatos());
            IsRefresh = false;        
        } 

        private async Task SeleccionarTipoServicio(TipoServicioModel model)
        {
            await App.Current.MainPage.Navigation.PushAsync(new ListaServiciosView(model.TipoServicio));
        }

        private async Task CargarDatos()
        {
            IsRefresh = true;
            var lista = await tipoService.ConsultarTiposServicios();
            if (lista.Count > 0)
            {
                string convertir = JsonConvert.SerializeObject(lista);
                ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<TipoServicioModel>>(convertir);
                await Task.Delay(2000);
            }
            else
            {
                messageService.ShortAlert(tipoService.MensajeError);
            }
            IsRefresh = false;
        }
        #endregion

    }
}
