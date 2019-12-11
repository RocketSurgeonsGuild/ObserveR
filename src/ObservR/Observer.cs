using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

namespace ObservR
{
    /// <summary>
    /// Observer that manages notifications.
    /// </summary>
    public class Observer : IObserver
    {
        /// <inheritdoc/>
        public IObservable<TResponse> Send<TResponse>(IRequest<TResponse> request) =>
            Observable.Return(default(TResponse));

        /// <inheritdoc/>
        public IObservable<TResponse> Publish<TResponse>(IRequest<TResponse> request) =>
            Observable.Return(default(TResponse));
    }
}
