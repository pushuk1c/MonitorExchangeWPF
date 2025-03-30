using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace MonitorExchangeWPF.Infrastructure.Helpers
{
    public static class FilterDataGridHelper
    {
        public static Dictionary<string, string> GetFiltersFromDataGrid(DataGrid dataGrid)
        {
            var filters = new Dictionary<string, string>();

            foreach (var column in dataGrid.Columns)
            {
                if (column is DataGridBoundColumn boundColumn && column.HeaderTemplate != null)
                {
                    var header = column.Header as string;
                    var headerTemplate = column.HeaderTemplate.LoadContent();

                    // Пошук всіх елементів з Tag усередині шаблону заголовка
                    var inputs = FindVisualChildren<FrameworkElement>(headerTemplate)
                        .Where(e => e.Tag is string);

                    foreach (var input in inputs)
                    {
                        var tag = input.Tag as string;
                        string? value = null;

                        switch (input)
                        {
                            case TextBox tb:
                                value = tb.Text;
                                break;
                            case CheckBox cb:
                                value = cb.IsChecked.HasValue ? cb.IsChecked.Value.ToString() : null;
                                break;
                            case ComboBox combo:
                                value = combo.SelectedValue?.ToString();
                                break;
                            case DatePicker dp:
                                value = dp.SelectedDate?.ToString("yyyy-MM-dd");
                                break;
                        }

                        if (!string.IsNullOrWhiteSpace(tag) && !string.IsNullOrWhiteSpace(value))
                        {
                            filters[tag] = value;
                        }
                    }
                }
            }

            return filters;
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t)
                    yield return t;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
    }

}
