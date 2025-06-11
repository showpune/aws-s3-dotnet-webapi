# Working with Azure Storage Account using ASP.NET Core – Upload, Download & Delete Files 

![Azure Storage using ASP.NET Core](https://codewithmukesh.com/wp-content/uploads/2022/03/Working-with-AWS-S3-using-ASP.NET-Core.png)

**Note: This project has been migrated from AWS S3 to Azure Storage Account (Blob Storage)**

This project demonstrates how to work with Azure Blob Storage using ASP.NET Core Web API to upload, download and delete files from Azure's scalable blob storage service. The API provides endpoints for managing containers (equivalent to S3 buckets) and blobs (equivalent to S3 objects).

## Migration from AWS S3 to Azure Storage Account

This codebase has been migrated from AWS S3 to Azure Blob Storage with the following key changes:
- **AWS S3 Buckets** → **Azure Blob Storage Containers**
- **AWS S3 Objects** → **Azure Blobs**
- **AWS S3 Presigned URLs** → **Azure SAS URLs**
- **AWS SDK** → **Azure Storage SDK**

## Topics Covered:

- Azure Blob Storage – Overview
- Azure Storage Account Configuration
- Working with Azure Blob Storage using ASP.NET Core – Getting Started
  - Working with Azure Containers: Creating Containers, Getting a List of Containers, Deleting a Container, Testing
  - File Operations in Azure Blob Storage: Upload Files to Azure Blob Storage, Get All the Files, Download Files, Delete Files, Testing File Operations
  - SAS URL Generation for secure access

## Configuration

Update `appsettings.json` with your Azure Storage Account endpoint:

```json
{
  "AzureStorageBlob": {
    "Endpoint": "https://yourstorageaccount.blob.core.windows.net"
  }
}
```

## Authentication

The application uses `DefaultAzureCredential` which supports multiple authentication methods including:
- Azure CLI
- Managed Identity
- Visual Studio
- Environment variables

## API Endpoints

### Container Operations
- `POST /api/buckets/create` - Create a new container
- `GET /api/buckets/get-all` - List all containers
- `DELETE /api/buckets/delete` - Delete a container

### File Operations
- `POST /api/files/upload` - Upload a file to a container
- `GET /api/files/get-all` - List all files in a container with SAS URLs
- `GET /api/files/get-by-key` - Download a specific file
- `DELETE /api/files/delete` - Delete a file

*Complete Source Code Included!

Original AWS S3 tutorial: https://codewithmukesh.com/blog/working-with-aws-s3-using-aspnet-core/

#azure #storage #blobstorage #dotnet #dotnet8 #codeblog #100daysofcode #blogger #tutorials 

![AWS S3 using ASP.NET Core](https://codewithmukesh.com/wp-content/uploads/2022/03/Working-with-AWS-S3-using-ASP.NET-Core.png)

In this article, we will be get started on working with AWS S3 using ASP.NET Core Web API to upload, download and delete files from Amazon’s Super Scalable S3! Apart from that, we will also learn more about Amazon’s S3, the problem it solves, dive a bit into the AWS Console for S3 Management, AWS CLI, and Credentials Store, Generate Access Keys for accessing S3 via SDKs, Creating and Deleting S3 buckets, a bit about pre-signed URLs and so on!

## Topics Covered:

- AWS S3 – In Short
- Creating User & Generating Access Keys via AWS IAM
- Creating your First AWS S3 Bucket via AWS Console
- AWS Configurations & CLI
- Working with AWS S3 using ASP.NET Core – Getting Started
  - Working with AWS S3 Bucket : Creating S3 Buckets, Getting a List of S3 Buckets, Deleting an S3 Bucket, Testing
  - File Operations in AWS S3 : Upload Files to AWS S3, Get All the Files in an AWS S3, Download Files from AWS S3, Delete Files from AWS S3, Testing File Operations
	   
*Complete Source Code Included!

Read the entire article - https://codewithmukesh.com/blog/working-with-aws-s3-using-aspnet-core/

#amazon #aws #management #testing #dotnet #dotnet6 #codeblog #100daysofcode #blogger #s3 #tutorials
