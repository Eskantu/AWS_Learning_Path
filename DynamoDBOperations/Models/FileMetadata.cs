using Amazon.DynamoDBv2.DataModel;

using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDBOperations.Models
{
    [DynamoDBTable("FileMetadata")]
    public class FileMetadata
    {
        [DynamoDBHashKey]
        public string Id { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey("UserFilesIndex")]
        public string UserId { get; set; }

        [DynamoDBGlobalSecondaryIndexRangeKey("UserFilesIndex")]
        public string CreateAt { get; set; }

        public string FileName { get; set; }

        public string S3Key { get; set; }

        public long FileSize { get; set; }

        public string ContentType { get; set; }
    }

}
