using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;

namespace FolderInspectorView
{
    public class GridViewSort
    {
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }
        
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(GridViewSort),
                new UIPropertyMetadata(
                    null,
                    (o, e) =>
                    {
                        ItemsControl listView = o as ItemsControl;

                        if (listView != null)
                        {
                            if (!GetAutoSort(listView))
                            {
                                if (e.OldValue != null && e.NewValue == null)
                                {
                                    listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }

                                if (e.OldValue == null && e.NewValue != null)
                                {
                                    listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }
                            }
                        }
                    }
                )
            );

        public static bool GetAutoSort(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSortProperty);
        }

        public static void SetAutoSort(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSortProperty, value);
        }
        
        public static readonly DependencyProperty AutoSortProperty =
            DependencyProperty.RegisterAttached(
                "AutoSort",
                typeof(bool),
                typeof(GridViewSort),
                new UIPropertyMetadata(
                    false,
                    (o, e) =>
                    {
                        ListView listView = o as ListView;

                        if (listView != null)
                        {
                            if (GetCommand(listView) == null)
                            {
                                bool oldValue = (bool)e.OldValue;
                                bool newValue = (bool)e.NewValue;

                                if (oldValue && !newValue)
                                {
                                    listView.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }

                                if (!oldValue && newValue)
                                {
                                    listView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                                }
                            }
                        }
                    }
                )
            );

        public static void SetPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }
        
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(GridViewSort), new UIPropertyMetadata(null));

        public static void SetShowSortGlyph(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowSortGlyphProperty, value);
        }
        
        public static readonly DependencyProperty ShowSortGlyphProperty =
            DependencyProperty.RegisterAttached("ShowSortGlyph", typeof(bool), typeof(GridViewSort), new UIPropertyMetadata(true));
        
        public static ImageSource GetSortGlyph(DependencyObject obj, ListSortDirection direction)
        {
            if (direction == ListSortDirection.Ascending)
            {
                return (ImageSource)obj.GetValue(SortGlyphAscendingProperty);
            }
            else
            {
                return (ImageSource)obj.GetValue(SortGlyphDescendingProperty);
            }
        }

        public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(SortGlyphAscendingProperty, value);
        }
        
        public static readonly DependencyProperty SortGlyphAscendingProperty =
            DependencyProperty.RegisterAttached("SortGlyphAscending", typeof(ImageSource), typeof(GridViewSort), new UIPropertyMetadata(null));

        public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(SortGlyphDescendingProperty, value);
        }
        
        public static readonly DependencyProperty SortGlyphDescendingProperty =
            DependencyProperty.RegisterAttached("SortGlyphDescending", typeof(ImageSource), typeof(GridViewSort), new UIPropertyMetadata(null));
        
        private static readonly DependencyProperty SortedColumnHeaderProperty =
            DependencyProperty.RegisterAttached("SortedColumnHeader", typeof(GridViewColumnHeader), typeof(GridViewSort), new UIPropertyMetadata(null));
        
        private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked != null && headerClicked.Column != null)
            {
                string propertyName = (string)headerClicked.Column.GetValue(PropertyNameProperty);

                if (!string.IsNullOrEmpty(propertyName))
                {
                    ListView listView = GetAncestor<ListView>(headerClicked);

                    if (listView != null)
                    {
                        ICommand command = GetCommand(listView);

                        if (command != null)
                        {
                            if (command.CanExecute(propertyName))
                            {
                                command.Execute(propertyName);
                            }
                        }
                        else if (GetAutoSort(listView))
                        {
                            ApplySort(listView.Items, propertyName, listView, headerClicked);
                        }
                    }
                }
            }
        }
        
        public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);

            while (!(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent != null)
            {
                return (T)parent;
            }
            else
            {
                return null;
            }
        }

        public static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader)
        {
            ListSortDirection direction = ListSortDirection.Ascending;

            if (view.SortDescriptions.Count > 0)
            {
                SortDescription currentSort = view.SortDescriptions[0];

                if (currentSort.PropertyName == propertyName)
                {
                    if (currentSort.Direction == ListSortDirection.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                }
                else
                {
                    sortedColumnHeader.Column.Width = sortedColumnHeader.ActualWidth + 25;
                }

                view.SortDescriptions.Clear();
                GridViewColumnHeader currentSortedColumnHeader = (GridViewColumnHeader)listView.GetValue(SortedColumnHeaderProperty);

                if (currentSortedColumnHeader != null)
                {
                    RemoveSortGlyph(currentSortedColumnHeader);
                    
                    if (currentSort.PropertyName != propertyName)
                    {
                        currentSortedColumnHeader.Column.Width = double.NaN;
                    }
                }
            }
            else
            {
                sortedColumnHeader.Column.Width = sortedColumnHeader.ActualWidth + 25;
            }

            if (!string.IsNullOrEmpty(propertyName))
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, direction));

                if ((bool)listView.GetValue(ShowSortGlyphProperty))
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(sortedColumnHeader);
                    adornerLayer.Add(new SortGlyphAdorner(sortedColumnHeader, direction, GetSortGlyph(listView, direction)));
                }
                
                listView.SetValue(SortedColumnHeaderProperty, sortedColumnHeader);
            }
        }

        private static void RemoveSortGlyph(GridViewColumnHeader columnHeader)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            Adorner[] adorners = adornerLayer.GetAdorners(columnHeader);

            if (adorners != null)
            {
                foreach (Adorner adorner in adorners)
                {
                    if (adorner is SortGlyphAdorner)
                    {
                        adornerLayer.Remove(adorner);
                    }
                }
            }
        }
    }
}