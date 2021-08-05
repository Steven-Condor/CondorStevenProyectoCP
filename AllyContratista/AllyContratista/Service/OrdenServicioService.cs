using AllyContratista.Model;
using AllyContratista.Model.Consultas;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllyContratista.Service
{
    public class OrdenServicioService
    {
        #region Instancias
        readonly FirebaseService firebaseService = new FirebaseService();
        readonly UsuarioService usuarioService = new UsuarioService();
        #endregion

        public string MensajeError { get; set; }

        #region Consultar Ordenes de serivicios
        //Conultar ordenes por cada servicio generado por parte del contratista
        public async Task<List<OrdenServicioModel>> ConsultarServiciosPorIdServicio(string idServicio)
        {
            try
            {
                var resultado = (await firebaseService.firebase
                .Child("OrdenesServicios/")
                .OnceAsync<OrdenServicioModel>()).Select(item => new OrdenServicioModel
                {
                    IdServicio = item.Object.IdServicio,
                    IdCliente = item.Object.IdCliente,
                    IdOrden = item.Object.IdOrden,
                    Estado = item.Object.Estado,
                    Pago = item.Object.Pago,
                    Fechas = new FechasModel
                    {
                        FechaCreacion = item.Object.Fechas.FechaCreacion,
                        FechaAceptacion = item.Object.Fechas.FechaAceptacion,
                        FechaAgendada = item.Object.Fechas.FechaAgendada,
                        FechaCancelacion = item.Object.Fechas.FechaCancelacion,
                        FechaFinalizacion = item.Object.Fechas.FechaFinalizacion
                    }
                }).Where(a => a.IdServicio == idServicio).OrderByDescending(a => a.Fechas.FechaCreacion).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo extraer los datos";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        #endregion

        #region Consulta Orden Servicio Join
        //Obtener las ordenes de servicio con una conuslta Join Linq
        public async Task<List<OrdenServicioQuery>> ConsultarOrdenServcioJoin(string idServicio, bool desc)
        {
            try
            {
                var usuarios = await usuarioService.ListaUsuarios();

                var servicio = (await firebaseService.firebase
                    .Child("Servicios")
                    .OnceAsync<ServicioModel>()).Select(item => new ServicioModel
                    {
                        IdServicio = item.Object.IdServicio,
                        IdContratista = item.Object.IdContratista,
                        TituloServicio = item.Object.TituloServicio
                    }).ToList();

                var orden = await ConsultarServiciosPorIdServicio(idServicio);

                var query = from o in orden
                            join u in usuarios on o.IdCliente equals u.IdUsuario
                            join s in servicio on o.IdServicio equals s.IdServicio
                            join c in usuarios on s.IdContratista equals c.IdUsuario
                            select new OrdenServicioQuery
                            {
                                NombreContratista = c.Nombre + " " + c.Apellido,
                                NombreCliente = u.Nombre + " " + u.Apellido,
                                NombreServicio = s.TituloServicio,
                                IdOrden = o.IdOrden,
                                Estado = o.Estado,
                                FechaCreacion = o.Fechas.FechaCreacion,
                                Imagen = u.Imagen
                            };

                if (desc == true)
                    return query.OrderByDescending(i => Convert.ToDateTime(i.FechaCreacion)).ToList();
                else
                    return query.OrderBy(i => Convert.ToDateTime(i.FechaCreacion)).ToList();
                
            }
            catch (Exception ex)
            {
                MensajeError = "Error no se pudo retornar la consulta";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        //Consulta
        public async Task<List<OrdenServicioQuery>> ConsultarOrdenesAgenda(string idContratista)
        {
            try
            {
                var usuarios = await usuarioService.ListaUsuarios();
                //Buscar servicios generados por el contartista con los campos de IdServicio, Id Contratista y Titulod el servicio
                var servicio = (await firebaseService.firebase
                    .Child("Servicios")
                    .OnceAsync<ServicioModel>()).Select(item => new ServicioModel
                    {
                        IdServicio = item.Object.IdServicio,
                        IdContratista = item.Object.IdContratista,
                        TituloServicio = item.Object.TituloServicio
                    }).Where(i => i.IdContratista == idContratista).ToList();
                //Buscra orden de servicio con fecha de agendamiento
                var orden = (await firebaseService.firebase
                    .Child("OrdenesServicios")
                    .OnceAsync<OrdenServicioModel>()).Select(item => new OrdenServicioModel
                    {
                        IdOrden = item.Object.IdOrden,
                        IdServicio = item.Object.IdServicio,
                        IdCliente = item.Object.IdCliente,
                        Fechas = new FechasModel
                        {
                            FechaCreacion = item.Object.Fechas.FechaCreacion,
                            FechaAgendada = item.Object.Fechas.FechaAgendada
                        },
                        Estado = item.Object.Estado
                    }).Where(i => i.Fechas.FechaAgendada != null).ToList();
                //Consulta las ordenes de servicio con el nombre de cleinte y titulo del servicio
                var query = from o in orden
                            join u in usuarios on o.IdCliente equals u.IdUsuario
                            join s in servicio on o.IdServicio equals s.IdServicio
                            select new OrdenServicioQuery
                            {
                                NombreCliente = u.Nombre + " " + u.Apellido,
                                NombreServicio = s.TituloServicio,
                                IdOrden = o.IdOrden,
                                Estado = o.Estado,
                                FechaCreacion = o.Fechas.FechaAgendada,
                                Imagen = u.Imagen
                            };
                var res = query.ToList();

                return query.ToList();

            }
            catch (Exception ex)
            {
                MensajeError = "Error no se pudo retornar la consulta";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }


        #endregion

        #region Actualizar Estado
        //Método actualizar estado
        public async Task ActualizarEstado(OrdenServicioModel ordenesSericio)
        {
            try
            {
                //lamar al método OrdenServicioId -> Restorna la clave la coleccion orden de servicio
                var consulta = await OrdenServicioId(ordenesSericio.IdOrden);

                //Acuallizar los datos de orden de servicio
                await firebaseService.firebase
                    .Child("OrdenesServicios/")
                    .Child(consulta.Key)
                    .PutAsync(ordenesSericio);
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo actualizar el estado, conslte con el administrador";
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        #endregion

        #region Buscar Orden

        public async Task<OrdenServicioModel> BuscarOrden(string idOrden)
        {
            try
            {
                var resultado = await OrdenServicioId(idOrden);

                OrdenServicioModel ordenServicioModel = new OrdenServicioModel
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
                    }
                };
                //Retornar un modelo de datos con la información actualizada.
                return ordenServicioModel;
            }
            catch (Exception ex)
            {
                MensajeError = "Error no se recupero la orden de servicio";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        //Obtener Objecto La clave de la orden de servicio
        private async Task<FirebaseObject<OrdenServicioModel>> OrdenServicioId(string idOrden)
        {
            return (await firebaseService.firebase
                .Child("OrdenesServicios/")
                .OnceAsync<OrdenServicioModel>()).FirstOrDefault(a => a.Object.IdOrden == idOrden);
        }

        #endregion

    }
}
