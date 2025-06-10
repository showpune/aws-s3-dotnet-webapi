using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using S3.Demo.API.Models;

namespace S3.Demo.API.Controllers;

[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly BlobServiceClient _blobServiceClient;
    public FilesController(BlobServiceClient blobServiceClient)
    {
       _blobServiceClient = blobServiceClient;
    }
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFileAsync(IFormFile file, string bucketName, string? prefix)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var containerExists = await containerClient.ExistsAsync();
        if (!containerExists) return NotFound($"Bucket {bucketName} does not exist.");
        
        var blobName = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}";
        var blobClient = containerClient.GetBlobClient(blobName);
        
        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
        };
        
        await blobClient.UploadAsync(file.OpenReadStream(), blobUploadOptions);
        return Ok($"File {prefix}/{file.FileName} uploaded to S3 successfully!");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllFilesAsync(string bucketName, string? prefix)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var containerExists = await containerClient.ExistsAsync();
        if (!containerExists) return NotFound($"Bucket {bucketName} does not exist.");
        
        var s3Objects = new List<S3ObjectDto>();
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(1));
            
            s3Objects.Add(new S3ObjectDto
            {
                Name = blobItem.Name,
                PresignedUrl = sasUri.ToString()
            });
        }

        return Ok(s3Objects);
    }

    [HttpGet("get-by-key")]
    public async Task<IActionResult> GetFileByKeyAsync(string bucketName, string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var containerExists = await containerClient.ExistsAsync();
        if (!containerExists) return NotFound($"Bucket {bucketName} does not exist.");
        
        var blobClient = containerClient.GetBlobClient(key);
        var downloadResult = await blobClient.DownloadStreamingAsync();
        
        return File(downloadResult.Value.Content, downloadResult.Value.Details.ContentType);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFileAsync(string bucketName, string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var containerExists = await containerClient.ExistsAsync();
        if (!containerExists) return NotFound($"Bucket {bucketName} does not exist");
        
        var blobClient = containerClient.GetBlobClient(key);
        await blobClient.DeleteAsync();
        return NoContent();
    }
}
