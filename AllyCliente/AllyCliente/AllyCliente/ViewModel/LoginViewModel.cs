using AllyCliente.Interface;
using AllyCliente.Model;
using AllyCliente.Service;
using AllyCliente.View.Cliente;
using AllyCliente.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyCliente.ViewModel
{
    public class LoginViewModel:LoginModel
    {
        #region Instancias
        private readonly LoginService loginService = new LoginService();
        readonly ILoginGoogle loginGoogle;
        readonly IMessageService messageService;
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
        #endregion

        #region Comandos
        public Command LoginCommand { get; set; }
        public Command LoginGoogleCommand { get; set; }
        public Command AbrirRegistroCommand { get; set; }
        public Command RestablecerPasswdCommand { get; set; }
        #endregion

        #region Contructor
        public LoginViewModel()
        {
            messageService = DependencyService.Get<IMessageService>();
            loginGoogle = DependencyService.Get<ILoginGoogle>();
            LoginGoogleCommand = new Command(async () => await LoginGoogle());
            LoginCommand = new Command(async () => await Login());
            AbrirRegistroCommand = new Command(async () => await AbrirRegistro());
            RestablecerPasswdCommand = new Command(async () => await RestablecerPasswd());
            Preferences.Remove("Uid", null);
        }
        #endregion

        #region Ejecutar Comandos

        private async Task RestablecerPasswd()
        {
            IsBusy = true;
            var confirmacion = await App.Current.MainPage.DisplayPromptAsync("Correo: ", "Ingrese su correo Por favor");
            if (confirmacion == null)
                await loginService.EnviarSolicitudCambioPasswd(confirmacion);
            else
                messageService.ShortAlert("Correo no valido");
            IsBusy = false;
        }

        private async Task AbrirRegistro()
        {
            await App.Current.MainPage.Navigation.PushAsync(new RegistroView());
        }

        private async Task LoginGoogle()
        {
            try
            {
                await Task.Run(() => loginGoogle.IniciarSesionGoogle());
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alerta", "Ha ocurrido algun error: " + ex.Message, "Ok");
            }
        }

        private async Task Login()
        {
            IsBusy = true;
            IsRunning = "True";
            Preferences.Set("Uid", null);
            if (string.IsNullOrEmpty(this.Email) || string.IsNullOrEmpty(this.Passwd))
            {
                await Application.Current.MainPage.DisplayAlert("Alerta", "Llene todos los correctamente, ¡Porfavor!", "Ok");
            }
            else if (this.Email.Contains("@") && this.Passwd.Length >= 6)
            {
                var login = await loginService.IniciarSesion(this.Email, this.Passwd);
                if (login)
                    App.Current.MainPage = new NavigationPage(new ClienteView());
                else
                    messageService.ShortAlert(loginService.MensajeError);
            }
            else    
            {
                await Application.Current.MainPage.DisplayAlert("Alerta", "Llene todos los correctamente, ¡Porfavor!", "Ok");
            }    
            IsRunning = "False";
            IsBusy = false;
        }

        #endregion

    }
}
