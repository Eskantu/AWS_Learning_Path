using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDBOperations.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDBOperations.Servicios
{
    public class MetadataService
    {
        private readonly DynamoDBContext _context;


        public MetadataService(IAmazonDynamoDB dynamoDb)
        {
            _context = new DynamoDBContext(dynamoDb);
        }

        public async Task SaveMetadata(FileMetadata metadata)
        {
            await _context.SaveAsync(metadata);
        }

        public async Task<List<FileMetadata>> GetFilesByUser(string userId)
        {

            var search = _context.QueryAsync<FileMetadata>(userId, new QueryConfig
            {
                IndexName = "UserFilesIndex",
                BackwardQuery = true,
                RetrieveDateTimeInUtc = true,
                QueryFilter = new List<ScanCondition>
               {
                   new ScanCondition("UserId", ScanOperator.Equal, [userId])
               }
            });

            var list = search.GetRemainingAsync().Result;
            return list;
        }

        public async Task<PagedResult<FileMetadata>> GetFilesByUserAsync(string userId, int limit = 3, string cursor = "")
        {
            var decodedCursor = string.IsNullOrEmpty(cursor) ? null : Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            var search = _context.FromQueryAsync<FileMetadata>(
            new QueryOperationConfig
            {
                IndexName = "UserFilesIndex",
                Limit = limit,
                PaginationToken = decodedCursor,
                BackwardSearch = true,
                //AttributesToGet = new List<string> { "Id", "CreateAt", "FileName" },// HAce proyeccion de atributos, si no se especifica trae todos los atributos
                KeyExpression = new Expression
                {   
                    ExpressionStatement = "UserId = :userId",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        { ":userId", userId }
                    }
                }
            });
            var items = await search.GetNextSetAsync();
            cursor = search.PaginationToken;
            var encodedCursor = cursor == null ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(cursor));
            return new PagedResult<FileMetadata> { Items = items, NextCursor = encodedCursor };

        }
    }
}
