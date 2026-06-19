using Microsoft.Data.Sqlite;
using StudentGradeManager.Models;

namespace StudentGradeManager.Data;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
        SeedData();
    }

    private void InitializeDatabase()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS Students (
                StudentId TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Gender TEXT,
                ClassName TEXT,
                EnrollmentDate TEXT
            );
            CREATE TABLE IF NOT EXISTS Courses (
                CourseId TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Credits REAL DEFAULT 0,
                Teacher TEXT
            );
            CREATE TABLE IF NOT EXISTS Grades (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId TEXT NOT NULL,
                CourseId TEXT NOT NULL,
                Score REAL NOT NULL,
                ExamDate TEXT,
                FOREIGN KEY (StudentId) REFERENCES Students(StudentId),
                FOREIGN KEY (CourseId) REFERENCES Courses(CourseId)
            );
            """;
        cmd.ExecuteNonQuery();
    }

    public void SeedData()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var countCmd = conn.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM Students";
        if ((long)countCmd.ExecuteScalar()! > 0) return;

        var cmd = conn.CreateCommand();

        cmd.CommandText = """
            INSERT INTO Students VALUES
                ('2024001', '张伟',   '男', '计算机2401', '2024-09-01'),
                ('2024002', '李娜',   '女', '计算机2401', '2024-09-01'),
                ('2024003', '王强',   '男', '计算机2401', '2024-09-01'),
                ('2024004', '赵敏',   '女', '计算机2402', '2024-09-01'),
                ('2024005', '刘洋',   '男', '计算机2402', '2024-09-01'),
                ('2024006', '陈静',   '女', '计算机2402', '2024-09-01'),
                ('2024007', '杨磊',   '男', '软件工程2401', '2024-09-01'),
                ('2024008', '周婷',   '女', '软件工程2401', '2024-09-01'),
                ('2024009', '吴昊',   '男', '软件工程2401', '2024-09-01'),
                ('2024010', '郑雨',   '女', '软件工程2402', '2024-09-01'),
                ('2024011', '林峰',   '男', '软件工程2402', '2024-09-01'),
                ('2024012', '黄丽',   '女', '计算机2401', '2024-09-01');
            """;
        cmd.ExecuteNonQuery();

        cmd.CommandText = """
            INSERT INTO Courses VALUES
                ('CS101', 'C#程序设计',       4, '肖老师'),
                ('CS102', '数据结构',         3, '王老师'),
                ('CS103', '数据库原理',       3, '李老师'),
                ('CS104', '操作系统',         3, '张老师'),
                ('CS105', '计算机网络',       3.5, '刘老师'),
                ('CS106', 'Windows程序设计',  4, '肖老师');
            """;
        cmd.ExecuteNonQuery();

        cmd.CommandText = """
            INSERT INTO Grades (StudentId, CourseId, Score, ExamDate) VALUES
                ('2024001', 'CS101', 88.5, '2025-01-15'),
                ('2024001', 'CS102', 91.0, '2025-06-20'),
                ('2024001', 'CS103', 85.0, '2025-12-18'),
                ('2024002', 'CS101', 76.0, '2025-01-15'),
                ('2024002', 'CS102', 82.5, '2025-06-20'),
                ('2024002', 'CS106', 90.0, '2026-06-10'),
                ('2024003', 'CS101', 63.0, '2025-01-15'),
                ('2024003', 'CS102', 58.0, '2025-06-20'),
                ('2024004', 'CS103', 95.0, '2025-12-18'),
                ('2024004', 'CS104', 88.0, '2026-01-10'),
                ('2024005', 'CS103', 72.0, '2025-12-18'),
                ('2024005', 'CS105', 68.5, '2026-06-15'),
                ('2024006', 'CS101', 94.0, '2025-01-15'),
                ('2024006', 'CS104', 91.5, '2026-01-10'),
                ('2024006', 'CS106', 87.0, '2026-06-10'),
                ('2024007', 'CS105', 55.0, '2026-06-15'),
                ('2024007', 'CS106', 62.0, '2026-06-10'),
                ('2024008', 'CS101', 79.5, '2025-01-15'),
                ('2024008', 'CS104', 84.0, '2026-01-10'),
                ('2024008', 'CS105', 77.0, '2026-06-15'),
                ('2024009', 'CS102', 93.0, '2025-06-20'),
                ('2024009', 'CS106', 96.0, '2026-06-10'),
                ('2024010', 'CS103', 81.0, '2025-12-18'),
                ('2024010', 'CS105', 74.5, '2026-06-15'),
                ('2024011', 'CS101', 67.0, '2025-01-15'),
                ('2024011', 'CS104', 71.0, '2026-01-10'),
                ('2024011', 'CS106', 65.5, '2026-06-10'),
                ('2024012', 'CS102', 86.0, '2025-06-20'),
                ('2024012', 'CS103', 78.0, '2025-12-18'),
                ('2024012', 'CS105', 92.5, '2026-06-15');
            """;
        cmd.ExecuteNonQuery();
    }

    // ===== Students =====

    public List<Student> GetAllStudents()
    {
        var list = new List<Student>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM Students ORDER BY StudentId";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(new Student
            {
                StudentId = reader["StudentId"].ToString()!,
                Name = reader["Name"].ToString()!,
                Gender = reader["Gender"].ToString()!,
                ClassName = reader["ClassName"].ToString()!,
                EnrollmentDate = reader["EnrollmentDate"].ToString()!
            });
        return list;
    }

    public void InsertStudent(Student s)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Students VALUES (@id, @name, @gender, @class, @date)";
        cmd.Parameters.AddWithValue("@id", s.StudentId);
        cmd.Parameters.AddWithValue("@name", s.Name);
        cmd.Parameters.AddWithValue("@gender", s.Gender);
        cmd.Parameters.AddWithValue("@class", s.ClassName);
        cmd.Parameters.AddWithValue("@date", s.EnrollmentDate);
        cmd.ExecuteNonQuery();
    }

    public void UpdateStudent(Student s)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Students SET Name=@name, Gender=@gender, ClassName=@class, EnrollmentDate=@date WHERE StudentId=@id";
        cmd.Parameters.AddWithValue("@id", s.StudentId);
        cmd.Parameters.AddWithValue("@name", s.Name);
        cmd.Parameters.AddWithValue("@gender", s.Gender);
        cmd.Parameters.AddWithValue("@class", s.ClassName);
        cmd.Parameters.AddWithValue("@date", s.EnrollmentDate);
        cmd.ExecuteNonQuery();
    }

    public void DeleteStudent(string studentId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Grades WHERE StudentId=@id; DELETE FROM Students WHERE StudentId=@id";
        cmd.Parameters.AddWithValue("@id", studentId);
        cmd.ExecuteNonQuery();
    }

    public List<Student> SearchStudents(string keyword)
    {
        var list = new List<Student>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM Students WHERE StudentId LIKE @kw OR Name LIKE @kw OR ClassName LIKE @kw ORDER BY StudentId";
        cmd.Parameters.AddWithValue("@kw", $"%{keyword}%");
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(new Student
            {
                StudentId = reader["StudentId"].ToString()!,
                Name = reader["Name"].ToString()!,
                Gender = reader["Gender"].ToString()!,
                ClassName = reader["ClassName"].ToString()!,
                EnrollmentDate = reader["EnrollmentDate"].ToString()!
            });
        return list;
    }

    // ===== Courses =====

    public List<Course> GetAllCourses()
    {
        var list = new List<Course>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM Courses ORDER BY CourseId";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(new Course
            {
                CourseId = reader["CourseId"].ToString()!,
                Name = reader["Name"].ToString()!,
                Credits = Convert.ToDouble(reader["Credits"]),
                Teacher = reader["Teacher"].ToString()!
            });
        return list;
    }

    public void InsertCourse(Course c)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Courses VALUES (@id, @name, @credits, @teacher)";
        cmd.Parameters.AddWithValue("@id", c.CourseId);
        cmd.Parameters.AddWithValue("@name", c.Name);
        cmd.Parameters.AddWithValue("@credits", c.Credits);
        cmd.Parameters.AddWithValue("@teacher", c.Teacher);
        cmd.ExecuteNonQuery();
    }

    public void UpdateCourse(Course c)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Courses SET Name=@name, Credits=@credits, Teacher=@teacher WHERE CourseId=@id";
        cmd.Parameters.AddWithValue("@id", c.CourseId);
        cmd.Parameters.AddWithValue("@name", c.Name);
        cmd.Parameters.AddWithValue("@credits", c.Credits);
        cmd.Parameters.AddWithValue("@teacher", c.Teacher);
        cmd.ExecuteNonQuery();
    }

    public void DeleteCourse(string courseId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Grades WHERE CourseId=@id; DELETE FROM Courses WHERE CourseId=@id";
        cmd.Parameters.AddWithValue("@id", courseId);
        cmd.ExecuteNonQuery();
    }

    // ===== Grades =====

    public List<Grade> GetAllGrades()
    {
        var list = new List<Grade>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT g.Id, g.StudentId, g.CourseId, g.Score, g.ExamDate,
                   s.Name AS StudentName, c.Name AS CourseName
            FROM Grades g
            JOIN Students s ON g.StudentId = s.StudentId
            JOIN Courses c ON g.CourseId = c.CourseId
            ORDER BY g.Id
            """;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(new Grade
            {
                Id = Convert.ToInt32(reader["Id"]),
                StudentId = reader["StudentId"].ToString()!,
                CourseId = reader["CourseId"].ToString()!,
                Score = Convert.ToDouble(reader["Score"]),
                ExamDate = reader["ExamDate"].ToString()!,
                StudentName = reader["StudentName"].ToString()!,
                CourseName = reader["CourseName"].ToString()!
            });
        return list;
    }

    public void InsertGrade(Grade g)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Grades (StudentId, CourseId, Score, ExamDate) VALUES (@sid, @cid, @score, @date)";
        cmd.Parameters.AddWithValue("@sid", g.StudentId);
        cmd.Parameters.AddWithValue("@cid", g.CourseId);
        cmd.Parameters.AddWithValue("@score", g.Score);
        cmd.Parameters.AddWithValue("@date", g.ExamDate);
        cmd.ExecuteNonQuery();
    }

    public void UpdateGrade(Grade g)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Grades SET StudentId=@sid, CourseId=@cid, Score=@score, ExamDate=@date WHERE Id=@id";
        cmd.Parameters.AddWithValue("@id", g.Id);
        cmd.Parameters.AddWithValue("@sid", g.StudentId);
        cmd.Parameters.AddWithValue("@cid", g.CourseId);
        cmd.Parameters.AddWithValue("@score", g.Score);
        cmd.Parameters.AddWithValue("@date", g.ExamDate);
        cmd.ExecuteNonQuery();
    }

    public void DeleteGrade(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Grades WHERE Id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    // ===== Statistics =====

    public List<CourseStatsDto> GetGradeStatsByCourse()
    {
        var list = new List<CourseStatsDto>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT c.CourseId, c.Name AS CourseName,
                   COUNT(g.Score) AS StudentCount,
                   ROUND(AVG(g.Score), 1) AS AvgScore,
                   ROUND(MIN(g.Score), 1) AS MinScore,
                   ROUND(MAX(g.Score), 1) AS MaxScore
            FROM Courses c
            LEFT JOIN Grades g ON c.CourseId = g.CourseId
            GROUP BY c.CourseId
            """;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(new CourseStatsDto
            {
                CourseId = reader["CourseId"].ToString()!,
                CourseName = reader["CourseName"].ToString()!,
                StudentCount = Convert.ToInt32(reader["StudentCount"]),
                AvgScore = Convert.ToDouble(reader["AvgScore"]),
                MinScore = Convert.ToDouble(reader["MinScore"]),
                MaxScore = Convert.ToDouble(reader["MaxScore"])
            });
        return list;
    }

    public Dictionary<string, int> GetGradeDistribution()
    {
        var dist = new Dictionary<string, int>
        {
            ["90-100"] = 0, ["80-89"] = 0, ["70-79"] = 0,
            ["60-69"] = 0, ["<60"] = 0
        };
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Score FROM Grades";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var score = Convert.ToDouble(reader["Score"]);
            if (score >= 90) dist["90-100"]++;
            else if (score >= 80) dist["80-89"]++;
            else if (score >= 70) dist["70-79"]++;
            else if (score >= 60) dist["60-69"]++;
            else dist["<60"]++;
        }
        return dist;
    }
}
