using System.ComponentModel.DataAnnotations;

namespace TodoWebApiProjectWithUserAuthentication.Models.Entities
{
    public class User
    {
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public List<List> lists { get; set; } = new List<List>();
    }
}
