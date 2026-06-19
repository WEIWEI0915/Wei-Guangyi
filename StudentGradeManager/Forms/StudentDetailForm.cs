using StudentGradeManager.Data;
using StudentGradeManager.Models;

namespace StudentGradeManager.Forms;

public class StudentDetailForm : Form
{
    private readonly DatabaseHelper _db;
    private readonly Student? _existing;

    private readonly TextBox _txtStudentId, _txtName, _txtClass;
    private readonly ComboBox _cmbGender;
    private readonly DateTimePicker _dtpEnrollment;

    public StudentDetailForm(DatabaseHelper db, Student? existing)
    {
        _db = db;
        _existing = existing;
        Text = existing == null ? "添加学生" : "编辑学生";
        Size = new Size(400, 280);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };

        layout.Controls.Add(new Label { Text = "学号：", TextAlign = ContentAlignment.MiddleRight }, 0, 0);
        _txtStudentId = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_txtStudentId, 1, 0);

        layout.Controls.Add(new Label { Text = "姓名：", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
        _txtName = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_txtName, 1, 1);

        layout.Controls.Add(new Label { Text = "性别：", TextAlign = ContentAlignment.MiddleRight }, 0, 2);
        _cmbGender = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbGender.Items.AddRange(["男", "女"]);
        layout.Controls.Add(_cmbGender, 1, 2);

        layout.Controls.Add(new Label { Text = "班级：", TextAlign = ContentAlignment.MiddleRight }, 0, 3);
        _txtClass = new TextBox { Dock = DockStyle.Fill };
        layout.Controls.Add(_txtClass, 1, 3);

        layout.Controls.Add(new Label { Text = "入学日期：", TextAlign = ContentAlignment.MiddleRight }, 0, 4);
        _dtpEnrollment = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };
        layout.Controls.Add(_dtpEnrollment, 1, 4);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var btnCancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, AutoSize = true };
        var btnSave = new Button { Text = "保存", AutoSize = true };
        btnPanel.Controls.AddRange(new Control[] { btnCancel, btnSave });
        layout.Controls.Add(btnPanel, 0, 5);
        layout.SetColumnSpan(btnPanel, 2);

        Controls.Add(layout);
        CancelButton = btnCancel;
        AcceptButton = btnSave;
        btnSave.Click += BtnSave_Click;

        if (existing != null)
        {
            _txtStudentId.Text = existing.StudentId;
            _txtName.Text = existing.Name;
            _cmbGender.SelectedItem = existing.Gender;
            _txtClass.Text = existing.ClassName;
            if (DateTime.TryParse(existing.EnrollmentDate, out var dt))
                _dtpEnrollment.Value = dt;
            _txtStudentId.ReadOnly = true;
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtStudentId.Text) || string.IsNullOrWhiteSpace(_txtName.Text))
        {
            MessageBox.Show("学号和姓名为必填项。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var student = new Student
        {
            StudentId = _txtStudentId.Text.Trim(),
            Name = _txtName.Text.Trim(),
            Gender = _cmbGender.SelectedItem?.ToString() ?? "",
            ClassName = _txtClass.Text.Trim(),
            EnrollmentDate = _dtpEnrollment.Value.ToString("yyyy-MM-dd")
        };

        try
        {
            if (_existing == null)
                _db.InsertStudent(student);
            else
                _db.UpdateStudent(student);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
