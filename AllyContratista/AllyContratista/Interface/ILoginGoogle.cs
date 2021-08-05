using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AllyContratista.Interface
{
    public interface ILoginGoogle
    {
        Task IniciarSesionGoogle();
        void CerrarSesion();
    }
}
