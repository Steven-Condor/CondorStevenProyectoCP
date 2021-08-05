using AllyContratista.Droid.Services;
using AllyContratista.Interface;
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

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace AllyContratista.Droid.Services
{
    public class MessageAndroid : IMessageService
    {
        //Mensaje Largo
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }
        //Mensaje Corto
        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}