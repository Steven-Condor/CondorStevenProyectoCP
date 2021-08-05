using AllyCliente.Interface;
using AllyCliente.Model.Query;
using AllyCliente.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;

namespace AllyCliente.ViewModel.Cliente
{
    public class AgendaViewModel: INotifyPropertyChanged
    {
        readonly OrdenesServicioService ordenService = new OrdenesServicioService();
        readonly IMessageService messageService;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        private EventCollection events;
        public EventCollection Events
        {
            get { return events; }
            set { events = value; OnPropertyChanged(); }
        }

        public AgendaViewModel()
        {
            Task.Run(() => CargarAgenda()).Wait();
            messageService = DependencyService.Get<IMessageService>();
        }

        private async Task CargarAgenda()
        {
            try
            {
                var lista = await ordenService.ConsultarOrdenServcioJoin(Preferences.Get("Uid", null));
                var query = from c in lista
                            group c by Convert.ToDateTime(c.FechaCreacion).ToString("dd/MM/yyyy") into grupo
                            select grupo;

                var eventos = new EventCollection();

                foreach (var items in query)
                {
                    var eventosLista = new List<OrdenServicioQuery>();
                    foreach (var consulta in items)
                    {
                        eventosLista.Add(new OrdenServicioQuery
                        {
                            FechaCreacion = Convert.ToDateTime(consulta.FechaCreacion).ToString("HH:mm"),
                            NombreServicio = consulta.NombreServicio
                        });
                    };
                    eventos.Add(Convert.ToDateTime(items.Key), eventosLista);
                };

                Events = eventos;
            }
            catch (Exception)
            {
                messageService.ShortAlert("No se han cargadado los datos");
            }
        }
    }
}
