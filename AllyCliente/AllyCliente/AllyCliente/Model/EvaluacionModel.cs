using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AllyCliente.Model
{
    public class EvaluacionModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        private string idEvaluacion;

        public string IdEvaluacion
        {
            get { return idEvaluacion; }
            set { idEvaluacion = value; }
        }

        private string  idServicio;

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
