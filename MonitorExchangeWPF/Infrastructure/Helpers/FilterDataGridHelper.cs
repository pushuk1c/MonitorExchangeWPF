using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MonitorExchangeWPF.Infrastructure.Helpers
{
    public static class FilterDataGridHelper
    {
        public static Dictionary<string, string> GetFilterValuesFromDataGrid(DataGrid dataGrid)
        {
            var filters = new Dictionary<string, string>();

            foreach (var header in FindVisualChildren<DataGridColumnHeader>(dataGrid))
            {
                foreach (var tb in FindVisualChildren<TextBox>(header))
                {
                    if (tb.Tag is string tag && tag.Contains("filter") && !string.IsNullOrWhiteSpace(tb.Text))
                    {
                        tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                        filters[tag.Replace("filter", "")] = tb.Text;
                    }
                }

                foreach (var cb in FindVisualChildren<CheckBox>(header))
                {
                    if (cb.Tag is string tag && tag.Contains("filter") && cb.IsChecked is not null)
                    {
                        filters[tag.Replace("filter", "")] = cb.IsChecked.HasValue ? cb.IsChecked.Value.ToString() : string.Empty;
                    }
                }

                foreach (var dp in FindVisualChildren<DatePicker>(header))
                {
                    if (dp.Tag is string tag && tag.Contains("filter"))
                    {
                        filters[tag.Replace("filter", "")] = dp.SelectedDate?.ToString("yyyy-MM-dd") ?? string.Empty;
                    }
                }

                foreach (var combo in FindVisualChildren<ComboBox>(header))
                {
                    if (combo.Tag is string tag && tag.Contains("filter"))
                    {
                        filters[tag.Replace("filter", "")] = combo.SelectedValue?.ToString() ?? string.Empty;
                    }
                }
            }

            return filters;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
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

}
