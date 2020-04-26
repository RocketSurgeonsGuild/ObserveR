using System;
using System.Reactive;

namespace ObserveR
{
    /// <summary>
    /// Interface defining a handler for a request.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public interface IRequestHandler<in TRequest, out TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Handles the given request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An observable response.</returns>
        IObservable<TResponse> Handle(TRequest request);
    }

    /// <summary>
    /// Interface defining a handler for a request.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
        where TRequest : IRequest
    {
        /// <summary>
        /// Handles the given request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An observable response.</returns>
        IObservable<Unit> Handle(TRequest request);
    }
}