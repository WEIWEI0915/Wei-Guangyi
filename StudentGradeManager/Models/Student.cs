namespace StudentGradeManager.Models;

// 学生实体，属性与 Students 表字段一一对应
public class Student
{
    public string StudentId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string EnrollmentDate { get; set; } = string.Empty;
}
