using AllyContratista.Model;
using AllyContratista.Model.SubModel;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllyContratista.Service
{
    public class EvaluacionService
    {
        readonly FirebaseService firebaseService = new FirebaseService();

        public string MensajeError { get; set; }

        #region Obtener el promedio de calificaciones
        public async Task<string> PromedioNotas(string idServicio)
        {
            try
            {
                //Obtener la evaluacion a traves del codigo del servicio
                var evaluaciones = (await firebaseService.firebase
                    .Child("Evaluaciones")
                    .OnceAsync<EvaluacionModel>()).FirstOrDefault(a => a.Object.IdServicio == idServicio);
                //Promediar la notas regitradas del servicio
                var promedio = (await firebaseService.firebase
                    .Child("Evaluaciones/" + evaluaciones.Key+"/Notas")
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
                Console.WriteLine("Error: "+ ex.Message);
                return "0.0";
            }
        }
        #endregion

        #region GenerarEvaluacion
        //Generar una nueva evaluación
        public async Task CrearEvaluacion(EvaluacionModel evaluacion)
        {
            try
            {
                await firebaseService.firebase
                    .Child("Evaluaciones")
                    .PostAsync(evaluacion);
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo crear la evaluacion";
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        #endregion

        #region Generar Id Evaluacion
        //Crear un Identificadr para las evaluaciones
        public string GenerarId()
        {
            Guid miGuid = Guid.NewGuid();
            string token = Convert.ToString(miGuid.ToString().Replace("-", string.Empty).Substring(0, 3));
            string fecha = DateTime.Now.ToString("dd/MM HH/mm").Replace("/", string.Empty).Replace(" ", string.Empty);

            //Retorna una clave unica generarda con una cadena de caracteres y la fecha actual
            return "EV" + token + "-" + fecha;
        }
        #endregion
    }
}
