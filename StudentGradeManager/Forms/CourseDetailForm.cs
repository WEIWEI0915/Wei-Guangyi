using Microsoft.Data.Sqlite;
using StudentGradeManager.Data;
using StudentGradeManager.Models;
using StudentGradeManager.Theme;

namespace StudentGradeManager.Forms;

public class CourseDetailForm : Form
{
    private readonly DatabaseHelper _db;
    private readonly Course? _existing;

    private readonly TextBox _txtCourseId, _txtName, _txtTeacher;
    private readonly NumericUpDown _numCredits;

    public CourseDetailForm(DatabaseHelper db, Course? existing)
    {
        _db = db;
        _existing = existing;
        Text = existing == null ? "添加课程" : "编辑课程";
        Size = new Size(420, 260);
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

        _txtCourseId = new TextBox { Dock = DockStyle.Fill, Font = AppTheme.BodyFont, MaxLength = ValidationHelper.Limits.CourseIdMax };
        AddRow("课程编号：", _txtCourseId, 0);

        _txtName = new TextBox { Dock = DockStyle.Fill, Font = AppTheme.BodyFont, MaxLength = ValidationHelper.Limits.NameMax };
        AddRow("课程名称：", _txtName, 1);

        _numCredits = new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 1, Minimum = 0, Maximum = 20, Font = AppTheme.BodyFont };
        AddRow("学分：", _numCredits, 2);

        _txtTeacher = new TextBox { Dock = DockStyle.Fill, Font = AppTheme.BodyFont, MaxLength = ValidationHelper.Limits.TeacherMax };
        AddRow("授课教师：", _txtTeacher, 3);

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

        if (existing != null)
        {
            _txtCourseId.Text = existing.CourseId;
            _txtName.Text = existing.Name;
            _numCredits.Value = (decimal)existing.Credits;
            _txtTeacher.Text = existing.Teacher;
            _txtCourseId.ReadOnly = true;
        }
    }

    private bool ValidateInput()
    {
        var err = ValidationHelper.FirstError(
            ValidationHelper.ValidateCourseId(_txtCourseId.Text),
            ValidationHelper.ValidateCourseName(_txtName.Text),
            ValidationHelper.ValidateTeacher(_txtTeacher.Text)
        );
        if (err == null) return true;
        MessageBox.Show(err, "输入校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;

        var course = new Course
        {
            CourseId = _txtCourseId.Text.Trim(),
            Name = _txtName.Text.Trim(),
            Credits = (double)_numCredits.Value,
            Teacher = _txtTeacher.Text.Trim()
        };

        try
        {
            if (_existing == null)
                _db.InsertCourse(course);
            else
                _db.UpdateCourse(course);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            if (ex is SqliteException { SqliteErrorCode: 19 })
                MessageBox.Show("课程编号已存在，请检查后重新输入。", "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
