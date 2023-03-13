namespace TodoWebApiProjectWithUserAuthentication.Models.Entities
{
    public class ListItem
    {
        public Guid Id { get; set; }
        public Guid listId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Amount { get; set; }
        public bool isCompleted { get; set; } = false;
    }
}
