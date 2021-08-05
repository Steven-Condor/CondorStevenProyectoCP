using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyContratista.ViewModel.Contratista
{
    public class PerfilViewModel:UsuarioModel
    {
        #region Instancias
        readonly UsuarioService usuarioService = new UsuarioService();
        readonly IMessageService messageService;
        #endregion

        #region Definir Comandos
        public Command ActualizarUsuarioCommand { get; set; }
        public Command CambiarPasswdCommand { get; set; }
        public Command SeleccionarFotoCommand { get; set; }
        public Command TomarFotoCommand { get; set; }

        #endregion

        #region Parametros
        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        private ImageSource fuenteImagen;
        public ImageSource FuenteImagen
        {
            get { return fuenteImagen; }
            set { fuenteImagen = value; OnPropertyChanged(); }
        }
        private FileResult file;
        public FileResult File
        {
            get { return file; }
            set { file = value; OnPropertyChanged(); }
        }
        #endregion

        #region Constructor
        public PerfilViewModel()
        {
            messageService = DependencyService.Get<IMessageService>();
            Task.Run(() => this.CargarDatos());
            ActualizarUsuarioCommand = new Command(async () => await ActualizarUsuario());
            SeleccionarFotoCommand = new Command(async () => await SeleccionarFoto());
            TomarFotoCommand = new Command(async () => await TomarFoto());
            CambiarPasswdCommand = new Command(async () => await CambiarPasswd());
        }
        #endregion

        #region Ejecutar Comandos
        //Enviar enlace de cambio de contraseña al coreo electronico
        private async Task CambiarPasswd()
        {
            IsBusy = true;
            try
            {
                var verificar = await usuarioService.CambiarPasswd(this.Email);
                await Task.Delay(1000);
                if(verificar)
                    await App.Current.MainPage.DisplayAlert("Alerta", "Se ha enviado un enlace a su correo para cambiar la contraseña", "Ok");
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", usuarioService.MensajeError, "Ok");
            }
            IsBusy = false;
        }

        private async Task ActualizarUsuario()
        {
            isBusy = true;
            try
            {
                //Vlidar si la imagen esta definida
                string imgUrl = null;
                if (File != null)
                {
                    int link = File.FileName.Length;
                    string extencion = File.FileName.Substring(link - 4, 4);
                    imgUrl = await usuarioService.SubirFoto(await File.OpenReadAsync(), File.FileName, this.TipoUsuario, this.IdUsuario + extencion);
                }
                else
                {
                    imgUrl = this.Imagen;
                }
                //Caragr datos a un nuevo mdoelo
                UsuarioModel usuario = new UsuarioModel
                {
                    FechaCreacion = this.FechaCreacion,
                    TipoUsuario = this.TipoUsuario,
                    IdUsuario = this.IdUsuario,
                    Apellido = this.Apellido,
                    Nombre = this.Nombre,
                    Cedula = this.Cedula,
                    Imagen = imgUrl,
                    Email = this.Email,
                    Telefono = this.Telefono,
                    Estado = "true"
                };
                //Llamar al método actalizar usuario y enviar el modelo con los nuevos datos
                await usuarioService.ActualzarUsuario(usuario);
                messageService.ShortAlert("Usuario Actualizado correctamente");
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", usuarioService.MensajeError, "Ok");
            }
            IsBusy = false;
        }

        private void CargarDatos()
        {
            try
            {
                //Llenar el modelo de datos con los datos del usuario
                string idUsuario = Preferences.Get("Uid", null);
                var usuario = usuarioService.BuscarUsuario(idUsuario);

                this.IdUsuario = usuario.Result.IdUsuario;
                this.Apellido = usuario.Result.Apellido;
                this.Nombre = usuario.Result.Nombre;
                this.Cedula = usuario.Result.Cedula;
                this.TipoUsuario = usuario.Result.TipoUsuario;
                this.Email = usuario.Result.Email;
                this.Telefono = usuario.Result.Telefono;
                this.Imagen = usuario.Result.Imagen;
                this.FechaCreacion = usuario.Result.FechaCreacion;
                FuenteImagen = this.Imagen;
            }
            catch (Exception)
            {
                App.Current.MainPage.DisplayAlert("Alerta", usuarioService.MensajeError, "Ok");
            }
        }

        //Abrir la galeria de fotos y seleccionar una
        private async Task SeleccionarFoto()
        {
            IsBusy = true;
            var resultado = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Por favor Toma una foto"
            });
            File = resultado;
            if (resultado != null)
            {
                var stream = await resultado.OpenReadAsync();
                FuenteImagen = ImageSource.FromStream(() => stream);
            }
            IsBusy = false;
        }

        //Tomar una foto y almacenar en una variable
        private async Task TomarFoto()
        {
            IsBusy = true;
            var resultado = await MediaPicker.CapturePhotoAsync();
            File = resultado;
            if (resultado != null)
            {
                var stream = await resultado.OpenReadAsync();
                FuenteImagen = ImageSource.FromStream(() => stream);
            }
            IsBusy = false;
        }
        #endregion
    }
}
