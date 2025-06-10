using Azure.Storage.Blobs;
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
        if (!containerExists.Value) return NotFound($"Bucket {bucketName} does not exist.");
        
        var blobName = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}";
        var blobClient = containerClient.GetBlobClient(blobName);
        
        var blobUploadOptions = new Azure.Storage.Blobs.Models.BlobUploadOptions()
        {
            HttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders()
            {
                ContentType = file.ContentType
            }
        };
        
        await blobClient.UploadAsync(file.OpenReadStream(), blobUploadOptions);
        return Ok($"File {prefix}/{file.FileName} uploaded to Azure Blob Storage successfully!");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllFilesAsync(string bucketName, string? prefix)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var containerExists = await containerClient.ExistsAsync();
        if (!containerExists.Value) return NotFound($"Bucket {bucketName} does not exist.");
        
        var blobObjects = new List<BlobObjectDto>();
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(1));
            
            blobObjects.Add(new BlobObjectDto()
            {
                Name = blobItem.Name,
                PresignedUrl = sasUri.ToString()
            });
        }

        return Ok(blobObjects);
    }

    [HttpGet("get-by-key")]
    public async Task<IActionResult> GetFileByKeyAsync(string bucketName, string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var containerExists = await containerClient.ExistsAsync();
        if (!containerExists.Value) return NotFound($"Bucket {bucketName} does not exist.");
        
        var blobClient = containerClient.GetBlobClient(key);
        var blobDownload = await blobClient.DownloadStreamingAsync();
        var contentType = blobDownload.Value.Details.ContentType;
        
        return File(blobDownload.Value.Content, contentType);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFileAsync(string bucketName, string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var containerExists = await containerClient.ExistsAsync();
        if (!containerExists.Value) return NotFound($"Bucket {bucketName} does not exist");
        
        var blobClient = containerClient.GetBlobClient(key);
        await blobClient.DeleteAsync();
        return NoContent();
    }
}
