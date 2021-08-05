using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AllyCliente.Interface
{
    public interface ILoginGoogle
    {
        Task IniciarSesionGoogle();
        void CerrarSesion();
    }
}
