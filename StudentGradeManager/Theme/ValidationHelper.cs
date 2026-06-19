namespace StudentGradeManager.Theme;

public static class ValidationHelper
{
    public static class Limits
    {
        public const int StudentIdMax = 20;
        public const int CourseIdMax  = 20;
        public const int NameMax      = 50;
        public const int ClassNameMax = 40;
        public const int TeacherMax   = 30;
        public const int SearchMax    = 50;
    }

    public static string? ValidateRequired(string value, string fieldName, int maxLen)
    {
        if (string.IsNullOrWhiteSpace(value))
            return $"{fieldName}不能为空。";
        var trimmed = value.Trim();
        if (trimmed.Length > maxLen)
            return $"{fieldName}不能超过 {maxLen} 个字符。";
        return null;
    }

    public static string? ValidateStudentId(string id)
    {
        var err = ValidateRequired(id, "学号", Limits.StudentIdMax);
        if (err != null) return err;
        // 学号只能是字母、数字和连字符
        if (!System.Text.RegularExpressions.Regex.IsMatch(id.Trim(), @"^[a-zA-Z0-9\-]+$"))
            return "学号只能包含字母、数字和连字符。";
        return null;
    }

    public static string? ValidateCourseId(string id)
    {
        var err = ValidateRequired(id, "课程编号", Limits.CourseIdMax);
        if (err != null) return err;
        if (!System.Text.RegularExpressions.Regex.IsMatch(id.Trim(), @"^[a-zA-Z0-9\-]+$"))
            return "课程编号只能包含字母、数字和连字符。";
        return null;
    }

    public static string? ValidateName(string name)
    {
        return ValidateRequired(name, "姓名", Limits.NameMax);
    }

    public static string? ValidateCourseName(string name)
    {
        return ValidateRequired(name, "课程名称", Limits.NameMax);
    }

    public static string? ValidateClassName(string className)
    {
        return ValidateRequired(className, "班级", Limits.ClassNameMax);
    }

    public static string? ValidateTeacher(string teacher)
    {
        if (string.IsNullOrWhiteSpace(teacher)) return null; // 教师选填
        return teacher.Trim().Length > Limits.TeacherMax
            ? $"授课教师不能超过 {Limits.TeacherMax} 个字符。"
            : null;
    }

    public static string? ValidateSearchInput(string keyword)
    {
        if (string.IsNullOrEmpty(keyword)) return null;
        return keyword.Length > Limits.SearchMax
            ? $"搜索关键词不能超过 {Limits.SearchMax} 个字符。"
            : null;
    }

    /// <summary>校验多个规则，返回第一条错误信息，无错返回 null。</summary>
    public static string? FirstError(params string?[] errors)
    {
        return errors.FirstOrDefault(e => e != null);
    }
}
