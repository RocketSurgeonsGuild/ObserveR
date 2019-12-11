using System;

namespace ObservR
{
    /// <summary>
    /// Interface representing the observer.
    /// </summary>
    public interface IObserver
    {
        /// <summary>
        /// Send a cold observable.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <returns>A response.</returns>
        IObservable<TResponse> Send<TResponse>(IRequest<TResponse> request);

        /// <summary>
        /// Publishes a hot observable.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <returns>A response.</returns>
        IObservable<TResponse> Publish<TResponse>(IRequest<TResponse> request);
    }
}