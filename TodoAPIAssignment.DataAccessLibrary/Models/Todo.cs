using System.ComponentModel.DataAnnotations;

namespace TodoAPIAssignment.DataAccessLibrary.Models;

public class Todo
{
    public string? Id { get; set; }
    [Required]
    public string? Title { get; set; }
    public bool IsDone { get; set; }
    public DateTime? CreatedAt { get; set; }
    [Required]
    public string? UserId { get; set; }
}
