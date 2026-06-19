using StudentGradeManager.Interfaces;
using StudentGradeManager.Models;
using StudentGradeManager.Theme;

namespace StudentGradeManager.Controls;

public partial class CourseControl : UserControl
{
    private readonly ICourseRepository _repo;
    private DataGridView _dgv = null!;
    private TextBox _txtSearch = null!;
    private Label _emptyHint = null!;

    public CourseControl(ICourseRepository repo)
    {
        _repo = repo;
        BuildUI();
        LoadData();
    }

    private void BuildUI()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };

        var topPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(5, 8, 5, 8) };
        topPanel.Controls.Add(AppTheme.CreateHeader("课程管理"));
        _txtSearch = AppTheme.CreateSearchBox("搜索课程编号 / 名称 / 教师...");
        var btnSearch = AppTheme.CreateButton("搜索", AppTheme.Secondary);
        var btnAdd = AppTheme.CreateButton("添加课程", AppTheme.Primary);
        var btnEdit = AppTheme.CreateButton("编辑", AppTheme.Primary);
        var btnDelete = AppTheme.CreateButton("删除", AppTheme.Danger);
        topPanel.Controls.AddRange(new Control[] { _txtSearch, btnSearch, btnAdd, btnEdit, btnDelete });

        _dgv = new DataGridView();
        AppTheme.StyleDataGridView(_dgv);

        _emptyHint = new Label
        {
            Text = "暂无课程数据\n点击「添加课程」开始录入",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.TextSecondary,
            Dock = DockStyle.Fill,
            Visible = false,
            BackColor = AppTheme.CardBg,
        };

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
            var data = string.IsNullOrEmpty(kw) ? _repo.GetAllCourses() : _repo.SearchCourses(kw);
            _dgv.DataSource = null;
            _dgv.DataSource = data;
            _emptyHint.Visible = data.Count == 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"加载数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenDetail(Course? existing)
    {
        using var form = new Forms.CourseDetailForm((Data.DatabaseHelper)_repo, existing);
        form.ShowDialog();
        LoadData();
    }

    private void EditSelected()
    {
        if (_dgv.CurrentRow?.DataBoundItem is Course c) OpenDetail(c);
    }

    private void DeleteSelected()
    {
        if (_dgv.CurrentRow?.DataBoundItem is not Course c) return;
        if (MessageBox.Show($"确认删除课程 {c.Name}({c.CourseId})？\n相关成绩将一并删除。",
            "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        try
        {
            _repo.DeleteCourse(c.CourseId);
            LoadData();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"删除失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
