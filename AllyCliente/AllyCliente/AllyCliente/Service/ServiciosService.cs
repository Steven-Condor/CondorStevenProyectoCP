using AllyCliente.Model;
using AllyCliente.Model.Query;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AllyCliente.Service
{
    public class ServiciosService
    {
        #region Instancias
        readonly FirebaseService firebaseService = new FirebaseService();
        readonly UsuarioService usuarioService = new UsuarioService();
        readonly EvaluacionService evaluacionService = new EvaluacionService();

        #endregion

        #region Parametros

        private string mensajeError;
        public string MensajeError
        {
            get { return mensajeError; }
            set { mensajeError = value; }
        }


        #endregion

        #region Devolver serivios por Tipo de Servicio
        public async Task<ObservableCollection<ServiciosQuery>> ConsultarServicios(string tipoServicio)
        {
            try
            {
                //Resultado develve como resultado una lista con datos extraidos desde Firebase
                var resultado = (await firebaseService.firebase
                .Child("Servicios/")
                .OnceAsync<ServiciosQuery>()).Select(item => new ServiciosQuery
                {
                    TituloServicio = item.Object.TituloServicio,
                    IdContratista = item.Object.IdContratista,
                    Precio = item.Object.Precio,
                    TipoServicio = item.Object.TipoServicio,
                    IdServicio = item.Object.IdServicio,
                    Estado = item.Object.Estado,
                    FechaCreacion = item.Object.FechaCreacion,
                    Bloquear = item.Object.Bloquear,
                    NombreContratista = ObtenerNombre(item.Object.IdContratista),
                    Nota = evaluacionService.PromedioNotas(item.Object.IdServicio).Result
                }).Where(a => a.TipoServicio == tipoServicio && a.Estado && !a.Bloquear).OrderByDescending(a => a.TituloServicio).ToList();

                var convertir = JsonConvert.SerializeObject(resultado);
                var lista = JsonConvert.DeserializeObject<ObservableCollection<ServiciosQuery>>(convertir);

                return lista;
            }
            catch (Exception ex)
            {
                mensajeError = "Error, Consulte con el administrador";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }           
        }

        #endregion

        #region Buscar servicios por IdServicio
        public async Task<ServiciosModel> BuscarServicioId(string idServicio)
        {
            try
            {
                var resultado = (await firebaseService.firebase
                .Child("Servicios/")
                .OnceAsync<ServiciosModel>()).FirstOrDefault(a => a.Object.IdServicio == idServicio);

                ServiciosModel serviciosModel = new ServiciosModel
                {
                    TituloServicio = resultado.Object.TituloServicio,
                    Precio = resultado.Object.Precio,
                    Descripcion = resultado.Object.Descripcion,
                    TipoServicio = resultado.Object.TipoServicio,
                    IdServicio = resultado.Object.IdServicio,
                    Estado = resultado.Object.Estado,
                    IdContratista = resultado.Object.IdContratista,
                    FechaCreacion = resultado.Object.FechaCreacion,
                    Bloquear = resultado.Object.Bloquear,
                    Apoyo = null
                };

                return serviciosModel;
            }
            catch (Exception ex)
            {
                mensajeError = "Error no se recupero la orden de servicio";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region  Buscar servicios po tipo

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
                mensajeError = "No se pudo consultar el nombre";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        #endregion

    }
}
