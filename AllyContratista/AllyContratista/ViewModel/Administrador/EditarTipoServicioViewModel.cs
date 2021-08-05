using AllyContratista.Interface;
using AllyContratista.Model;
using AllyContratista.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AllyContratista.ViewModel.Administrador
{
    public class EditarTipoServicioViewModel:TipoServicioModel
    {
        #region Instancias
        readonly TipoServicioService tipoServicioService = new TipoServicioService();
        readonly IMessageService messageService;
        #endregion

        #region Parametros
        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; OnPropertyChanged(); }
        }

        private bool nuevoVisible;
        public bool NuevoVisible
        {
            get { return nuevoVisible; }
            set { nuevoVisible = value; OnPropertyChanged(); }
        }

        private bool actualizarVisible;
        public bool ActualizarVisible
        {
            get { return actualizarVisible; }
            set { actualizarVisible = value; OnPropertyChanged(); }
        }

        private ImageSource fuenteImagen;
        public ImageSource FuenteImagen
        {
            get { return fuenteImagen; }
            set { fuenteImagen = value; OnPropertyChanged(); }
        }

        private FileResult file;
        public FileResult File
        {
            get { return file; }
            set { file = value; OnPropertyChanged(); }
        }

        #endregion

        #region Comandos
        public Command NuevoTipoServicioCommand { get; set; }
        public Command ActualizarTipoServicioCommand { get; set; }
        public Command AgregarFotoCommand { get; set; }
        #endregion

        #region Constructor
        public EditarTipoServicioViewModel(TipoServicioModel tipoServicio)
        {
            CargarDatos(tipoServicio);
            messageService = DependencyService.Get<IMessageService>();
            NuevoTipoServicioCommand = new Command(async () => await NuevoTipoServicio());
            ActualizarTipoServicioCommand = new Command(async () => await ActualizarTipoServicio());
            AgregarFotoCommand = new Command(async () => await AgregarFoto());
        }


        #endregion

        #region Ejecutar Comandos
        private async Task AgregarFoto()
        {
            IsBusy = true;
            try
            {
                var resultado = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Por favor Toma una foto"
                });
                File = resultado;
                if (resultado != null)
                {
                    var stream = await resultado.OpenReadAsync();
                    FuenteImagen = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception)
            {

                throw;
            }
            IsBusy = false;
        }

        //Regstrar un nuevo tipo de tarea
        private async Task NuevoTipoServicio()
        {
            IsBusy = true;
            try
            {
                string imgUrl = null;

                if (File != null)
                {
                    imgUrl = await tipoServicioService.SubirFoto(await File.OpenReadAsync(), File.FileName, this.TipoServicio);
                }
                else
                {
                    imgUrl = this.Imagen;
                }         

                var tipoServicio = new TipoServicioModel()
                {
                    IdTipoServicio = this.IdTipoServicio,
                    Imagen = imgUrl,
                    Descripcion = this.Descripcion,
                    TipoServicio = this.TipoServicio,
                    Estado = true
                };
                await tipoServicioService.NuevoTipoServicios(tipoServicio);
                messageService.ShortAlert("Datos Ingresados Correctamente");
                ActualizarVisible = true;
                NuevoVisible = false;

            }
            catch (Exception)
            {
                messageService.ShortAlert(tipoServicioService.MensajeError);
            }
            IsBusy = false;
        }

        //Comando para actualizar la infromación del tipo de servicio
        private async Task ActualizarTipoServicio()
        {
            try
            {
                string imgUrl = null;

                if (File != null)
                {
                    imgUrl = await tipoServicioService.SubirFoto(await File.OpenReadAsync(), File.FileName, this.TipoServicio);
                }
                else
                {
                    imgUrl = this.Imagen;
                }

                var tipoServicio = new TipoServicioModel()
                {
                    IdTipoServicio = this.IdTipoServicio,
                    Imagen = imgUrl,
                    Descripcion = this.Descripcion,
                    TipoServicio = this.TipoServicio,
                    Estado = true
                };
                await tipoServicioService.ActualizarTipoServicios(tipoServicio);
                messageService.ShortAlert("Datos Actualizados Correctamente");
            }
            catch (Exception)
            {
                messageService.ShortAlert(tipoServicioService.MensajeError);
            }
        }

        //Cragar datos del tipo de tarea en el modelo heredado
        public void CargarDatos(TipoServicioModel tipoServicio)
        {
            try
            {
                if(tipoServicio != null)
                {
                    this.IdTipoServicio = tipoServicio.IdTipoServicio;
                    this.Imagen = tipoServicio.Imagen;
                    this.Descripcion = tipoServicio.Descripcion;
                    this.TipoServicio = tipoServicio.TipoServicio;
                    FuenteImagen = this.Imagen;
                    ActualizarVisible = true;
                    NuevoVisible = false;
                }
                else
                {
                    ActualizarVisible = false;
                    NuevoVisible = true;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                messageService.ShortAlert("No se puede visualizar los datos");
            }
        }
        #endregion

    }
}
