
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProySimuRed
{
    internal class Red
    {
        private Dictionary<string, SubRed> subredes = new Dictionary<string, SubRed>();
        private Dictionary<int, Mensajes> mensajesRegistrados = new Dictionary<int, Mensajes>();
        int secuenciaglobal = 0;
        int idmensaje = 0;

        public void ConfigurarRed()
        {
            Console.Clear();
            int cantidad = 0;
            Console.WriteLine("===============Configuracion de redes=================");
            do
            {
                try
                {


                    Console.Write("Ingrese cantidades de red a configurar: ");
                    cantidad = int.Parse(Console.ReadLine());
                    if (cantidad <= 0) Console.WriteLine("Error: No se permiten números negativos o cero");
                    
                }
                catch (FormatException)
                {
                    Console.WriteLine(" Error: Ingrese un número válido (ej: 190).");
                }
            } while (cantidad < 0 || cantidad == 0);



            {
                for (int i = 0; i < cantidad; i++)
                {
                    int redvalido;
                    string NumRedStr;
                    do
                    {
                        redvalido = 1;
                        int NumRed = LeerNumero("Ingrese el numero de red(Ej: 190): ");
                        NumRedStr = NumRed.ToString();
                        foreach (var subRed in subredes.Values)
                        {
                            if (subRed.NumRed == NumRedStr)
                            {
                                redvalido = 0;
                                Console.WriteLine("Este número de red ya esta configurado. Intente con otro.");
                                break;
                            }
                        }
                    } while (redvalido == 0);

                    SubRed nuevared = new SubRed();
                    nuevared.NumRed = NumRedStr;
                    nuevared.IPpc = NumRedStr + ".01";
                    nuevared.IProuter = NumRedStr + ".0";
                    subredes.Add(NumRedStr, nuevared);
                    Console.WriteLine("=====================================================");
                    Console.WriteLine("Numero de Red: " + NumRedStr + " Configurado!");
                    Console.WriteLine("IP ROUTER: " + nuevared.IProuter);
                    Console.WriteLine("IP PC: " + nuevared.IPpc);
                    Console.WriteLine("=====================================================");

                }
                tecladocontinuar();
            }
        }

        public void CrearMensaje()
        {
            Console.Clear();

            if (subredes.Count < 2)
            {
                Console.WriteLine("==================Crear un Mensaje==================");
                Console.WriteLine("Deben configurar al menos 2 red primero");
                tecladocontinuar();
                return;
            }
            else
            {
                string iporigen;
                bool ipvalido = false;
                do
                {
                    Console.WriteLine("==================Crear un Mensaje==================");
                    Console.Write("Ingresa IP origen(Ejm: 190.01): ");
                    iporigen = Console.ReadLine();
                    ipvalido = ExisteIP_PC(iporigen);
                    if (ipvalido == false)
                    {
                        Console.WriteLine("Esta IP no esta en el sistema. Intenta con otra.");
                    }
                } while (!ipvalido);
                SubRed subredOrigen = subredes.Values.First(s => s.IPpc == iporigen);

                if (!subredOrigen.colaenviopc.Vacia())
                {
                    Console.WriteLine(" Esta subred aún tiene paquetes pendientes. Espere a que se vacíe la cola.");
                    tecladocontinuar();
                    return;
                }
                string ipdestino;
                ipvalido = false;
                do
                {
                    Console.Write("Ingresa IP destino(Ejm: 190.01): ");
                    ipdestino = Console.ReadLine();
                    ipvalido = ExisteIP_PC(ipdestino) && ipdestino != iporigen;
                    if (!ExisteIP_PC(ipdestino))
                    {
                        Console.WriteLine("Esta IP no esta en el sistema. Intenta con otra.");
                    }
                    else if (ipdestino == iporigen)
                    {
                        Console.WriteLine("IP destino no puede ser igual que de origen");
                    }
                } while (!ipvalido);

                string mensaje = "";
                while (true)
                {
                    Console.WriteLine("Ingrese el mensaje, maximo 9 caracteres");
                    mensaje = Console.ReadLine();
                    if (mensaje.Length > 9)
                    {
                        Console.WriteLine($" Excede el límite ({mensaje.Length}/9 caracteres)");
                        continue;
                    }
                    break;
                }
                char final = '\0';
                string MensajeFinalizada = mensaje + final;
                SubRed subredorigen = ObtenerSubRedPorIP(iporigen);
                Mensajes nuevomensaje = new Mensajes(iporigen, ipdestino, MensajeFinalizada, secuenciaglobal,
                    ++idmensaje, subredorigen);
                mensajesRegistrados.Add(nuevomensaje.IdMensaje, nuevomensaje);
                secuenciaglobal += MensajeFinalizada.Length;
                tecladocontinuar();
            }
        }

        public void Enviar()

        {
            if (subredes.Count < 2)
            {
                Console.Clear();
                Console.WriteLine("===================Enviar Paquete===================");
                Console.WriteLine("Error: No hay red configurado o hay una sola red configurado en el sistema");
                tecladocontinuar();
                return;
            }
            Console.Clear();
            Console.WriteLine("===================Enviar Paquete===================");

            string ipenviar;
            bool ipValida = false;
            do
            {
                Console.Write("Ingrese IP  (PC o Router. Ej: 1.01 o 1.0): ");
                ipenviar = Console.ReadLine();


                ipValida = ExisteIP_PC(ipenviar) || ExisteIP_RT(ipenviar);

                if (!ipValida)
                {
                    Console.WriteLine(" Error: La IP no esta en el sistema");
                }
            } while (!ipValida);


            if (ipenviar.EndsWith(".01"))
            {
                EnviarDesdePC(ipenviar);
            }
            else if (ipenviar.EndsWith(".0"))
            {
                EnviarDesdeRouter(ipenviar);
            }
        }
        public void EnviarDesdePC(string ipPC)
        {
            SubRed subred = subredes.Values.First(s => s.IPpc == ipPC);
            if (subred.colaenviopc.Vacia())
            {
                Console.WriteLine(" No hay paquetes para enviar en esta PC");
                tecladocontinuar();
                return;
            }
            Paquete paquete = subred.colaenviopc.pop();
            Mensajes mensaje = ObtenerMensaje(paquete.IdMensaje);
            if (subred.colarouter.Llena())
            {
                Console.WriteLine($"Router ocupado.Ejecuta 'Enviar' desde el router {subred.IProuter}.");
                Console.WriteLine("El paquete volverá a la cola de la PC");
                subred.colaenviopc.push(paquete);
                tecladocontinuar();
                return;
            }
            else
            {
                if (mensaje.Estado == "Nuevo" && !subred.colaenviopc.Vacia()) mensaje.ActualizarEstado("Enviando");
                else if (mensaje.Estado == "Enviando" && subred.colaenviopc.Vacia()) mensaje.ActualizarEstado("Enviado");
                subred.colarouter.push(paquete);
                paquete.ActualizarUbicacion(subred.IProuter);
                Console.WriteLine($"✓ Paquete {paquete} ");
                tecladocontinuar();
            }

        }

        public void EnviarDesdeRouter(string ipRouter)
        {
            SubRed subredenviar = subredes.Values.FirstOrDefault(s => s.IProuter == ipRouter);
            if (subredenviar.colarouter.Vacia())
            {
                Console.WriteLine("No hay paquetes para enviar en este router");
                tecladocontinuar();
                return;
            }
            else
            {
                Paquete paquete = subredenviar.colarouter.pop();
                if (paquete.IPDestino == subredenviar.IPpc)
                {
                    if (subredenviar.colarecibpc.Vacia())
                    {
                        subredenviar.IDrecibir = paquete.IdMensaje;
                        subredenviar.colarecibpc.push(paquete);
                        paquete.ActualizarUbicacion(subredenviar.IPpc);
                        Console.WriteLine($"✓ Paquete {paquete} enviado");
                        tecladocontinuar();
                        return;
                    }

                    if (subredenviar.IDrecibir != paquete.IdMensaje)
                    {
                        subredenviar.colarouter.push(paquete);
                        Console.WriteLine(" La PC está recibiendo otro mensaje," +
                            "y este paquete no pertenece a este mensaje, " +
                            "por lo que este paquete será devuelto al final del router..");
                        tecladocontinuar();
                        return;
                    }
                    else
                    {
                        subredenviar.colarecibpc.push(paquete);
                        paquete.ActualizarUbicacion(subredenviar.IPpc);
                        Console.WriteLine($"✓ Paquete {paquete} enviado");
                    }

                    if (!ExistePaqueteIP(subredenviar.colarouter, paquete.IdMensaje))
                    {
                        SubRed subredOrigen = subredes.Values.First(s => s.IPpc == paquete.IPOrigen);
                        if (!ExistePaqueteIP(subredOrigen.colarouter, paquete.IdMensaje) && !ExistePaqueteIP(subredOrigen.colaenviopc, paquete.IdMensaje))
                        {
                            subredenviar.AgregarMenRecibidos(mensajesRegistrados);
                        }
                    }

                }
                else
                {

                    SubRed subredrecibir = ObtenerSubRedPorIP(paquete.IPDestino);
                    if (subredrecibir.colarouter.Llena())
                    {
                        Console.WriteLine($"Router ocupado.Ejecuta 'Enviar' desde el router {subredrecibir.IProuter}.");
                        Console.WriteLine("El paquete volverá a la cola de Router: " + subredenviar.IProuter);
                        subredenviar.colarouter.push(paquete);
                        tecladocontinuar();
                        return;
                    }
                    else
                    {
                        subredrecibir.colarouter.push(paquete);
                        paquete.ActualizarUbicacion(subredrecibir.IProuter);
                        Console.WriteLine($"✓ Paquete {paquete} enviado");
                    }


                }
            }

            tecladocontinuar();
        }

        public void mostrartodasredconpaq()
        {
            Console.Clear();
            
            if (subredes.Count == 0)
            {
                Console.WriteLine("=============Mostrar el status de la Red=============");                
                Console.WriteLine("No hay red configurado en el sistema");
                tecladocontinuar();
                return;
            }

            foreach (var subred in subredes.Values)
            {

                Console.WriteLine("=============Mostrar el status de la Red=============");
                Console.WriteLine("======================================");
                Console.WriteLine("Numero de Red: " + subred.NumRed );
                Console.WriteLine("\nIP ROUTER: " + subred.IProuter);
                Console.WriteLine("\nCOLA DE PAQUETE DE ROUTER: ");
                mostrarCOLApaquete(subred.colarouter);
                Console.WriteLine("\nIP PC: " + subred.IPpc);
                Console.WriteLine("\nCOLA DE PAQUETE DE ENVIO: ");
                mostrarCOLApaquete(subred.colaenviopc);
                Console.WriteLine("\nCOLA DE PAQUETE DE RECIBIDO: ");
                mostrarCOLApaquete(subred.colarecibpc);
                Console.WriteLine("======================================");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nPresione enter para ver siguiente red, si no tiene mas se regresara al menu");
                Console.ResetColor(); 
                Console.ReadKey();
                Console.Clear();
            }
        }

        public void mostrarSubredconpaq()
        {
            Console.Clear();
            if (subredes.Count == 0)
            {
                
                Console.WriteLine("===========Mostrar el status de la SubRed============");
                Console.WriteLine("No hay red configurado en el sistema");
                tecladocontinuar();
                return;
            }
            string numRed = "";
            Console.WriteLine("===========Mostrar el status de la SubRed============");
            Console.Write("Ingrese Numero de Red ");
            numRed = Console.ReadLine();
            if (!subredes.ContainsKey(numRed))
            {
                Console.WriteLine($"❌ No existe la subred {numRed}");
                return;
            }
            Console.Clear();
            Console.WriteLine("===========Mostrar el status de la SubRed============");
            SubRed subredmostrar = subredes[numRed];
            Console.WriteLine("Numero de Red: " + subredmostrar.NumRed);
            Console.WriteLine("\nIP ROUTER: " + subredmostrar.IProuter);
            mostrarCOLApaquete(subredmostrar.colarouter);
            Console.WriteLine("------------------------");


            Console.WriteLine("\nIP PC: " + subredmostrar.IPpc);
            Console.WriteLine("COLA DE PAQUETE DE ENVIO: ");
            mostrarCOLApaquete(subredmostrar.colaenviopc);
            Console.WriteLine("------------------------");
            
            Console.WriteLine("\nCOLA DE PAQUETES RECIBIDOS: ");
            mostrarCOLApaquete(subredmostrar.colarecibpc);
            Console.WriteLine("------------------------");

            tecladocontinuar();



        }
        public void mostrarequipoconpaq()
        {
            Console.Clear();
            if (subredes.Count == 0)
            {
                Console.WriteLine("===========Mostrar el Status de un Equipo============");

                Console.WriteLine("No hay red configurado en el sistema");
                tecladocontinuar();
                return;
            }
            string ipmostrar;
            bool ipValida = false;
            Console.WriteLine("===========Mostrar el Status de un Equipo============");

            Console.Write("\nIngrese IP  del equipo(PC o Router. Ej: 1.01 o 1.0): ");
            ipmostrar = Console.ReadLine();
            ipValida = ExisteIP_PC(ipmostrar) || ExisteIP_RT(ipmostrar);
            if (!ipValida)
            {
                Console.WriteLine(" Error: La IP no esta en el sistema");
                tecladocontinuar();
                return;
            }
            if (ipmostrar.EndsWith(".01"))
            {
                Console.Clear();
                Console.WriteLine("=============Mostrar el Status de un PC==============");
                SubRed subredmostrar = subredes.Values.First(s => s.IPpc == ipmostrar);
                Console.WriteLine("IP PC: " + subredmostrar.IPpc);
                Console.WriteLine("COLA DE PAQUETE DE ENVIO: ");
                mostrarCOLApaquete(subredmostrar.colaenviopc);
                Console.WriteLine("COLA DE PAQUETE DE RECIBIDO: ");
                mostrarCOLApaquete(subredmostrar.colarecibpc);
                Console.WriteLine("------------------------");

            }
            else if (ipmostrar.EndsWith(".0"))
            {
                Console.Clear();
                Console.WriteLine("===========Mostrar el Status de un Router============");
                SubRed subredmostrar = subredes.Values.First(s => s.IProuter == ipmostrar);
                Console.WriteLine("IP ROUTER: " + subredmostrar.IProuter);
                mostrarCOLApaquete(subredmostrar.colarouter);
                Console.WriteLine("------------------------");
            }
            tecladocontinuar();
        }

        public void mostrarCOLApaquete(colapaquete colamostrar)
        {


            colapaquete colatemporal = new colapaquete(10);
            Console.WriteLine("Total paquete: " + colamostrar.contador);
            if (!colamostrar.Vacia())
            {
                Console.WriteLine("Secuencia del Paq| Id de Mensaje | Contenido  |     RUTA      | Ubicacion de IP  ");
                while (!colamostrar.Vacia())
                {
                    Paquete paquete = colamostrar.pop();
                    Console.WriteLine($" {paquete}");
                    colatemporal.push(paquete);
                }
                while (!colatemporal.Vacia())
                {
                    Paquete paquete = colatemporal.pop();
                    colamostrar.push(paquete);
                }
            }
            

        }




        public void EliminarPaquete()
        {
            Console.Clear();
            Console.WriteLine("===========Eliminar un paquete de una Cola============");
            if (subredes.Count == 0)
            {
                Console.WriteLine("❌ No hay subredes configuradas");
                tecladocontinuar();
                return;
            }
            string ipEquipo;
            bool ipValida = false;
            Console.Write("Ingrese IP del equipo donde está el paquete (ej: 1.01 o 1.0): ");
            ipEquipo = Console.ReadLine();
            ipValida = ExisteIP_PC(ipEquipo) || ExisteIP_RT(ipEquipo);
            if (!ipValida)
            {
                Console.WriteLine("❌ Error: La IP no esta en el sistema");
                return;
            }
            Console.Write("Ingrese la secuencia del paquete que desea eliminar ");
            int secuenciaeliminar = int.Parse(Console.ReadLine());
            SubRed subred = subredes.Values.First(s => s.IPpc == ipEquipo || s.IProuter == ipEquipo);
            bool eliminado = false;
            if (ipEquipo.EndsWith(".01"))
            {
                eliminado = EliminarDeCola(subred.colaenviopc, secuenciaeliminar);
            }
            else if (ipEquipo.EndsWith(".0"))
            {
                eliminado = EliminarDeCola(subred.colarouter, secuenciaeliminar);
            }

            if (eliminado)
            {
                Console.WriteLine($"Paquete {secuenciaeliminar} eliminado de {ipEquipo}");
            }
            else Console.WriteLine($"Paquete {secuenciaeliminar} no encontrado en {ipEquipo}\n");
            tecladocontinuar();
        }

        public void MensajesRecibidos()
        {
            Console.Clear();
            Console.WriteLine("=================Mensajes Recibidos===================");


            if (subredes.Count == 0)
            {
                Console.WriteLine("No hay red configurado en el sistema");
                tecladocontinuar();
                return;
            }
            else
            {
                Console.Write("Ingrese IP(PC) ");
                string IPmostrar = Console.ReadLine();
                if (!ExisteIP_PC(IPmostrar))
                {
                    Console.WriteLine("Este IP no esta en el sistema");
                    tecladocontinuar();
                    return;
                }
                else
                {
                    SubRed subRedmostrar = subredes.Values.First(s => s.IPpc == IPmostrar);
                    foreach (var mensaje in subRedmostrar.MensajesRecibidos)
                    {
                        Console.WriteLine(mensaje.ToString());
                    }
                }

            }
            tecladocontinuar();
        }

        public void ConsultarPaquete()
        {

            Console.Clear();
            
            Console.WriteLine("================Consultar un Paquete==================");
            if (subredes.Count == 0)
            {
                Console.WriteLine("❌ No hay subredes configuradas");
                tecladocontinuar();
                return;
            }
            if (mensajesRegistrados.Count == 0)
            {
                Console.WriteLine("No hay paquete en el sistema!");
                tecladocontinuar();
                return;
            }
            Dictionary<int, Paquete> TodosPaquete = new Dictionary<int, Paquete>();
            int secuenciabuscar;
            while (true)
            {
                Console.Write("Ingrese el numero de secuencia:");
                try
                {
                    secuenciabuscar = Convert.ToInt32(Console.ReadLine());
                    if (secuenciabuscar > 0) break;
                    else Console.WriteLine("ERROR! NO PUEDE SER 0 O UN NUMERO NEGATIVO!");
                }
                catch (FormatException)
                {
                    Console.WriteLine("DEBE SER UN NUMERO ENTERO!");
                }

            }
            foreach (var subred in subredes.Values)
            {
                MeterTodosPaquete(subred.colaenviopc, TodosPaquete);
                MeterTodosPaquete(subred.colarecibpc, TodosPaquete);
                MeterTodosPaquete(subred.colarouter, TodosPaquete);
            }

            if (TodosPaquete.ContainsKey(secuenciabuscar))
            {

                Console.Write($"Informacion del paquete: \n " +
                    $"Secuencia del Paq| Id de Mensaje | Contenido  |     RUTA      | Ubicacion de IP  \n"+
                    $"{ TodosPaquete[secuenciabuscar]}");
                SubRed subredposicion = new SubRed();
                foreach(var subred in subredes.Values)
                {
                    if(TodosPaquete[secuenciabuscar].Ubicacion == subred.IPpc)
                    {
                        subredposicion = subred;
                        break;
                    }
                    else if (TodosPaquete[secuenciabuscar].Ubicacion == subred.IProuter)
                    {
                        subredposicion = subred;
                        break ;
                    }
                }
                if (TodosPaquete[secuenciabuscar].Ubicacion.EndsWith(".01"))
                {
                    int posicion;
                    posicion = subredposicion.colaenviopc.ObtenerPosicionPorSecuencia(secuenciabuscar);
                    if (posicion != -1)
                    {
                        Console.WriteLine($"Posicion en la cola de envio de PC: {TodosPaquete[secuenciabuscar].Ubicacion}, posicion: {posicion}");
                    }
                    else 
                    if(posicion == -1)
                    {
                        posicion = subredposicion.colarecibpc.ObtenerPosicionPorSecuencia(secuenciabuscar);
                        Console.WriteLine($"Posicion en la cola de paquete recibido de PC: {TodosPaquete[secuenciabuscar].Ubicacion}, posicion: {posicion}");

                    }

                }
                else 
                {
                    int posicion;
                    posicion = subredposicion.colarouter.ObtenerPosicionPorSecuencia(secuenciabuscar);
                    Console.WriteLine($"Posicion en la cola de  router: {TodosPaquete[secuenciabuscar].Ubicacion}, posicion: {posicion}");
                }



            }
            else Console.WriteLine($"NO SE ENCONTRO PAQUETE {secuenciabuscar}, puede ser que no existe o fue eliminado");
            tecladocontinuar();
        }

        public void MeterTodosPaquete(colapaquete cola, Dictionary<int, Paquete> TodosPaquete)
        {
            colapaquete colatemporal = new colapaquete(10);
            while (!cola.Vacia())
            {
                Paquete paquete = cola.pop();
                TodosPaquete.Add(paquete.Secuencia, paquete);
                colatemporal.push(paquete);
            }
            while (!colatemporal.Vacia())
            {
                Paquete paquete = colatemporal.pop();
                cola.push(paquete);
            }
        }

        private bool EliminarDeCola(colapaquete cola, int secuencia)
        {
            
            if (cola.Vacia()) return false;

            colapaquete temp = new colapaquete(cola.capacidad);
            bool encontrado = false;
            
            
            while (!cola.Vacia())
            {
                Paquete p = cola.pop();
                if (p.Secuencia == secuencia)
                {
                    encontrado = true;
                    continue;
                }
                temp.push(p);
            }
            while (!temp.Vacia())
            {
                cola.push(temp.pop());
            }

            return encontrado;
        }



        private bool ExisteIP_PC(string ipABuscar)
        {
            foreach (var subred in subredes.Values)
            {

                if (subred.IPpc == ipABuscar)
                {
                    return true;
                }
            }
            return false;
        }

        public Mensajes ObtenerMensaje(int idMensaje)
        {
            if (mensajesRegistrados.TryGetValue(idMensaje, out Mensajes mensaje))
            {
                return mensaje;
            }
            return null;
        }

        private SubRed ObtenerSubRedPorIP(string ip)
        {
            return subredes.Values.FirstOrDefault(s => s.IPpc == ip);
        }

        private bool ExisteIP_RT(string ipABuscar)
        {
            foreach (var subred in subredes.Values)
            {

                if (subred.IProuter == ipABuscar)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void tecladocontinuar()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Presione enter para continuar");
            Console.ResetColor();
            Console.ReadKey(); Console.Clear();
        }
        public int LeerNumero(string mensaje)
        {
            while (true)
            {
                try
                {
                    Console.Write(mensaje);
                    int numero = int.Parse(Console.ReadLine());
                    if (numero <= 0) Console.WriteLine("❌ Error: No se permiten números negativos o cero");
                    return numero;
                }
                catch (FormatException)
                {
                    Console.WriteLine("❌ Error: Ingrese un número válido (ej: 190).");
                }

            }
        }

        public bool ExistePaqueteIP(colapaquete colarevisar, int IDrevisar)
        {
            colapaquete temporal = new colapaquete(10);
            bool Existe = false;
            while (!colarevisar.Vacia())
            {
                Paquete PaqueteRevisar = colarevisar.pop();
                temporal.push(PaqueteRevisar);
                if (PaqueteRevisar.IdMensaje == IDrevisar)
                {
                    Existe = true;
                    break;
                }
            }
            while (!colarevisar.Vacia())
            {
                Paquete PaqueteRevisar = colarevisar.pop();
                temporal.push(PaqueteRevisar);
            }
            while (!temporal.Vacia())
            {
                colarevisar.push(temporal.pop());
            }
            return Existe;
        }


        public void borrarTODOREDES()
        {
            Console.Clear();
            if (subredes.Count == 0) {
                Console.WriteLine("No hay red configurado");
                tecladocontinuar();
                return;
            }
            int opcion = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("================Consultar un Paquete==================");
                Console.WriteLine("Esta seguro de eliminar todas las redes del Sistema?");
                Console.WriteLine("<1> Si, estoy seguro ");
                Console.WriteLine("<2> No, regresa");
                opcion = LeerNumero("Ingrese una opcion:");
            } while (opcion != 1 && opcion != 2);
            if (opcion == 1)
            {
                subredes.Clear();
                Console.WriteLine("Se elimino todas las redes");
                tecladocontinuar();
            }
            else 
            {
                Console.Clear();
                return;
            }
            
            
        }

        public void TodosMensjas()
        {
            Console.Clear();
            Console.WriteLine("================Mostrar Todos Los Mensajes=================");
            if (mensajesRegistrados.Count == 0)
            {
                Console.WriteLine("No hay mensaje registrado en el sistema");
                tecladocontinuar();
                return;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("================Mostrar Todos Los Mensajes=================");
                foreach (var mensaje in mensajesRegistrados)
                {
                    Console.WriteLine(mensaje);
                }
            }
            tecladocontinuar();
        }
        public void LiberarColaRe()
        {
            Console.Clear();
            Console.WriteLine("===============Liberar Cola de Recibido================");
            Console.Write("Ingresa IP de PC(Ejm: 190.01): ");
            string ipcolaliberar = Console.ReadLine();
            bool ipvalido = ExisteIP_PC(ipcolaliberar);
            if (ipvalido == false)
            {
                Console.WriteLine("Esta IP no esta configurado en el sistema.");
                tecladocontinuar();
                return;
            }
            else
            {
                SubRed subredliberar = subredes.Values.FirstOrDefault(s => s.IPpc == ipcolaliberar);
                subredliberar.colarecibpc.borrar();
                Console.WriteLine("Ya la de cola de PC:" + ipcolaliberar  +" esta vacia");
                tecladocontinuar();
            }

        }

    }
}

