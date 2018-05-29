using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ADE.Extras.ServiceManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Run();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run()
        {
            Log.Instance.LogPath = ConfigurationManager.AppSettings["Path"];
            Log.Instance.LogFileName = "ADEServiceManagerService";

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            try
            {
                watcher.Path = ConfigurationManager.AppSettings["Path"];

                /* Watch for changes in LastAccess and LastWrite times, and
                   the renaming of files or directories. */
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                // Only watch text files.
                watcher.Filter = "*.txt";

                // Add event handlers.
                watcher.Created += new FileSystemEventHandler(OnCreated);

                // Begin watching.
                watcher.EnableRaisingEvents = true;


                // Wait for the user to quit the program.
                Console.WriteLine(" Presione \'q\' para salir de la prueba.");
                while (Console.Read() != 'q') ;
            }
            catch (Exception e)
            {
                Log.WriteLine("Configuración errónea de monitoreo");
            }
        }

        // Define the event handlers.
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            try
            {
                if (GetExclusiveFileLock(e.FullPath))
                {
                    string[] data = null;

                    using (StreamReader sr = new StreamReader(e.FullPath))
                    {
                        data = sr.ReadToEnd().Split(' ');
                        sr.Close();
                    }

                    if (data.Length > 1)
                    {
                        if (data[1].ToUpper() == "I") StartService(data[0], 5000);
                        if (data[1].ToUpper() == "D") StopService(data[0], 5000);
                        if (data[1].ToUpper() == "R") RestartService(data[0], 5000);
                    }

                    //File.Delete(e.FullPath);
                }

            }
            catch (Exception r)
            {
                Log.WriteLine(r.Message);
            }

        }

        private static bool GetExclusiveFileLock(string path)
        {
            var fileReady = false;
            const int MaximumAttemptsAllowed = 50;
            var attemptsMade = 0;

            while (!fileReady && attemptsMade <= MaximumAttemptsAllowed)
            {
                try
                {
                    using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        fileReady = true;
                    }
                }
                catch (IOException)
                {
                    attemptsMade++;
                    Thread.Sleep(100);
                }
            }

            return fileReady;
        }

        public static void StartService(string serviceName, int timeoutMilliseconds)
        {
            var psi = new ProcessStartInfo("net.exe", "start " + serviceName);
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;
            psi.WorkingDirectory = Environment.SystemDirectory;
            var st = Process.Start(psi);
            st.WaitForExit();
        }

        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            var psi = new ProcessStartInfo("net.exe", "stop " + serviceName);
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.WorkingDirectory = Environment.SystemDirectory;
            var st = Process.Start(psi);
            st.WaitForExit();
        }

        public static void RestartService(string serviceName, int timeoutMilliseconds)
        {
            var psi = new ProcessStartInfo("net.exe", "stop " + serviceName);
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;
            psi.WorkingDirectory = Environment.SystemDirectory;
            var st = Process.Start(psi);
            st.WaitForExit();

            psi = new ProcessStartInfo("net.exe", "start " + serviceName);
            psi.UseShellExecute = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.WorkingDirectory = Environment.SystemDirectory;
            st = Process.Start(psi);
            st.WaitForExit();
        }
    }
}
