namespace DataAccess.Models
{
    public class AddCollectionRequest
    {
        public string Name { get; set; }
        public bool IsDeck { get; set; }
        public int? MainboardId { get; set; }
        public int? SideboardId { get; set; }
    }
}
