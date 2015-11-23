using System.Collections.Generic;
using EntityTestFramework.Tests.TestHelpers;
using Xunit;

namespace EntityTestFramework.Tests
{
    //todo use AutoFixture
    public class ConfigurableContextSynchronousWriteTests
    {
        [Fact]
        public void Can_verify_new_entity_has_been_saved_with_correct_properties()
        {
            var testContext = new ConfigurableContext<TestDbContext>(ctx =>
            {
                ctx.Setup(x => x.TestRecords, new List<TestRecord>());
            });

            var dbOperationHelper = new DbOperationsHelper(testContext);
            var testRecord = new TestRecord
            {
                Id = 1,
                Name = "Anything"
            };
            dbOperationHelper.CreateTestRecord(testRecord);

            testContext.HasBeenSaved<TestRecord>(x => x.Id == 1 && x.Name == "Anything");
        }

        [Fact]
        public void Can_verify_updated_entity_has_been_saved_with_modified_properties()
        {
            var testRecord = new TestRecord
            {
                Id = 1,
                Name = "Anything"
            };
            var testContext = new ConfigurableContext<TestDbContext>(ctx =>
            {
                ctx.Setup(x => x.TestRecords, testRecord);
            });

            var dbOperationHelper = new DbOperationsHelper(testContext);

            dbOperationHelper.UpdateTestRecordName(1, "Joe");

            testContext.HasBeenSaved<TestRecord>(x => x.Name == "Joe");
        }

        [Fact]
        public void Can_verify_no_entities_of_type_were_saved()
        {
            var testContext = new ConfigurableContext<TestDbContext>(ctx =>
            {
                ctx.Setup(x => x.TestRecords, new List<TestRecord>());
            });

            var dbOperationHelper = new DbOperationsHelper(testContext);
            var testRecord = new TestRecord
            {
                Id = 1,
                Name = "Anything"
            };
            dbOperationHelper.CreateInvalidTestRecord(testRecord);

            testContext.HasNotBeenSaved<TestRecord>();
        }

        [Fact]
        public void Can_verify_specific_entity_was_not_saved()
        {
            var testContext = new ConfigurableContext<TestDbContext>(ctx =>
            {
                ctx.Setup(x => x.TestRecords, new TestRecord { Id = 1, Name = "Not Joe"});
            });

            var dbOperationHelper = new DbOperationsHelper(testContext);
            var testRecord = new TestRecord
            {
                Id = 2,
                Name = "Anything"
            };
            dbOperationHelper.CreateInvalidTestRecord(testRecord);

            testContext.HasNotBeenSaved<TestRecord>(x => x.Id == 1 && x.Name == "Anything");
        }

        [Fact]
        public void Can_remove_existing_entity()
        {
            var testRecord = new TestRecord
            {
                Id = 1,
                Name = "Anything"
            };
            var testContext = new ConfigurableContext<TestDbContext>(ctx =>
            {
                ctx.Setup(x => x.TestRecords, testRecord);
            });

            var dbOperationHelper = new DbOperationsHelper(testContext);

            dbOperationHelper.RemoveTestRecord(1);

            testContext.HasNotBeenSaved<TestRecord>(x => x.Id == 1);
        }
        //todo range operations
    }
}
