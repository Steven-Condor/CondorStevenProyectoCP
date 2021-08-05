using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AllyContratista.Model
{
    public class FechasModel: INotifyPropertyChanged
    {
        #region Implementacion de Notify Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        #endregion

        private string fechaCreacion;
        public string FechaCreacion
        {
            get { return fechaCreacion; }
            set { fechaCreacion = value; OnPropertyChanged(); }
        }

        private string fechaAceptacion;
        public string FechaAceptacion
        {
            get { return fechaAceptacion; }
            set { fechaAceptacion = value; OnPropertyChanged(); }
        }


        private string fechaAgendada;
        public string FechaAgendada
        {
            get { return fechaAgendada; }
            set { fechaAgendada = value; OnPropertyChanged(); }
        }

        private string fechaFinalizacion;
        public string FechaFinalizacion
        {
            get { return fechaFinalizacion; }
            set { fechaFinalizacion = value; OnPropertyChanged(); }
        }

        private string fechaCancelacion;

        public string FechaCancelacion
        {
            get { return fechaCancelacion; }
            set { fechaCancelacion = value; OnPropertyChanged(); }
        }
    }
}
