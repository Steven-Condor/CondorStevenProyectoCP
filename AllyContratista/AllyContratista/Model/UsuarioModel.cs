using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AllyContratista.Model
{
    public class UsuarioModel : INotifyPropertyChanged
    {
        #region Implementacion de Notify Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        #endregion

        private string idUsuario;
        public string IdUsuario
        {
            get { return idUsuario; }
            set { idUsuario = value; OnPropertyChanged(); }
        }

        private string nombre;
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; OnPropertyChanged(); }
        }

        private string apellido;
        public string Apellido
        {
            get { return apellido; }
            set { apellido = value; OnPropertyChanged(); }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; OnPropertyChanged(); }
        }

        private string cedula;
        public string Cedula
        {
            get { return cedula; }
            set { cedula = value; OnPropertyChanged(); }
        }

        private string telefono;
        public string Telefono
        {
            get { return telefono; }
            set { telefono = value; OnPropertyChanged(); }
        }

        private string passwd;
        public string Passwd
        {
            get { return passwd; }
            set { passwd = value; OnPropertyChanged(); }
        }

        private string rPasswd;
        public string RPasswd
        {
            get { return rPasswd; }
            set { rPasswd = value; OnPropertyChanged(); }
        }

        private string fechaCreacion;
        public string FechaCreacion
        {
            get { return fechaCreacion; }
            set { fechaCreacion = value; OnPropertyChanged(); }
        }

        private string tipoUsuario;
        public string TipoUsuario
        {
            get { return tipoUsuario; }
            set { tipoUsuario = value; OnPropertyChanged(); }
        }

        private string imagen;
        public string Imagen
        {
            get { return imagen; }
            set { imagen = value; OnPropertyChanged(); }
        }

        private string estado;

        public string Estado
        {
            get { return estado; }
            set { estado = value; OnPropertyChanged(); }
        }


    }
}
