using StudentGradeManager.Interfaces;
using StudentGradeManager.Models;
using StudentGradeManager.Theme;

namespace StudentGradeManager.Controls;

public partial class StudentControl : UserControl
{
    private readonly IStudentRepository _repo;
    private DataGridView _dgv = null!;
    private TextBox _txtSearch = null!;
    private Label _emptyHint = null!;

    public StudentControl(IStudentRepository repo)
    {
        _repo = repo;
        BuildUI();
        LoadData();
    }

    private void BuildUI()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };

        var topPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(5, 8, 5, 8) };
        topPanel.Controls.Add(AppTheme.CreateHeader("学生管理"));
        _txtSearch = AppTheme.CreateSearchBox("搜索学号 / 姓名 / 班级...");
        var btnSearch = AppTheme.CreateButton("搜索", AppTheme.Secondary);
        var btnAdd = AppTheme.CreateButton("添加学生", AppTheme.Primary);
        var btnEdit = AppTheme.CreateButton("编辑", AppTheme.Primary);
        var btnDelete = AppTheme.CreateButton("删除", AppTheme.Danger);
        topPanel.Controls.AddRange(new Control[] { _txtSearch, btnSearch, btnAdd, btnEdit, btnDelete });

        _dgv = new DataGridView();
        AppTheme.StyleDataGridView(_dgv);

        _emptyHint = new Label
        {
            Text = "暂无学生数据\n点击「添加学生」开始录入",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.TextSecondary,
            Dock = DockStyle.Fill,
            Visible = false,
            BackColor = AppTheme.CardBg,
        };

        // Panel holds both grid (bottom) and hint (top overlay)
        var gridContainer = new Panel { Dock = DockStyle.Fill };
        gridContainer.Controls.Add(_dgv);
        gridContainer.Controls.Add(_emptyHint);
        _emptyHint.BringToFront();

        layout.Controls.Add(topPanel, 0, 0);
        layout.Controls.Add(gridContainer, 0, 1);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(layout);

        btnSearch.Click += (_, _) => LoadData();
        btnAdd.Click += (_, _) => OpenDetail(null);
        btnEdit.Click += (_, _) => EditSelected();
        btnDelete.Click += (_, _) => DeleteSelected();
        _txtSearch.KeyPress += (_, e) => { if (e.KeyChar == 13) LoadData(); };
    }

    public void LoadData()
    {
        var kw = _txtSearch.Text.Trim();

        var searchErr = ValidationHelper.ValidateSearchInput(kw);
        if (searchErr != null)
        {
            MessageBox.Show(searchErr, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var data = string.IsNullOrEmpty(kw) ? _repo.GetAllStudents() : _repo.SearchStudents(kw);
            _dgv.DataSource = null;
            _dgv.DataSource = data;

            // 空提示覆盖层：有数据时隐藏，无数据时显示
            _emptyHint.Visible = data.Count == 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"加载数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenDetail(Student? existing)
    {
        using var form = new Forms.StudentDetailForm((Data.DatabaseHelper)_repo, existing);
        form.ShowDialog();
        LoadData();
    }

    private void EditSelected()
    {
        if (_dgv.CurrentRow?.DataBoundItem is Student s) OpenDetail(s);
    }

    private void DeleteSelected()
    {
        if (_dgv.CurrentRow?.DataBoundItem is not Student s) return;
        if (MessageBox.Show($"确认删除学生 {s.Name}({s.StudentId})？\n相关成绩将一并删除。",
            "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        try
        {
            _repo.DeleteStudent(s.StudentId);
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"删除失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
