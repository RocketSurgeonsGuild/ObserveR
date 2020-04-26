using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileHeaderFileNameDocumentationMustMatchTypeName", Justification = "Reviewed.")]
namespace ObserveR
{
    public delegate object HandlerFactory(Type serviceType);

    /// <summary>
    /// Extensions for the handler factory.
    /// </summary>
    public static class HandlerFactoryExtensions
    {
        /// <summary>
        /// Gets an instance of the provided type from the handler factory.
        /// </summary>
        /// <param name="factory">The handler factory.</param>
        /// <typeparam name="T">The requested type.</typeparam>
        /// <returns>An instance of the provided type.</returns>
        public static T GetInstance<T>(this HandlerFactory factory) => (T)factory(typeof(T));

        /// <summary>
        /// Gets instance of the provided type from the handler factory.
        /// </summary>
        /// <param name="factory">The handler factory.</param>
        /// <typeparam name="T">The requested type.</typeparam>
        /// <returns>An instance of the provided type.</returns>
        public static IEnumerable<T> GetInstances<T>(this HandlerFactory factory) => (IEnumerable<T>)factory(typeof(IEnumerable<T>));
    }
}