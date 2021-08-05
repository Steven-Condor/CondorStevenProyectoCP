using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using AllyCliente.Service;
using Task = System.Threading.Tasks.Task;
using AllyCliente.Model;

namespace AllyCliente.Droid.Activities
{
    [Activity]
    public class GoogleActivity : AppCompatActivity, IOnSuccessListener, IOnFailureListener
    {
        GoogleSignInOptions gso;
        GoogleApiClient googleApiClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
              .RequestIdToken("830322992249-ah59tql8i8ek9b8jle5t1knqk0k7lvss.apps.googleusercontent.com")
              .RequestEmail()
              .Build();


            var user = Preferences.Get("Uid", null);
            if (user != null)
            {
                Preferences.Clear("Uid");
                Preferences.Clear("TokenSesion");
                Preferences.Clear("MyFirebaseRefreshContent");
                Finish();
            }
            else
            {
                googleApiClient = new GoogleApiClient.Builder(this)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, gso).Build();

                Intent signInIntent = Auth.GoogleSignInApi.GetSignInIntent(googleApiClient);
                StartActivityForResult(signInIntent, 1);
            }

        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                if (result.IsSuccess)
                {
                    GoogleSignInAccount account = result.SignInAccount;
                    UsuarioModel loginModel = new UsuarioModel()
                    {
                        Email = account.Email,
                        IdUsuario = account.Id,
                        Imagen = Convert.ToString(account.PhotoUrl),
                        Nombre = account.GivenName,
                        TipoUsuario = "Cliente",
                        Apellido = account.FamilyName,
                        FechaCreacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                    };
                    AllyCliente.Service.LoginService loginService = new AllyCliente.Service.LoginService();
                    Task.Run(() => loginService.IniciarSesionGoogle(loginModel, account.IdToken));
                    Toast.MakeText(this, "Login Existoso", ToastLength.Short).Show();
                    loginService.AbrirPagina();
                    //Finalizar la actividad
                    Finish();
                }
            }
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            throw new NotImplementedException();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            Toast.MakeText(this, "Login Existoso", ToastLength.Short).Show();
            Task.Delay(1000);
            Finish();
        }
    }
}