using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AllyContratista.Model
{
    public class OrdenServicioModel: INotifyPropertyChanged
    {
        #region Implementacion de Notify Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        #endregion

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

    }
}
