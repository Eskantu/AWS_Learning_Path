using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using FileUploadedProcessor.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace FileUploadedProcessor.Services
{
    public class MetadataService
    {
        private readonly DynamoDBContext _context;

        public MetadataService()
        {
            var client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            });
            _context = new DynamoDBContext(client);
        }

        public async Task SaveMetadataAsync(FileMetadata metadata)
        {
            await _context.SaveAsync(metadata);
        }
    }
}
