namespace TodoAPIAssignment.DataAccessLibrary.Models;

public class TodoItem
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDone { get; set; }
}
