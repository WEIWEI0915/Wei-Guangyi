using StudentGradeManager.Data;
using StudentGradeManager.Models;
using StudentGradeManager.Theme;

namespace StudentGradeManager.Forms;

public class GradeDetailForm : Form
{
    private readonly DatabaseHelper _db;
    private readonly Grade? _existing;

    private readonly ComboBox _cmbStudent, _cmbCourse;
    private readonly NumericUpDown _numScore;
    private readonly DateTimePicker _dtpExam;

    public GradeDetailForm(DatabaseHelper db, Grade? existing)
    {
        _db = db;
        _existing = existing;
        Text = existing == null ? "录入成绩" : "编辑成绩";
        Size = new Size(460, 260);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        AppTheme.ApplyFormStyle(this);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(12) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        void AddRow(string label, Control ctrl, int row)
        {
            layout.Controls.Add(new Label { Text = label, Font = AppTheme.BodyFont, ForeColor = AppTheme.TextPrimary, TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, row);
            layout.Controls.Add(ctrl, 1, row);
        }

        _cmbStudent = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.BodyFont };
        _cmbStudent.DisplayMember = "DisplayText";
        _cmbStudent.ValueMember = "StudentId";
        AddRow("学生：", _cmbStudent, 0);

        _cmbCourse = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.BodyFont };
        _cmbCourse.DisplayMember = "DisplayText";
        _cmbCourse.ValueMember = "CourseId";
        AddRow("课程：", _cmbCourse, 1);

        _numScore = new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 1, Minimum = 0, Maximum = 100, Font = AppTheme.BodyFont };
        AddRow("成绩：", _numScore, 2);

        _dtpExam = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Font = AppTheme.BodyFont, MaxDate = DateTime.Today };
        AddRow("考试日期：", _dtpExam, 3);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Margin = new Padding(0, 8, 0, 0) };
        var btnCancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, Font = AppTheme.BodyFont, AutoSize = true, Padding = new Padding(12, 4, 12, 4) };
        var btnSave = AppTheme.CreateButton("保存");
        btnPanel.Controls.AddRange(new Control[] { btnCancel, btnSave });
        layout.Controls.Add(btnPanel, 0, 4);
        layout.SetColumnSpan(btnPanel, 2);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        Controls.Add(layout);
        CancelButton = btnCancel;
        AcceptButton = btnSave;
        btnSave.Click += BtnSave_Click;

        LoadComboData();

        if (existing != null)
        {
            _cmbStudent.SelectedValue = existing.StudentId;
            _cmbCourse.SelectedValue = existing.CourseId;
            _numScore.Value = (decimal)existing.Score;
            if (DateTime.TryParse(existing.ExamDate, out var dt))
                _dtpExam.Value = dt;
        }
    }

    private void LoadComboData()
    {
        var students = _db.GetAllStudents();
        _cmbStudent.DataSource = students.Select(s => new { s.StudentId, DisplayText = $"{s.StudentId} - {s.Name}" }).ToList();

        var courses = _db.GetAllCourses();
        _cmbCourse.DataSource = courses.Select(c => new { c.CourseId, DisplayText = $"{c.CourseId} - {c.Name}" }).ToList();
    }

    private bool ValidateInput()
    {
        if (_cmbStudent.Items.Count == 0)
        {
            MessageBox.Show("没有学生数据，请先添加学生。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (_cmbCourse.Items.Count == 0)
        {
            MessageBox.Show("没有课程数据，请先添加课程。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (_cmbStudent.SelectedValue == null || _cmbCourse.SelectedValue == null)
        {
            MessageBox.Show("请选择学生和课程。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;

        var grade = new Grade
        {
            Id = _existing?.Id ?? 0,
            StudentId = _cmbStudent.SelectedValue?.ToString() ?? "",
            CourseId = _cmbCourse.SelectedValue?.ToString() ?? "",
            Score = (double)_numScore.Value,
            ExamDate = _dtpExam.Value.ToString("yyyy-MM-dd")
        };

        try
        {
            if (_existing == null)
                _db.InsertGrade(grade);
            else
                _db.UpdateGrade(grade);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
