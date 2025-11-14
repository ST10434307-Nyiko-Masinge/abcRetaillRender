using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Queues;


/* 
Code attribution
This code was adapted from Microsoft Learn
https://learn.microsoft.com/en-us/azure/storage/files/storage-dotnet-how-to-use-files
Accessed 6 October 2025
**/
namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Services.Storage
{



    public class FileShareStorageServices
    {
        private readonly ShareClient shareClient;

        public FileShareStorageServices(string storageConnectionString, string shareName)
        {
            var serviceClient = new ShareServiceClient(storageConnectionString);
            shareClient = serviceClient.GetShareClient(shareName);
            shareClient.CreateIfNotExists();
        }

        //upoad log to file share 
        public async Task UploadFileAsync(string fileName, Stream fileStream) 
        {
            var directoryClient = shareClient.GetRootDirectoryClient(); 
            var fileClient = directoryClient.GetFileClient(fileName);

            await fileClient.CreateAsync(fileStream.Length);

            //handling the csv 
            long position = 0;
            int bufferSize = 4 * 1024 * 1024;
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = await fileStream.ReadAsync(buffer, 0,bufferSize)) > 0) 
            {
                using var memoryStream = new MemoryStream(buffer, 0, bytesRead);
                await fileClient.UploadRangeAsync
                    (
                    Azure.Storage.Files.Shares.Models.ShareFileRangeWriteType.Update,new HttpRange(position, bytesRead), memoryStream
                    );
                position += bytesRead;  
            }

        }
    }
}
