using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace MyCalendarApp
{
    public partial class Form1 : Form
    {
        private DateTime currentViewDate = DateTime.Now;
        private TableLayoutPanel calendarGrid;
        private Label monthYearLabel;
        private Dictionary<string, string> memoData = new Dictionary<string, string>();
        private string filePath = "calendar_data.json";
        private int expandedCol = -1; // 拡張する列（曜日）のインデックス

        public Form1()
        {
            this.Text = "C# メモカレンダー";
            this.Size = new Size(900, 700);
            LoadData();
            InitializeLayout();
            RenderCalendar();
        }

        private void InitializeLayout()
        {
            var rootPanel = new DockLayoutPanel(); // 全体レイアウト
            
            // ヘッダー（月移動）
            var header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(45, 45, 48) };
            monthYearLabel = new Label { ForeColor = Color.White, Font = new Font("Arial", 16, FontStyle.Bold), AutoSize = true, Location = new Point(350, 15) };
            
            var btnPrev = new Button { Text = "◀ 前月", Location = new Point(250, 15), FlatStyle = FlatStyle.Flat, ForeColor = Color.White };
            btnPrev.Click += (s, e) => { ChangeMonth(-1); };
            
            var btnNext = new Button { Text = "次月 ▶", Location = new Point(550, 15), FlatStyle = FlatStyle.Flat, ForeColor = Color.White };
            btnNext.Click += (s, e) => { ChangeMonth(1); };

            header.Controls.Add(monthYearLabel);
            header.Controls.Add(btnPrev);
            header.Controls.Add(btnNext);

            // カレンダーグリッド
            calendarGrid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 7, RowCount = 7 };
            for (int i = 0; i < 7; i++) calendarGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28f));
            
            this.Controls.Add(calendarGrid);
            this.Controls.Add(header);
        }

        private void RenderCalendar()
        {
            calendarGrid.Controls.Clear();
            calendarGrid.RowStyles.Clear();
            calendarGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // 曜日ヘッダー行

            monthYearLabel.Text = $"{currentViewDate.Year}年 {currentViewDate.Month}月";

            // 曜日ヘッダー作成
            string[] weekDays = { "日", "月", "火", "水", "木", "金", "土" };
            for (int i = 0; i < 7; i++)
            {
                var lbl = new Label { Text = weekDays[i], Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Arial", 10, FontStyle.Bold) };
                if (i == 0) lbl.ForeColor = Color.Red;
                if (i == 6) lbl.ForeColor = Color.Blue;
                calendarGrid.Controls.Add(lbl, i, 0);
            }

            DateTime firstDay = new DateTime(currentViewDate.Year, currentViewDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(currentViewDate.Year, currentViewDate.Month);
            int startOffset = (int)firstDay.DayOfWeek;

            int row = 1;
            for (int i = 0; i < 42; i++)
            {
                int dayNum = i - startOffset + 1;
                int col = i % 7;
                if (col == 0 && i > 0) row++;
                if (row > 6) break;

                var cell = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
                
                // 列拡張の背景色
                if (col == expandedCol) cell.BackColor = Color.LightYellow;

                if (dayNum > 0 && dayNum <= daysInMonth)
                {
                    string dateKey = $"{currentViewDate.Year}-{currentViewDate.Month}-{dayNum}";

                    // 数字ラベル（クリックで列拡張）
                    var lbl = new Label { Text = dayNum.ToString(), Dock = DockStyle.Top, Cursor = Cursors.Hand, Font = new Font("Arial", 9, FontStyle.Bold) };
                    if (col == 0) lbl.ForeColor = Color.Red;
                    if (col == 6) lbl.ForeColor = Color.Blue;
                    lbl.Click += (s, e) => { ToggleColumn(col); };

                    // メモ入力欄
                    var txt = new TextBox { Multiline = true, Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, Font = new Font("MS UI Gothic", 9) };
                    txt.Text = memoData.ContainsKey(dateKey) ? memoData[dateKey] : "";
                    txt.TextChanged += (s, e) => { memoData[dateKey] = txt.Text; SaveData(); };

                    cell.Controls.Add(txt);
                    cell.Controls.Add(lbl);
                }
                calendarGrid.Controls.Add(cell, col, row);
            }

            // 行の高さ調整（拡張時）
            for (int r = 1; r <= 6; r++) calendarGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6f));
        }

        private void ToggleColumn(int colIndex)
        {
            expandedCol = (expandedCol == colIndex) ? -1 : colIndex;
            // 列の幅を動的に変更
            calendarGrid.ColumnStyles.Clear();
            for (int i = 0; i < 7; i++)
            {
                float width = (expandedCol == -1) ? 14.28f : (i == expandedCol ? 40f : 10f);
                calendarGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, width));
            }
            RenderCalendar();
        }

        private void ChangeMonth(int diff) { currentViewDate = currentViewDate.AddMonths(diff); expandedCol = -1; RenderCalendar(); }

        private void SaveData() { File.WriteAllText(filePath, JsonSerializer.Serialize(memoData)); }
        private void LoadData() { if (File.Exists(filePath)) memoData = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(filePath)); }
    }

    // レイアウト調整用ヘルパー
    public class DockLayoutPanel : Panel { public DockLayoutPanel() { this.Dock = DockStyle.Fill; } }
}
