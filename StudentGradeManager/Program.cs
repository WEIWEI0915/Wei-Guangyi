using StudentGradeManager.Data;
using StudentGradeManager.Forms;
using StudentGradeManager.Interfaces;

namespace StudentGradeManager;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // 数据库初始化可能在磁盘权限不足、文件被锁等情况下失败
        IDatabaseHelper? db = null;
        try
        {
            var dbPath = Path.Combine(Application.StartupPath, "grades.db");
            db = new DatabaseHelper(dbPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"数据库初始化失败，程序无法启动。\n\n原因：{ex.Message}",
                "启动错误",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        Application.Run(new MainForm(db));
    }
}
