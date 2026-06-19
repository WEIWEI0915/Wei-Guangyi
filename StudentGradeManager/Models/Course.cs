namespace StudentGradeManager.Models;

// 课程实体，属性与 Courses 表字段一一对应
public class Course
{
    public string CourseId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Credits { get; set; }
    public string Teacher { get; set; } = string.Empty;
}
