using AllyCliente.Model;
using AllyCliente.Model.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace AllyCliente.Service
{
    public class ReportesService
    {
        #region Intancias
        readonly FirebaseService firebaseService = new FirebaseService();
        #endregion

        public string MensajeError { get; set; }

        #region Resportes del Cliente
        public async Task<List<ReporteMesQuery>> OrdenesPorMes(string anio)
        {
            try
            {
                //Extraer los datos del cliente
                var cliente = Preferences.Get("Uid", null);

                //Extraer los datos de las ordenes de servicio generadas por el cliente
                var resultado = (await firebaseService.firebase
                    .Child("OrdenesServicios/")
                    .OnceAsync<OrdenesServicioModel>()).Where(a => a.Object.IdCliente == cliente).ToList();

                //Genear la consuta con linq para el reporte de ordenes por año y mes
                var query = from s in resultado
                            where Convert.ToDateTime(s.Object.Fechas.FechaCreacion).Year == Convert.ToInt32(anio)
                            group s by new
                            {
                                mes = Convert.ToDateTime(s.Object.Fechas.FechaCreacion).ToString("MMMM"),
                                anio = Convert.ToDateTime(s.Object.Fechas.FechaCreacion).ToString("yyyy")
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

        #region Utilidades del resporte
        private string GenerarColor()
        {
            var random = new Random();
            var color = String.Format("#{0:X6}", random.Next(0x1000000));
            return color;
        }
        #endregion
    }
}
