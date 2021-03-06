﻿using System;
using System.Reactive;

namespace ObserveR
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
        /// Send a cold observable.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response.</returns>
        IObservable<object> Send(object request);

        /// <summary>
        /// Publishes a hot observable.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <returns>A response.</returns>
        IObservable<Unit> Publish(IRequest request);
    }
}