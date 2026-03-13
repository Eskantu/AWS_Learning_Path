using Amazon.S3;
using Amazon.S3.Model;

namespace BasicFileOperations.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        public S3Service(IAmazonS3 s3, IConfiguration config)
        {
            _s3Client = s3;
            _bucketName = config["S3:BucketName"];
        }

        public async Task UploadFileAsync(string key, Stream fileStream)
        {
            var putRequest = new Amazon.S3.Model.PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = fileStream
            };
            await _s3Client.PutObjectAsync(putRequest);
        }

        public async Task<List<string>> ListFilesAsync()
        {
            var listRequest = new Amazon.S3.Model.ListObjectsV2Request
            {
                BucketName = _bucketName
            };
            var response = await _s3Client.ListObjectsV2Async(listRequest);
            return [.. response.S3Objects.Select(o => o.Key)];
        }

        public async Task<Stream> DownloadFileAsync(string key)
        {
            // Ensure the bucket exists and is accessible
            if (!(await ListFilesAsync()).Contains(key))
            {
                throw new FileNotFoundException($"The file with key '{key}' does not exist in the bucket.");
            }
            var getRequest = new Amazon.S3.Model.GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };
            var response = await _s3Client.GetObjectAsync(getRequest);
            return response.ResponseStream;
        }

        public async Task DeleteFileAsync(string key)
        {
            var deleteRequest = new Amazon.S3.Model.DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };
            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

        public string GeneratePreSignedUploadURL(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };
            return _s3Client.GetPreSignedURL(request);
        }

        public async Task<List<string>> ListFilesByUserAsync(string userId)
        {
            var response = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = $"users/{userId}/"
            });

            return response.S3Objects
                .Select(o => Path.GetFileName(o.Key))
                .ToList();
        }

        public string GeneratePreSignedDownloadUrl(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }
}
