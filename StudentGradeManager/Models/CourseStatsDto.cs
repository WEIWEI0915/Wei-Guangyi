namespace StudentGradeManager.Models;

// 统计报表 DTO：按课程分组的平均分 / 最高分 / 最低分 / 学生数
public class CourseStatsDto
{
    public string CourseId { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public double AvgScore { get; set; }
    public double MinScore { get; set; }
    public double MaxScore { get; set; }
}
