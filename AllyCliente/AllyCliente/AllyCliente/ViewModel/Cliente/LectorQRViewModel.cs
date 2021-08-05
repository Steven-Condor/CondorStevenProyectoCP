using AllyCliente.Interface;
using AllyCliente.Model;
using AllyCliente.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyCliente.ViewModel.Cliente
{
    public class LectorQRViewModel:OrdenesServicioModel
    {
        readonly IMessageService messageService;
        readonly UsuarioService usuarioService = new UsuarioService();
        readonly ServiciosService serviciosService = new ServiciosService();
        readonly OrdenesServicioService ordenesService = new OrdenesServicioService();

        public LectorQRViewModel(string idOrden)
        {
            messageService = DependencyService.Get<IMessageService>();
            Task.Run(()=>CargarDatos(idOrden));               
        }

        private UsuarioModel usuarioModelo;

        public UsuarioModel UsuarioModelo
        {
            get { return usuarioModelo; }
            set { usuarioModelo = value; OnPropertyChanged(); }
        }

        private ServiciosModel serviciosModelo;

        public ServiciosModel ServiciosModelo
        {
            get { return serviciosModelo; }
            set { serviciosModelo = value; OnPropertyChanged(); }
        }

        private OrdenesServicioModel ordenModel;

        public OrdenesServicioModel OrdenModel
        {
            get { return ordenModel; }
            set { ordenModel = value; OnPropertyChanged(); }
        }

        public async Task CargarDatos(string idOrden)
        {
            try
            {
                OrdenModel = await ordenesService.BuscarOrden(idOrden);
                UsuarioModelo = await usuarioService.BuscarUsuario(Preferences.Get("Uid", null));
                ServiciosModelo = await serviciosService.BuscarServicioId(OrdenModel.IdServicio);               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: "+ex.Message);
                messageService.ShortAlert("no se encontro ningun mensaje");
            }
        }
    }
}
