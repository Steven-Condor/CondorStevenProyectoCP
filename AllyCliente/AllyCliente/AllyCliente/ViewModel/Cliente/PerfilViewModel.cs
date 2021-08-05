using AllyCliente.Interface;
using AllyCliente.Model;
using AllyCliente.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyCliente.ViewModel.Cliente
{
    public class PerfilViewModel:UsuarioModel
    {
        #region Instancias
        UsuarioService usuarioService = new UsuarioService();
        IMessageService messageService;
        #endregion

        #region Definir Comandos
        public Command ActualizarUsuarioCommand { get; set; }
        public Command CambiarPasswdCommand { get; set; }
        public Command PickImageCommand { get; set; }
        public Command TakeFotoCommand { get; set; }

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
            PickImageCommand = new Command(async () => await PickImage());
            TakeFotoCommand = new Command(async () => await TakeFoto());
            CambiarPasswdCommand = new Command(async () => await CambiarPasswd());
        }
        #endregion

        #region Ejecutar Comandos
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
                if (this.Passwd == this.RPasswd)
                {
                    string imgUrl = null;

                    if (File != null)
                    {
                        int link = File.FileName.Length;
                        string extencion = File.FileName.Substring(link - 4, 4);
                        imgUrl = await usuarioService.SubirFoto(await File.OpenReadAsync(), File.FileName, this.TipoUsuario, this.IdUsuario + extencion);
                    }
                    else
                        imgUrl = this.Imagen;


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
                        Telefono = this.Telefono
                    };

                    await usuarioService.ActualzarUsuario(usuario);
                    messageService.ShortAlert("Usuario Actualizado Correctamente");

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alerta", "Las contraseñas no coinciden", "Ok");
                }
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

        private async Task PickImage()
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

        private async Task TakeFoto()
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
