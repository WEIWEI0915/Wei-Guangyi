using StudentGradeManager.Data;
using StudentGradeManager.Models;

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
        Size = new Size(450, 240);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };

        layout.Controls.Add(new Label { Text = "学生：", TextAlign = ContentAlignment.MiddleRight }, 0, 0);
        _cmbStudent = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbStudent.DisplayMember = "DisplayText";
        _cmbStudent.ValueMember = "StudentId";
        layout.Controls.Add(_cmbStudent, 1, 0);

        layout.Controls.Add(new Label { Text = "课程：", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
        _cmbCourse = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbCourse.DisplayMember = "DisplayText";
        _cmbCourse.ValueMember = "CourseId";
        layout.Controls.Add(_cmbCourse, 1, 1);

        layout.Controls.Add(new Label { Text = "成绩：", TextAlign = ContentAlignment.MiddleRight }, 0, 2);
        _numScore = new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 1, Minimum = 0, Maximum = 100 };
        layout.Controls.Add(_numScore, 1, 2);

        layout.Controls.Add(new Label { Text = "考试日期：", TextAlign = ContentAlignment.MiddleRight }, 0, 3);
        _dtpExam = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
        layout.Controls.Add(_dtpExam, 1, 3);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var btnCancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, AutoSize = true };
        var btnSave = new Button { Text = "保存", AutoSize = true };
        btnPanel.Controls.AddRange(new Control[] { btnCancel, btnSave });
        layout.Controls.Add(btnPanel, 0, 4);
        layout.SetColumnSpan(btnPanel, 2);

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

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (_cmbStudent.SelectedValue == null || _cmbCourse.SelectedValue == null)
        {
            MessageBox.Show("请选择学生和课程。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var grade = new Grade
        {
            Id = _existing?.Id ?? 0,
            StudentId = _cmbStudent.SelectedValue.ToString()!,
            CourseId = _cmbCourse.SelectedValue.ToString()!,
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
