using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AllyCliente.Model
{
    public class LoginModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        private string email;

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private string passwd;

        public string Passwd
        {
            get { return passwd; }
            set { passwd = value; }
        }
    }
}
