using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProySimuRed
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            Red mired = new Red();
            int opcion;
            do
            {
                Console.WriteLine("===================Menú Principal===================");
                Console.WriteLine("| 0. Salir                                          |");
                Console.WriteLine("| 1. Configurar Red                                 |");
                Console.WriteLine("| 2. Crear Mensaje                                  |");
                Console.WriteLine("| 3. Enviar                                         |");
                Console.WriteLine("| 4. Mostrar el status de la Red                    |");
                Console.WriteLine("| 5. Mostrar el status de la SubRed                 |");
                Console.WriteLine("| 6. Mostrar el Status de un Equipo.                |");
                Console.WriteLine("| 7. Eliminar un paquete de una Cola                |");
                Console.WriteLine("| 8. Consultar Paquete                              |");
                Console.WriteLine("| 9. Mensaje Recibidos de PC                        |");
                Console.WriteLine("| 10. Mostrar Todos Los Mensajes                    |");
                Console.WriteLine("| 11. Liberar La Cola de paquete recibido de PC     |");
                Console.WriteLine("| 12. Eliminar Todas Las Redes                      |");
                Console.WriteLine("=====================================================");
                opcion = mired.LeerNumero("Ingresa una opcion: ");

                switch (opcion)
                {

                    case 0:
                        break;
                    case 1:
                        mired.ConfigurarRed();
                        break;
                    case 2:
                        mired.CrearMensaje();
                        break;
                    case 3:
                        mired.Enviar();
                        break;
                    case 4:
                        mired.mostrartodasredconpaq();
                        break;
                    case 5:
                        mired.mostrarSubredconpaq();
                        break;
                    case 6:
                        mired.mostrarequipoconpaq();
                        break;
                    case 7:
                        mired.EliminarPaquete();
                        break;
                    case 8:
                        mired.ConsultarPaquete();
                        break;
                    case 9:
                        mired.MensajesRecibidos();
                        break;
                    case 10:
                        mired.TodosMensjas();
                        break;
                    case 11:
                        mired.LiberarColaRe();
                        break;
                    case 12:
                        mired.borrarTODOREDES();
                        break;
                    
                    
                }
            } while (opcion != 0);


        }
    }
}



