using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDBOperations.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }

        public string NextCursor { get; set; }
    }
}
