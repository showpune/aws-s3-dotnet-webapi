# Working with Azure Storage Account (Blob Storage) using ASP.NET Core – Upload, Download & Delete Files 

![Azure Storage using ASP.NET Core](https://codewithmukesh.com/wp-content/uploads/2022/03/Working-with-AWS-S3-using-ASP.NET-Core.png)

In this application, we demonstrate working with Azure Storage Account (Blob Storage) using ASP.NET Core Web API to upload, download and delete files from Azure's scalable blob storage! The application covers Azure Storage configuration, authentication using Azure Identity, container management (equivalent to S3 buckets), and blob operations with SAS token generation for secure access.

## Features Covered:

- Azure Storage Account – Blob Storage Service
- Authentication using Azure Default Credential
- Container Management using Azure Storage Client
- ASP.NET Core Integration with Azure Storage
  - Container Operations: Creating Containers, Listing Containers, Deleting Containers
  - Blob Operations: Upload Files to Azure Storage, List Files in Container, Download Files from Azure Storage, Delete Files from Azure Storage
  - SAS Token Generation for Secure Access (equivalent to pre-signed URLs)

## Configuration

Update your `appsettings.json` with your Azure Storage Account endpoint:

```json
{
  "AzureStorageBlob": {
    "Endpoint": "https://yourstorageaccount.blob.core.windows.net"
  }
}
```

## Authentication

The application uses `DefaultAzureCredential` for authentication, which supports multiple authentication methods:
- Managed Identity (recommended for Azure-hosted applications)
- Azure CLI authentication (for local development)
- Environment variables
- Interactive authentication

## API Endpoints

### Container Management (Buckets)
- `POST /api/buckets/create?bucketName={name}` - Create a new container
- `GET /api/buckets/get-all` - List all containers
- `DELETE /api/buckets/delete?bucketName={name}` - Delete a container

### File Management (Blobs)
- `POST /api/files/upload` - Upload file to container
- `GET /api/files/get-all?bucketName={container}&prefix={prefix}` - List files in container
- `GET /api/files/get-by-key?bucketName={container}&key={filename}` - Download specific file
- `DELETE /api/files/delete?bucketName={container}&key={filename}` - Delete specific file

*Complete Source Code for Azure Storage Integration Included!

#azure #storage #management #testing #dotnet #dotnet6 #codeblog #100daysofcode #blogger #blobstorage #tutorials