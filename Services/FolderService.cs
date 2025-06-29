using AzureFuncBe.ContainerManager;
using AzureFuncBe.DTOs;
using Microsoft.Azure.Cosmos;

namespace AzureFuncBe.Services
{
    public class FolderService
    {
        private DBContainerManager _dBContainerManager;
        public FolderService
        (
            DBContainerManager dBContainerManager
        )
        {
            _dBContainerManager = dBContainerManager;
        }

        public async Task<SingleFolderResponseDTO?> GetSingleFolderAsync(string userId, string folderId)
        {
            try
            {
                var query =
                    "SELECT TOP 1 f.id, " +
                    "f.folderName, " +
                    "f.folderDesc, " +
                    "f.cardCount, " +
                    "f.isFavorite," +
                    " f.createdBy " +
                    "FROM Folder f WHERE f.id = @folderId AND f.userId = @userId";
                var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFolderContainerName());
                var queryDefinition = new QueryDefinition(query)
                    .WithParameter("@folderId", folderId)
                    .WithParameter("@userId", userId);
                var queryResultSetIterator = container.GetItemQueryIterator<SingleFolderResponseDTO>(queryDefinition);
                var singleFolder = await queryResultSetIterator.ReadNextAsync();
                return singleFolder.First();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public async Task<PaginatedFoldersDTO?> GetFoldersAsync(string userId, string continuationToken)
        {
            try
            {
                List<SingleFolderResponseDTO> folders = new List<SingleFolderResponseDTO>();
                var query =
                    "SELECT f.id, " +
                    "f.folderName, " +
                    "f.folderDesc, " +
                    "f.cardCount, " +
                    "f.isFavorite," +
                    " f.createdBy " +
                    "FROM Folder f WHERE f.userId = @userId";
                var queryDefinition = new QueryDefinition(query)
                    .WithParameter("@userId", userId);
                if (string.IsNullOrEmpty(continuationToken))
                {
                    continuationToken = null;
                }

                var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFolderContainerName());
                using (FeedIterator<SingleFolderResponseDTO> setIterator = container.GetItemQueryIterator<SingleFolderResponseDTO>(
                    queryDefinition,
                    continuationToken,
                    requestOptions:
                    new QueryRequestOptions
                    {

                        MaxItemCount = 10
                    }
                    ))
                {
                    var foldersResponse = await setIterator.ReadNextAsync();
                    folders.AddRange(foldersResponse);
                    return new PaginatedFoldersDTO
                    {
                        ListResponses = folders,
                        ContinuationToken = foldersResponse.ContinuationToken
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
