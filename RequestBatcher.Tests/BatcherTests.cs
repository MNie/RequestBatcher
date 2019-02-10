using System;
using Xunit;

namespace RequestBatcher.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DTO;
    using Shouldly;
    using Strategy.LastOne;

    public class BatcherTests
    {
        [Fact]
        public async Task Batcher_WhenSizeIsSetTo2AndWeSend3Items_ReturnTwoBatchedItemsWithDateIntervalPlusMinus500msAndAllSendRequestsEndsWithSuccess()
        {
            var output = new Dictionary<DateTime, int>();
            var sut = new Batcher<IExample>(2, 5000, new LastOneStrategy(), (data) =>
            {
                output.Add(DateTime.Now, data.Sum(x => x.Payload));
            });

            var first = await sut.SendAsync(new Example("1", 1));
            var second = await sut.SendAsync(new Example("3", 1));
            var third = await sut.SendAsync(new Example("2", 1));
            
            (first.IsSuccess && second.IsSuccess && third.IsSuccess).ShouldBeTrue();
            while (output.Count != 2)
            {
            }
            output.Count.ShouldBe(2);
            
            output.First().Value.ShouldBe(2);
            output.Last().Value.ShouldBe(1);

            var interval = (output.Last().Key - output.First().Key).TotalSeconds;
            
            (interval >= 4.99d && interval <= 5.01d).ShouldBeTrue();
        }
    }
}