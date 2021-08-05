using AllyContratista.Model;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllyContratista.Service
{
    public class TipoServicioService
    {
        readonly FirebaseService firebaseService = new FirebaseService();

        #region Parametros
        public string MensajeError { get; set; }
        #endregion

        #region Recuperar Tipos de Servicios
        //Obtener lista de Tipos de Traeas o Cataegorias 
        public async Task<List<TipoServicioModel>> ConsultarTiposServicios()
        {
            try
            {
                var resultado = (await firebaseService.firebase
                    .Child("TipoServicios")
                    .OnceAsync<TipoServicioModel>()).Select(item => new TipoServicioModel
                    {
                        IdTipoServicio = item.Object.IdTipoServicio,
                        TipoServicio = item.Object.TipoServicio,
                        Imagen = item.Object.Imagen,
                        Descripcion = item.Object.Descripcion,
                        Estado = item.Object.Estado
                    }).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                MensajeError = "No se retorno ningun valor";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region Buscar tipo de servicio
        //Buscar los tipos de servicio
        public async Task<TipoServicioModel> BuscarTipoServicio(string _tipoServicio)
        {
            try
            {
                var resultado = (await firebaseService.firebase
                    .Child("TipoServicios")
                    .OnceAsync<TipoServicioModel>()).FirstOrDefault(a => a.Object.TipoServicio == _tipoServicio);

                TipoServicioModel tipoServicio = new TipoServicioModel()
                {
                    IdTipoServicio = resultado.Object.IdTipoServicio,
                    Estado = resultado.Object.Estado,
                    Imagen = resultado.Object.Imagen,
                    TipoServicio = resultado.Object.TipoServicio,
                    Descripcion = resultado.Object.TipoServicio
                };
                //Retornar el modelo de Tipo de servicio con los datos actualizados
                return tipoServicio;
            }
            catch (Exception ex)
            {
                MensajeError = "No se retorno ningun valor";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region Administrador

        #region Actualizar Servicios
        //Método de actualizar Tipo de serivicios 
        public async Task ActualizarTipoServicios(TipoServicioModel tipoServicio)
        {
            try
            {
                var resultado = (await firebaseService.firebase
                    .Child("TipoServicios")
                    .OnceAsync<TipoServicioModel>()).FirstOrDefault(a => a.Object.IdTipoServicio == tipoServicio.IdTipoServicio);

                await firebaseService.firebase.Child("TipoServicios/" + resultado.Key).PutAsync(tipoServicio);

            }
            catch (Exception ex)
            {
                MensajeError = "No se retorno ningun valor";
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        #endregion

        #region Actualizar Servicios
        //Generar nuevos tipos de servicios
        public async Task NuevoTipoServicios(TipoServicioModel tipoServicio)
        {
            try
            {
                tipoServicio.IdTipoServicio = GenerarId();
                await firebaseService.firebase
                    .Child("TipoServicios")
                    .PostAsync(tipoServicio);

            }
            catch (Exception ex)
            {
                MensajeError = "No se retorno ningun valor";
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        #endregion

        #region Subir foto
        //Subir fotografa a Firebase Storage y retornar la url del archivo
        public async Task<string> SubirFoto(Stream file, string nombreArchivo, string tipoUsuario)
        {
            try
            {
                var resultado = await firebaseService.firebaseStorage
                    .Child("Users/" + tipoUsuario)
                    .Child(nombreArchivo.Replace(nombreArchivo, tipoUsuario))
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

        private string GenerarId()
        {
            Guid miGuid = Guid.NewGuid();
            string token = Convert.ToString(miGuid.ToString().Replace("-", string.Empty).Substring(0, 5));
            string fecha = DateTime.Now.ToString("dd/MM/yy").Replace("/", string.Empty).Replace(" ", string.Empty);

            //Retrna una clave unica generarda con una cadena de caracteres y la fecha actual
            return "TS" + token + "-" + fecha;
        }

        #endregion
    }
}
