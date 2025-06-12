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
    public async Task<IActionResult> UploadFileAsync(IFormFile file, string containerName, string? prefix)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var exists = await containerClient.ExistsAsync();
        if (!exists.Value) return NotFound($"Container {containerName} does not exist.");
        
        var blobName = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}";
        var blobClient = containerClient.GetBlobClient(blobName);
        
        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
        
        return Ok($"File {blobName} uploaded to Azure Storage successfully!");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllFilesAsync(string containerName, string? prefix)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var exists = await containerClient.ExistsAsync();
        if (!exists.Value) return NotFound($"Container {containerName} does not exist.");
        
        var blobs = new List<BlobObjectDto>();
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            
            // Generate SAS URI for the blob (1 minute expiry)
            string? sasUrl = null;
            if (blobClient.CanGenerateSasUri)
            {
                sasUrl = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(1)).ToString();
            }
            
            blobs.Add(new BlobObjectDto
            {
                Name = blobItem.Name,
                SasUrl = sasUrl
            });
        }

        return Ok(blobs);
    }

    [HttpGet("get-by-key")]
    public async Task<IActionResult> GetFileByKeyAsync(string containerName, string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var exists = await containerClient.ExistsAsync();
        if (!exists.Value) return NotFound($"Container {containerName} does not exist.");
        
        var blobClient = containerClient.GetBlobClient(key);
        var blobExists = await blobClient.ExistsAsync();
        if (!blobExists.Value) return NotFound($"Blob {key} does not exist.");
        
        var response = await blobClient.DownloadStreamingAsync();
        var properties = await blobClient.GetPropertiesAsync();
        
        return File(response.Value.Content, properties.Value.ContentType ?? "application/octet-stream");
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFileAsync(string containerName, string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var exists = await containerClient.ExistsAsync();
        if (!exists.Value) return NotFound($"Container {containerName} does not exist");
        
        var blobClient = containerClient.GetBlobClient(key);
        await blobClient.DeleteIfExistsAsync();
        return NoContent();
    }
}
