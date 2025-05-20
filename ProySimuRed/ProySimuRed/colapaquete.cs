using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProySimuRed
{
    internal class colapaquete
    {
        public Paquete[] cola;
        public int frente;
        public int final;
        public int capacidad { get; }
        public int contador { get; private set; }

        public colapaquete(int capacidad)
        {
            this.capacidad = capacidad;
            this.cola = new Paquete[capacidad];
            this.frente = 0;
            this.final = -1;
            this.contador = 0;
        }

        public bool Llena() => contador == capacidad;
        public bool Vacia() => contador == 0;

        public void push(Paquete paquete)
        {
            if (Llena())
            {
                Console.WriteLine("Cola Llena, No se puede agregar mas paquete.");
                return;
            }

            final = (final + 1) % capacidad;
            cola[final] = paquete;
            contador++;
        }

        public Paquete pop()
        {
            if (Vacia())
            {
                Console.WriteLine("No existe paquete dentro de la cola.");
                return null;
            }

            Paquete paquete = cola[frente];
            cola[frente] = null; 
            frente = (frente + 1) % capacidad;
            contador--;
            return paquete;
        }

        

        public void borrar()
        {
            this.frente = 0;
            this.final = -1;
            this.contador = 0;
        }

        public Paquete Primero()
        {
            if (Vacia())
            {
                Console.WriteLine("No existe paquete dentro de la cola.");
                return null;
            }
            return cola[frente];
        }
        

        public int ObtenerPosicionPorSecuencia(int secuencia)
        {
            int posicion = 0;
            int current = frente;

            for (int i = 0; i < contador; i++)
            {
                if (cola[current] != null && cola[current].Secuencia == secuencia)
                {
                    return posicion ; 
                }

                current = (current + 1) % capacidad;
                posicion++;
            }

            return -1; // No encontrado
        }




    }
}
