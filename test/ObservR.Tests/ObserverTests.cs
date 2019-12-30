using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using FluentAssertions;
using Xunit;

namespace ObservR.Tests
{
    public sealed class ObserverTests
    {
        public class TheSendMethod
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

            private IContainer _container;
            private ContainerBuilder _containerBuilder;

            public TheSendMethod()
            {
                var builder = new ContainerBuilder();
                builder.RegisterAssemblyTypes(typeof(IObserver).GetTypeInfo().Assembly).AsImplementedInterfaces();
                builder
                    .RegisterAssemblyTypes(typeof(Mic).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(typeof(IRequestHandler<,>))
                    .AsImplementedInterfaces();
                builder.Register<HandlerFactory>(ctx =>
                {
                    var c = ctx.Resolve<IComponentContext>();
                    return t => c.Resolve(t);
                });
                builder.RegisterType<Observer>().As<IObserver>().SingleInstance();
                _container = builder.Build();
            }

            [Fact]
            public async Task Should_Resolve_Handler()
            {
                // Given
                var observer = _container.Resolve<IObserver>();

                // When
                var response = await observer.Send(new Mic { Count = 1 });

                // Then
                response.Message.Should().Be("Mic Check 1 2");
            }

            [Fact]
            public async Task Should_Return_Correct_Type()
            {
                // Given
                var observer = _container.Resolve<IObserver>();

                // When
                var response = await observer.Send(new Mic { Count = 1 });

                // Then
                response.Should().BeOfType<Check>();
            }

            [Fact]
            public async Task Should_Resolve_Handler_From_Dynamic_Dispatch()
            {
                // Given
                var observer = _container.Resolve<IObserver>();

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
            private IContainer _container;
            private StringBuilder _stringBuilder;

            public class Notification : IRequest
            {
                public string Message { get; set; }
            }

            public class NotificationHandler : IRequestHandler<Notification>
            {
                private readonly TextWriter _writer;

                public NotificationHandler(TextWriter writer)
                {
                    _writer = writer;
                }

                public IObservable<Unit> Handle(Notification request) =>
                    Observable.FromAsync(async () => await _writer.WriteLineAsync(request.Message + " Notification"));
            }

            public ThePublishMethod()
            {
                _stringBuilder = new StringBuilder();
                var writer = new StringWriter(_stringBuilder);
                var builder = new ContainerBuilder();
                builder.RegisterAssemblyTypes(typeof(IObserver).GetTypeInfo().Assembly).AsImplementedInterfaces();
                builder.RegisterInstance(writer).As<TextWriter>();
                builder
                    .RegisterAssemblyTypes(typeof(Notification).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(typeof(IRequestHandler<>))
                    .AsImplementedInterfaces();
                builder.Register<HandlerFactory>(ctx =>
                {
                    var c = ctx.Resolve<IComponentContext>();
                    return t => c.Resolve(t);
                });
                builder.RegisterType<Observer>().As<IObserver>().SingleInstance();
                _container = builder.Build();
            }

            [Fact]
            public async Task Should_Resolve_Handlers()
            {
                // Given
                var observer = _container.Resolve<IObserver>();

                // When
                await observer.Publish(new Notification { Message = "Message" });
                var result = _stringBuilder.ToString().TrimEnd();

                // Then
                result.Should().Be("Message Notification");
            }

            [Fact]
            public async Task Should_Resolve_Handlers_From_Interface()
            {
                // Given
                var observer = _container.Resolve<IObserver>();

                // When
                IRequest request = new Notification { Message = "Message" };
                await observer.Publish(request);
                var result = _stringBuilder.ToString().TrimEnd();

                // Then
                result.Should().Be("Message Notification");
            }

            [Fact]
            public async Task Should_Resolve_Handlers_From_Object()
            {
                // Given
                var observer = _container.Resolve<IObserver>();

                // When
                var request = new Notification { Message = "Message" };
                await observer.Publish(request);
                var result = _stringBuilder.ToString().TrimEnd();

                // Then
                result.Should().Be("Message Notification");
            }

            [Fact]
            public async Task Should_Return_Unit()
            {
                // Given
                var observer = _container.Resolve<IObserver>();

                // When
                var result = await observer.Publish(new Notification());

                // Then
                result.Should().Be(Unit.Default);
            }
        }
    }
}