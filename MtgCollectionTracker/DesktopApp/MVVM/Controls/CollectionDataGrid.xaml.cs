using System;
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

using DesktopApp.MVVM.View;

namespace DesktopApp.MVVM.Controls
{
    /// <summary>
    /// Interaction logic for CollectionDataGrid.xaml
    /// </summary>
    public partial class CollectionDataGrid : UserControl
    {
        public CollectionDataGrid()
        {
            InitializeComponent();
        }

        private void btnAddCollectionDialog_Click(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new AddCollectionDialogWindow();

            var result = dialogWindow.ShowDialog();
        }
    }
}
