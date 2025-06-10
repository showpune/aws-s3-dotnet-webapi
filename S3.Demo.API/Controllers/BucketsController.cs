using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace S3.Demo.API.Controllers;

[Route("api/buckets")]
[ApiController]
public class BucketsController : ControllerBase
{
    private readonly BlobServiceClient _blobServiceClient;
    public BucketsController(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBucketAsync(string bucketName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var containerExists = await containerClient.ExistsAsync();
        if (containerExists.Value) return BadRequest($"Bucket {bucketName} already exists.");
        await _blobServiceClient.CreateBlobContainerAsync(bucketName);
        return Ok($"Bucket {bucketName} created.");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllBucketAsync()
    {
        var containers = new List<string>();
        await foreach (var containerItem in _blobServiceClient.GetBlobContainersAsync())
        {
            containers.Add(containerItem.Name);
        }
        return Ok(containers);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteBucketAsync(string bucketName)
    {
        await _blobServiceClient.DeleteBlobContainerAsync(bucketName);
        return NoContent();
    }

}
