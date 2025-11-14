using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage
{

    /*
 Code attribution
 this code was adapted from Microsoft ASP.NET Core MVC CRUD tutorial
 https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud?view=aspnetcore-8.0
 accessed 7 October 2025
*/

    public class BlobStorageServices
    {
        private readonly BlobContainerClient blobcontainerClient;

        public BlobStorageServices(string storageConnectionString, string containerName)
        {
            var serviceClient = new BlobServiceClient(storageConnectionString);
            blobcontainerClient = serviceClient.GetBlobContainerClient(containerName);
            blobcontainerClient.CreateIfNotExists();
        }

        // pic with SAS URL (Create.cshtml)
        public async Task<string> UploadPhotoAsync(string blobName, Stream stream)
        {
            var blobClient = blobcontainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(stream, true);
            return GetBlobUriWithSas(blobClient);
        }

        // Get blob URI with SAS token
        private string GetBlobUriWithSas(BlobClient blobclient)
        {
            if (blobclient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = blobclient.BlobContainerName,
                    BlobName = blobclient.Name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(5),
                };
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                var sasUri = blobclient.GenerateSasUri(sasBuilder);
                return sasUri.ToString();
            }
            else
            {
                throw new InvalidOperationException("Blob client does not support generating SAS URIs");
            }
        }

        // Upload image (IFormFile from form upload)
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null!;

            string blobName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            BlobClient blobClient = blobcontainerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            return blobClient.Uri.ToString(); // return the image URL
        }

        // Delete image (using URL from DB or storage)
        public async Task DeleteImageAsync(string blobUrl)
        {
            if (string.IsNullOrEmpty(blobUrl))
                return;

            string blobName = Path.GetFileName(new Uri(blobUrl).AbsolutePath);
            BlobClient blobClient = blobcontainerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> UploadImageAsync(IFormFile photoFile, string? existingPhotoURL)
        {
            if (photoFile == null || photoFile.Length == 0)
                return existingPhotoURL ?? string.Empty;

            // Delete existing image if provided
            if (!string.IsNullOrEmpty(existingPhotoURL))
            {
                await DeleteImageAsync(existingPhotoURL);
            }

            return await UploadImageAsync(photoFile);
        }
    }
}
