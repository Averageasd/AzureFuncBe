namespace AzureFuncBe.Utils
{
    public class GenerateNewDateUtil
    {
        public static string GenerateNewDate(DateTimeOffset dateTime)
        {
            string formattedCreatedAt = dateTime.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
            return formattedCreatedAt;
        }
    }
}
