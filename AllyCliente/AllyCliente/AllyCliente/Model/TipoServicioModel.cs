using System;
using System.Collections.Generic;
using System.Text;

namespace AllyCliente.Model
{
    public class TipoServicioModel
    {
        private string tipoServicio;

        public string TipoServicio
        {
            get { return tipoServicio; }
            set { tipoServicio = value; }
        }

        private string imagen;

        public string Imagen
        {
            get { return imagen; }
            set { imagen = value; }
        }

        private string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

    }
}
