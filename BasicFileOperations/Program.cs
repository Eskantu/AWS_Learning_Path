using Amazon.S3;

using BasicFileOperations.Services;

using DynamoDBOperations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var awsConfig = new AmazonS3Config
    {
        RegionEndpoint = Amazon.RegionEndpoint.USEast2
    };
    return new AmazonS3Client(awsConfig);
});

builder.Services.ConfigureDynamoService();


// Add services to the container.

builder.Services.AddScoped<S3Service>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
