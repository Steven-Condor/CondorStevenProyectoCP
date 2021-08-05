using AllyCliente.Service;
using AllyCliente.View;
using AllyCliente.View.Cliente;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyCliente
{
    public partial class App : Application
    {
        UsuarioService usuarioService = new UsuarioService();
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            var verificar = Task.Run(()=>VerificarUsuario());
            if (!verificar.Result)
                MainPage = new NavigationPage(new LoginView());
            else
                MainPage = new NavigationPage(new ClienteView());
        }

        public async Task<bool> VerificarUsuario()
        {
            var userId = Preferences.Get("Uid", null);
            var usuario =  await usuarioService.BuscarUsuario(userId);
            if(usuario == null)
            {
                return false;
            }
            else if (usuario.Estado == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
