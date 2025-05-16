using System;
using System.Collections;
using System.Windows.Forms;

namespace MusicPlaylistBuilder
{
    public class GenericComparer : IComparer
    {
        private int columnIndex;
        private SortOrder sortOrder;

        public GenericComparer(int columnIndex, SortOrder sortOrder)
        {
            this.columnIndex = columnIndex;
            this.sortOrder = sortOrder;
        }

        public int Compare(object x, object y)
        {
            ListViewItem item1 = x as ListViewItem;
            ListViewItem item2 = y as ListViewItem;

            // Récupérez les valeurs des colonnes
            string value1 = item1.SubItems[columnIndex].Text;
            string value2 = item2.SubItems[columnIndex].Text;

            int result;

            // Essayez de comparer en tant que nombre
            if (int.TryParse(value1, out int int1) && int.TryParse(value2, out int int2))
            {
                result = int1.CompareTo(int2);
            }
            // Essayez de comparer en tant que date
            else if (DateTime.TryParse(value1, out DateTime date1) && DateTime.TryParse(value2, out DateTime date2))
            {
                result = date1.CompareTo(date2);
            }
            // Sinon, comparez en tant que chaîne
            else
            {
                result = string.Compare(value1, value2, StringComparison.OrdinalIgnoreCase);
            }

            // Inversez le résultat si l'ordre est descendant
            return (sortOrder == SortOrder.Descending) ? -result : result;
        }
    }
}
