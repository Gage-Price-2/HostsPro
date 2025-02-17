using HostsPro.Models;
using HostsPro.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HostsPro.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        //Lost Focus Event 
        private void RoutesTo_LostFocus(object sender, RoutedEventArgs e)
        {
            //check it is coming from the correct place
            if (DataContext is EntryViewModel vm && sender is TextBox textBox)
            {
                var entry = textBox.DataContext as HostEntryModel;
                if (entry != null)
                {
                    //Call the view model Method, where is will get the Ip address value and update it's observable value
                    vm.RoutesTo_LostFocus(entry);
                }
            }
        }
    }
}