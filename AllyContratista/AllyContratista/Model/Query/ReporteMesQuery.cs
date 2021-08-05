using System;
using System.Collections.Generic;
using System.Text;

namespace AllyContratista.Model.Query
{
    public class ReporteMesQuery
    {
        private string mes;
        public string Mes
        {
            get { return mes; }
            set { mes = value; }
        }

        private string anio;
        public string Anio
        {
            get { return anio; }
            set { anio = value; }
        }


        private int contar;
        public int Contar
        {
            get { return contar; }
            set { contar = value; }
        }

        private string color;

        public string Color
        {
            get { return color; }
            set { color = value; }
        }



    }
}
