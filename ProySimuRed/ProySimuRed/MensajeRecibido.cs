using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProySimuRed
{
    internal class MensajeRecibido
    {
        public int IdMensaje { get; set; }
        public string Contenido { get; set; }
        public string Estado { get; set; }
       

        public MensajeRecibido(int IdMensaje, string Contenido, string Estado)
        {
            this.IdMensaje = IdMensaje;
            this.Contenido = Contenido;
            this.Estado = Estado;
        }
        public MensajeRecibido() : this(int.MinValue, "", "") { }

        public override string ToString()
        {
            
            return $"Mensaje.({IdMensaje})\n" +
                   $"Contenido: {Contenido} \n " +
                   $"Estado: {Estado}\n" +
                   $"----------------------";

        }
    }
}
