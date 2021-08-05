using AllyCliente.Model;
using AllyCliente.Service;
using AllyCliente.Model.Query;
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

namespace AllyCliente.ViewModel.Cliente
{
    public class ReportesViewModel: INotifyPropertyChanged
    {
        #region Instancias
        readonly ReportesService reportesService = new ReportesService();
        #endregion

        #region Implementacion de Notify Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        #endregion

        #region Atributos
        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        private bool lineVisible;
        public bool LineVisible
        {
            get { return lineVisible; }
            set { lineVisible = value; OnPropertyChanged(); }
        }

        private LineChart chart;
        public LineChart LChart
        {
            get { return chart; }
            set { chart = value; OnPropertyChanged(); }
        }

       private ObservableCollection<ReporteMesQuery> listViewSource;
        public ObservableCollection<ReporteMesQuery> ListViewSource
        {
            get { return listViewSource; }
            set { listViewSource = value; OnPropertyChanged(); }
        }
        #endregion

        public Command ReporteMensualCommand { get; set; }

        public ReportesViewModel()
        {
            ReporteMensualCommand = new Command(async () => await ReporteMensual());
        }

        #region Array de años disponibles
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

        private async Task ReporteMensual()
        {
            IsBusy = true;
            LineVisible = true;
            var arrayAnios = this.Anios();

            var anio = await App.Current.MainPage.DisplayActionSheet("Seleccione el Año", "Cancelar", null, arrayAnios);
            if (anio != null)
            {
                var lista = await reportesService.OrdenesPorMes(anio);
                var serializar = JsonConvert.SerializeObject(lista);
                ListViewSource = JsonConvert.DeserializeObject<ObservableCollection<ReporteMesQuery>>(serializar);
                var entries = new List<ChartEntry>();

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
                LChart = new LineChart() { Entries = entries, LabelTextSize = 40f };
            }
            isBusy = false;
        }
    }
}
