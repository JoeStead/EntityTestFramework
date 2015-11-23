using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace EntityTestFramework.Tests.TestHelpers
{
    internal class DbOperationsHelper
    {
        private readonly TestDbContext _context;

        public DbOperationsHelper(TestDbContext context)
        {
            _context = context;
        }

        public void CreateTestRecord(TestRecord record)
        {
            _context.TestRecords.Add(record);
            _context.SaveChanges();
        }

        public void CreateInvalidTestRecord(TestRecord record)
        {
            //some arbritary test that does nothing.
            if (record != null)
            {
                _context.SaveChanges();
            }
        }

        public void UpdateTestRecordName(int recordId, string newName)
        {
            var record = _context.TestRecords.First(x => x.Id == recordId);
            record.Name = newName;

            _context.TestRecords.Update(record);
            _context.SaveChanges();
        }

        public void RemoveTestRecord(int recordId)
        {
            var record = _context.TestRecords.First(x => x.Id == recordId);
            _context.TestRecords.Remove(record);
            _context.SaveChanges();
        }

        public IEnumerable<TestRecord> GetRecords(Expression<Func<TestRecord, bool>> predicate)
        {
            return _context.TestRecords.Where(predicate);
        }

        public TestRecord GetRecord(Expression<Func<TestRecord, bool>> predicate)
        {
            return _context.TestRecords.FirstOrDefault(predicate);
        }

        public Task<TestRecord> GetRecordAsync(Expression<Func<TestRecord, bool>> predicate)
        {
            return _context.TestRecords.FirstOrDefaultAsync(predicate);
        }

        public Task<List<TestRecord>> GetRecordsAsync(Expression<Func<TestRecord, bool>> predicate)
        {
            return _context.TestRecords.Where(predicate).ToListAsync();
        }
    }
}
