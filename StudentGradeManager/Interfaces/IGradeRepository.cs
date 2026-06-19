using StudentGradeManager.Models;

namespace StudentGradeManager.Interfaces;

public interface IGradeRepository
{
    List<Grade> GetAllGrades();
    List<Grade> SearchGrades(string keyword);
    void InsertGrade(Grade g);
    void UpdateGrade(Grade g);
    void DeleteGrade(int id);
    List<CourseStatsDto> GetGradeStatsByCourse();
    Dictionary<string, int> GetGradeDistribution();
    List<Student> GetAllStudents();
    List<Course> GetAllCourses();
}
