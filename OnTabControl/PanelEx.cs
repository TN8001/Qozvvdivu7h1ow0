using System.Drawing;
using System.Windows.Forms;

namespace OnTabControl
{
    public class PanelEx : Panel // グリッド線付きPanel
    {
        public int CellWidth { get; set; } = 50;
        public int CellHeight { get; set; } = 50;

        public PanelEx() => DoubleBuffered = true;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var right = ClientRectangle.Right;
            var bottom = ClientRectangle.Bottom;

            for (var x = 0; x < right; x += CellWidth)
            {
                e.Graphics.DrawLine(Pens.Gray, x, 0, x, bottom);
            }
            for (var y = 0; y < bottom; y += CellHeight)
            {
                e.Graphics.DrawLine(Pens.Gray, 0, y, right, y);
            }
        }
    }
}
