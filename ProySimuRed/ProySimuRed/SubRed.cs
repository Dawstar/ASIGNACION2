using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProySimuRed
{
    internal class SubRed
    {
        #region atributos
        
        public string NumRed {  get; set; }
        public string IPpc {  get; set; }
        public string IProuter {  get; set; }
        public colapaquete colaenviopc { get; set; }
        public colapaquete colarecibpc { get; set; }
        public colapaquete colarouter {  get; set; }

        public Dictionary<int, MensajeRecibido> MensajesRecibidos = new Dictionary<int, MensajeRecibido>();
        public int IDrecibir { get; set; }

        #endregion

        #region Constructor
        public SubRed(string numRed, string ipPC, string ipRouter)
        {
            NumRed = numRed;
            IPpc = ipPC;
            IProuter = ipRouter;
            IDrecibir = 0;
            colaenviopc = new colapaquete(10); // PC tiene máximo 10 paquetes
            colarecibpc = new colapaquete(10);
            colarouter = new colapaquete(4); // Router tiene máximo 4 paquetes
        }
        public SubRed()

            : this( "", "","")
        {
        }
        #endregion
        #region metodo

        public void AgregarMenRecibidos(Dictionary<int, Mensajes> mensajesRegistrados)
        {
            
            colapaquete colatemporal = new colapaquete(10);
            MensajeRecibido mensaje = new MensajeRecibido();
            while (!colarecibpc.Vacia())
            {
                Paquete paquete = colarecibpc.pop();
                mensaje.IdMensaje = paquete.IdMensaje;
                mensaje.Contenido += paquete.Dato.ToString();
                
                colatemporal.push(paquete);
            }
            while (!colatemporal.Vacia())
            {
                Paquete paquete = colatemporal.pop();
                colarecibpc.push(paquete);
            }
            Mensajes MensajeOriginal = mensajesRegistrados[mensaje.IdMensaje];
            if (mensaje.Contenido == MensajeOriginal.Contenido)
            {
                
                mensaje.Estado = "RECIBIDO";
                MensajeOriginal.ActualizarEstado("RECIBIDO");
            }
            else
            {
                mensaje.Estado= "DAÑADO";
                MensajeOriginal.ActualizarEstado("ERROR");
            }
            if(mensaje.Contenido.Length != MensajeOriginal.Contenido.Length)
            {
                mensaje.Estado = "INCOMPLETO";
            }
            MensajesRecibidos.Add(mensaje.IdMensaje, mensaje);
        }
        #endregion



    }
}
