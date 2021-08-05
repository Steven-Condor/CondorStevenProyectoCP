using System;
using System.Collections.Generic;
using System.Text;

namespace AllyCliente.Interface
{
    public interface IMessageService
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
