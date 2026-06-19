using StudentGradeManager.Data;
using StudentGradeManager.Models;

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
        Size = new Size(400, 240);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };

        layout.Controls.Add(new Label { Text = "课程编号：", TextAlign = ContentAlignment.MiddleRight }, 0, 0);
        _txtCourseId = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_txtCourseId, 1, 0);

        layout.Controls.Add(new Label { Text = "课程名称：", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
        _txtName = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_txtName, 1, 1);

        layout.Controls.Add(new Label { Text = "学分：", TextAlign = ContentAlignment.MiddleRight }, 0, 2);
        _numCredits = new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 1, Minimum = 0, Maximum = 20 };
        layout.Controls.Add(_numCredits, 1, 2);

        layout.Controls.Add(new Label { Text = "授课教师：", TextAlign = ContentAlignment.MiddleRight }, 0, 3);
        _txtTeacher = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_txtTeacher, 1, 3);

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

        if (existing != null)
        {
            _txtCourseId.Text = existing.CourseId;
            _txtName.Text = existing.Name;
            _numCredits.Value = (decimal)existing.Credits;
            _txtTeacher.Text = existing.Teacher;
            _txtCourseId.ReadOnly = true;
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtCourseId.Text) || string.IsNullOrWhiteSpace(_txtName.Text))
        {
            MessageBox.Show("课程编号和名称为必填项。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

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
            MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
