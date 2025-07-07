using AzureFuncBe.ContainerManager;
using AzureFuncBe.DTOs.FolderDTOs;
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

        public async Task UpdateFolderAsync(string userId, string folderId, FolderUpdateRequestDTO folderUpdateRequestDTO)
        {
            var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFolderContainerName());
            try
            {
                var patchOperations = new PatchOperation[]
                {
                    PatchOperation.Replace("/folderName", folderUpdateRequestDTO.Name),
                    PatchOperation.Replace("/folderDesc", folderUpdateRequestDTO.FolderDescription),
                    PatchOperation.Replace("/isFavorite", folderUpdateRequestDTO.IsFavorite)
                };

                TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(userId));
                batch.PatchItem(folderId, patchOperations);
                TransactionalBatchResponse response = await batch.ExecuteAsync();
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
                throw new Exception();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task IncrementFolderCardCountAsync(string userId, string folderId)
        {
            var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFolderContainerName());
            try
            {
                var singleFolder = await GetSingleFolderAsync(userId, folderId);
                if (singleFolder == null)
                {
                    throw new Exception("folder not found");
                }
                var patchOperations = new PatchOperation[]
                {
                    PatchOperation.Increment("/cardCount", 1),
                };
                TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(userId));
                batch.PatchItem(folderId, patchOperations);
                TransactionalBatchResponse response = await batch.ExecuteAsync();
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
                throw new Exception();
            }
            catch (Exception)
            {
                throw;
            }
            
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
                    "f.createdBy, " +
                    "f.createdAt " +
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
                if (paginatedFoldersSearchDTO.FolderIsFavorite == 0 || paginatedFoldersSearchDTO.FolderIsFavorite == 1)
                {
                    query += " AND f.isFavorite = @folderIsFavorite";
                }
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

        // delete folder by id
        // step 1: create db container
        // step 2: delete folder by id
        // NOTE: we will comeback to this method later when we start adding flashcards to container
        // we will delegate the job of deleteing all flashcards in the folder to an queue-based azure function. this function will poll the id of the folder we want to delete and remove all flashcards within it along with the folder itself.
        // we have to do this later because each folder can contain multiple flashcards and we don't want users to wait for this potentially time-consuming operation.
        // step 3: call this method from controller at delete route
        public async Task DeleteFolderAsync(string userId, string folderId)
        {
            var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFolderContainerName());
            try
            {
                var response = await container.DeleteItemAsync<FolderModel>(folderId, new PartitionKey(userId));
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new Exception("Failed to delete folder");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
