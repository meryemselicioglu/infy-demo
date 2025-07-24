public class Notice
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedOn { get; set; }

    public string Username { get; set; } = null!;
}