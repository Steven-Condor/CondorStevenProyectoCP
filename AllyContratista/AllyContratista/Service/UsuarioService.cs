using AllyContratista.Interface;
using AllyContratista.Model;
using Firebase.Auth;
using Firebase.Database.Query;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyContratista.Service
{
    public class UsuarioService
    {
        #region Instancias
        readonly FirebaseService firebaseService = new FirebaseService();
        readonly LoginService loginService = new LoginService();
        #endregion

        public string MensajeError{ get; set; }

        #region Contratista

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
                Preferences.Set("Uid", auth.User.LocalId);
                await authProvider.SendEmailVerificationAsync(auth);
                usuario.IdUsuario = auth.User.LocalId;
                await this.CrearUsuarioDB(usuario);
                return true;
            }
            catch (FirebaseAuthException ex)
            {
                string mensaje = ex.Message;
                if (mensaje.Contains("InvalidEmailAddress"))
                {
                    MensajeError = "Dirección de correo electrónica invalida, Ingrese otra porfavor";
                    Console.WriteLine("Error: " + mensaje);
                    return false;
                }
                else if (mensaje.Contains("WeakPassword"))
                {
                    MensajeError = "La Contraseña debe tener mas de 6 Caracteres";
                    Console.WriteLine("Error: " + mensaje);
                    return false;
                }
                else if (mensaje.Contains("EmailExists"))
                {
                    MensajeError = "El Email ingresado ya existe en la base de datos";
                    Console.WriteLine("Error: " + mensaje);
                    return false;
                }
                else
                {
                    MensajeError = " Error consulte con el administrador";
                    Console.WriteLine("Error: "+ mensaje);
                    return false;
                }
            }
        }
        #endregion

        #region Crear Usuario En la base de datos
        //Almancenar usuario en la base de datos
        public async Task CrearUsuarioDB(UsuarioModel usuario)
        {
            usuario.Passwd = null;
            await firebaseService.firebase
                .Child("Usuarios/")
                .PostAsync(usuario);
        }
        #endregion

        #region Buscar Usuario
        //Buscra Usuario a traves de Id
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
                    Estado = resultado.Object.Estado
                };

                return usuario;
            }
            catch (Exception ex)
            {
                MensajeError = "Error no se recupero el usuario";
                Console.WriteLine("Error: "+ ex.Message);
                return null;
            }
        }
        #endregion

        #region Actualizar Usuario
        //Actualizar usuario
        public async Task<bool> ActualzarUsuario(UsuarioModel usuarioModel)
        {
            try
            {
                //Obtener la clave del ususario
                var consulta = (await firebaseService.firebase
                    .Child("Usuarios")
                    .OnceAsync<UsuarioModel>()).FirstOrDefault(a => a.Object.IdUsuario == usuarioModel.IdUsuario);

                await firebaseService.firebase
                    .Child("Usuarios/" + consulta.Key)
                    .PutAsync(usuarioModel);

                return true;
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo ingresar el servicio";
                Console.WriteLine("Error: "+ ex.Message);
                return false;
            }
        }
        #endregion

        #region Subir foto
        //Almacenar fotografia en Firebase Storage
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
                Console.WriteLine("Error: "+ ex.Message);
                return null;
            }
        }
        #endregion

        #region Cambiar Contraseña
        //Enviar un enlace de cambio de contraseña al correo
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
                MensajeError = "Su sesion ha expirado. Cierre la sesion y vuelva a iniciar para realizar el cambio";
                Console.WriteLine("Error: "+ ex.Message);
                return false;
            }
        }
        #endregion

        #endregion

        #region Administrador

        #region Restornar lista de Usuarios
        //Obtener una lista de usuarios regitsrados en la base de datos
        public async Task<List<UsuarioModel>> ListaUsuarios()
        {
            try
            {
                var resultado = (await firebaseService.firebase
                    .Child("Usuarios")
                    .OnceAsync<UsuarioModel>()).Select( u => new UsuarioModel()
                    {
                        Apellido = u.Object.Apellido,
                        Nombre = u.Object.Nombre,
                        IdUsuario = u.Object.IdUsuario,
                        Cedula = u.Object.Cedula,
                        Email = u.Object.Email,
                        Imagen = u.Object.Imagen,
                        Estado = u.Object.Estado,
                        TipoUsuario = u.Object.TipoUsuario,
                        Telefono = u.Object.Telefono
                    }).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                MensajeError = "No existen registros";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #endregion
        //Descifrar las contraseñas
        private static string DesCifrar(string cadenaAdesencriptar)
        {
            byte[] decryted = Convert.FromBase64String(cadenaAdesencriptar);
            string result = System.Text.Encoding.Unicode.GetString(decryted);
            return result;
        }

    }
}
