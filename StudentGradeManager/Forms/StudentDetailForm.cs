using Microsoft.Data.Sqlite;
using StudentGradeManager.Data;
using StudentGradeManager.Models;
using StudentGradeManager.Theme;

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
        Size = new Size(420, 300);
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
            layout.Controls.Add(MakeLabel(label), 0, row);
            layout.Controls.Add(ctrl, 1, row);
        }

        _txtStudentId = new TextBox { Dock = DockStyle.Fill, Font = AppTheme.BodyFont, MaxLength = ValidationHelper.Limits.StudentIdMax };
        AddRow("学号：", _txtStudentId, 0);

        _txtName = new TextBox { Dock = DockStyle.Fill, Font = AppTheme.BodyFont, MaxLength = ValidationHelper.Limits.NameMax };
        AddRow("姓名：", _txtName, 1);

        _cmbGender = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.BodyFont };
        _cmbGender.Items.AddRange(["男", "女"]);
        AddRow("性别：", _cmbGender, 2);

        _txtClass = new TextBox { Dock = DockStyle.Fill, Font = AppTheme.BodyFont, MaxLength = ValidationHelper.Limits.ClassNameMax };
        AddRow("班级：", _txtClass, 3);

        _dtpEnrollment = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Font = AppTheme.BodyFont, MaxDate = DateTime.Today };
        AddRow("入学日期：", _dtpEnrollment, 4);

        var btnPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Margin = new Padding(0, 8, 0, 0) };
        var btnCancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, Font = AppTheme.BodyFont, AutoSize = true, Padding = new Padding(12, 4, 12, 4) };
        var btnSave = AppTheme.CreateButton("保存");
        btnPanel.Controls.AddRange(new Control[] { btnCancel, btnSave });
        layout.Controls.Add(btnPanel, 0, 5);
        layout.SetColumnSpan(btnPanel, 2);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

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

    private static Label MakeLabel(string text) => new()
    {
        Text = text,
        Font = AppTheme.BodyFont,
        ForeColor = AppTheme.TextPrimary,
        TextAlign = ContentAlignment.MiddleRight,
        Dock = DockStyle.Fill,
    };

    private bool ValidateInput()
    {
        var err = ValidationHelper.FirstError(
            ValidationHelper.ValidateStudentId(_txtStudentId.Text),
            ValidationHelper.ValidateName(_txtName.Text),
            ValidationHelper.ValidateClassName(_txtClass.Text)
        );
        if (err == null) return true;
        MessageBox.Show(err, "输入校验", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (!ValidateInput()) return;

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
            if (ex is SqliteException { SqliteErrorCode: 19 })
                MessageBox.Show("学号已存在，请检查后重新输入。", "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
