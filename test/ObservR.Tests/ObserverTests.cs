using System;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Xunit;

namespace ObservR.Tests
{
    public sealed class ObserverTests
    {
        public class Mic : IRequest<Check>
        {
            public int Count { get; set; }
        }

        public class Check
        {
            public string Message { get; set; }
        }

        public class MicHandler : IRequestHandler<Mic, Check>
        {
            public IObservable<Check> Handle(Mic request) =>
                Observable.Return(new Check { Message = $"Mic Check {request.Count} {request.Count + 1}" });
        }

        public class TheSendMethod
        {
            private ContainerBuilder _containerBuilder;

            public TheSendMethod()
            {
                _containerBuilder = new ContainerBuilder();
                _containerBuilder.RegisterAssemblyTypes(typeof(IObserver).GetTypeInfo().Assembly).AsImplementedInterfaces();
                _containerBuilder
                    .RegisterAssemblyTypes(typeof(Mic).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(typeof(IRequestHandler<,>))
                    .AsImplementedInterfaces();
                _containerBuilder.Register<HandlerFactory>(ctx =>
                {
                    var c = ctx.Resolve<IComponentContext>();
                    return t => c.Resolve(t);
                });
                _containerBuilder.RegisterType<Observer>().As<IObserver>();
            }

            [Fact]
            public async Task Should_Resolve_Handler()
            {
                // Given
                var container = _containerBuilder.Build();
                var observer = container.Resolve<IObserver>();

                // When
                var response = await observer.Send(new Mic { Count = 1 });

                // Then
                response.Message.Should().Be("Mic Check 1 2");
            }

            [Fact]
            public async Task Should_Resolve_Handler_Via_Dynamic_Dispatch()
            {
                // Given
                var container = _containerBuilder.Build();
                var observer = container.Resolve<IObserver>();

                // When
                object request = new Mic { Count = 1 };
                var response = await observer.Send(request);

                // Then
                response.Should().BeOfType<Check>();
                ((Check)response).Message.Should().Be("Mic Check 1 2");
            }
        }

        public class ThePublishMethod
        {
        }
    }
}