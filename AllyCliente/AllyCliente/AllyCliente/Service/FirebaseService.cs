using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllyCliente.Service
{
    public class FirebaseService
    {
        public FirebaseClient firebase = new FirebaseClient("https://servicios-ally-default-rtdb.firebaseio.com/");

        public FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyD62dt9A4kT7VHOvpI9UXO68vHsJSca-po"));

        public FirebaseStorage firebaseStorage = new FirebaseStorage("servicios-ally.appspot.com");
    }
}
