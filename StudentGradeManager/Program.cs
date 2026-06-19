using StudentGradeManager.Data;
using StudentGradeManager.Forms;

namespace StudentGradeManager;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var dbPath = Path.Combine(Application.StartupPath, "grades.db");
        var db = new DatabaseHelper(dbPath);

        Application.Run(new MainForm(db));
    }
}
