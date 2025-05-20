using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProySimuRed
{
    internal class Mensajes
    {
        #region Atributos
        
        public string IPOrigen { get; set; }
        public string IPDestino { get; set; }
        public string Contenido { get; set; }
        public string Estado { get; set; }
        public colapaquete paquetesGenerados { get; set; }
        public int SecuenciaInicial {  get; set; }
        public int IdMensaje { get; set; }
        public int TotalPaquetes => Contenido.Length + 1;
        public SubRed subredinicial { get; set; }
        


        #endregion

        #region Constructor

        public Mensajes(string ipOrigen, string ipDestino, string contenido,
                  int secuencia, int idMensaje, SubRed subredOrigen)
        {

            IPOrigen = ipOrigen;
            IPDestino = ipDestino;
            Contenido = contenido;
            SecuenciaInicial = secuencia+1;
            IdMensaje = idMensaje;
            Estado = "Nuevo";
            subredinicial = subredOrigen;
            paquetesGenerados = new colapaquete(TotalPaquetes);
            GenerarYEncolarPaquetes(subredOrigen);
        }



        #endregion

        #region metodo
        private void GenerarYEncolarPaquetes(SubRed subredOrigen)
        {
            for(int i = 0; i < Contenido.Length; i++)
            {
                Paquete paquete = new Paquete(
                    IPOrigen,
                    IPDestino,
                    SecuenciaInicial + i,
                    Contenido[i],
                    IdMensaje
                ); 
                paquetesGenerados.push(paquete);
                EncolarEnPC(subredOrigen, paquete);
            }

        }

        private void EncolarEnPC(SubRed subred, Paquete paquete)
        {
            
            subred.colaenviopc.push(paquete);
            Console.WriteLine($"✓ Paquete {paquete.Secuencia} encolado en {subred.IPpc}");
        }
        public void ActualizarEstado(string nuevoEstado)
        {
            Estado = nuevoEstado;
            
        }

        public override string ToString()
        {
            return $"Mensaje: {IdMensaje} \n" +
                $" Contenido: {Contenido} \n" +
                $" DE {IPOrigen} a {IPDestino} \n" +
                $" Estado: {Estado} \n" +
                $"======================";
        }

        

        #endregion


    }
}
 