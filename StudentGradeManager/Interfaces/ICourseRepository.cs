using StudentGradeManager.Models;

namespace StudentGradeManager.Interfaces;

public interface ICourseRepository
{
    List<Course> GetAllCourses();
    List<Course> SearchCourses(string keyword);
    void InsertCourse(Course c);
    void UpdateCourse(Course c);
    void DeleteCourse(string courseId);
}
