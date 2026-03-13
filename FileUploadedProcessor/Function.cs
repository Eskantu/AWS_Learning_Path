using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;

using FileUploadedProcessor.Models;
using FileUploadedProcessor.Services;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FileUploadedProcessor;

public class Function
{
    private readonly MetadataService _metadataService;
    public Function()
    {
        _metadataService = new MetadataService();
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task FunctionHandler(S3Event events, ILambdaContext context)
    {
        foreach (var item in events.Records)
        {
            //Comment to test code change detection, this is not a real change
            var bucketName = item.S3.Bucket.Name;
            var key = Uri.UnescapeDataString(item.S3.Object.Key);
            context.Logger.LogInformation($"File uploaded -> Bucket: {item.S3.Bucket.Name}, Key: {item.S3.Object.Key}");

            var metadata = new FileMetadata
            {
                FileName = Path.GetFileName(key),
                S3Key = key,
                FileSize = item.S3.Object.Size,
                ContentType = "unknown",// S3 event does not provide content type, you may want to fetch it using S3 API if needed, this is temp value for testing
                CreateAt = DateTime.UtcNow.ToString("o") // ISO 8601 format
            };

            await _metadataService.SaveMetadataAsync(metadata);
        }
    }
}
