using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoWebApiProjectWithUserAuthentication.Data;
using TodoWebApiProjectWithUserAuthentication.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TodoWebApiProjectWithUserAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController : ControllerBase
    {
        private readonly ListDbContext _listDbContext;
        public DbSet<List> List { get; set; }
        public DbSet<ListItem> ListItem { get; set; }
        public ListsController(ListDbContext listDbContext)
        {
            this._listDbContext = listDbContext;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllLists()
        {
            var currentUser = GetCurrentUser();
            var id = new Guid(currentUser.Id);
            var lists = await _listDbContext.List.Where(i => i.UserId == id).Include(i => i.ListItems).ToListAsync();
                                 
            return Ok(lists);
            
        }
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize]
        //[ActionName("GetListById")]
        public async Task<IActionResult> GetListById([FromRoute]Guid id)
        {
            //get current user id
            var currentUser = GetCurrentUser();
            Guid user_id = new Guid(currentUser.Id);
            //get the list == to id, of the current user
            var exists = await _listDbContext.List.Where(lst => lst.UserId== user_id).FirstOrDefaultAsync(lst => lst.Id == id);             ;
            if(exists == null)
            {
                return NotFound();
            }
            var list = await _listDbContext.List.Include(lst => lst.ListItems).SingleOrDefaultAsync(l => l.Id == id);
            return Ok(list);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTodoList(List list)
        {
            var currentUser = GetCurrentUser();
            list.Id = Guid.NewGuid();
         
            list.UserId = new Guid(currentUser.Id);
            foreach (var item in list.ListItems)
            {
                item.Id = Guid.NewGuid();
                item.listId = list.Id;
            }
            await _listDbContext.List.AddAsync(list);
            await _listDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetListById), new { id = list.Id }, list);
        }
        // updates list by changing names or adding items to the itemList 
        // will not delete old items in list & cannot update existing item
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateList([FromRoute]Guid id,[FromBody] List updatedList)
        {
            var currentUser = GetCurrentUser();
            Guid user_id = new Guid(currentUser.Id);
            var existingList = await _listDbContext.List.Where(lst => lst.UserId == user_id).FirstOrDefaultAsync(lst => lst.Id == id);
            if (existingList == null)
            {
                return NotFound();
            }
            existingList.Name = updatedList.Name;
            existingList.ListItems = updatedList.ListItems;
            await _listDbContext.SaveChangesAsync();
            return Ok(existingList.ListItems);
        }
        [HttpPut]
        [Route("items/{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateListitem(Guid id, ListItem item)
        {
            var currentUser = GetCurrentUser();
            Guid user_id = new Guid(currentUser?.Id);

            var itmId = await (
                from i in _listDbContext.ListItem
                join lst in _listDbContext.List on i.listId equals lst.Id
                where lst.UserId == user_id
                where i.Id == id
                select new
                {
                    id = i.Id,
                }).FirstOrDefaultAsync();
            var existingListItem = await _listDbContext.ListItem.FirstOrDefaultAsync(itm => itm.Id == itmId.id);
            if (existingListItem != null)
            {
                existingListItem.Description = item.Description;
                existingListItem.Amount = item.Amount;
                existingListItem.isCompleted = item.isCompleted;
            }
            await _listDbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteList(Guid id)
        {
            var currentUser = GetCurrentUser();
            Guid user_id = new Guid(currentUser.Id);
            
            var itemToDelete = await _listDbContext.List.Where(lst => lst.UserId == user_id).FirstOrDefaultAsync(lst => lst.Id == id);
            _listDbContext.List.Remove(itemToDelete);
                               
            await _listDbContext.SaveChangesAsync();
            return Ok();
        }
        [Route("items/{id:Guid}")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleItem([FromRoute] Guid id)
        {
            var currentUser = GetCurrentUser();
            Guid user_id = new Guid(currentUser.Id);
            var li = await (
                from item in _listDbContext.ListItem
                join lst in _listDbContext.List on item.listId equals lst.Id
                where (lst.UserId == user_id)
                where(item.Id == id)
                select new
                {
                    id = item.Id
                }).FirstOrDefaultAsync();
            var liItem = await _listDbContext.ListItem.FirstOrDefaultAsync(i => i.Id == li.id);
            _listDbContext.ListItem.Remove(liItem);

            await _listDbContext.SaveChangesAsync();
            return Ok();
        }
        /// <summary>
        /// verify the current user based on the
        /// jwt token in the header
        /// </summary>
        /// <returns>User object</returns>
        private User? GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if(identity != null)
            {
                var userClaims = identity.Claims;
                return new User
                {
                    Id = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    UserName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value
                };
            }
            return null;
        }
    }
}
