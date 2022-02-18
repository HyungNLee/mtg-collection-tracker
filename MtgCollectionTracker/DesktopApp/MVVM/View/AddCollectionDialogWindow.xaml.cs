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
using System.Windows.Shapes;

using DesktopApp.Event;
using DesktopApp.Event.EventModels;

namespace DesktopApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for AddCollectionDialogWindow.xaml
    /// </summary>
    public partial class AddCollectionDialogWindow : Window
    {
        public AddCollectionDialogWindow()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            var collectionName = txtCollectionName.Text;
            var isDeck = chkIsDeck.IsChecked ?? false;

            var request = new CreateCollectionRequestEvent(collectionName, isDeck);

            ApplicationEventManager.Instance.Publish(request);

            DialogResult = true;
        }
    }
}
