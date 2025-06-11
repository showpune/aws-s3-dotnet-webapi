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
        var existsResponse = await containerClient.ExistsAsync();
        if (!existsResponse.Value) return NotFound($"Bucket {bucketName} does not exist.");
        
        var blobName = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}";
        var blobClient = containerClient.GetBlobClient(blobName);
        
        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
        
        return Ok($"File {prefix}/{file.FileName} uploaded to S3 successfully!");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllFilesAsync(string bucketName, string? prefix)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var existsResponse = await containerClient.ExistsAsync();
        if (!existsResponse.Value) return NotFound($"Bucket {bucketName} does not exist.");
        
        var blobObjects = new List<S3ObjectDto>();
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
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
        var existsResponse = await containerClient.ExistsAsync();
        if (!existsResponse.Value) return NotFound($"Bucket {bucketName} does not exist.");
        
        var blobClient = containerClient.GetBlobClient(key);
        var blobExistsResponse = await blobClient.ExistsAsync();
        if (!blobExistsResponse.Value) return NotFound($"File {key} does not exist.");
        
        var download = await blobClient.DownloadStreamingAsync();
        var properties = await blobClient.GetPropertiesAsync();
        
        return File(download.Value.Content, properties.Value.ContentType);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFileAsync(string bucketName, string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var existsResponse = await containerClient.ExistsAsync();
        if (!existsResponse.Value) return NotFound($"Bucket {bucketName} does not exist");
        
        var blobClient = containerClient.GetBlobClient(key);
        await blobClient.DeleteIfExistsAsync();
        
        return NoContent();
    }
}
