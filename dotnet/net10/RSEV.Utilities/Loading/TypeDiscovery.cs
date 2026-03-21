using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RSEV.Utilities.Loading
{
    /// <summary>
    /// Bietet Funktionen zur Typ-Erkennung innerhalb von Assemblies.
    /// TypeDiscovery ermöglicht das Auffinden von Klassen, Interfaces,
    /// Attributen oder beliebigen anderen Strukturen und dient als
    /// universelles Werkzeug für Plugin-Systeme, automatische Registrierung
    /// von Controllern, Commands oder Modulen.
    /// </summary>
    public class TypeDiscovery
    {
        /// <summary>
        /// Sucht in einer Assembly nach allen Klassen, die ein bestimmtes
        /// Interface implementieren oder von einer bestimmten Basisklasse erben.
        /// </summary>
        /// <typeparam name="T">Das Interface oder die Basisklasse.</typeparam>
        /// <param name="assembly">Die Assembly, in der gesucht werden soll.</param>
        /// <returns>Eine Liste aller passenden Typen.</returns>
        public virtual List<Type> FindTypesImplementing<T>(Assembly assembly)
        {
            var target = typeof(T);

            return assembly
                .GetTypes()
                .Where(t => target.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                .ToList();
        }

        /// <summary>
        /// Sucht in mehreren Assemblies nach allen Klassen, die ein bestimmtes
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
        /// Sucht in einer Assembly nach allen Klassen, die ein bestimmtes Attribut besitzen.
        /// </summary>
        /// <typeparam name="TAttribute">Der Typ des gesuchten Attributs.</typeparam>
        /// <param name="assembly">Die Assembly, in der gesucht werden soll.</param>
        /// <returns>Eine Liste aller passenden Typen.</returns>
        public virtual List<Type> FindTypesWithAttribute<TAttribute>(Assembly assembly)
            where TAttribute : Attribute
        {
            return assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<TAttribute>() != null)
                .ToList();
        }

        /// <summary>
        /// Sucht in mehreren Assemblies nach allen Klassen, die ein bestimmtes Attribut besitzen.
        /// </summary>
        /// <typeparam name="TAttribute">Der Typ des gesuchten Attributs.</typeparam>
        /// <param name="assemblies">Die Assemblies, in denen gesucht werden soll.</param>
        /// <returns>Eine Liste aller passenden Typen.</returns>
        public virtual List<Type> FindTypesWithAttribute<TAttribute>(IEnumerable<Assembly> assemblies)
            where TAttribute : Attribute
        {
            var result = new List<Type>();

            foreach (var asm in assemblies)
            {
                result.AddRange(FindTypesWithAttribute<TAttribute>(asm));
            }

            return result;
        }

        /// <summary>
        /// Sucht in einer Assembly nach allen Klassen, die ein bestimmtes Prädikat erfüllen.
        /// Dies ermöglicht flexible, benutzerdefinierte Filterlogik.
        /// </summary>
        /// <param name="assembly">Die Assembly, in der gesucht werden soll.</param>
        /// <param name="predicate">Die Filterfunktion, die auf jeden Typ angewendet wird.</param>
        /// <returns>Eine Liste aller passenden Typen.</returns>
        public virtual List<Type> FindTypesByPredicate(Assembly assembly, Func<Type, bool> predicate)
        {
            return assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && predicate(t))
                .ToList();
        }

        /// <summary>
        /// Sucht in mehreren Assemblies nach allen Klassen, die ein bestimmtes Prädikat erfüllen.
        /// </summary>
        /// <param name="assemblies">Die Assemblies, in denen gesucht werden soll.</param>
        /// <param name="predicate">Die Filterfunktion, die auf jeden Typ angewendet wird.</param>
        /// <returns>Eine Liste aller passenden Typen.</returns>
        public virtual List<Type> FindTypesByPredicate(IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
        {
            var result = new List<Type>();

            foreach (var asm in assemblies)
            {
                result.AddRange(FindTypesByPredicate(asm, predicate));
            }

            return result;
        }

    }

}
