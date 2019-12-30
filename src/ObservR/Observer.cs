using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly ConcurrentDictionary<Type, object> RequestHandlers = new ConcurrentDictionary<Type, object>();
        private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> NotificationHandlers = new ConcurrentDictionary<Type, NotificationHandlerWrapper>();

        private readonly HandlerFactory _handlerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Observer"/> class.
        /// </summary>
        /// <param name="handlerFactory">The handler factory.</param>
        public Observer(HandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        /// <inheritdoc/>
        public IObservable<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            var handler =
                (RequestHandlerWrapper<TResponse>)RequestHandlers
                    .GetOrAdd(
                        requestType,
                        t =>
                            Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse))));

            return handler.Handle(request, _handlerFactory);
        }

        /// <inheritdoc/>
        public IObservable<object> Send(object request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            var requestInterfaceType =
                requestType
                    .GetInterfaces()
                    .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IRequest<>));
            var isValidRequest = requestInterfaceType != null;

            if (!isValidRequest)
            {
                throw new ArgumentException($"{nameof(request)} does not implement ${nameof(IRequest)}");
            }

            var responseType = requestInterfaceType.GetGenericArguments()[0];
            var handler =
                RequestHandlers
                    .GetOrAdd(
                        requestType,
                        t =>
                            Activator
                                .CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, responseType)));

            return ((RequestHandlerBase)handler).Handle(request, _handlerFactory);
        }

        /// <inheritdoc/>
        public IObservable<object> Send<TResponse>(object request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var requestType = request.GetType();
            var requestInterfaceType =
                requestType
                    .GetInterfaces()
                    .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IRequest<>));
            var isValidRequest = requestInterfaceType != null;

            if (!isValidRequest)
            {
                throw new ArgumentException($"{nameof(request)} does not implement ${nameof(IRequest)}");
            }

            var responseType = requestInterfaceType.GetGenericArguments()[0];
            var handler =
                RequestHandlers
                    .GetOrAdd(
                        requestType,
                        t =>
                            Activator.CreateInstance(typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, responseType)));

            // call via dynamic dispatch to avoid calling through reflection for performance reasons
            return ((RequestHandlerBase)handler).Handle(request, _handlerFactory);
        }

        /// <inheritdoc/>
        public IObservable<Unit> Publish(IRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return PublishRequest(request);
        }

        /// <inheritdoc/>
        public IObservable<Unit> Publish(object notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            if (notification is IRequest instance)
            {
                return PublishRequest(instance);
            }

            throw new ArgumentException($"{nameof(notification)} does not implement ${nameof(IRequest)}");
        }

        /// <summary>
        /// Override in a derived class to control how the tasks are awaited. By default the implementation is a foreach and await of each handler.
        /// </summary>
        /// <param name="allHandlers">Enumerable of tasks representing invoking each notification handler.</param>
        /// <param name="notification">The notification being published.</param>
        /// <returns>A task representing invoking all handlers.</returns>
        protected virtual Unit PublishCore(IEnumerable<Func<IRequest, IObservable<Unit>>> allHandlers, IRequest notification)
        {
            foreach (var handler in allHandlers)
            {
                handler(notification);
            }

            return Unit.Default;
        }

        private IObservable<Unit> PublishRequest(IRequest request)
        {
            var requestType = request.GetType();
            var handler = NotificationHandlers.GetOrAdd(
                requestType,
                t => (NotificationHandlerWrapper)Activator.CreateInstance(typeof(NotificationHandlerWrapperImpl<>).MakeGenericType(requestType)));

            return handler.Handle(request, _handlerFactory, PublishCore);
        }
    }
}
