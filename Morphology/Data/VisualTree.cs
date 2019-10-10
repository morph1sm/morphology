using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Morphology.Data
{
    //Helper Class for the VisualTreeHelper for XAML Items
    public static class VisualTree
    {
        public static object GetParent(object child, Type parentType)
        {
            DependencyObject Item = child as DependencyObject;

            while (Item != null)
            {
                if (Item.GetType() == parentType)

                    return Item;

                Item = VisualTreeHelper.GetParent(Item);
            }
            return null;
        }
    }
}
