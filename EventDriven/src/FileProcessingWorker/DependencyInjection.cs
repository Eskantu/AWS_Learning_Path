using Amazon.DynamoDBv2;
using Amazon.S3;

using Microsoft.Extensions.DependencyInjection;

using FileProcessingWorker.Services;

namespace FileProcessingWorker;

public static class DependencyInjection
{
    public static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddAWSService<IAmazonS3>();
        services.AddAWSService<IAmazonDynamoDB>();

        services.AddTransient<FileProcessingService>();

        return services.BuildServiceProvider();
    }
}