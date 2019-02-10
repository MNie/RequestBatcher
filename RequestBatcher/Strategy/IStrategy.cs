namespace RequestBatcher.Strategy
{
    using System.Collections.Generic;
    using DTO;

    public interface IStrategy<TPayload>
    {
        IReadOnlyCollection<TPayload> Apply(IEnumerable<TPayload> data);
    }
}