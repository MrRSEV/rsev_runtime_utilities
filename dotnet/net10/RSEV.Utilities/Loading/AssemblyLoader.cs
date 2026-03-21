using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RSEV.Utilities.Loading
{
    /// <summary>
    /// Bietet Funktionen zum Laden von Assemblies, zum Auffinden von Typen
    /// und zum Erzeugen von Instanzen. Der AssemblyLoader ist universell
    /// einsetzbar und eignet sich ideal für Plugin-Systeme, automatische
    /// Discovery von Controllern, Commands oder beliebigen anderen Klassen.
    /// </summary>
    public class AssemblyLoader
    {
        /// <summary>
        /// Lädt eine Assembly aus einer Datei.
        /// </summary>
        /// <param name="path">Der Pfad zur Assembly-Datei (.dll).</param>
        /// <returns>Die geladene Assembly oder null, falls ein Fehler auftritt.</returns>
        public virtual Assembly LoadAssembly(string path)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                return Assembly.LoadFrom(path);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Lädt alle Assemblies aus einem Verzeichnis, die die Endung ".dll" besitzen.
        /// </summary>
        /// <param name="directory">Das Zielverzeichnis.</param>
        /// <returns>Eine Liste aller erfolgreich geladenen Assemblies.</returns>
        public virtual List<Assembly> LoadAssembliesFromDirectory(string directory)
        {
            var result = new List<Assembly>();

            if (!Directory.Exists(directory))
                return result;

            var files = Directory.GetFiles(directory, "*.dll");

            foreach (var file in files)
            {
                var asm = LoadAssembly(file);
                if (asm != null)
                    result.Add(asm);
            }

            return result;
        }

        /// <summary>
        /// Sucht in einer Assembly nach allen Typen, die ein bestimmtes Interface
        /// implementieren oder von einer bestimmten Basisklasse erben.
        /// </summary>
        /// <typeparam name="T">Das Interface oder die Basisklasse.</typeparam>
        /// <param name="assembly">Die Assembly, in der gesucht werden soll.</param>
        /// <returns>Eine Liste aller passenden Typen.</returns>
        public virtual List<Type> FindTypesImplementing<T>(Assembly assembly)
        {
            var target = typeof(T);

            return assembly
                .GetTypes()
                .Where(t => target.IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                .ToList();
        }

        /// <summary>
        /// Sucht in mehreren Assemblies nach allen Typen, die ein bestimmtes
        /// Interface implementieren oder von einer bestimmten Basisklasse erben.
        /// </summary>
        /// <typeparam name="T">Das Interface oder die Basisklasse.</typeparam>
        /// <param name="assemblies">Die Assemblies, in denen gesucht werden soll.</param>
        /// <returns>Eine Liste aller passenden Typen.</returns>
        public virtual List<Type> FindTypesImplementing<T>(IEnumerable<Assembly> assemblies)
        {
            var result = new List<Type>();

            foreach (var asm in assemblies)
            {
                result.AddRange(FindTypesImplementing<T>(asm));
            }

            return result;
        }

        /// <summary>
        /// Erstellt eine Instanz eines Typs, sofern dieser einen parameterlosen
        /// Konstruktor besitzt.
        /// </summary>
        /// <typeparam name="T">Der erwartete Rückgabetyp.</typeparam>
        /// <param name="type">Der zu instanziierende Typ.</param>
        /// <returns>Eine Instanz des Typs oder null, falls die Instanziierung fehlschlägt.</returns>
        public virtual T CreateInstance<T>(Type type)
        {
            try
            {
                if (Activator.CreateInstance(type) is T instance)
                    return instance;
            }
            catch
            {
                // Ignorieren
            }

            return default;
        }

        /// <summary>
        /// Erstellt Instanzen aller Typen, die ein bestimmtes Interface
        /// implementieren oder von einer bestimmten Basisklasse erben.
        /// </summary>
        /// <typeparam name="T">Das Interface oder die Basisklasse.</typeparam>
        /// <param name="assembly">Die Assembly, in der gesucht werden soll.</param>
        /// <returns>Eine Liste aller erfolgreich erzeugten Instanzen.</returns>
        public virtual List<T> CreateInstancesImplementing<T>(Assembly assembly)
        {
            var result = new List<T>();

            var types = FindTypesImplementing<T>(assembly);

            foreach (var type in types)
            {
                var instance = CreateInstance<T>(type);
                if (instance != null)
                    result.Add(instance);
            }

            return result;
        }

        /// <summary>
        /// Erstellt Instanzen aller Typen, die ein bestimmtes Interface
        /// implementieren oder von einer bestimmten Basisklasse erben,
        /// über mehrere Assemblies hinweg.
        /// </summary>
        /// <typeparam name="T">Das Interface oder die Basisklasse.</typeparam>
        /// <param name="assemblies">Die Assemblies, in denen gesucht werden soll.</param>
        /// <returns>Eine Liste aller erfolgreich erzeugten Instanzen.</returns>
        public virtual List<T> CreateInstancesImplementing<T>(IEnumerable<Assembly> assemblies)
        {
            var result = new List<T>();

            foreach (var asm in assemblies)
            {
                result.AddRange(CreateInstancesImplementing<T>(asm));
            }

            return result;
        }

    }

}