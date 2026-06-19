using StudentGradeManager.Data;
using StudentGradeManager.Models;
using System.Windows.Forms.DataVisualization.Charting;

namespace StudentGradeManager.Forms;

public partial class MainForm : Form
{
    private readonly DatabaseHelper _db;
    private readonly TabControl _tabControl;

    // Students tab controls
    private DataGridView _dgvStudents;
    private TextBox _txtSearchStudent;
    private Button _btnAddStudent, _btnEditStudent, _btnDeleteStudent, _btnSearchStudent;

    // Courses tab controls
    private DataGridView _dgvCourses;
    private Button _btnAddCourse, _btnEditCourse, _btnDeleteCourse;

    // Grades tab controls
    private DataGridView _dgvGrades;
    private Button _btnAddGrade, _btnEditGrade, _btnDeleteGrade;

    // Statistics tab controls
    private DataGridView _dgvStats;
    private Chart _chartDist;
    private Button _btnExport;

    public MainForm(DatabaseHelper db)
    {
        _db = db;
        Text = "学生成绩管理系统";
        Size = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;
        _tabControl = new TabControl { Dock = DockStyle.Fill };
        Controls.Add(_tabControl);

        BuildStudentsTab();
        BuildCoursesTab();
        BuildGradesTab();
        BuildStatisticsTab();

        _tabControl.SelectedIndexChanged += (_, _) => RefreshCurrentTab();
    }

    // ========== Students Tab ==========
    private void BuildStudentsTab()
    {
        var tab = new TabPage("学生管理");
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };

        var topPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(5) };
        _txtSearchStudent = new TextBox { Width = 200, PlaceholderText = "搜索学号/姓名/班级..." };
        _btnSearchStudent = new Button { Text = "搜索", AutoSize = true };
        _btnAddStudent = new Button { Text = "添加学生", AutoSize = true };
        _btnEditStudent = new Button { Text = "编辑", AutoSize = true };
        _btnDeleteStudent = new Button { Text = "删除", AutoSize = true };
        topPanel.Controls.AddRange(new Control[] { _txtSearchStudent, _btnSearchStudent, _btnAddStudent, _btnEditStudent, _btnDeleteStudent });

        _dgvStudents = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        layout.Controls.Add(topPanel, 0, 0);
        layout.Controls.Add(_dgvStudents, 0, 1);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        tab.Controls.Add(layout);
        _tabControl.TabPages.Add(tab);

        _btnSearchStudent.Click += (_, _) => LoadStudents();
        _btnAddStudent.Click += (_, _) => { new StudentDetailForm(_db, null).ShowDialog(); LoadStudents(); };
        _btnEditStudent.Click += (_, _) => EditSelectedStudent();
        _btnDeleteStudent.Click += (_, _) => DeleteSelectedStudent();
        _txtSearchStudent.KeyPress += (_, e) => { if (e.KeyChar == 13) LoadStudents(); };
    }

    private void LoadStudents()
    {
        var keyword = _txtSearchStudent.Text.Trim();
        var data = string.IsNullOrEmpty(keyword)
            ? _db.GetAllStudents()
            : _db.SearchStudents(keyword);
        _dgvStudents.DataSource = null;
        _dgvStudents.DataSource = data;
    }

    private void EditSelectedStudent()
    {
        if (_dgvStudents.CurrentRow?.DataBoundItem is Student s)
            new StudentDetailForm(_db, s).ShowDialog();
        LoadStudents();
    }

    private void DeleteSelectedStudent()
    {
        if (_dgvStudents.CurrentRow?.DataBoundItem is not Student s) return;
        if (MessageBox.Show($"确认删除学生 {s.Name}({s.StudentId})？\n相关成绩记录将一并删除。", "确认删除",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            _db.DeleteStudent(s.StudentId);
            LoadStudents();
        }
    }

    // ========== Courses Tab ==========
    private void BuildCoursesTab()
    {
        var tab = new TabPage("课程管理");
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };

        var topPanel = new FlowLayoutPanel { Padding = new Padding(5) };
        _btnAddCourse = new Button { Text = "添加课程", AutoSize = true };
        _btnEditCourse = new Button { Text = "编辑", AutoSize = true };
        _btnDeleteCourse = new Button { Text = "删除", AutoSize = true };
        topPanel.Controls.AddRange(new Control[] { _btnAddCourse, _btnEditCourse, _btnDeleteCourse });

        _dgvCourses = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        layout.Controls.Add(topPanel, 0, 0);
        layout.Controls.Add(_dgvCourses, 0, 1);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        tab.Controls.Add(layout);
        _tabControl.TabPages.Add(tab);

        _btnAddCourse.Click += (_, _) => { new CourseDetailForm(_db, null).ShowDialog(); LoadCourses(); };
        _btnEditCourse.Click += (_, _) => EditSelectedCourse();
        _btnDeleteCourse.Click += (_, _) => DeleteSelectedCourse();
    }

    private void LoadCourses()
    {
        _dgvCourses.DataSource = null;
        _dgvCourses.DataSource = _db.GetAllCourses();
    }

    private void EditSelectedCourse()
    {
        if (_dgvCourses.CurrentRow?.DataBoundItem is Course c)
            new CourseDetailForm(_db, c).ShowDialog();
        LoadCourses();
    }

    private void DeleteSelectedCourse()
    {
        if (_dgvCourses.CurrentRow?.DataBoundItem is not Course c) return;
        if (MessageBox.Show($"确认删除课程 {c.Name}({c.CourseId})？\n相关成绩记录将一并删除。", "确认删除",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            _db.DeleteCourse(c.CourseId);
            LoadCourses();
        }
    }

    // ========== Grades Tab ==========
    private void BuildGradesTab()
    {
        var tab = new TabPage("成绩管理");
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };

        var topPanel = new FlowLayoutPanel { Padding = new Padding(5) };
        _btnAddGrade = new Button { Text = "录入成绩", AutoSize = true };
        _btnEditGrade = new Button { Text = "编辑", AutoSize = true };
        _btnDeleteGrade = new Button { Text = "删除", AutoSize = true };
        topPanel.Controls.AddRange(new Control[] { _btnAddGrade, _btnEditGrade, _btnDeleteGrade });

        _dgvGrades = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        layout.Controls.Add(topPanel, 0, 0);
        layout.Controls.Add(_dgvGrades, 0, 1);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        tab.Controls.Add(layout);
        _tabControl.TabPages.Add(tab);

        _btnAddGrade.Click += (_, _) => { new GradeDetailForm(_db, null).ShowDialog(); LoadGrades(); };
        _btnEditGrade.Click += (_, _) => EditSelectedGrade();
        _btnDeleteGrade.Click += (_, _) => DeleteSelectedGrade();
    }

    private void LoadGrades()
    {
        _dgvGrades.DataSource = null;
        _dgvGrades.DataSource = _db.GetAllGrades();
    }

    private void EditSelectedGrade()
    {
        if (_dgvGrades.CurrentRow?.DataBoundItem is Grade g)
            new GradeDetailForm(_db, g).ShowDialog();
        LoadGrades();
    }

    private void DeleteSelectedGrade()
    {
        if (_dgvGrades.CurrentRow?.DataBoundItem is not Grade g) return;
        if (MessageBox.Show($"确认删除成绩记录（{g.StudentName} - {g.CourseName}: {g.Score}分）？",
            "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        {
            _db.DeleteGrade(g.Id);
            LoadGrades();
        }
    }

    // ========== Statistics Tab ==========
    private void BuildStatisticsTab()
    {
        var tab = new TabPage("统计报表");
        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical };

        // Left: chart
        _chartDist = new Chart { Dock = DockStyle.Fill };
        _chartDist.ChartAreas.Add(new ChartArea("Default"));
        _chartDist.Titles.Add("成绩分布");

        // Right: data grid + export
        var rightPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };

        _btnExport = new Button { Text = "导出到 Excel", AutoSize = true, Margin = new Padding(5) };
        rightPanel.Controls.Add(_btnExport, 0, 0);
        rightPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _dgvStats = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        rightPanel.Controls.Add(_dgvStats, 0, 1);
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        split.Panel1.Controls.Add(_chartDist);
        split.Panel2.Controls.Add(rightPanel);
        tab.Controls.Add(split);
        _tabControl.TabPages.Add(tab);

        _btnExport.Click += (_, _) => ExportToExcel();
    }

    private void LoadStatistics()
    {
        var stats = _db.GetGradeStatsByCourse();
        _dgvStats.DataSource = null;
        _dgvStats.DataSource = stats.Select(s => new
        {
            课程编号 = s.CourseId,
            课程名称 = s.CourseName,
            平均分 = s.AvgScore
        }).ToList();

        _chartDist.Series.Clear();
        var dist = _db.GetGradeDistribution();
        var series = new Series("人数")
        {
            ChartType = SeriesChartType.Column,
            IsValueShownAsLabel = true
        };
        foreach (var kv in dist)
            series.Points.AddXY(kv.Key, kv.Value);
        _chartDist.Series.Add(series);
    }

    private void ExportToExcel()
    {
        using var saveDialog = new SaveFileDialog
        {
            Filter = "Excel 文件 (*.xlsx)|*.xlsx",
            FileName = $"成绩统计_{DateTime.Now:yyyyMMdd}.xlsx"
        };
        if (saveDialog.ShowDialog() != DialogResult.OK) return;

        try
        {
            var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("成绩统计");

            var grades = _db.GetAllGrades();
            ws.Cell(1, 1).Value = "学号";
            ws.Cell(1, 2).Value = "学生姓名";
            ws.Cell(1, 3).Value = "课程";
            ws.Cell(1, 4).Value = "成绩";
            ws.Cell(1, 5).Value = "考试日期";

            for (int i = 0; i < grades.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = grades[i].StudentId;
                ws.Cell(i + 2, 2).Value = grades[i].StudentName;
                ws.Cell(i + 2, 3).Value = grades[i].CourseName;
                ws.Cell(i + 2, 4).Value = grades[i].Score;
                ws.Cell(i + 2, 5).Value = grades[i].ExamDate;
            }

            ws.Columns().AdjustToContents();
            wb.SaveAs(saveDialog.FileName);
            MessageBox.Show($"导出成功！\n共 {grades.Count} 条记录。", "导出完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RefreshCurrentTab()
    {
        switch (_tabControl.SelectedIndex)
        {
            case 0: LoadStudents(); break;
            case 1: LoadCourses(); break;
            case 2: LoadGrades(); break;
            case 3: LoadStatistics(); break;
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        RefreshCurrentTab();
    }
}
