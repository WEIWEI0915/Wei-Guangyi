namespace StudentGradeManager.Models;

public class Course
{
    public string CourseId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Credits { get; set; }
    public string Teacher { get; set; } = string.Empty;
}
