using AllyContratista.Model;
using AllyContratista.Model.Query;
using AllyContratista.Service;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AllyContratista.ViewModel.Contratista
{
    public class ReportesViewModel: INotifyPropertyChanged
    {
        #region Instancias
        readonly ReportesService resporteService = new ReportesService();
        #endregion

        #region Implementacion de Notify Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        #endregion

        #region Parametros

        private bool pieVisible;

        public bool PieVisible
        {
            get { return pieVisible; }
            set { pieVisible = value; OnPropertyChanged(); }
        }

        private bool lineVisible;

        public bool LineVisible
        {
            get { return lineVisible; }
            set { lineVisible = value; OnPropertyChanged(); }
        }



        private PieChart chart;

        public PieChart Charts
        {
            get { return chart; }
            set { chart = value; OnPropertyChanged(); }
        }

        private LineChart lChart;

        public LineChart LChart
        {
            get { return lChart; }
            set { lChart = value; OnPropertyChanged(); }
        }


        private ObservableCollection<ServicioModel> listViewSource;
        public ObservableCollection<ServicioModel> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ReporteMesQuery> listViewSource2;
        public ObservableCollection<ReporteMesQuery> ListViewSource2
        {
            get { return listViewSource2; }
            set { listViewSource2 = value; OnPropertyChanged(); }
        }
        #endregion

        #region Inicializar Comandos
        public Command ReporteMensualCommand { get; set; }
        public Command ReporteServicioCommand { get; set; }
        #endregion

        #region Constructor
        public ReportesViewModel()
        {
            Task.Run(() =>CargarCharts());
            ReporteMensualCommand = new Command(async () => await ReporteMensual());
            ReporteServicioCommand = new Command(async () => await ReporteServicio());
        }

        #endregion

        #region Array de años disponibles
        //Generar un array tipo string con los años diposibles del usuario
        private string[] Anios()
        {
            var fechaInicio = Convert.ToDateTime("01/07/2021").Year;
            var fechaFin = DateTime.Now.Year;
            int j = fechaFin - fechaInicio + 1;
            string[] fechas = new string[j];
            for (int i = 0; i < j; i++)
            {
                var anios = fechaInicio + i;
                fechas[i] = anios.ToString();
            }
            return fechas;
        }
        #endregion

        #region Ejecutar Comandos
        //Reporte de servicios agrupaos por años y meses
        private async Task ReporteMensual()
        {
            try
            {
                LineVisible = true;
                var arrayAnios = this.Anios();
                //Seleccione el año que desea visualizar
                var anio = await App.Current.MainPage.DisplayActionSheet("Seleccione el Año", "Cancelar", null, arrayAnios);
                if(anio != null)
                {
                    var lista = await resporteService.ServiciosPorMes(anio);
                    var serializar = JsonConvert.SerializeObject(lista);
                    ListViewSource2 = JsonConvert.DeserializeObject<ObservableCollection<ReporteMesQuery>>(serializar);
                    PieVisible = false;
                    ListViewSource = null;
                    Charts = null;
                    var entries = new List<ChartEntry>();
                    //Llenar la valriable del grafico con los datos extaridos de la base de datos
                    foreach (var items in lista)
                    {
                        entries.Add(new ChartEntry(items.Contar)
                        {
                            Label = items.Mes,
                            ValueLabel = items.Contar.ToString(),
                            Color = SKColor.Parse(items.Color),
                            ValueLabelColor = SKColor.Parse(items.Color)
                        });
                    }
                    //Envair los prametros al grafico
                    LChart = new LineChart() { Entries = entries, LabelTextSize = 40f };
                }
                

            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Error", resporteService.MensajeError, "Ok");
            }
        }

        private Task ReporteServicio()
        {
            LineVisible = false;
            LChart = null;
            ListViewSource2 = null;
            return Task.Run(() => CargarCharts()); 
        }

        //Reporte de cantidad de ordenes generadas por sericio
        private async Task CargarCharts()
        {
            try
            {
                PieVisible = true;
                var lista = await resporteService.ReportesServicio();
                var convertir = JsonConvert.SerializeObject(lista);
                ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<ServicioModel>>(convertir);

                var entries = new List<ChartEntry>();

                foreach (var elementos in lista)
                {
                    entries.Add(new ChartEntry(Convert.ToInt32(elementos.Apoyo))
                    {
                        Label = elementos.TituloServicio,
                        Color = SKColor.Parse(elementos.Descripcion),
                        ValueLabelColor = SKColor.Parse(elementos.Descripcion)
                    });
                };
                Charts = new PieChart() { Entries = entries, LabelTextSize = 40f };
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Error", resporteService.MensajeError, "Ok");
            }              
        }
        #endregion
    }
}
