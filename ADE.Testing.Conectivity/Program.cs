using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ADE.Testing.Conectivity
{
    class Program
    {
        static void Main(string[] args)
        {
            var FullPath = Directory.GetCurrentDirectory();
            var LevelsUp = @"..\..\";
            var ADE_ROOT = Path.GetFullPath(Path.Combine(FullPath, LevelsUp));

            if (args.Length < 1)
            {
                args = new string[1];
                args[0] = Console.ReadLine();
            }

            
            if (args.Length > 0)
            {
                string[] a = args[0].Split(' ');
                int ArgsQty = a.Length;

                if (a[0].ToLower() == "-p")
                {
                    if(ArgsQty < 3)
                    {
                        Console.WriteLine(" Para usar el comando ping, se necesitan");
                        Console.WriteLine(" 2 parámetros: -p [host] [port]");
                        return;
                    }

                    PingHost(a[1], int.Parse(a[2]));
                }
                if(a[0].ToLower() == "-h" || a[0].ToLower() == "--help")
                {
                    Console.WriteLine("===================================================");
                    Console.WriteLine(" Herramienta de Testing de Conexiones / ADE");
                    Console.WriteLine("");
                    Console.WriteLine(" Lista de Comandos");
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine(" -h: --help");
                    Console.WriteLine("     Este texto de ayuda");
                    Console.WriteLine(" -p: --ping [host] [port]");
                    Console.WriteLine("     prueba de conexion a [host]");
                    Console.WriteLine("     por el puerto definido [port]");
                    Console.WriteLine(" -d: --database");
                    Console.WriteLine("     realiza una prueba de conexión a la ");
                    Console.WriteLine("     base de datos con el ConectionString");
                    Console.WriteLine("     definido en el archivo MainConfig.xml");
                    Console.WriteLine("===================================================");
                }
            } 
        }

        public static bool PingHost(string _HostURI, int _PortNumber)
        {
            try
            {
                TcpClient client = new TcpClient(_HostURI, _PortNumber);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error pinging host:'" + _HostURI + ":" + _PortNumber.ToString() + "'");
                return false;
            }
        }

        //public static 
    }
}
