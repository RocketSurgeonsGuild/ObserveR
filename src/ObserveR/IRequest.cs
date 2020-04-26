using System.Diagnostics.CodeAnalysis;
using System.Reactive;

[assembly:SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Reviewed.")]
namespace ObserveR
{
    /// <summary>
    /// Represents a request sent to the observer.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public interface IRequest<TResponse>
    {
    }

    /// <summary>
    /// Represents a request sent to the observer that signals completion.
    /// </summary>
    public interface IRequest : IRequest<Unit>
    {
    }
}