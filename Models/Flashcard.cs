namespace AzureFuncBe.Models
{
    public class Flashcard
    {
        public required string Id { get; set; }
        public string? CardFrontText { get; set; }
        public string? CardBackText { get; set; }    
        public List<string>? Tags { get; set; }
        public required bool IsFavorite { get; set; } 

    }
}
