using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyContratista.ViewModel.Administrador
{
    public class UsuarioViewModel:UsuarioModel
    {
        #region Instancias
        readonly UsuarioService servicioUsuario = new UsuarioService();
        readonly IMessageService messageService;
        #endregion

        #region Parametros
        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; OnPropertyChanged(); }
        }

        private ObservableCollection<UsuarioModel> listViewSource;
        public ObservableCollection<UsuarioModel> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged(); }
        }

        private ObservableCollection<UsuarioModel> TempListViewSource { get; set; }

        #endregion

        #region Comandos
        public Command DesactivarUsuarioCommand { get; set; }
        public Command ActivarUsuarioCommand { get; set; }
        public Command RefreshCommand { get; set; }
        public Command BuscarCommand { get; set; }
        #endregion

        #region Constructor
        public UsuarioViewModel()
        {
            messageService = DependencyService.Get<IMessageService>();
            Task.Run(() => this.CargarDatos());
            DesactivarUsuarioCommand = new Command<UsuarioModel>(async (u) => await DesactivarUsuario(u));
            ActivarUsuarioCommand = new Command<UsuarioModel>(async (u) => await ActivarUsuario(u));
            RefreshCommand = new Command(async () => await Refresh());
            BuscarCommand = new Command<string>(async(cadena)=> await Buscar(cadena));
        }
        #endregion

        #region Ejecutar Comandos

        //Acttivar usuario seleccionado
        private async Task ActivarUsuario(UsuarioModel usuario)
        {
            IsRunning = true;
            try
            {
                var activar = await App.Current.MainPage.DisplayAlert("Verificacion", "Desea Activar este usuario?", "Aceptar", "Cancelar");
                if (activar)
                {
                    usuario.Estado = "true";
                    var elimiarDB = await servicioUsuario.ActualzarUsuario(usuario);
                    messageService.ShortAlert("Usuario Bloqueado Correctamente");
                    await Task.Run(() => Refresh());
                }
            }
            catch (Exception)
            {
                messageService.ShortAlert(servicioUsuario.MensajeError);
            }
            IsRunning = false;
        }
        //Buscar usuario por Nombre apellido o Id
        private async Task Buscar(string cadena)
        {
            try
            {
                var tempLista = TempListViewSource.Where(item => item.Nombre.StartsWith(cadena) || item.Apellido.StartsWith(cadena) || item.IdUsuario.StartsWith(cadena)).ToList();
                string convertir = JsonConvert.SerializeObject(tempLista);
                var lista = JsonConvert.DeserializeObject<ObservableCollection<UsuarioModel>>(convertir);
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
                messageService.ShortAlert(servicioUsuario.MensajeError);
            }
        }

        private async Task CargarDatos()
        {
            try
            {
                var lista = await servicioUsuario.ListaUsuarios();
                var convertir = JsonConvert.SerializeObject(lista);
                ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<UsuarioModel>>(convertir);
                TempListViewSource = ListViewSource;
            }
            catch (Exception)
            {
                messageService.ShortAlert(servicioUsuario.MensajeError);
            }
        }

        private async Task Refresh()
        {
            IsRunning = true;
            await Task.Run(() => CargarDatos());
            await Task.Delay(3000);
            IsRunning = false;
        }

        //Desactivar usuario
        private async Task DesactivarUsuario(UsuarioModel usuario)
        {
            IsRunning = true;
            try
            {
                var desactivar = await App.Current.MainPage.DisplayAlert("Verificacion", "Desea Desactivar este usuario?", "Aceptar", "Cancelar");
                if (desactivar)
                {
                    usuario.Estado = "false";
                    var elimiarDB = await servicioUsuario.ActualzarUsuario(usuario);
                    messageService.ShortAlert("Usuario Bloqueado Correctamente");
                    await Task.Run(()=>Refresh());
                }
            }
            catch (Exception)
            {
                messageService.ShortAlert(servicioUsuario.MensajeError);
            }
            IsRunning = false;
        }

        #endregion
    }
}
