using AllyContratista.Model;
using AllyContratista.Model.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AllyContratista.Service
{
    public class ReportesService
    {
        #region Intancias
        readonly FirebaseService firebaseService = new FirebaseService();
        readonly ServicioService servicioService = new ServicioService();
        #endregion

        public string MensajeError { get; set; }

        #region Reportes
        //Reporte de servicios agrupados por servicios generados
        public async Task<List<ServicioModel>> ReportesServicio()
        {
            var contratista = Preferences.Get("Uid", null);
            try
            {
                var resultado = (await firebaseService.firebase
                .Child("Servicios/")
                .OnceAsync<ServicioModel>()).Select(item => new ServicioModel
                {
                    IdContratista = item.Object.IdContratista,
                    TituloServicio = item.Object.TituloServicio,
                    IdServicio = item.Object.IdServicio,
                    Estado = item.Object.Estado,
                    Descripcion = GenerarColor(),
                    Apoyo = servicioService.ContarOrdenesPorServicio(item.Object.IdServicio)
                }).Where(a => a.IdContratista == contratista).ToList();

                return resultado;

            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo Visualizar os servicios";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        //Obtener los servicios prestados agrupados por años 
        public async Task<List<ReporteMesQuery>> ServiciosPorMes(string anio)
        {
            try
            {
                CultureInfo cult = new CultureInfo("es-ES", false);
                var contratista = Preferences.Get("Uid", null);
                var resultado = (await firebaseService.firebase
                    .Child("Servicios")
                    .OnceAsync<ServicioModel>()).Where(a => a.Object.IdContratista == contratista).ToList();


                var query = from s in resultado
                            where Convert.ToDateTime(s.Object.FechaCreacion).Year == Convert.ToInt32(anio)
                            group s by new
                            {
                                mes = Convert.ToDateTime(s.Object.FechaCreacion).ToString("MMMM"),
                                anio = Convert.ToDateTime(s.Object.FechaCreacion).ToString("yyyy")
                            } into fecha
                            select new ReporteMesQuery()
                            {
                                Mes = fecha.Key.mes,
                                Anio = fecha.Key.anio,
                                Contar = fecha.Count(),
                                Color = GenerarColor()
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo generar el reporte";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region Utilidades del reporte
        //Generar codigo de colores hexageximales aleatorios para los resportes
        private string GenerarColor()
        {
            var random = new Random();
            var color = String.Format("#{0:X6}", random.Next(0x1000000));
            return color;
        }
        #endregion
    }
}
