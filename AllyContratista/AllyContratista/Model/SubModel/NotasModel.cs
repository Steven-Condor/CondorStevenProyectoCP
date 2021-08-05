using System;
using System.Collections.Generic;
using System.Text;

namespace AllyContratista.Model.SubModel
{
    public class NotasModel
    {
        private double nota;
        public double Nota
        {
            get { return nota; }
            set { nota = value; }
        }

        private string idUsuario;
        public string IdUsuario
        {
            get { return idUsuario; }
            set { idUsuario = value; }
        }

    }
}
