using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Morphology
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Morphs morphs = new Morphs();
        private readonly Regions regions = new Regions();
        ListBox dragSource = null;
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
        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox source = (ListBox)sender;
            dragSource = source;
            DragDrop.DoDragDrop(source, source.SelectedItems, DragDropEffects.Move);
        }
        private void Region_DragEnter(object sender, DragEventArgs args)
        {
            Console.Write("Region enter");

        }
        private void Region_DragLeave(object sender, DragEventArgs args)
        {
            Console.Write("Region leave");

        }
        private void Region_Drop(object sender, DragEventArgs args)
        {
            Console.Write("Region received drop");

        }        
    }
}
