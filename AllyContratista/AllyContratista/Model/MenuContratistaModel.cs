using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AllyContratista.Model
{
    public class MenuContratistaModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Type TargetType { get; set; }
        public ImageSource Icon { get; set; }
        public Page Page { get; set; }
    }
}
