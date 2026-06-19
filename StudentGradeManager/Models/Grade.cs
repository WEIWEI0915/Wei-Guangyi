namespace StudentGradeManager.Models;

// 成绩实体，含 JOIN 查询扩展字段 StudentName / CourseName
public class Grade
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string CourseId { get; set; } = string.Empty;
    public double Score { get; set; }
    public string ExamDate { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
}
