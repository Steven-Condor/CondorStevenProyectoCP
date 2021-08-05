using AllyCliente.Model.SubModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AllyCliente.Model
{
    public class OrdenesServicioModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        private string idOrden;

        public string IdOrden
        {
            get { return idOrden; }
            set { idOrden = value; OnPropertyChanged(); }
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

        private double pago;

        public double Pago
        {
            get { return pago; }
            set { pago = value; OnPropertyChanged(); }
        }

        private string estado;

        public string Estado
        {
            get { return estado; }
            set { estado = value; OnPropertyChanged(); }
        }

        private FechasModel fechas;

        public FechasModel Fechas
        {
            get { return fechas; }
            set { fechas = value; OnPropertyChanged(); }
        }

        private string nombreServicio;

        public string NombreServicio
        {
            get { return nombreServicio; }
            set { nombreServicio = value; OnPropertyChanged(); }
        }

        private string calificado;

        public string Calificado
        {
            get { return calificado; }
            set { calificado = value; OnPropertyChanged(); }
        }



    }
}
