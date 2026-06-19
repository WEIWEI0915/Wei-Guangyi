namespace StudentGradeManager.Models;

public class CourseStatsDto
{
    public string CourseId { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public double AvgScore { get; set; }
    public double MinScore { get; set; }
    public double MaxScore { get; set; }
}
