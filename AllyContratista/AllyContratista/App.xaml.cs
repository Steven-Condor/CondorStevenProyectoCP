using AllyContratista.Service;
using AllyContratista.View.Administrador;
using AllyContratista.View.Contratista;
using AllyContratista.View.Registro;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllyContratista
{
    public partial class App : Application
    {
        readonly UsuarioService usuarioService = new UsuarioService();
        public App()
        {
            InitializeComponent();
            var usuario = Task.Run(()=>Verificar());
            if (usuario.Result == "Administrador")
                MainPage = new NavigationPage(new AdministradorPage());
            else if (usuario.Result == "Contratista")
                MainPage = new NavigationPage(new ContratistaView());
            else
                MainPage = new NavigationPage(new LoginView());
        }

        public async Task<string> Verificar()
        {
            var user = Preferences.Get("Uid", null);
            var usuario = await usuarioService.BuscarUsuario(user);

            if (usuario != null && usuario.Estado == "true" && usuario.TipoUsuario == "Administrador")
                return "Administrador";
            else if (usuario != null && usuario.Estado == "true" && usuario.TipoUsuario == "Contratista")
                return "Contratista";
            else
                return "Login";
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
