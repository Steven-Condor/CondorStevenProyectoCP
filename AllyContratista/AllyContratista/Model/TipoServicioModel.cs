using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AllyContratista.Model
{
    public class TipoServicioModel: INotifyPropertyChanged
    {
        #region Implementacion de Notify Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        #endregion

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

        private string idTipoServicio;

        public string IdTipoServicio
        {
            get { return idTipoServicio; }
            set { idTipoServicio = value; }
        }

        private bool estado;

        public bool Estado
        {
            get { return estado; }
            set { estado = value; }
        }


    }
}
