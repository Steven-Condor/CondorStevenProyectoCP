using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllyContratista.Service
{
    public class FirebaseService
    {
        //Inicializar y Establecer conexion con Firebase RealtimeDtabase
        public FirebaseClient firebase = new FirebaseClient("https://servicios-ally-default-rtdb.firebaseio.com/");
        //Inicializar y Establecer conexion con Firebase Auth
        public FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyD62dt9A4kT7VHOvpI9UXO68vHsJSca-po"));
        //Inicializar y Establecer conexion con Firebase Storage
        public FirebaseStorage firebaseStorage = new FirebaseStorage("servicios-ally.appspot.com");
    }
}
