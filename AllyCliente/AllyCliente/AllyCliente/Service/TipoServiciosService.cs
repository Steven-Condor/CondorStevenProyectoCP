using AllyCliente.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllyCliente.Service
{
    public class TipoServiciosService
    {
        readonly FirebaseService firebseService = new FirebaseService();

        #region Parametros
        private string mensajeError;
        public string MensajeError
        {
            get { return mensajeError; }
            set { mensajeError = value; }
        }

        #endregion

        #region Recuperar Tipos de Servicios
        public async Task<List<TipoServicioModel>> ConsultarTiposServicios()
        {
            try
            {
                var resultado = (await firebseService.firebase
                    .Child("TipoServicios")
                    .OnceAsync<TipoServicioModel>()).Select(item => new TipoServicioModel
                    {
                        TipoServicio = item.Object.TipoServicio,
                        Imagen = item.Object.Imagen,
                        Descripcion= item.Object.Descripcion
                    }).ToList();

                return resultado;
            }
            catch (Exception ex)
            {
                mensajeError = "Nose pudo obtener los servicios";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region Buscar tipo de servicio
        public async Task<TipoServicioModel> BuscarTipoServicio(string _tipoServicio)
        {
            try
            {
                var resultado = (await firebseService.firebase
                    .Child("TipoServicios")
                    .OnceAsync<TipoServicioModel>()).FirstOrDefault(a => a.Object.TipoServicio == _tipoServicio);

                TipoServicioModel tipoServicio = new TipoServicioModel()
                {
                    Imagen = resultado.Object.Imagen,
                    TipoServicio = resultado.Object.TipoServicio,
                    Descripcion = resultado.Object.TipoServicio
                };

                return tipoServicio;
            }
            catch (Exception ex)
            {
                mensajeError = "No se retorno ningun valor";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion
    }
}
