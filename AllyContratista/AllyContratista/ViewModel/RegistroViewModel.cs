using AllyContratista.Model;
using AllyContratista.Service;
using AllyContratista.View.Contratista;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyContratista.ViewModel
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
        //Abrir la cámara del dispositivo y tomar una foto
        private async  Task TomarFoto()
        {
            var resultado = await MediaPicker.CapturePhotoAsync();
            
            if (resultado != null)
            {
                var stream = await resultado.OpenReadAsync();
                FuenteImagen = ImageSource.FromStream(() => stream);
                File = resultado;
            }
        }

        //Mostrar la contraseña durante 3 segundos
        private async Task MostrarPasswd()
        {
            IsPassword = false;
            await Task.Delay(3000);
            IsPassword = true;
        }

        //Abrir la galeria de fotos y seleccionar una
        private async Task SeleccionarFoto()
        {
            var resultado = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Por favor Toma una foto"
            });

            if (resultado != null)
            {
                var stream = await resultado.OpenReadAsync();
                FuenteImagen = ImageSource.FromStream(() => stream);
                File = resultado;
            }
        }

        //Comando de registro de ussuario
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
                    //validar si la fotografia fue almacenada en la variable File, obtener la url 
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
                    //LLenar los datos de un nuevo modelo con los ingresados
                    UsuarioModel usuario = new UsuarioModel
                    {
                        Imagen = imgUrl,
                        IdUsuario = null,
                        Nombre = this.Nombre,
                        Apellido = this.Apellido,
                        Email = this.Email,
                        Telefono = this.Telefono,
                        Cedula = this.Cedula,
                        Passwd = this.Encriptar(this.Passwd),
                        FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        Estado="true",
                        TipoUsuario = "Contratista"
                    };
                    //llamara al mátodo de registro de usuario en Firebase Auth y luego a Firebase DB 
                    var insertar = await usuarioService.Registrar(usuario);
                    //validar si la funcion se ejecuto correctamente
                    if (insertar == true)
                    {
                        await App.Current.MainPage.Navigation.PushAsync(new ContratistaView());
                        await App.Current.MainPage.DisplayAlert("Alerta", "verifique su correo por favor", "Ok" );
                    }
                    else
                        await App.Current.MainPage.DisplayAlert("Alerta", usuarioService.MensajeError, "Ok");

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alerta", "Las contraseñas no coinciden" +
                        " o no tienen un minimo de 6 caracteres", "Ok");
                }

                await Task.Delay(1000);
                
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", usuarioService.MensajeError + ": " + ex.Message, "Ok");
            }
            IsRunning = "False";
        }
        #endregion

        //Encripatr las contraseñas
        private string Encriptar(string _cadenaEncriptar)
        {
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaEncriptar);
            string result = Convert.ToBase64String(encryted);
            return result;
        }


    }
}
