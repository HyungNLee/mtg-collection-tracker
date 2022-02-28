using Prism.Mvvm;

namespace DesktopApp.MVVM.Model
{
    public class OwnedCardPrintAggregate : BindableBase
    {
        public int CardId { get; set; }
        public string CardName { get; set; }
        public int CardPrintId { get; set; }
        public int SetId { get; set; }
        public string SetName { get; set; }
        public int CollectionId { get; set; }
        public string CollectionName { get; set; }
        public bool IsFoil { get; set; }
        public string FrontPictureUrl { get; set; }
        public string BackPictureUrl { get; set; }

        /// <summary>
        /// Using BindableBase so that if `Count` is updated, the DataGrids get notified of the change.
        /// </summary>
        private int _count;
        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }
    }
}
