using AllyCliente.Droid.Activities;
using AllyCliente.Droid.Service;
using AllyCliente.Interface;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(LoginGoogle))]
namespace AllyCliente.Droid.Service
{
    class LoginGoogle : ILoginGoogle
    {
        public void CerrarSesion()
        {
            var googleIntent = new Intent(AndroidApp.Context, typeof(GoogleActivity));
            ((Activity)AndroidApp.Context).StartActivityForResult(googleIntent, 1);
        }

        public async Task IniciarSesionGoogle()
        {
            try
            {
                var googleIntent = new Intent(Forms.Context, typeof(GoogleActivity));
                ((Activity)Forms.Context).StartActivityForResult(googleIntent, 1);
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}