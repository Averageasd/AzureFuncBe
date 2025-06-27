using AzureFuncBe.ContainerManager;
using Microsoft.Extensions.Logging;

namespace AzureFuncBe.Controllers
{
    public class FolderController
    {
        private readonly ILogger<FolderController> _logger;

        public FolderController(ILogger<FolderController> logger, DBContainerManager dBContainerManager)
        {
            _logger = logger;
        }
    }
}