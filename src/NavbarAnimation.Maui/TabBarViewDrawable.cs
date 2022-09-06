using Microsoft.Maui.Controls.Shapes;

namespace NavbarAnimation.Maui
{
    public record TabContent(PathGeometry TitleGeometry, PathGeometry IconGeometry);

    public class TabBarViewDrawable : IDrawable
    {
        private Color tabBarColor;
        private SolidPaint tabBarPaint;
        private IList<TabContent> tabContents;
        private IList<PathF> titlePaths;
        private IList<PathF> iconPaths;
        private readonly float margin = 10;
        private readonly float titleTopMargin = 8;
        private readonly float titleHeight = 15;
        private readonly float thumbHeight = 10;

        public IList<TabContent> TabContents
        {
            get => tabContents;
            private set
            {
                tabContents = value;
                titlePaths = TabContents.Select(c => c.TitleGeometry.ParseToPathF()).ToList();
                iconPaths = TabContents.Select(c => c.IconGeometry.ParseToPathF()).ToList();
            }
        }
        public float IconHeight { private get; set; } = -1;
        public Color SelectionColor { private get; set; }
        public Color DefaultColor { private get; set; }
        public Color TabBarColor
        {
            private get => tabBarColor;
            set
            {
                tabBarColor = value;
                tabBarPaint = new SolidPaint(value);
            }
        }
        public float SelectedRectPosition { get; set; }


        public TabBarViewDrawable(IEnumerable<TabContent> tabContents, float margin)
        {
            TabContents = tabContents.ToList();
            this.margin = margin;
        }


        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            float width = dirtyRect.Width - (2 * margin);
            float selectedRectWidth = width / TabContents.Count;
            float selectedRectPosition = margin + (SelectedRectPosition * selectedRectWidth);
            var rect = new RectF(margin, dirtyRect.Y, width, dirtyRect.Height);
            float left = (selectedRectPosition - margin) / width;
            float right = (selectedRectPosition - margin + selectedRectWidth) / width;
            
            var linearGradientPaint = new LinearGradientPaint
            {
                StartColor = left > 0 ? DefaultColor : SelectionColor,
                EndColor = right < 1 ? DefaultColor : SelectionColor,
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };

            linearGradientPaint.AddOffset(left, DefaultColor);
            linearGradientPaint.AddOffset(left, SelectionColor);
            linearGradientPaint.AddOffset(right, SelectionColor);
            linearGradientPaint.AddOffset(right, DefaultColor);

            canvas.SetFillPaint(linearGradientPaint, rect);

            float pathHeight = IconHeight == -1 ? rect.Height : IconHeight;

            for (int i = 0; i < TabContents.Count; i++)
            {
                var iconPath = iconPaths[i].AsUniformScaledPath(new RectF(0, 0, selectedRectWidth, pathHeight));
                iconPath.CenterMove(new RectF(0, 0, selectedRectWidth, rect.Height));
                iconPath.Move((i * selectedRectWidth) + margin, 0);

                var titlePath = titlePaths[i].AsUniformScaledPath(new RectF(0, 0, selectedRectWidth, titleHeight));
                titlePath.CenterMove(new RectF(0, 0, selectedRectWidth, titleHeight));
                titlePath.Move((i * selectedRectWidth) + margin, titleTopMargin + IconHeight + (rect.Height - IconHeight) / 2);

                canvas.FillPath(iconPath);
                canvas.SetFillPaint(linearGradientPaint, rect);
                canvas.FillRectangle(GetTitleRect(titlePath));
                canvas.SetFillPaint(tabBarPaint, rect);
                canvas.FillPath(titlePath);
                canvas.SetFillPaint(linearGradientPaint, rect);
            }

            var thumbPath = new PathF()
                .MoveTo(selectedRectPosition, 0)
                .LineTo(selectedRectPosition + selectedRectWidth, 0)
                .QuadTo(selectedRectPosition + selectedRectWidth, thumbHeight, selectedRectPosition + selectedRectWidth - thumbHeight, thumbHeight)
                .LineTo(selectedRectPosition + thumbHeight, thumbHeight)
                .QuadTo(selectedRectPosition, thumbHeight, selectedRectPosition, 0);
            thumbPath.Close();
            canvas.SetFillPaint(linearGradientPaint, rect);
            canvas.FillPath(thumbPath);

            canvas.RestoreState();
        }

        private RectF GetTitleRect(PathF path)
        {
            float minX = path.Points.Min(p => p.X);
            float maxX = path.Points.Max(p => p.X);
            float minY = path.Points.Min(p => p.Y);
            float maxY = path.Points.Max(p => p.Y);

            float shrink = 0.75f;

            return new RectF(minX + shrink, minY + shrink, maxX - minX - (2 * shrink), maxY - minY - (2 * shrink));
        }
    }
}
