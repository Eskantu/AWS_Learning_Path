using Amazon.DynamoDBv2.DataModel;

using System;
using System.Collections.Generic;
using System.Text;

namespace FileUploadedProcessor.Models
{
    public class FileMetadata
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UserId { get; set; }

        public string CreateAt { get; set; }

        public string FileName { get; set; }

        public string S3Key { get; set; }

        public long FileSize { get; set; }

        public string ContentType { get; set; }
    }
}
