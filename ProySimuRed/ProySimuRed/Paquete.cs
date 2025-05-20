using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProySimuRed
{
    internal class Paquete
    {
        #region Atributos

        public string IPOrigen { get; }
        public string IPDestino { get; }
        public int Secuencia { get; }
        public char Dato { get; }
        public string Ubicacion { get; private set; }
        public int IdMensaje { get; }  

        #endregion

        #region Constructor

        public Paquete(string ipOrigen, string ipDestino, int secuencia, char dato, int idMensaje)
        {
            IPOrigen = ipOrigen;
            IPDestino = ipDestino;
            Secuencia = secuencia;
            Dato = dato;
            IdMensaje = idMensaje;
            Ubicacion = ipOrigen;  // Inicialmente en el origen
        }

        public Paquete() : this("", "", -1, '\0', -1) { }

        #endregion

        #region Métodos

        public void ActualizarUbicacion(string nuevaUbicacion)
        {
            Ubicacion = nuevaUbicacion;
        }

        public override string ToString()
        {
            
            return $"Paquete. {Secuencia}      |     ({IdMensaje})     |      {Dato}      |  De {IPOrigen} a {IPDestino}  |   {Ubicacion} |\n";
            
        }

        #endregion


    }
}

