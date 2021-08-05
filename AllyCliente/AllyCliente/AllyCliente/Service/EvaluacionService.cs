using AllyCliente.Model;
using AllyCliente.Model.SubModel;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllyCliente.Service
{
    public class EvaluacionService
    {
        readonly FirebaseService firebaseService = new FirebaseService();

        public string MensajeError { get; set; }

        #region Calificar Servicio

        public async Task CalificarServicio(string Key, NotasModel nota)
        {
            try
            {
                await firebaseService.firebase
                    .Child("Evaluaciones/" + Key + "/Notas")
                    .PostAsync(nota);
            }
            catch (Exception ex)
            {
                MensajeError = "No se a podido calficar este ervicio;";
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        #endregion

        #region Obteter Key del Servicio

        public async Task<string> ConsultarEvaluacion(string idServicio)
        {
            try
            {
                var evaluacion = (await firebaseService.firebase
                                    .Child("Evaluaciones")
                                    .OnceAsync<EvaluacionModel>()).FirstOrDefault(a => a.Object.IdServicio == idServicio);
                var id = evaluacion.Object.IdEvaluacion;
                var key = evaluacion.Key;
                return key;
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo obtener la evaluacion";
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        #endregion

        #region Promedio de Notas del Servicio

        public async Task<string> PromedioNotas(string idServicio)
        {
            try
            {
                var evaluaciones = (await firebaseService.firebase
                    .Child("Evaluaciones")
                    .OnceAsync<EvaluacionModel>()).FirstOrDefault(a => a.Object.IdServicio == idServicio);

                var promedio = (await firebaseService.firebase
                    .Child("Evaluaciones/" + evaluaciones.Key + "/Notas")
                    .OnceAsync<NotasModel>()).Average(a => Convert.ToDouble(a.Object.Nota));

                var redondear = System.Math.Round(promedio, 1);
                //Retornar el valor
                if (redondear > 0)
                    return redondear.ToString();
                else
                    return "0.0";

            }
            catch (Exception ex)
            {
                MensajeError = "No se ha podido promediar la nota";
                Console.WriteLine("Error: " + ex.Message);
                return "0.0";
            }
        }

        #endregion

    }
}
