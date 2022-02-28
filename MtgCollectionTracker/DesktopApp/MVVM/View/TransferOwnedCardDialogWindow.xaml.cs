using System.Collections.Generic;
using System.Windows;

using DesktopApp.MVVM.Model;

namespace DesktopApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for TransferOwnedCardDialogWindow.xaml
    /// </summary>
    public partial class TransferOwnedCardDialogWindow : Window
    {
        public int TransferCount { get; set; }
        public CardCollection DestinationCollection { get; set; }

        public TransferOwnedCardDialogWindow(List<CardCollection> collections, OwnedCardPrintAggregate selectedCard)
        {
            InitializeComponent();

            labelCardName.Content = selectedCard.CardName;
            labelSet.Content = selectedCard.SetName;
            comboBoxCount.ItemsSource = CreateCountComboBox(selectedCard.Count);
            comboBoxCollections.ItemsSource = collections;
        }

        private List<int> CreateCountComboBox(int count)
        {
            var countOptions = new List<int>();
            for (int i = 1; i <= count; i++)
            {
                countOptions.Add(i);
            }

            return countOptions;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            TransferCount = (int)comboBoxCount.SelectedItem;
            DestinationCollection = (CardCollection)comboBoxCollections.SelectedItem;

            DialogResult = true;
        }
    }
}
