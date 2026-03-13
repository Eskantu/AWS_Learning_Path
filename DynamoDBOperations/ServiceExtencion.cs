using Amazon.DynamoDBv2;

using DynamoDBOperations.Servicios;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DynamoDBOperations
{
    public static class ServiceExtencion
    {
        public static void ConfigureDynamoService(this IServiceCollection services)
        {
            services.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                return new AmazonDynamoDBClient(
                    new AmazonDynamoDBConfig
                    {
                        RegionEndpoint = Amazon.RegionEndpoint.USEast2
                    });
            });
            services.AddScoped<MetadataService>();
        }
    }
}
