using System;
using System.Collections.Generic;
using System.Text;

namespace AllyCliente.Model.Query
{
    public class OrdenServicioQuery
    {
        private string nombreServicio;

        public string NombreServicio
        {
            get { return nombreServicio; }
            set { nombreServicio = value; }
        }

        private string nombreCliente;

        public string NombreCliente
        {
            get { return nombreCliente; }
            set { nombreCliente = value; }
        }

        private string nombreContratista;

        public string NombreContratista
        {
            get { return nombreContratista; }
            set { nombreContratista = value; }
        }

        private string idOrden;

        public string IdOrden
        {
            get { return idOrden; }
            set { idOrden = value; }
        }

        private string idServicio;

        public string IdServicio
        {
            get { return idServicio; }
            set { idServicio = value; }
        }

        private string idCliente;

        public string IdCliente
        {
            get { return idCliente; }
            set { idCliente = value; }
        }

        private string estado;

        public string Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        private string fechaCreacion;

        public string FechaCreacion
        {
            get { return fechaCreacion; }
            set { fechaCreacion = value; }
        }

        private string imagen;

        public string Imagen
        {
            get { return imagen; }
            set { imagen = value; }
        }


    }
}
