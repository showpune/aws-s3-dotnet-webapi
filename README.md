# Working with Azure Storage Account using ASP.NET Core – Upload, Download & Delete Files 

![Azure Storage Account using ASP.NET Core](https://codewithmukesh.com/wp-content/uploads/2022/03/Working-with-AWS-S3-using-ASP.NET-Core.png)

In this article, we will be get started on working with Azure Storage Account using ASP.NET Core Web API to upload, download and delete files from Azure's highly scalable Blob Storage! Apart from that, we will also learn more about Azure's Storage Account, the problem it solves, dive a bit into the Azure Portal for Storage Account Management, Azure CLI, and Credentials, Generate Access Keys for accessing Storage via SDKs, Creating and Deleting Azure blob containers, a bit about SAS URLs and so on!

## Topics Covered:

- Azure Storage Account – In Short
- Creating Service Principal & Configuring Access via Azure IAM
- Creating your First Azure Storage Account and Container via Azure Portal
- Azure Configurations & CLI
- Working with Azure Storage Account using ASP.NET Core – Getting Started
  - Working with Azure Blob Containers : Creating Blob Containers, Getting a List of Blob Containers, Deleting a Blob Container, Testing
  - File Operations in Azure Storage : Upload Files to Azure Storage, Get All the Files in an Azure Container, Download Files from Azure Storage, Delete Files from Azure Storage, Testing File Operations
	   
*Complete Source Code Included!

## Migration from AWS S3

This project has been migrated from AWS S3 to Azure Storage Account. The key changes include:

- **API Endpoints**: `/api/buckets` → `/api/containers` (bucket operations now work with Azure blob containers)
- **Dependencies**: AWS SDK replaced with Azure.Storage.Blobs and Azure.Identity
- **Authentication**: Uses DefaultAzureCredential for Azure authentication
- **Configuration**: Azure Storage endpoint in appsettings.json
- **Functionality**: All CRUD operations preserved with Azure equivalents

**Mapping:**
- AWS S3 Bucket → Azure Blob Container
- AWS S3 Object → Azure Blob
- AWS Presigned URLs → Azure SAS URIs

#azure #storage #management #testing #dotnet #dotnet6 #codeblog #100daysofcode #blogger #blob #tutorials