namespace RequestBatcher.Strategy.LastOne
{
    using System.Collections.Generic;
    using System.Linq;
    using DTO;

    public class LastOneStrategy : IStrategy<IExample>
    {
        public IReadOnlyCollection<IExample> Apply(IEnumerable<IExample> data) => 
            data
                .GroupBy(x => x.Id)
                .Select(x => new Example(x.Key, x.Last().Payload))
                .ToList();
    }
}