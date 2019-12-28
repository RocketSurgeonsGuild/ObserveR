using System;
using System.Linq;

namespace ObservR
{
    internal abstract class RequestHandlerBase
    {
        public abstract IObservable<object> Handle(object request, HandlerFactory serviceFactory);

        protected static THandler GetHandler<THandler>(HandlerFactory factory)
        {
            THandler handler;

            try
            {
                handler = factory.GetInstance<THandler>();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Error constructing handler for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.", e);
            }

            if (handler == null)
            {
                throw new InvalidOperationException($"Handler was not found for request of type {typeof(THandler)}. Register your handlers with the container. See the samples in GitHub for examples.");
            }

            return handler;
        }
    }

    internal abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
    {
        public abstract IObservable<TResponse> Handle(IRequest<TResponse> request, HandlerFactory serviceFactory);
    }

    internal class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override IObservable<object> Handle(object request, HandlerFactory serviceFactory)
        {
            return (IObservable<object>)Handle((IRequest<TResponse>)request, serviceFactory);
        }

        public override IObservable<TResponse> Handle(IRequest<TResponse> request, HandlerFactory serviceFactory)
        {
            IObservable<TResponse> Handler() => GetHandler<IRequestHandler<TRequest, TResponse>>(serviceFactory).Handle((TRequest)request);

            return Handler();
        }
    }
}