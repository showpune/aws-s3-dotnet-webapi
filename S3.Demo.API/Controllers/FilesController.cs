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
        if (!containerExists.Value) return NotFound($"Container {bucketName} does not exist.");
        
        var blobName = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}";
        var blobClient = containerClient.GetBlobClient(blobName);
        
        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
        
        return Ok($"File {prefix}/{file.FileName} uploaded to Azure Blob Storage successfully!");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllFilesAsync(string bucketName, string? prefix)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var containerExists = await containerClient.ExistsAsync();
        if (!containerExists.Value) return NotFound($"Container {bucketName} does not exist.");
        
        var blobObjects = new List<S3ObjectDto>();
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            
            // Generate SAS URI for temporary access (1 minute)
            var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(1));
            
            blobObjects.Add(new S3ObjectDto
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
        if (!containerExists.Value) return NotFound($"Container {bucketName} does not exist.");
        
        var blobClient = containerClient.GetBlobClient(key);
        var blobExists = await blobClient.ExistsAsync();
        if (!blobExists.Value) return NotFound($"Blob {key} does not exist.");
        
        var response = await blobClient.DownloadStreamingAsync();
        var properties = await blobClient.GetPropertiesAsync();
        
        return File(response.Value.Content, properties.Value.ContentType);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFileAsync(string bucketName, string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var containerExists = await containerClient.ExistsAsync();
        if (!containerExists.Value) return NotFound($"Container {bucketName} does not exist");
        
        var blobClient = containerClient.GetBlobClient(key);
        await blobClient.DeleteIfExistsAsync();
        return NoContent();
    }
}
