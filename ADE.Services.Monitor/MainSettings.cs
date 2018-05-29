using System.IO;

namespace ADE.Services.Monitor
{
    public class MainSettings
    {
        //Variables para el uso del servicio
        public string ADE_ROOT { get; set; }
        public string LOGS { get; set; }

        public MainSettings()
        {
            var FullPath = Directory.GetCurrentDirectory();
            var LevelsUp = @"..\..\";
            ADE_ROOT = Path.GetFullPath(Path.Combine(FullPath, LevelsUp));
            //ADE_ROOT = @"D:\SLIN-ADE\";
            LOGS = $@"{ADE_ROOT}Logs\";
        }
    }
}
