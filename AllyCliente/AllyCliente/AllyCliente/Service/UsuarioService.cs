using AllyCliente.Model;
using AllyCliente.Service;
using Firebase.Auth;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyCliente.Service
{
    public class UsuarioService
    {
        #region Instancias
        readonly FirebaseService firebaseService = new FirebaseService();
        #endregion

        public string MensajeError{ get; set; }

        #region Servicios

        #region Registar Usuario Firebase Auth
        public async Task<bool> Registrar(UsuarioModel usuario)
        {
            try
            {
                //Contactar con el servidor de autentcacion
                var authProvider = firebaseService.authProvider;

                //Crear el nuevo usuario a traves de un email
                var auth = await authProvider
                    .CreateUserWithEmailAndPasswordAsync(usuario.Email, DesCifrar(usuario.Passwd));

                //Obtener el token
                string getToken = auth.FirebaseToken;
                    
                var correoVerificacion = authProvider.SendEmailVerificationAsync(auth).IsCompleted;
                usuario.IdUsuario = auth.User.LocalId;
                await this.CrearUsuarioDB(usuario);
                await authProvider.DeleteUserAsync(getToken);
                Preferences.Set("Uid", auth.User.LocalId);
                return true;
                
            }
            catch (FirebaseAuthException ex)
            {
                string mensaje = ex.Message;
                if (mensaje.Contains("InvalidEmailAddress"))
                {
                    MensajeError = "Dirección de correo electrónica no valida";
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
                else if (mensaje.Contains("WeakPassword"))
                {
                    MensajeError = "La Contraseña debe tener mas de 6 Caracteres";
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
                else if (mensaje.Contains("EmailExists"))
                {
                    MensajeError = "El Email ingresado ya existe en la base de datos";
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
                else
                {
                    MensajeError = " Error consulte con el administrador";
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }
        #endregion

        #region Crear Usuario En la base de datos
        public async Task CrearUsuarioDB(UsuarioModel usuario)
        {
            usuario.Passwd = null;
            await firebaseService.firebase
                .Child("Usuarios/")
                .PostAsync(usuario);
        }
        #endregion

        #region Buscar Usuario
        public async Task<UsuarioModel> BuscarUsuario(string idUsuario)
        {
            try
            {
                var resultado = (await firebaseService.firebase
                .Child("Usuarios/")
                .OnceAsync<UsuarioModel>()).FirstOrDefault(a => a.Object.IdUsuario == idUsuario);

                UsuarioModel usuario = new UsuarioModel
                {
                    Email = resultado.Object.Email,
                    Telefono = resultado.Object.Telefono,
                    Nombre = resultado.Object.Nombre,
                    Apellido = resultado.Object.Apellido,
                    Imagen = resultado.Object.Imagen,
                    IdUsuario = resultado.Object.IdUsuario,
                    Cedula = resultado.Object.Cedula,
                    FechaCreacion = resultado.Object.FechaCreacion,
                    TipoUsuario = resultado.Object.TipoUsuario,
                    Passwd = resultado.Object.Passwd,
                    Estado = resultado.Object.Estado
                };

                return usuario;
            }
            catch (Exception ex)
            {
                MensajeError = "Error no se recupero el usuario";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region Actualizar Usuario
        public async Task ActualzarUsuario(UsuarioModel usuarioModel)
        {
            try
            {
                var consulta = (await firebaseService.firebase
                    .Child("Usuarios")
                    .OnceAsync<UsuarioModel>()).FirstOrDefault(a => a.Object.IdUsuario == usuarioModel.IdUsuario);

                await firebaseService.firebase
                    .Child("Usuarios/" + consulta.Key)
                    .PutAsync(usuarioModel);
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo ingresar el servicio";
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        #endregion

        #region Subir foto a Firebase
        public async Task<string> SubirFoto(Stream file, string nombreArchivo, string tipoUsuario, string userId)
        {
            try
            {
                var resultado = await firebaseService.firebaseStorage
                    .Child("Users/" + tipoUsuario)
                    .Child(nombreArchivo.Replace(nombreArchivo, userId))
                    .PutAsync(file);
                return resultado;

            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo ingresar el servicio";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region Cambiar Contraseña Firebase Auth
        public async Task<bool> CambiarPasswd(string email)
        {
            try
            {
                await firebaseService.authProvider
                    .SendPasswordResetEmailAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                MensajeError = "Su sesion ha expirado. Vuelva a iiciar Sesion";
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        #endregion

        #endregion

        #region Utilidades de la clase
        private static string DesCifrar(string cadenaAdesencriptar)
        {
            byte[] decryted = Convert.FromBase64String(cadenaAdesencriptar);
            var result = System.Text.Encoding.Unicode.GetString(decryted);
            return result;
        }
        #endregion
    }
}
