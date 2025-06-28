using AzureFuncBe.ContainerManager;
using AzureFuncBe.Models;

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

        public async Task<FolderModel?> GetSingleFolderAsync(string userId, string folderId)
        {
            var query = "SELECT TOP 1* FROM Folder f WHERE f.id = @folderId";
            return null;
        }
    }
}
