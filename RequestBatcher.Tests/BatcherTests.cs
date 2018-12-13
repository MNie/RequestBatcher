using System;
using Xunit;

namespace RequestBatcher.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Shouldly;

    public class UnitTest1
    {
        [Fact]
        public async Task Batcher_WhenSizeIsSetTo2AndWeSend3Items_ReturnTwoBatchedItemsWithDateIntervalPlusMinus500msAndAllSendRequestsEndsWithSuccess()
        {
            var output = new Dictionary<DateTime, int>();
            var sut = new Batcher<int>(2, 5000, (data) =>
            {
                output.Add(DateTime.Now, data.Sum(x => x));
            });

            var first = await sut.SendAsync(1);
            var second = await sut.SendAsync(1);
            var third = await sut.SendAsync(1);
            
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