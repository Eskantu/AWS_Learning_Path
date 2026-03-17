using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;

using FileProcessingWorker.Models;
using FileProcessingWorker.Services;

using Microsoft.Extensions.DependencyInjection;

using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FileProcessingWorker;

public class Function
{
    private readonly FileProcessingService _fileService;

    public Function()
    {
        //Fake comment to create PR
        //Fake comment to create PR
        //Fake comment to create PR
        //Fake comment to create PR
        //Fake comment to create PR
        //Fake comment to create PR
        //Fake comment to create PR
        //Fake comment to create PR
        //Fake comment to create PR


        var serviceProvider = DependencyInjection.BuildServiceProvider();
        _fileService = serviceProvider.GetService<FileProcessingService>()!;
    }

    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        foreach (var message in evnt.Records)
        {
            context.Logger.LogInformation($"Processing message: {message.Body}");

            try
            {
                var s3Event = JsonSerializer.Deserialize<S3EventWrapper>(message.Body);

                var record = s3Event?.Records.FirstOrDefault();

                if (record == null)
                {
                    context.Logger.LogWarning("No S3 records found");
                    continue;
                }

                string bucket = record.S3.Bucket.Name;
                string key = record.S3.Object.Key;

                await _fileService.ProcessFile(bucket, key);
            }
            catch (Exception ex)
            {
                context.Logger.LogError(ex.Message);
                throw;
            }
        }
    }
}