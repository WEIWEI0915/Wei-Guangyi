using System.Windows.Forms.DataVisualization.Charting;
using StudentGradeManager.Interfaces;
using StudentGradeManager.Theme;

namespace StudentGradeManager.Controls;

public partial class StatisticsControl : UserControl
{
    private readonly IGradeRepository _repo;
    private DataGridView _dgvStats = null!;
    private Chart _chart = null!;
    private Label _emptyHint = null!;

    public StatisticsControl(IGradeRepository repo)
    {
        _repo = repo;
        BuildUI();
        LoadData();
    }

    private void BuildUI()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };

        var headerPanel = new FlowLayoutPanel { Padding = new Padding(5, 8, 5, 8) };
        headerPanel.Controls.Add(AppTheme.CreateHeader("统计报表"));
        var btnExport = AppTheme.CreateButton("导出到 Excel", AppTheme.Success);
        headerPanel.Controls.Add(btnExport);
        layout.Controls.Add(headerPanel, 0, 0);
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
        split.SplitterDistance = 260;

        _chart = new Chart { Dock = DockStyle.Fill };
        _chart.ChartAreas.Add(new ChartArea("Default"));
        var ca = _chart.ChartAreas[0];
        ca.AxisX.Title = "分数段";
        ca.AxisX.TitleFont = AppTheme.BodyFont;
        ca.AxisY.Title = "人数";
        ca.AxisY.TitleFont = AppTheme.BodyFont;
        ca.AxisX.LabelStyle.Font = AppTheme.BodyFont;
        ca.AxisY.LabelStyle.Font = AppTheme.BodyFont;
        ca.BackColor = Color.Transparent;
        _chart.Titles.Add(new Title("成绩分布", Docking.Top, AppTheme.TitleFont, AppTheme.TextPrimary));

        _emptyHint = new Label
        {
            Text = "暂无成绩数据\n录入成绩后将自动生成统计图表",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.TextSecondary,
            Dock = DockStyle.Fill,
            Visible = false,
            BackColor = AppTheme.CardBg,
        };

        var chartContainer = new Panel { Dock = DockStyle.Fill };
        chartContainer.Controls.Add(_chart);
        chartContainer.Controls.Add(_emptyHint);
        _emptyHint.BringToFront();
        split.Panel1.Controls.Add(chartContainer);

        _dgvStats = new DataGridView();
        AppTheme.StyleDataGridView(_dgvStats);
        split.Panel2.Controls.Add(_dgvStats);

        layout.Controls.Add(split, 0, 1);
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(layout);

        btnExport.Click += (_, _) => ExportToExcel();
    }

    public void LoadData()
    {
        try
        {
            var stats = _repo.GetGradeStatsByCourse();
            _dgvStats.DataSource = null;
            _dgvStats.DataSource = stats.Select(s => new
            {
                课程编号 = s.CourseId,
                课程名称 = s.CourseName,
                人数 = s.StudentCount,
                平均分 = s.AvgScore,
                最低分 = s.MinScore,
                最高分 = s.MaxScore
            }).ToList();

            _chart.Series.Clear();
            var dist = _repo.GetGradeDistribution();
            bool hasData = dist.Values.Any(v => v > 0);

            _emptyHint.Visible = !hasData;

            if (hasData)
            {
                var series = new Series("人数")
                {
                    ChartType = SeriesChartType.Column,
                    IsValueShownAsLabel = true,
                    Font = AppTheme.BodyFont,
                    LabelForeColor = AppTheme.TextSecondary,
                };

                int i = 0;
                foreach (var kv in dist)
                {
                    int idx = series.Points.AddXY(kv.Key, kv.Value);
                    series.Points[idx].Color = AppTheme.ChartColors[i % AppTheme.ChartColors.Length];
                    series.Points[idx].Label = kv.Value.ToString();
                    i++;
                }
                _chart.Series.Add(series);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"加载统计数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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
            var grades = _repo.GetAllGrades();
            if (grades.Count == 0)
            {
                MessageBox.Show("没有成绩数据，无法导出。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("成绩统计");

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
            MessageBox.Show($"导出成功！共 {grades.Count} 条记录。", "导出完成",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (IOException)
        {
            MessageBox.Show("导出失败：文件被占用，请关闭已打开的 Excel 文件后重试。", "导出失败",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show("导出失败：没有写入权限，请选择其他位置保存。", "导出失败",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"导出失败：{ex.Message}", "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
