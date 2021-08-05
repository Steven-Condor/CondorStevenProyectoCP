using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AllyCliente.Model
{
    public class ServiciosModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        private string idServicio;

        public string IdServicio
        {
            get { return idServicio; }
            set { idServicio = value; OnPropertyChanged(); }
        }

        private string idContratista;

        public string IdContratista
        {
            get { return idContratista; }
            set { idContratista = value; OnPropertyChanged(); }
        }

        private string tituloServicio;

        public string TituloServicio
        {
            get { return tituloServicio; }
            set { tituloServicio = value; OnPropertyChanged();}
        }

        private string descripcion;

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; OnPropertyChanged(); }
        }

        private double precio;

        public double Precio
        {
            get { return precio; }
            set { precio = value; OnPropertyChanged(); }
        }

        private string tipoServicio;
        public string TipoServicio
        {
            get { return tipoServicio; }
            set { tipoServicio = value; OnPropertyChanged(); }
        }

        private bool estado;
        public bool Estado
        {
            get { return estado; }
            set { estado = value; OnPropertyChanged(); }
        }

        private string fechaCreacion;

        public string FechaCreacion
        {
            get { return fechaCreacion; }
            set { fechaCreacion = value; OnPropertyChanged(); }
        }


        private string apoyo;
        public string Apoyo
        {
            get { return apoyo; }
            set { apoyo = value; OnPropertyChanged(); }
        }

        private bool bloquear;

        public bool Bloquear
        {
            get { return bloquear; }
            set { bloquear = value; }
        }


    }
}
