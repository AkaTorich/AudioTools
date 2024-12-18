using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WAVConverter
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                // Получаем имя сборки без версии, культуры и публичного ключа
                string assemblyName = new AssemblyName(args.Name).Name + ".dll";

                // Определяем путь к папке Libraries относительно исполняемого файла
                string librariesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libraries");

                // Формируем полный путь к DLL
                string assemblyPath = Path.Combine(librariesPath, assemblyName);

                // Проверяем, существует ли файл
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AssemblyResolve.log");
                string logMessage = $"{DateTime.Now}: Failed to resolve assembly {args.Name} - {ex.Message}{Environment.NewLine}";
                File.AppendAllText(logPath, logMessage);
            }

            // Если файл не найден или произошла ошибка, возвращаем null
            return null;
        }
    }
}
