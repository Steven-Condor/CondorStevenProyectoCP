using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AllyCliente.Model.Query
{
    public class ServiciosQuery: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        private string tituloServicio;
        public string TituloServicio
        {
            get { return tituloServicio; }
            set { tituloServicio = value; OnPropertyChanged(); }
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

        private string tipoServicio;
        public string TipoServicio
        {
            get { return tipoServicio; }
            set { tipoServicio = value; OnPropertyChanged(); }
        }

        private double precio;
        public double Precio
        {
            get { return precio; }
            set { precio = value; OnPropertyChanged(); }
        }

        private string fechaCreacion;
        public string FechaCreacion
        {
            get { return fechaCreacion; }
            set { fechaCreacion = value; OnPropertyChanged(); }
        }

        private bool estado;
        public bool Estado
        {
            get { return estado; }
            set { estado = value; OnPropertyChanged(); }
        }

        private bool bloquear;
        public bool Bloquear
        {
            get { return bloquear; }
            set { bloquear = value; }
        }

        private string nota;
        public string Nota
        {
            get { return nota; }
            set { nota = value; OnPropertyChanged(); }
        }

        private string nombreContratista;
        public string NombreContratista
        {
            get { return nombreContratista; }
            set { nombreContratista = value; }
        }
    }
}
