namespace TodoWebApiProjectWithUserAuthentication.Models.Entities
{
    public class List
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<ListItem> ListItems { get; set; } = new List<ListItem>();
    }
}
