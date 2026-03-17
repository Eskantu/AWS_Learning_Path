using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessingWorker.Models
{
    public class S3EventWrapper
    {
        public List<S3Record> Records { get; set; } = [];
    }

    public class S3Record
    {
        public S3Entity S3 { get; set; }
    }

    public class S3Entity
    {
        public S3Bucket Bucket { get; set; }
        public S3Object Object { get; set; }
    }

    public class S3Bucket
    {
        public string Name { get; set; }
    }

    public class S3Object
    {
        public string Key { get; set; }
    }
}
