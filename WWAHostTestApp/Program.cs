using System;
using System.Windows.Forms;

namespace WWAHostTestApp
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Permettre le chargement des DLL managées depuis le dossier "Libs"
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var assemblyName = new System.Reflection.AssemblyName(args.Name).Name + ".dll";
                var probePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", assemblyName);
                if (System.IO.File.Exists(probePath))
                    return System.Reflection.Assembly.LoadFrom(probePath);
                return null;
            };

            // Ajouter le dossier "Libs" au PATH pour les DLL natives
            var path = Environment.GetEnvironmentVariable("PATH");
            var libsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs");
            if (System.IO.Directory.Exists(libsPath) && !path.Contains(libsPath))
            {
                Environment.SetEnvironmentVariable("PATH", libsPath + ";" + path);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}