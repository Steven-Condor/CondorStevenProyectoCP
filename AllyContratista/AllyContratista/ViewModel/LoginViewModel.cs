using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Service;
using AllyContratista.View.Administrador;
using AllyContratista.View.Contratista;
using AllyContratista.View.Registro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyContratista.ViewModel
{
    public class LoginViewModel:LoginModel
    {
        #region Instancias
        readonly LoginService loginService = new LoginService();
        readonly ILoginGoogle loginGoogle;
        #endregion

        #region Atributos
        private string isRunning;
        public string IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; OnPropertyChanged(); }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        public Command LoginCommand { get; set; }
        public Command LoginGoogleCommand { get; set; }
        public Command AbrirRegistroCommand { get; set; }
        public Command RestablecerPasswdCommand { get; set; }
        #endregion

        #region Contructor
        public LoginViewModel()
        {
            loginGoogle = DependencyService.Get<ILoginGoogle>();
            LoginGoogleCommand = new Command(async () => await LoginGoogle());
            LoginCommand = new Command(async () => await Login());
            AbrirRegistroCommand = new Command(async () => await AbrirRegistro());
            RestablecerPasswdCommand = new Command(async () => await RestablecerPasswd());
            Preferences.Remove("Uid", null);
        }
        #endregion

        #region Ejecutar Comandos
        //Restablecer contraseña enviando un introduciendo un correo electrónico
        private async Task RestablecerPasswd()
        {
            IsBusy = true;
            try
            {
                var confirmacion = await App.Current.MainPage.DisplayPromptAsync("Correo: ", "Ingrese su correo Por favor");
                if (confirmacion == null)
                    await loginService.EnviarSolicitudCambioPasswd(confirmacion);
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", "Correo no valido", "Ok");
            }
            IsBusy = false;
        }
        //Abrir el fomulario de registro
        private async Task AbrirRegistro()
        {
            await App.Current.MainPage.Navigation.PushAsync(new RegistroView());
        }
        //Logearse con google
        private async Task LoginGoogle()
        {
            try
            {
                await Task.Run(()=>loginGoogle.IniciarSesionGoogle());
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", "Ha ocurrido algun error: "+ex.Message, "Ok");
            }
        }

        private async Task Login()
        {
            IsBusy = true;
            IsRunning = "True";
            try
            {
                Preferences.Set("Uid", null);
                //Si Email y Password es nulo mostrar advertencia
                if (string.IsNullOrEmpty(this.Email) || string.IsNullOrEmpty(this.Passwd))
                {
                    await Application.Current.MainPage.DisplayAlert("Alerta", "Llene todos los correctamente, ¡Porfavor!", "Ok");
                }
                //Si es un usuario administrador
                else if (this.Email.Contains("@allyservicios.com"))
                {
                    var login = await loginService.IniciarSesionAdministrador(this.Email, this.Passwd);
                    if(login)
                        App.Current.MainPage = new NavigationPage(new AdministradorPage());
                    else
                        await App.Current.MainPage.DisplayAlert("Alerta", loginService.MensajeError, "Ok");
                }
                //Validar Si cotiene @ y la contraseña es mayor a 6 caracteres
                else if(this.Email.Contains("@") && this.Passwd.Length >= 6)
                {
                    var login = await loginService.IniciarSesion(this.Email, this.Passwd);
                    if (login)
                        App.Current.MainPage = new NavigationPage(new ContratistaView());
                    else
                        await App.Current.MainPage.DisplayAlert("Alerta", loginService.MensajeError, "Ok");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Alerta", "Llene todos los correctamente, ¡Porfavor!", "Ok");
                }   
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", loginService.MensajeError, "Ok");
            }
            IsRunning = "False";
            IsBusy = false;
        }

        #endregion

    }
}
