using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AllyContratista.Model.SubModel
{
    public class VisibleModel: INotifyPropertyChanged
    {
        #region Implementacion de Notify Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        #endregion

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        private string lblFechaAgendada;
        public string LblFechaAgendada
        {
            get { return lblFechaAgendada; }
            set { lblFechaAgendada = value; OnPropertyChanged(); }
        }

        private string pickerVisible;

        public string PickerVisible
        {
            get { return pickerVisible; }
            set { pickerVisible = value; OnPropertyChanged(); }
        }

        private DateTime fecha;
        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; OnPropertyChanged(); }
        }


        private TimeSpan hora;
        public TimeSpan Hora
        {
            get { return hora; }
            set { hora = value; OnPropertyChanged(); }
        }

        private string btnAceptar;

        public string BtnAceptar
        {
            get { return btnAceptar; }
            set { btnAceptar = value; OnPropertyChanged(); }
        }

        private string btnAgendar;

        public string BtnAgendar
        {
            get { return btnAgendar; }
            set { btnAgendar = value; OnPropertyChanged(); }
        }

        private string btnFinalizar;

        public string BtnFinalizar
        {
            get { return btnFinalizar; }
            set { btnFinalizar = value; OnPropertyChanged(); }
        }

        private string btnCancelar;

        public string BtnCancelar
        {
            get { return btnCancelar; }
            set { btnCancelar = value; OnPropertyChanged(); }
        }

        private string btnHabilitar;

        public string BtnHabilitar
        {
            get { return btnHabilitar; }
            set { btnHabilitar = value; OnPropertyChanged(); }
        }

        private bool fechaAgendadaVisible;

        public bool FechaAgendadaVisible
        {
            get { return fechaAgendadaVisible; }
            set { fechaAgendadaVisible = value; OnPropertyChanged(); }
        }

        private bool fechaFinalizacionVisible;

        public bool FechaFinalizacionVisible
        {
            get { return fechaFinalizacionVisible; }
            set { fechaFinalizacionVisible = value; OnPropertyChanged(); }
        }

        private bool fechaCancelacionVisible;

        public bool FechaCancelacionVisible
        {
            get { return fechaCancelacionVisible; }
            set { fechaCancelacionVisible = value; OnPropertyChanged(); }
        }

        private bool fechaAceptacionVisible;

        public bool FechaAceptacionVisible
        {
            get { return fechaAceptacionVisible; }
            set { fechaAceptacionVisible = value; OnPropertyChanged(); }
        }
    }
}
