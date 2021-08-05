using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Model.Consultas;
using AllyContratista.Service;
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
    public class ServiciosViewModel:ServicioModel
    {
        #region Instancias
        readonly ServicioService serviciosService =new ServicioService();
        readonly IMessageService messageService;
        #endregion

        #region Parametros
        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ServicioModel> listViewSource;
        public ObservableCollection<ServicioModel> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ServicioModel> TempListViewSource { set; get; }
        #endregion

        #region Comandos
        public Command RefreshCommand { get; set; }
        public Command ActivarServicioCommand { get; set; }
        public Command BuscarCommand { get; set; }
        public Command BloquearServicioCommand { get; set; }

        #endregion

        #region Constructor
        public ServiciosViewModel()
        {
            messageService = DependencyService.Get<IMessageService>();
            Task.Run(() => CargarDatos());
            RefreshCommand = new Command(async () => await Refresh());
            ActivarServicioCommand = new Command<ServicioModel>(async (s) => await ActivarServicio(s));
            BuscarCommand = new Command<string>(async (cadena) => await Buscar(cadena));
            BloquearServicioCommand = new Command<ServicioModel>(async (s) => await BloquearServicio(s));
        }
        #endregion

        #region Ejecutar Comandos

        private async Task BloquearServicio(ServicioModel servicio)
        {
            IsRunning = true;
            try
            {
                var verificacion = await App.Current.MainPage.DisplayAlert("Verificacion", "Desea Bloquear este Servicio?", "Aceptar", "Cancelar");
                if (verificacion)
                {
                    servicio.Estado = false;
                    servicio.Bloquear = true;
                    await serviciosService.ActualizarServicio(servicio);
                    messageService.ShortAlert("Servicio Desactivado Correctamente");
                    await Refresh();
                    await Task.Delay(1000);
                }    

            }
            catch (Exception)
            {
                messageService.ShortAlert(serviciosService.MensajeError);
            }
            IsRunning = false;
        }


        private async Task Buscar(string cadena)
        {
            try
            {
                var tempLista = TempListViewSource;
                var lista = tempLista.Where(item => item.TituloServicio.StartsWith(cadena) || item.IdServicio.StartsWith(cadena)).ToList();
                string convertir = JsonConvert.SerializeObject(lista);
                if (lista.Count == 0)
                {
                    messageService.ShortAlert("No se ha encontrado datos");
                    ListViewSource = null;
                }
                else
                    ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<ServicioModel>>(convertir);

                await Task.Delay(1000);

            }
            catch (Exception)
            {
                messageService.ShortAlert(serviciosService.MensajeError);
            }
        }

        private async Task ActivarServicio(ServicioModel servicio)
        {
            try
            {
                var verificacion = await App.Current.MainPage.DisplayAlert("Verificacion", "Desea Activar este Servicio?", "Aceptar", "Cancelar");
                if (verificacion)
                {
                    servicio.Estado = true;
                    servicio.Bloquear = false;
                    await serviciosService.ActualizarServicio(servicio);
                    messageService.ShortAlert("Servicio Activado Correctamente");
                    await Refresh();
                    await Task.Delay(2000);
                }
            }
            catch (Exception)
            {
                messageService.ShortAlert(serviciosService.MensajeError);
            }
        }

        private async Task Refresh()
        {
            IsRunning = true;
            await Task.Run(() => CargarDatos());
            await Task.Delay(3000);
            IsRunning = false;
        }

        private async Task CargarDatos()
        {
            try
            {
                var lista = await serviciosService.ConsultarServicios();
                var convertir = JsonConvert.SerializeObject(lista);
                ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<ServicioModel>>(convertir);
                TempListViewSource = ListViewSource;
            }
            catch (Exception)
            {
                messageService.ShortAlert(serviciosService.MensajeError);
            }
        }
        #endregion
    }
}
