using StudentGradeManager.Models;

namespace StudentGradeManager.Interfaces;

public interface IStudentRepository
{
    List<Student> GetAllStudents();
    List<Student> SearchStudents(string keyword);
    void InsertStudent(Student s);
    void UpdateStudent(Student s);
    void DeleteStudent(string studentId);
}
