using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.S3;
using Amazon.S3.Model;

namespace FileProcessingWorker.Services;

public class FileProcessingService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IAmazonDynamoDB _dynamoDb;

    public FileProcessingService(IAmazonS3 s3Client, IAmazonDynamoDB dynamoDb)
    {
        _s3Client = s3Client;
        _dynamoDb = dynamoDb;
    }

    public async Task ProcessFile(string bucket, string key)
    {

        using GetObjectResponse response = await _s3Client.GetObjectAsync(bucket, key);

        await SaveMetadata(bucket, key, response.ContentLength);
    }

    private async Task SaveMetadata(string bucket, string filename, long contentLength)
    {
        string objectKey = $"{bucket}/{filename}";

        var request = new PutItemRequest
        {
            TableName = "file-metadata-serverless",
            ConditionExpression = "attribute_not_exists(Id)",
            Item = new Dictionary<string, AttributeValue>
            {
                {"Id", new AttributeValue { S = objectKey }},
                {"FileName", new AttributeValue { S = filename }},
                {"Bucket", new AttributeValue { S = bucket }},
                {"FileSize", new AttributeValue { N = contentLength.ToString() }},
                {"UploadTime", new AttributeValue { S = DateTime.UtcNow.ToString("o") }}
            }
        };

        try
        {
            await _dynamoDb.PutItemAsync(request);
        }
        catch (ConditionalCheckFailedException ex)
        {
         
            // Handle the case where the item already exists
            Console.WriteLine($"Item with Id {objectKey} already exists: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Error saving metadata for {objectKey}: {ex.Message}");

        }
    }
}