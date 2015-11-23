using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityTestFramework.Tests.TestHelpers;
using Xunit;
using FluentAssertions;

namespace EntityTestFramework.Tests
{
    public class ConfigurableContextReadTests
    {
        [Fact]
        public void Can_retrieve_records_using_where_method()
        {
            var configContext = new ConfigurableContext<TestDbContext>(ctx =>
            {
                ctx.Setup(x => x.TestRecords, new TestRecord { Id = 1, Name = "A" }, new TestRecord { Id = 2, Name = "B" }, new TestRecord { Id = 1, Name = "C" });
            });

            var opHelper = new DbOperationsHelper(configContext);

            var results = opHelper.GetRecords(t => t.Id == 1)?.ToArray();

            results.Should().HaveCount(2);
            results.Should().Contain(t => t.Id == 1 && t.Name == "A");
            results.Should().Contain(t => t.Id == 1 && t.Name == "C");
        }

        [Fact]
        public void Can_retrieve_record_using_first_or_default_synchronously()
        {
            var configContext = new ConfigurableContext<TestDbContext>(ctx =>
            {
                ctx.Setup(x => x.TestRecords, new TestRecord { Id = 1, Name = "A" }, new TestRecord { Id = 2, Name = "B" });
            });

            var opHelper = new DbOperationsHelper(configContext);

            var result = opHelper.GetRecord(t => t.Id == 1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("A");
        }

        [Fact]
        public async Task Can_retrieve_record_using_first_or_Default_asynchronously()
        {
            var configContext = new ConfigurableContext<TestDbContext>(ctx =>
            {
                ctx.Setup(x => x.TestRecords, new TestRecord { Id = 1, Name = "A" }, new TestRecord { Id = 2, Name = "B" });
            });

            var opHelper = new DbOperationsHelper(configContext);

            var result = await opHelper.GetRecordAsync(t => t.Id == 1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("A");
        }

        [Fact]
        public async Task Can_retrieve_records_with_async()
        {
            var configContext = new ConfigurableContext<TestDbContext>(ctx =>
            {
                ctx.Setup(x => x.TestRecords, new TestRecord { Id = 1, Name = "A" }, new TestRecord { Id = 2, Name = "B" }, new TestRecord { Id = 1, Name = "C" });
            });

            var opHelper = new DbOperationsHelper(configContext);

            var results = await opHelper.GetRecordsAsync(t => t.Id == 1);

            results.Should().HaveCount(2);
            results.Should().Contain(t => t.Id == 1 && t.Name == "A");
            results.Should().Contain(t => t.Id == 1 && t.Name == "C");

        }
    }
}
