using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Qozvvdivu7h1ow0
{
    public partial class Form1 : Form
    {
        private const int cellWidth = 60;
        private const int cellHeight = 30;

        private readonly TableLayoutPanel tableLayoutPanel1;
        private readonly Size halfCellSize = new Size(cellWidth / 2, cellHeight / 2);

        private Label label;
        private Point start;
        private Point offset;

        public Form1()
        {
            InitializeComponent();

            tableLayoutPanel1 = new TableLayoutPanel
            {
                ColumnCount = 25,
                Width = cellWidth * 24 + 1,
                Height = 20,
            };

            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 24 / 2));
            tableLayoutPanel1.Controls.Add(new Label());

            for (var i = 1; i < 24; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 24));
                var label = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = $"{i}",
                    TextAlign = ContentAlignment.MiddleCenter,
                };
                tableLayoutPanel1.Controls.Add(label);
            }

            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 24 / 2));

            splitContainer2.Panel2.Controls.Add(tableLayoutPanel1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var firsttime = (int)new TimeSpan(9, 0, 0).TotalMinutes;
            splitContainer1.Panel2.AutoScrollPosition = new Point(firsttime, 0);
        }

        private void SplitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            tableLayoutPanel1.Left = splitContainer1.Panel2.AutoScrollPosition.X;
        }

        private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (splitContainer2 != null)
                splitContainer2.SplitterDistance = splitContainer1.SplitterDistance;
        }

        private void SplitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (splitContainer1 != null)
                splitContainer1.SplitterDistance = splitContainer2.SplitterDistance;
        }


        private void PanelEx1_MouseDown(object sender, MouseEventArgs e)
        {
            start = panelEx1.PointToClient(Cursor.Position); // 同じ new Point(e.X, e.Y);
            offset = new Point(e.X % cellWidth, e.Y % cellHeight);
            Cursor = Cursors.SizeWE;

            var x = e.X / cellWidth * cellWidth;
            var y = e.Y / cellHeight * cellHeight;

            label = new Label
            {
                BackColor = Color.MistyRose,
                BorderStyle = BorderStyle.FixedSingle,
                ContextMenuStrip = contextMenuStrip1,
                Location = new Point(x, y),
                Size = new Size(cellWidth, cellHeight),
                Text = "テスト",
                TextAlign = ContentAlignment.MiddleCenter,
            };
            label.MouseDown += Label_MouseDown;
            label.MouseMove += Label_MouseMove;
            label.MouseUp += Label_MouseUp;

            panelEx1.Controls.Add(label);
        }
        private void PanelEx1_MouseMove(object sender, MouseEventArgs e)
        {
            if (label != null) TryResize(label);
        }
        private void PanelEx1_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
            if (label != null) toolTip1.Hide(label);
            label = null;
        }

        private void Label_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Label label)
            {
                // リサイズ時に反対の辺をstartに設定
                offset = new Point(e.X, e.Y);
                if (e.X < 8)
                {
                    start = label.Location + label.Size - halfCellSize;
                }
                if (label.Width - 8 < e.X)
                {
                    start = label.Location + halfCellSize;
                }
            }
        }
        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Label label)
            {
                if (e.Button == MouseButtons.None)
                {
                    if (e.X < 8 || label.Width - 8 < e.X) label.Cursor = Cursors.SizeWE;
                    else label.Cursor = Cursors.Hand;
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (label.Cursor == Cursors.SizeWE) TryResize(label);
                    if (label.Cursor == Cursors.Hand) TryMove(label);
                }
            }
        }
        private void Label_MouseUp(object sender, MouseEventArgs e)
        {
            toolTip1.Hide((Label)sender);
        }

        private void 削除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Owner is ContextMenuStrip contextMenu)
            {
                if (contextMenu.SourceControl is Label label)
                {
                    label.MouseDown -= Label_MouseDown;
                    label.MouseMove -= Label_MouseMove;
                    label.MouseUp -= Label_MouseUp;
                    panelEx1.Controls.Remove(label);
                }
            }
        }


        private void TryMove(Label target)
        {
            var p = panelEx1.PointToClient(Cursor.Position);
            var x = (p.X - offset.X + cellWidth / 2) / cellWidth * cellWidth;
            var y = (p.Y - offset.Y + cellHeight / 2) / cellHeight * cellHeight;

            var rect = target.Bounds;
            rect.X = x;
            rect.Y = y;
            TrySetBounds(target, rect);
        }
        private void TryResize(Label target)
        {
            var end = panelEx1.PointToClient(Cursor.Position);
            var s = start.X / cellWidth * cellWidth;
            var e = end.X / cellWidth * cellWidth;
            var rect = target.Bounds;

            if (start.X < end.X)
            {
                rect.X = s; // 本来不要なはずだが謎挙動に悩まされた...
                rect.Width = e - s + cellWidth;
            }
            else
            {
                rect.X = e;
                rect.Width = s - e + cellWidth;
            }
            TrySetBounds(target, rect);
        }
        private void TrySetBounds(Label target, Rectangle rect)
        {
            if (target.Bounds == rect) return; // ToolTipがちらつくし無駄なので

            // panel2の中で自分以外と交差するものがあったら動かさない（＝重ねられない）
            foreach (var label in panelEx1.Controls.OfType<Label>())
            {
                if (label == target) continue;
                if (label.Bounds.IntersectsWith(rect)) return;
            }
            target.Bounds = rect;

            var t = $"Location: {target.Location}\nSize: {target.Size}";
            toolTip1.Show(t, target, 0, -50);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var m = (int)DateTime.Now.TimeOfDay.TotalMinutes;
            var p = new Point(m, -splitContainer1.Panel2.AutoScrollPosition.Y);
            splitContainer1.Panel2.AutoScrollPosition = p;
        }
    }
}
