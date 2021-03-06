﻿namespace RequestBatcher
{
    using System.Threading;
    using System.Threading.Tasks;
    using ResultType;
    using ResultType.Results;
    using Strategy;

    public interface IBatcher<TMessage>
    {
        Task<Result<Unit>> SendAsync(TMessage msg, CancellationToken token = new CancellationToken());
    }
}