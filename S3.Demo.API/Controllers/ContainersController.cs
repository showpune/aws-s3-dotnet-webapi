using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace S3.Demo.API.Controllers;

[Route("api/containers")]
[ApiController]
public class ContainersController : ControllerBase
{
    private readonly BlobServiceClient _blobServiceClient;
    public ContainersController(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var exists = await containerClient.ExistsAsync();
        if (exists.Value) return BadRequest($"Container {containerName} already exists.");
        
        await _blobServiceClient.CreateBlobContainerAsync(containerName);
        return Ok($"Container {containerName} created.");
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllContainersAsync()
    {
        var containers = new List<string>();
        await foreach (var containerItem in _blobServiceClient.GetBlobContainersAsync())
        {
            containers.Add(containerItem.Name);
        }
        return Ok(containers);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteContainerAsync(string containerName)
    {
        await _blobServiceClient.DeleteBlobContainerAsync(containerName);
        return NoContent();
    }

}
