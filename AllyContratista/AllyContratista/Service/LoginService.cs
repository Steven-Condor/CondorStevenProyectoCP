using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.View.Contratista;
using Firebase.Auth;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyContratista.Service
{
    public class LoginService
    {
        #region Instancias
        private readonly FirebaseService firebaseSerivice = new FirebaseService();
        #endregion

        #region Parametros
        public string MensajeError { get; set; }
        public bool EmailVerificacion { get; set; }
        #endregion

        #region Serivios de Sesion
        //Enviar al correo electrónico un enlace para cambiar la contraseña
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

        //Metodo de acceso exclusivo para los administradores
        public async Task<bool> IniciarSesionAdministrador(string email, string passwd)
        {
            try
            {
                var authProvider = firebaseSerivice.authProvider;
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, passwd);
                var token = auth.FirebaseToken;

                var content = auth.GetFreshAuthAsync();

                var serializedContent = JsonConvert.SerializeObject(content);

                Preferences.Set("MyFirebaseRefreshContent", serializedContent);
                Preferences.Set("TokenSesion", token);
                Preferences.Set("Uid", auth.User.LocalId);

                return true;
            }
            catch (FirebaseAuthException ex)
            {
                MensajeError = "Ha sucedido un error con su sesion, verifique sus datos porfavor";
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        //Método de Inicio de sesion con una cuenta local
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
                    return false;//Cambiar mas adelante
                else
                    return true;
            }
            catch (FirebaseAuthException ex)
            {
                MensajeError = "Ha sucedido un error con su sesion, verifique sus datos porfavor";
                Console.WriteLine("Error: "+ ex.Message);
                return false;
            }
            
        }

        #region Cerrar Sesion
        //Ceerar sesion y elimianr los datos de sesion
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
        //Iniciar sesion con google a traves del token de la cuenta de google
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
            App.Current.MainPage = new NavigationPage(new ContratistaView());
        }
    }
}
