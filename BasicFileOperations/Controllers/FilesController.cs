using Amazon.S3;

using BasicFileOperations.Services;

using DynamoDBOperations.Models;
using DynamoDBOperations.Servicios;

using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BasicFileOperations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly S3Service _s3Service;
        public FilesController(S3Service s3Service)
        {
            _s3Service = s3Service;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, string userId, [FromServices] MetadataService metadataService)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");


            using var stream = file.OpenReadStream();
            var fileId = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(file.FileName).ToLower();

            await _s3Service.UploadFileAsync($"users/{userId}/files/{fileId}{extension}", stream);

            var metadata = new FileMetadata
            {
                Id = fileId,
                UserId = userId,
                CreateAt = DateTime.UtcNow.ToString("o"),
                FileName = file.FileName,
                S3Key = $"{userId}/files/{fileId}{extension}",
                FileSize = file.Length,
                ContentType = file.ContentType
            };

            await metadataService.SaveMetadata(metadata);

            return Ok("File uploaded successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var files = await _s3Service.ListFilesAsync();
            return Ok(files);
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Download(string key)
        {
            var stream = await _s3Service.DownloadFileAsync(key);
            return File(stream, "application/octet-stream", key);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            await _s3Service.DeleteFileAsync(key);
            return Ok("Deleted");
        }

        [HttpPost("presigned-upload")]
        public IActionResult GenerateUploadUrl(string userId, string fileName)
        {
            var key = $"users/{userId}/files/{fileName}";

            var url = _s3Service.GeneratePreSignedUploadURL(key);

            return Ok(new
            {
                uploadUrl = url,
                key = key
            });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFilesByUser(string userId)
        {
            var files = await _s3Service.ListFilesByUserAsync(userId);

            return Ok(files);
        }

        [HttpGet("download")]
        public IActionResult GenerateDownloadUrl(string key)
        {
            var url = _s3Service.GeneratePreSignedDownloadUrl(key);

            return Ok(new
            {
                downloadUrl = url
            });
        }

        [HttpGet("users/{userId}/files")]
        public async Task<IActionResult> GetUserFiles(string userId, [FromServices] MetadataService metadataService)
        {
            var files = await metadataService.GetFilesByUser(userId);
            return Ok(files);
        }

        [HttpGet("users/{userId}/files/{limit:int}")]
        public async Task<IActionResult> GetFiles([FromServices] MetadataService metadataService,string userId,int limit = 2,[FromQuery]string cursor = null)
        {
            var result = await metadataService.GetFilesByUserAsync(userId, limit, cursor);

            return Ok(result);
        }
    }
}
