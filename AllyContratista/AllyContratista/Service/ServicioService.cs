using AllyContratista.Model;
using AllyContratista.Model.Consultas;
using AllyContratista.Model.Query;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AllyContratista.Service
{
    public class ServicioService
    {
        #region Instancias
        readonly FirebaseService firebaseService = new FirebaseService();
        readonly UsuarioService usuarioService = new UsuarioService();
        readonly EvaluacionService  evaluacionService = new EvaluacionService();
        #endregion

        public string MensajeError { get; set; }

        #region Servicios del contrtista

        #region Lista de servicios
        //Obtener una lista con los servicios generados por el contratista con un promedio de las notas recibidas
        public async Task<List<ServicioModel>> ObtenerServicios()
        {
            try
            {
                var contratista = Preferences.Get("Uid", null);
                var listaServicios = (await firebaseService.firebase
                .Child("Servicios/")
                .OnceAsync<ServicioModel>()).Select(item => new ServicioModel
                {
                    IdServicio = item.Object.IdServicio,
                    TituloServicio = item.Object.TituloServicio,
                    TipoServicio = item.Object.TipoServicio,
                    IdContratista = item.Object.IdContratista,
                    Descripcion = item.Object.Descripcion,
                    Precio = item.Object.Precio,                  
                    FechaCreacion = item.Object.FechaCreacion,
                    Bloquear = item.Object.Bloquear,
                    Estado = item.Object.Estado,
                    Apoyo = evaluacionService.PromedioNotas(item.Object.IdServicio).Result
                }).Where(a => a.IdContratista == contratista && !a.Bloquear).OrderByDescending(a => Convert.ToDateTime(a.FechaCreacion)).ToList();

                return listaServicios;

            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo Visualizar os servicios";
                Console.WriteLine("Error: "+ex.Message);
                return null;
            }
        }

        #region Obtener la lista de servcios del contratista
        //Lista de servicios con un contador de clientes por servicio
        public async Task<List<ServicioModel>> ObtenerServiciosContador()
        {
            try
            {
                var contratista = Preferences.Get("Uid", null);
                var resultado = (await firebaseService.firebase
                .Child("Servicios/")
                .OnceAsync<ServicioModel>()).Select(item => new ServicioModel
                {
                    TituloServicio = item.Object.TituloServicio,
                    Precio = item.Object.Precio,
                    Descripcion = item.Object.Descripcion,
                    TipoServicio = item.Object.TipoServicio,
                    IdServicio = item.Object.IdServicio,
                    Estado = item.Object.Estado,
                    IdContratista = item.Object.IdContratista,
                    FechaCreacion = item.Object.FechaCreacion,
                    Bloquear = item.Object.Bloquear,
                    Apoyo = this.ContarOrdenesPorServicio(item.Object.IdServicio)
                }).Where(a => a.IdContratista == contratista && a.Estado && !a.Bloquear)
                .OrderByDescending(a => Convert.ToDateTime(a.FechaCreacion)).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo Visualizar os servicios";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        //Contar el total de ordenes por servicios
        public string ContarOrdenesPorServicio(string idServicio)
        {
            try
            {
                OrdenServicioService ordenServicio = new OrdenServicioService();
                var lista = ordenServicio.ConsultarServiciosPorIdServicio(idServicio);
                return lista.Result.Count().ToString();
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo retornar el Servicio";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #endregion

        #region Buscar servicios por IdServicio
        //Consulta servicio por Id
        public async Task<ServicioModel> BuscarServicioId(string idServicio)
        {
            try
            {
                var resultado = (await firebaseService.firebase
                .Child("Servicios/")
                .OnceAsync<ServicioModel>()).FirstOrDefault(a => a.Object.IdServicio == idServicio);

                ServicioModel serviciosModel = new ServicioModel
                {
                    TituloServicio = resultado.Object.TituloServicio,
                    Precio = resultado.Object.Precio,
                    Descripcion = resultado.Object.Descripcion,
                    TipoServicio = resultado.Object.TipoServicio,
                    IdServicio = resultado.Object.IdServicio,
                    Estado = resultado.Object.Estado,
                    IdContratista = resultado.Object.IdContratista,
                    Bloquear = resultado.Object.Bloquear,
                    FechaCreacion = resultado.Object.FechaCreacion,
                };
                //Retornar el modelo de Servicio con los datos actualizados
                return serviciosModel;
            }
            catch (Exception ex)
            {
                MensajeError = "Error no se recupero la orden de servicio";
                Console.WriteLine("Error: "+ ex.Message);
                return null;
            }
        }
        #endregion

        #region Metodo Registrar nuevo servicio
        public async Task NuevoServicio(ServicioModel serviciosModel, EvaluacionModel evaluacionModel)
        {
            try
            {
                await firebaseService.firebase
                   .Child("Servicios/")
                   .PostAsync(serviciosModel);
                //Crear una evaluacion por cada servicio generado
                await evaluacionService.CrearEvaluacion(evaluacionModel);
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo Ingresar el servicio, a la Base de datos, COnsulte con el administrador";
                Console.WriteLine("Error: "+ex.Message);
            }
        }
        #endregion

        #region Actalizar los servicios
        //Método de actualización de serivcios
        public async Task ActualizarServicio(ServicioModel servicioModel)
        {
            try
            {
                var consulta = (await firebaseService.firebase
                .Child("Servicios/")
                .OnceAsync<ServicioModel>()).FirstOrDefault(a => a.Object.IdServicio == servicioModel.IdServicio);

                await firebaseService.firebase
                    .Child("Servicios/")
                    .Child(consulta.Key)
                    .PutAsync(servicioModel);
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo Actualizar el servicio, a la Base de datos, COnsulte con el administrador";
                Console.WriteLine("Error: "+ ex.Message);
            }

        }
        #endregion

        #region Generar el Id de los nuevos Servicios
        //Generar Id para los serivicios
        public string GenerarId()
        {
            try
            {
                Guid miGuid = Guid.NewGuid();
                string token = Convert.ToString(miGuid.ToString().Replace("-", string.Empty).Substring(0, 5));
                string fecha = DateTime.Now.ToString("dd/MM/yy fffff").Replace("/", string.Empty).Replace(" ", string.Empty);

                //Retrna una clave unica generarda con una cadena de caracteres y la fecha actual
                return "S" + token + "-" + fecha;
            }
            catch (Exception ex)
            {
                MensajeError = "Error, Consulte con el administrador";
                Console.WriteLine("Error: "+ex.Message);
                return null;
            }

        }
        #endregion

        #endregion

        #region Administrador

        #region Consultar Servicios
        //Consultar todos los servicios disponibles
        public async Task<List<ServicioModel>> ConsultarServicios()
        {
            try
            {
                var resultado = (await firebaseService.firebase
               .Child("Servicios/")
               .OnceAsync<ServicioModel>()).Select(item => new ServicioModel
               {
                   TituloServicio = item.Object.TituloServicio,
                   Precio = item.Object.Precio,
                   Descripcion = item.Object.Descripcion,
                   TipoServicio = item.Object.TipoServicio,
                   IdServicio = item.Object.IdServicio,
                   Estado = item.Object.Estado,
                   IdContratista = item.Object.IdContratista,
                   FechaCreacion = item.Object.FechaCreacion,
                   Bloquear = item.Object.Bloquear,
                   Apoyo = ObtenerNombre(item.Object.IdContratista)
               }).OrderByDescending(a => Convert.ToDateTime(a.FechaCreacion)).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo extraer los datos";
                Console.WriteLine("Error: "+ex.Message);
                return null;
            }
        }

        #endregion

        #region Obtener Nombre del Contratista
        //Obtener el nombre del contratista
        public string ObtenerNombre(string idContratista)
        {
            try
            {
                var usuarioModel = usuarioService.BuscarUsuario(idContratista);
                string nombre = usuarioModel.Result.Nombre + " " + usuarioModel.Result.Apellido;
                return nombre;
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo consultar el nombre";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #endregion
    }
}
