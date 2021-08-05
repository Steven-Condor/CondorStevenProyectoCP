using AllyCliente.Model;
using AllyCliente.View.Cliente;
using Firebase.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyCliente.Service
{
    public class LoginService
    {
        #region Instancias
        private readonly FirebaseService firebaseSerivice = new FirebaseService();
        #endregion

        #region Atributos
        public string MensajeError { get; set; }
        public bool EmailVerificacion { get; set; }
        #endregion

        #region Serivios de Autenticación
        //Enviar Email de solicitud de cambio de contraseña
        public async Task<bool> EnviarSolicitudCambioPasswd(string email)
        {
            try
            {
                await firebaseSerivice.authProvider
                    .SendPasswordResetEmailAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> IniciarSesion(string email, string passwd)
        {
            try
            {
                var authProvider = firebaseSerivice.authProvider;
                //Validar las credenciales ingresadas
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, passwd);
                var token = auth.FirebaseToken;

                //Guardar elcontenido de la sesion en una variable
                var content = auth.GetFreshAuthAsync();

                //Serializar el contenido en un string tipo JSON
                var serializedContent = JsonConvert.SerializeObject(content);

                //Guardar contenido en una variable del sistema
                Preferences.Set("MyFirebaseRefreshContent", serializedContent);
                Preferences.Set("TokenSesion", token);
                Preferences.Set("Uid", auth.User.LocalId);

                EmailVerificacion = auth.User.IsEmailVerified;

                if (EmailVerificacion == false)
                    return false;
                else
                    return true;
            }
            catch (FirebaseAuthException ex)
            {
                MensajeError = "Ha sucedido un error con su sesion, verifique sus datos porfavor";
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
            
        }

        #region Cerrar Sesion
        //Cerrar sesion
        public bool CerrarSesion()
        {
            try
            {
                Preferences.Clear();
                return true;
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo cerrar sesion";
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        #endregion

        //Iniciar sesion con una Cuenta de Google
        public async Task IniciarSesionGoogle(UsuarioModel usuario, string tokenId)
        {
            var auth = await firebaseSerivice.authProvider.SignInWithGoogleIdTokenAsync(tokenId);
            var token = auth.FirebaseToken;
            var content = auth.GetFreshAuthAsync();
            var serializedContent = JsonConvert.SerializeObject(content);
            Preferences.Set("MyFirebaseRefreshContent", serializedContent);
            Preferences.Set("TokenSesion", token);
            Preferences.Set("Uid", auth.User.LocalId);
			
            UsuarioService usuarioService = new UsuarioService();
            usuario.IdUsuario = auth.User.LocalId;
            var user = await usuarioService.BuscarUsuario(auth.User.LocalId);
            if(user.IdUsuario == null)
                await usuarioService.CrearUsuarioDB(usuario);
        }

        #endregion
        //Abrir la pagina principal del contratista despues del inicio de sesion con google
        public void AbrirPagina()
        {
            App.Current.MainPage = new NavigationPage(new ClienteView());
        }
    }
}
