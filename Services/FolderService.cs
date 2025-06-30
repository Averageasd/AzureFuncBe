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
        
        // step 1: perform searching
        // 1.1 : search by folder name
        // step 2: filtering
        // 2.1: filter by favorite
        // 2.2: filter by created by username
        // 2.3 : filter by date created
        // step 3: ordering
        // 3.1 : order by date
        public async Task<PaginatedFoldersDTO?> GetFoldersAsync(string userId, PaginatedFoldersSearchDTO paginatedFoldersSearchDTO)
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
                    " f.createdBy, " +
                    " f.createdAt " +
                    "FROM Folder f WHERE f.userId = @userId";
                if (string.IsNullOrEmpty(paginatedFoldersSearchDTO.ContinuationToken))
                {
                    paginatedFoldersSearchDTO.ContinuationToken = null!;
                }

                query += " AND f.folderName LIKE @folderNameSearch";
                query += " AND f.isFavorite = @folderIsFavorite";
                query += " AND f.createdBy LIKE @createdByUserNameSearch";
                query += " AND f.createdAt >= @createdDateSearchMin";
                if (paginatedFoldersSearchDTO.CreatedDateSearchMax > paginatedFoldersSearchDTO.CreatedDateSearchMin)
                {
                    query += " AND f.createdAt <= @createdDateSearchMax";
                }

                var queryDefinition = new QueryDefinition(query)
                   .WithParameter("@userId", userId)
                   .WithParameter("@folderNameSearch", $"{paginatedFoldersSearchDTO.FolderNameSearch}%")
                   .WithParameter("@folderIsFavorite", paginatedFoldersSearchDTO.FolderIsFavorite)
                   .WithParameter("@createdByUserNameSearch", $"{paginatedFoldersSearchDTO.CreatedByUsernameSearch}%")
                   .WithParameter("@createdDateSearchMin", paginatedFoldersSearchDTO.CreatedDateSearchMin)
                   .WithParameter("@createdDateSearchMax", paginatedFoldersSearchDTO.CreatedDateSearchMax);
                var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFolderContainerName());
                using (FeedIterator<SingleFolderResponseDTO> setIterator = container.GetItemQueryIterator<SingleFolderResponseDTO>(
                    queryDefinition,
                    paginatedFoldersSearchDTO.ContinuationToken,
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
