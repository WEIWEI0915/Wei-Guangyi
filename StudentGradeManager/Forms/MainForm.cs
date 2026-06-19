using StudentGradeManager.Controls;
using StudentGradeManager.Interfaces;
using StudentGradeManager.Theme;

namespace StudentGradeManager.Forms;

public partial class MainForm : Form
{
    private readonly IDatabaseHelper _db;
    private readonly TabControl _tabControl;

    public MainForm(IDatabaseHelper db)
    {
        _db = db;
        Text = "学生成绩管理系统";
        Size = new Size(1200, 750);
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 600);
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        AppTheme.ApplyFormStyle(this);

        _tabControl = new TabControl { Dock = DockStyle.Fill, Font = AppTheme.BodyFont };
        _tabControl.TabPages.Add(BuildTab("学生管理", new StudentControl(_db)));
        _tabControl.TabPages.Add(BuildTab("课程管理", new CourseControl(_db)));
        _tabControl.TabPages.Add(BuildTab("成绩管理", new GradeControl(_db)));
        _tabControl.TabPages.Add(BuildTab("统计报表", new StatisticsControl(_db)));
        _tabControl.SelectedIndexChanged += (_, _) => RefreshCurrentTab();
        Controls.Add(_tabControl);

        // Status bar
        var statusStrip = new StatusStrip
        {
            BackColor = AppTheme.CardBg,
            SizingGrip = false,
            Font = AppTheme.SmallFont,
        };
        var statusLabel = new ToolStripStatusLabel(" 就绪 ")
        {
            ForeColor = AppTheme.TextSecondary,
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        statusStrip.Items.Add(statusLabel);
        Controls.Add(statusStrip);
    }

    private static TabPage BuildTab(string title, Control content)
    {
        content.Dock = DockStyle.Fill;
        return new TabPage(title) { Controls = { content }, UseVisualStyleBackColor = true };
    }

    private void RefreshCurrentTab()
    {
        if (_tabControl.SelectedTab?.Controls[0] is StudentControl sc) sc.LoadData();
        else if (_tabControl.SelectedTab?.Controls[0] is CourseControl cc) cc.LoadData();
        else if (_tabControl.SelectedTab?.Controls[0] is GradeControl gc) gc.LoadData();
        else if (_tabControl.SelectedTab?.Controls[0] is StatisticsControl stc) stc.LoadData();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        RefreshCurrentTab();
    }
}
