using System;
using System.Collections.Generic;
using System.Text;

namespace AllyContratista.Model
{
    public class EvaluacionModel
    {
        private string idEvaluacion;
        public string IdEvaluacion
        {
            get { return idEvaluacion; }
            set { idEvaluacion = value; }
        }

        private string idServicio;
        public string IdServicio
        {
            get { return idServicio; }
            set { idServicio = value; }
        }

        private string fechaCreacion;
        public string FechaCreacion
        {
            get { return fechaCreacion; }
            set { fechaCreacion = value; }
        }
    }
}
