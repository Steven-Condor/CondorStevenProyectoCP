using AllyContratista.Interface;
using AllyContratista.Model.Consultas;
using AllyContratista.Service;
using Newtonsoft.Json;
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

namespace AllyContratista.ViewModel.Contratista
{
    public class AgendaViewModel: INotifyPropertyChanged
    {
        #region Instancias
        readonly OrdenServicioService ordenService = new OrdenServicioService();
        readonly IMessageService messageService;
        #endregion

        #region Implementacion de PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        #endregion

        #region Atributos de la clase
        private EventCollection events;
        public EventCollection Events
        {
            get { return events; }
            set { events = value; OnPropertyChanged(); }
        }
        #endregion

        #region Constructor
        public AgendaViewModel()
        {
            Task.Run(()=>CargarAgenda()).Wait();
            messageService = DependencyService.Get<IMessageService>();
        }
        #endregion

        #region Ejecutar Comados
        public async Task CargarAgenda()
        {
            try
            {
                //Agrupar por fechas los serivicios
                var lista = await ordenService.ConsultarOrdenesAgenda(Preferences.Get("Uid", null));
                var query = from c in lista
                            group c by Convert.ToDateTime(c.FechaCreacion).ToString("dd/MM/yyyy") into grupo
                            select grupo;
                //Inicializar la colección de eventos
                var eventos = new EventCollection();
                //Llenar la coleccion de eventos
                foreach (var items in query)
                {
                    var eventosLista = new List<OrdenServicioQuery>();
                    foreach (var consulta in items)
                    {
                        eventosLista.Add(new OrdenServicioQuery 
                        { 
                            FechaCreacion = Convert.ToDateTime(consulta.FechaCreacion).ToString("HH:mm"), 
                            NombreCliente = consulta.NombreCliente 
                        });
                    };
                    eventos.Add(Convert.ToDateTime(items.Key), eventosLista);
                };
                //Almacenar la colección de eventos en un elemento visible para la vista
                Events = eventos;
            }
            catch (Exception)
            {
                messageService.ShortAlert("No se han cargadado los datos");
            }
        }
        #endregion

    }
}
