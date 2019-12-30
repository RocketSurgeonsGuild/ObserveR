using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace ObservR
{
    internal abstract class NotificationHandlerWrapper
    {
        public abstract IObservable<Unit> Handle(IRequest notification, HandlerFactory handlerFactory, Func<IEnumerable<Func<IRequest, IObservable<Unit>>>, IRequest, Unit> publish);
    }

    internal class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapper
        where TNotification : IRequest<Unit>
    {
        public override IObservable<Unit> Handle(IRequest notification, HandlerFactory handlerFactory, Func<IEnumerable<Func<IRequest, IObservable<Unit>>>, IRequest, Unit> publish)
        {
            var handlers = handlerFactory
                .GetInstances<IRequestHandler<TNotification, Unit>>()
                .Select(x => new Func<IRequest, IObservable<Unit>>((theNotification) =>
                {
                    // have to call subscribe to ensure execution.
                    // The return Unit does not emit from the original sequence, is this okay?
                    x.Handle((TNotification)theNotification).Subscribe();
                    return Observable.Return(Unit.Default);
                }));

            return Observable.Return(publish(handlers, notification));
        }
    }
}