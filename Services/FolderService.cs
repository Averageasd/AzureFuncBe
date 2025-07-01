using AzureFuncBe.ContainerManager;
using AzureFuncBe.DTOs;
using AzureFuncBe.Models;
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

        public async Task CreateFolderAsync(string userId, CreateFolderRequestDTO createFolderRequestDTO)
        {
            var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFolderContainerName());
            var folder = new FolderModel
            {
                Id = Guid.NewGuid().ToString(),
                Name = createFolderRequestDTO.Name,
                CardCount = 0,
                UserId = userId,
                FolderDescription = createFolderRequestDTO.FolderDescription,
                IsFavorite = createFolderRequestDTO.IsFavorite,
                CreatedBy = userId,
                CreatedDate = DateOnly.FromDateTime(DateTime.Today)
            };
            try
            {
                var response = await container.CreateItemAsync(folder, new PartitionKey(folder.UserId));
                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    throw new Exception("Failed to create folder");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating folder: {ex.Message}");
                throw;
            }
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
                if (paginatedFoldersSearchDTO.OrderedProperty.Equals(OrderPropertiesConstants.CreatedAt))
                {
                    query += " ORDER BY f.createdAt";
                    if (paginatedFoldersSearchDTO.SortDirection.Equals(OrderPropertiesConstants.DescOrder))
                    {
                        query += " DESC";
                    }
                    else if (paginatedFoldersSearchDTO.SortDirection.Equals(OrderPropertiesConstants.AscOrder))
                    {
                        query += " ASC";
                    }
                }

                if (paginatedFoldersSearchDTO.OrderedProperty.Equals(OrderPropertiesConstants.FolderName))
                {
                    query += " ORDER BY f.folderName";
                    if (paginatedFoldersSearchDTO.SortDirection.Equals(OrderPropertiesConstants.DescOrder))
                    {
                        query += " DESC";
                    }
                    else if (paginatedFoldersSearchDTO.SortDirection.Equals(OrderPropertiesConstants.AscOrder))
                    {
                        query += " ASC";
                    }

                    // break tie in ordering
                    query += ", f.createdAt DESC"; 
                }

                if (paginatedFoldersSearchDTO.OrderedProperty.Equals(OrderPropertiesConstants.CardCount))
                {
                    query += " ORDER BY f.cardCount";
                    if (paginatedFoldersSearchDTO.SortDirection.Equals(OrderPropertiesConstants.DescOrder))
                    {
                        query += " DESC";
                    }
                    else if (paginatedFoldersSearchDTO.SortDirection.Equals(OrderPropertiesConstants.AscOrder))
                    {
                        query += " ASC";
                    }

                    // break tie in ordering
                    query += ", f.createdAt DESC";
                }

                if (paginatedFoldersSearchDTO.OrderedProperty.Equals(OrderPropertiesConstants.CreatedBy))
                {
                    query += " ORDER BY f.createdBy";
                    if (paginatedFoldersSearchDTO.SortDirection.Equals(OrderPropertiesConstants.DescOrder))
                    {
                        query += " DESC";
                    }
                    else if (paginatedFoldersSearchDTO.SortDirection.Equals(OrderPropertiesConstants.AscOrder))
                    {
                        query += " ASC";
                    }

                    // break tie in ordering
                    query += ", f.createdAt DESC";
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
