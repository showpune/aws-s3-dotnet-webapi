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
        var exists = await containerClient.ExistsAsync();
        if (!exists.Value) return NotFound($"Bucket {bucketName} does not exist.");
        
        string blobName = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}";
        var blobClient = containerClient.GetBlobClient(blobName);
        
        var blobUploadOptions = new BlobUploadOptions()
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
        };
        
        await blobClient.UploadAsync(file.OpenReadStream(), blobUploadOptions);
        return Ok($"File {prefix}/{file.FileName} uploaded to Azure Storage successfully!");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllFilesAsync(string bucketName, string? prefix)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var exists = await containerClient.ExistsAsync();
        if (!exists.Value) return NotFound($"Bucket {bucketName} does not exist.");
        
        var blobs = new List<BlobObjectDto>();
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            
            // Generate SAS token for the blob (equivalent to presigned URL)
            var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(1));
            
            blobs.Add(new BlobObjectDto()
            {
                Name = blobItem.Name,
                PresignedUrl = sasUri.ToString()
            });
        }

        return Ok(blobs);
    }

    [HttpGet("get-by-key")]
    public async Task<IActionResult> GetFileByKeyAsync(string bucketName, string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var exists = await containerClient.ExistsAsync();
        if (!exists.Value) return NotFound($"Bucket {bucketName} does not exist.");
        
        var blobClient = containerClient.GetBlobClient(key);
        var blobExists = await blobClient.ExistsAsync();
        if (!blobExists.Value) return NotFound($"Blob {key} does not exist.");
        
        var downloadResponse = await blobClient.DownloadStreamingAsync();
        var properties = await blobClient.GetPropertiesAsync();
        
        return File(downloadResponse.Value.Content, properties.Value.ContentType);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFileAsync(string bucketName, string key)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var exists = await containerClient.ExistsAsync();
        if (!exists.Value) return NotFound($"Bucket {bucketName} does not exist");
        
        var blobClient = containerClient.GetBlobClient(key);
        await blobClient.DeleteIfExistsAsync();
        return NoContent();
    }
}
