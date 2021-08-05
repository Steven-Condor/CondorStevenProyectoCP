using AllyCliente.Model;
using AllyCliente.Service;
using AllyCliente.View.Cliente;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyCliente.ViewModel
{
    public class RegistroViewModel:UsuarioModel
    {
        #region Instancias 
        readonly UsuarioService usuarioService = new UsuarioService();
        #endregion

        #region Parametros
        private string isRunning;
        public string IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; OnPropertyChanged(); }
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

        private bool isPassword;

        public bool IsPassword
        {
            get { return isPassword; }
            set { isPassword = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commandos
        public Command RegistrarseCommand { get; set; }
        public Command TomarFotoCommand { get; set; }
        public Command SeleccionarFotoCommand { get; set; }
        public Command VerPasswordCommand { get; }
        #endregion

        #region Constructor
        public RegistroViewModel()
        {
            IsPassword = true;
            RegistrarseCommand = new Command(async () => await Registrarse());
            SeleccionarFotoCommand = new Command(async () => await SeleccionarFoto());
            VerPasswordCommand = new Command(async () => await MostrarPasswd());
            TomarFotoCommand = new Command(async () => await TomarFoto());
        }
        #endregion

        #region Ejecutar Comandos
        //Tomar fotografia mediante Xamarin Essentials
        private async  Task TomarFoto()
        {
            IsRunning = "True";
            var resultado = await MediaPicker.CapturePhotoAsync();
            File = resultado;
            if (resultado != null)
            {
                var stream = await resultado.OpenReadAsync();
                FuenteImagen = ImageSource.FromStream(() => stream);
            }
            IsRunning = "False";
        }

        //Comando para mostrar la contraseña temporalmente
        private async Task MostrarPasswd()
        {
            IsPassword = false;
            await Task.Delay(3000);
            IsPassword = true;
        }

        //Seleccionar fotografia mediante Xamarin Essentials
        private async Task SeleccionarFoto()
        {
            IsRunning = "True";
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
            IsRunning = "False";
        }

        //Resgistrar el usuario en el sistema y el el API de Autenticación
        private async Task Registrarse()
        {
            IsRunning = "True";
            try
            {
                if (string.IsNullOrEmpty(this.Email) || string.IsNullOrEmpty(this.Passwd) || string.IsNullOrEmpty(this.RPasswd))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Llene todos los campos, ¡Porfavor!", "Ok");
                }
                else if (Passwd == RPasswd && Passwd.Length >= 6 && Email.Contains("@"))
                {
                    string imgUrl = null;

                    if (File != null)
                    {
                        int link = File.FileName.Length;
                        string extencion = File.FileName.Substring(link - 4, 4);
                        imgUrl = await usuarioService.SubirFoto(await File.OpenReadAsync(), File.FileName, "Cliente", File.FileName);
                    }
                    else
                    {
                        imgUrl = this.Imagen;
                    }

                    UsuarioModel usuario = new UsuarioModel
                    {
                        Imagen = imgUrl,
                        IdUsuario = null,
                        RPasswd = this.Passwd,
                        Nombre = this.Nombre,
                        Apellido = this.Apellido,
                        Email = this.Email,
                        Telefono = this.Telefono,
                        Cedula = this.Cedula,
                        Passwd = this.Encriptar(this.Passwd),
                        TipoUsuario = "Cliente",
                        FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    };
                    //Registrar usuario en Firebase Auth
                    var insertar = await usuarioService.Registrar(usuario);

                    if (insertar == true)
                        await App.Current.MainPage.Navigation.PushAsync(new ClienteView());
                    else
                        await App.Current.MainPage.DisplayAlert("Alerta", usuarioService.MensajeError, "Ok");

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alerta", "Las contraseñas no coinciden" +
                        " o no tienen un minimo de 6 caracteres", "Ok");
                }
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", usuarioService.MensajeError + ": " + ex.Message, "Ok");
            }
            IsRunning = "False";
        }
        #endregion

        private string Encriptar(string _cadenaEncriptar)
        {
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaEncriptar);
            var result = Convert.ToBase64String(encryted);
            return result;
        }


    }
}
