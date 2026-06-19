using System.Drawing.Drawing2D;

namespace StudentGradeManager.Theme;

public static class AppTheme
{
    // ── Color Palette ──
    public static readonly Color Primary     = Color.FromArgb(37, 99, 235);   // Blue-600
    public static readonly Color PrimaryDark = Color.FromArgb(29, 78, 216);   // Blue-700
    public static readonly Color PrimaryLight = Color.FromArgb(219, 234, 254); // Blue-100
    public static readonly Color Secondary   = Color.FromArgb(71, 85, 105);   // Slate-600
    public static readonly Color Surface     = Color.FromArgb(248, 250, 252); // Slate-50
    public static readonly Color CardBg      = Color.White;
    public static readonly Color TextPrimary = Color.FromArgb(30, 41, 59);    // Slate-800
    public static readonly Color TextSecondary = Color.FromArgb(100, 116, 139); // Slate-500
    public static readonly Color Border      = Color.FromArgb(226, 232, 240); // Slate-200
    public static readonly Color GridAlt     = Color.FromArgb(241, 245, 249); // Slate-100
    public static readonly Color Danger      = Color.FromArgb(220, 38, 38);   // Red-600
    public static readonly Color Success     = Color.FromArgb(22, 163, 74);   // Green-600
    public static readonly Color WarningBg   = Color.FromArgb(254, 249, 195); // Amber-100

    // Chart color palette (distinct enough for segments)
    public static readonly Color[] ChartColors =
    [
        Color.FromArgb(37, 99, 235),    // Blue
        Color.FromArgb(6, 182, 212),    // Cyan
        Color.FromArgb(16, 185, 129),   // Emerald
        Color.FromArgb(245, 158, 11),   // Amber
        Color.FromArgb(239, 68, 68),    // Red
    ];

    // ── Fonts ──
    public static readonly Font TitleFont    = new("Segoe UI", 12, FontStyle.Bold);
    public static readonly Font HeaderFont   = new("Segoe UI", 10, FontStyle.Bold);
    public static readonly Font BodyFont     = new("Segoe UI", 9);
    public static readonly Font SmallFont    = new("Segoe UI", 8);

    // ── Styling Helpers ──

    public static void StyleDataGridView(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.BackgroundColor = CardBg;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.RowHeadersVisible = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;

        // Header
        grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = HeaderFont;
        grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 4, 0, 4);
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

        // Cells
        grid.DefaultCellStyle.Font = BodyFont;
        grid.DefaultCellStyle.ForeColor = TextPrimary;
        grid.DefaultCellStyle.BackColor = CardBg;
        grid.DefaultCellStyle.SelectionBackColor = PrimaryLight;
        grid.DefaultCellStyle.SelectionForeColor = PrimaryDark;
        grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
        grid.RowTemplate.Height = 32;

        // Alternating rows
        grid.AlternatingRowsDefaultCellStyle.BackColor = GridAlt;
        grid.AlternatingRowsDefaultCellStyle.ForeColor = TextPrimary;
    }

    public static Button CreateButton(string text, Color? backColor = null, int width = 0)
    {
        var btn = new Button
        {
            Text = text,
            Font = BodyFont,
            FlatStyle = FlatStyle.Flat,
            BackColor = backColor ?? Primary,
            ForeColor = Color.White,
            Cursor = Cursors.Hand,
            AutoSize = true,
            Padding = new Padding(12, 4, 12, 4),
            MinimumSize = new Size(width == 0 ? 80 : width, 32),
            TextAlign = ContentAlignment.MiddleCenter,
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(btn.BackColor);
        btn.FlatAppearance.MouseDownBackColor = PrimaryDark;
        return btn;
    }

    public static TextBox CreateSearchBox(string placeholder = "")
    {
        return new TextBox
        {
            Font = BodyFont,
            BackColor = CardBg,
            ForeColor = TextPrimary,
            Width = 200,
            PlaceholderText = placeholder,
            BorderStyle = BorderStyle.FixedSingle,
        };
    }

    public static Label CreateHeader(string text)
    {
        return new Label
        {
            Text = text,
            Font = TitleFont,
            ForeColor = TextPrimary,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(4, 0, 16, 0),
            MinimumSize = new Size(0, 32),
        };
    }

    public static void ApplyFormStyle(Form form)
    {
        form.BackColor = Surface;
        form.Font = BodyFont;
        form.ForeColor = TextPrimary;
    }
}
