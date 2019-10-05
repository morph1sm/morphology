using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Morphology
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly InstalledMorphs morphs = new InstalledMorphs();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string region = (sender as ListBox).SelectedItem.ToString();

            DataContext = from morph in morphs
                          where morph.Region == region
                          select morph;
        }
        // Handlers for the drag/drop events.  
        private void EhDragEnter(object sender, DragEventArgs args)
        {
            Console.Write(EventFireStrings.DragEnter, args);
        }

        private void EhDragLeave(object sender, DragEventArgs args)
        {
            Console.Write(EventFireStrings.DragLeave, args);
        }

        private void EhDragOver(object sender, DragEventArgs args)
        {
            Console.Write(EventFireStrings.DragOver, args);
        }

        
        private void EhPreviewDragEnter(object sender, DragEventArgs args)
        {
            Console.Write(EventFireStrings.PreviewDragEnter, args);
        }

        private void EhPreviewDragLeave(object sender, DragEventArgs args)
        {
            Console.Write(EventFireStrings.PreviewDragLeave, args);
        }

        private void EhPreviewDragOver(object sender, DragEventArgs args)
        {
            Console.Write(EventFireStrings.PreviewDragOver, args);
        }

        private void EhPreviewDrop(object sender, DragEventArgs args)
        {
            Console.Write(EventFireStrings.PreviewDrop, args);
        }
        private struct EventFireStrings
        {
            public static readonly string DragEnter = "]: The DragEnter event just fired.\n";
            public static readonly string DragLeave = "]: The DragLeave event just fired.\n";
            public static readonly string DragOver = "]: The DragOver event just fired.\n";
            public static readonly string Drop = "]: The Drop event just fired.\n";
            public static readonly string PreviewDragEnter = "]: The PreviewDragEnter event just fired.\n";
            public static readonly string PreviewDragLeave = "]: The PreviewDragLeave event just fired.\n";
            public static readonly string PreviewDragOver = "]: The PreviewDragOver event just fired.\n";
            public static readonly string PreviewDrop = "]: The PreviewDrop event just fired.\n";
        }



        ListBox dragSource = null;

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox source = (ListBox)sender;
            dragSource = source;
            DragDrop.DoDragDrop(source, source.SelectedItems, DragDropEffects.Move);
        }
        private void ListBox_Drop(object sender, DragEventArgs args)
        {
            Console.Write(EventFireStrings.Drop, args);
            
            ListBox destination = (ListBox)sender;

            if (dragSource != null && dragSource.Name == "MorphList")
            {
                Console.Write("dragged items", dragSource.SelectedItems);
            }
        }
    }
}
