using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;

namespace FolderInspectorView
{
    internal class SortGlyphAdorner : Adorner
    {
        private GridViewColumnHeader _columnHeader;
        private ListSortDirection _direction;
        private ImageSource _sortGlyph;

        public SortGlyphAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph) : base(columnHeader)
        {
            _columnHeader = columnHeader;
            _direction = direction;
            _sortGlyph = sortGlyph;
            IsHitTestVisible = false;
        }

        private Geometry GetDefaultGlyph()
        {
            double x1 = _columnHeader.ActualWidth - 13;
            double x2 = x1 + 10;
            double x3 = x1 + 5;
            double y1 = _columnHeader.ActualHeight / 2 - 3;
            double y2 = y1 + 5;

            if (_direction == ListSortDirection.Ascending)
            {
                double tempNumber = y1;
                y1 = y2;
                y2 = tempNumber;
            }

            PathSegmentCollection pathSegmentCollection = new PathSegmentCollection();
            pathSegmentCollection.Add(new LineSegment(new Point(x2, y1), true));
            pathSegmentCollection.Add(new LineSegment(new Point(x3, y2), true));
            PathFigure pathFigure = new PathFigure(new Point(x1, y1), pathSegmentCollection, true);
            PathFigureCollection pathFigureCollection = new PathFigureCollection();
            pathFigureCollection.Add(pathFigure);

            return new PathGeometry(pathFigureCollection);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_sortGlyph != null)
            {
                double x = _columnHeader.ActualWidth - 13;
                double y = _columnHeader.ActualHeight / 2 - 5;
                Rect rect = new Rect(x, y, 10, 10);
                drawingContext.DrawImage(_sortGlyph, rect);
            }
            else
            {
                drawingContext.DrawGeometry(new SolidColorBrush(Colors.LightGray) { Opacity = 0.5 }, new Pen(Brushes.Gray, 0.5), GetDefaultGlyph());
            }
        }
    }
}