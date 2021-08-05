using AllyCliente.Model.Query;
using AllyCliente.Model;
using AllyCliente.Model.SubModel;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;

namespace AllyCliente.Service
{
    public class OrdenesServicioService
    {
        #region instancias
        readonly FirebaseService firebaseService = new FirebaseService();
        readonly ServiciosService serviciosService = new ServiciosService();
        #endregion

        #region Atributos
        public string MensajeError { get; set; }
        #endregion

        #region Generar Id unidco de la orden
        public string GenerarIdOrden()
        {
            try
            {
                Guid miGuid = Guid.NewGuid();
                string token = Convert.ToString(miGuid.ToString().Replace("-", string.Empty).Substring(0, 3));

                string fecha = DateTime.Now.ToString("dd/yy:ss:FFF").Replace("/", string.Empty).Replace(":", string.Empty);
                return "OS" + token + "-" + fecha;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }

        }
        #endregion

        #region Obtener Objeto Firebase con una Orden Servicio
        private async Task<FirebaseObject<OrdenesServicioModel>> OrdenServicioId(string idOrden)
        {
            return (await firebaseService.firebase
                .Child("OrdenesServicios/")
                .OnceAsync<OrdenesServicioModel>()).FirstOrDefault(a => a.Object.IdOrden == idOrden);
        }
        #endregion

        #region Actualizar Estado
        public async Task ActualizarEstado(OrdenesServicioModel ordenesSericio)
        {
            try
            {
                var consulta = await OrdenServicioId(ordenesSericio.IdOrden);

                await firebaseService.firebase
                    .Child("OrdenesServicios/")
                    .Child(consulta.Key)
                    .PutAsync(ordenesSericio);
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo actualizar el estado";
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        #endregion

        #region Buscar Orden

        public async Task<OrdenesServicioModel> BuscarOrden(string idOrden)
        {
            try
            {
                var resultado = await OrdenServicioId(idOrden);

                OrdenesServicioModel ordenServicioModel = new OrdenesServicioModel
                {
                    IdOrden = resultado.Object.IdOrden,
                    IdServicio = resultado.Object.IdServicio,
                    IdCliente = resultado.Object.IdCliente,
                    Pago = resultado.Object.Pago,
                    Estado = resultado.Object.Estado,
                    Fechas = new FechasModel
                    {
                        FechaAceptacion = resultado.Object.Fechas.FechaAceptacion,
                        FechaAgendada = resultado.Object.Fechas.FechaAgendada,
                        FechaCancelacion = resultado.Object.Fechas.FechaCancelacion,
                        FechaCreacion = resultado.Object.Fechas.FechaCreacion,
                        FechaFinalizacion = resultado.Object.Fechas.FechaFinalizacion
                    },
                    Calificado = resultado.Object.Calificado
                };

                return ordenServicioModel;
            }
            catch (Exception ex)
            {
                MensajeError = "Error no se recupero la orden de servicio";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        #endregion

        #region Insertar Nueva Orden de servicio

        public async Task CrearOrdenServicio(OrdenesServicioModel ordenesServicio)
        {
            try
            {
                await firebaseService.firebase
                    .Child("OrdenesServicios")
                    .PostAsync(ordenesServicio);
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo crear la orden de servicio";
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        #endregion

        #region Consultar Ordenes de servicio por Cliente

        public async Task<List<OrdenesServicioModel>> ConsultarOrdenesPorCliente(string IdCliente)
        {
            try
            {
                var listaOrdenes = (await firebaseService.firebase
                .Child("OrdenesServicios/")
                .OnceAsync<OrdenesServicioModel>()).Select(item => new OrdenesServicioModel
                {
                    IdServicio = item.Object.IdServicio,
                    IdCliente = item.Object.IdCliente,
                    IdOrden = item.Object.IdOrden,
                    Pago = item.Object.Pago,
                    Estado = item.Object.Estado,
                    Fechas = new FechasModel
                    {
                        FechaCreacion = item.Object.Fechas.FechaCreacion,
                        FechaAceptacion = item.Object.Fechas.FechaAceptacion,
                        FechaAgendada = item.Object.Fechas.FechaAgendada,
                        FechaFinalizacion = item.Object.Fechas.FechaFinalizacion,
                        FechaCancelacion = item.Object.Fechas.FechaCancelacion
                    },
                    Calificado = item.Object.Calificado,
                    NombreServicio = ObtenerNombreservicio(item.Object.IdServicio)
                }).Where(a => a.IdCliente == IdCliente).OrderByDescending(a => Convert.ToDateTime(a.Fechas.FechaCreacion)).ToList();

                return listaOrdenes;
             }
            catch (Exception ex)
            {
                MensajeError = "No se puede mostrar los datos";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        #endregion

        #region Obtener Nombre del servicio

        public string ObtenerNombreservicio(string idServicio)
        {
            var modelo = serviciosService.BuscarServicioId(idServicio);
            string nombreServicio = modelo.Result.TituloServicio;
            return nombreServicio;
        }

        #endregion

        #region ConsultaOrdenServicioJoin
        public async Task<List<OrdenServicioQuery>> ConsultarOrdenServcioJoin(string idCliente)
        {
            try
            {
                var servicio = (await firebaseService.firebase
                    .Child("Servicios")
                    .OnceAsync<ServiciosModel>()).Select(item => new ServiciosModel
                    {
                        IdServicio = item.Object.IdServicio,
                        IdContratista = item.Object.IdContratista,
                        TituloServicio = item.Object.TituloServicio
                    }).ToList();

                var orden = await ConsultarOrdenesPorCliente(idCliente);

                var query = from o in orden
                            join s in servicio on o.IdServicio equals s.IdServicio
                            where o.Fechas.FechaAgendada!=null
                            select new OrdenServicioQuery
                            {
                                NombreServicio = s.TituloServicio,
                                IdOrden = o.IdOrden,
                                Estado = o.Estado,
                                FechaCreacion = o.Fechas.FechaAgendada,
                            };
                var retornar = query.ToList();
                return retornar;
            }
            catch (Exception ex)
            {
                MensajeError = "Error no se pudo retornar la consulta";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        #endregion

    }
}
