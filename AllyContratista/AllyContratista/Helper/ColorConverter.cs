using AllyContratista.Model;
using AllyContratista.Model.Consultas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AllyContratista.Helper
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            var itemServicio = value as ServicioModel;
            var itemOrdenServicio = value as OrdenServicioQuery;
            var itemUsuario = value as UsuarioModel;
            var itemTipo = value as TipoServicioModel;

            if (itemServicio != null)
            {
                if (itemServicio.Estado == true)
                    return Color.FromHex("0455BF");
                else if (itemServicio.Estado == false)
                    return Color.LightGray;
                else
                    return Color.BlueViolet;
            }
            else if (itemOrdenServicio != null)
            {
                if (itemOrdenServicio.Estado == "Solicitado")
                    return Color.LightGray;
                if (itemOrdenServicio.Estado == "Aceptado")
                    return Color.FromHex("F1C40F");
                if (itemOrdenServicio.Estado == "Agendado")
                    return Color.FromHex("0455BF");
                if (itemOrdenServicio.Estado == "Finalizado")
                    return Color.FromHex("28B463");
                if (itemOrdenServicio.Estado == "Cancelado")
                    return Color.FromHex("DF0039");
                else
                    return Color.BlueViolet;
            }else if(itemTipo !=null)
            {
                if (itemTipo.Estado == true)
                    return Color.FromHex("0455BF");
                if (itemTipo.Estado == false)
                    return Color.LightGray;
                else
                    return Color.BlueViolet;
            }
            else if(itemUsuario != null )
            {
                if(itemUsuario.Estado == "true")
                    return Color.FromHex("0455BF");
                if(itemUsuario.Estado == "false")
                    return Color.LightGray;
                else
                    return Color.BlueViolet;
            }
            else
            {
                return Color.BlueViolet;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
