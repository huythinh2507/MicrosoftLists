using DataLayer;
using Microsoft.AspNetCore.Mvc;
using MsLServiceLayer;
using Newtonsoft.Json;
using System.Drawing;

namespace PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListController(ListService listService) : ControllerBase
    {
        private readonly ListService _listService = listService;

        [HttpPost("createList")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult CreateList(string listName, string description)
        {
            var list = _listService.CreateBlankList(listName, description);

            var savedLists = ListService.LoadLists();

            savedLists.Add(list);

            _listService.SaveLists(savedLists);

            return Ok(list.Id);
        }

        [HttpGet("allLists")]
        [ProducesResponseType(typeof(IEnumerable<List>), StatusCodes.Status200OK)]
        public IActionResult GetAllLists()
        {
            try
            {
                var savedLists = ListService.LoadLists();

                return Ok(savedLists);
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., file not found, invalid JSON, etc.)
                return BadRequest($"Error reading lists: {ex.Message}");
            }
        }

        [HttpPost("addCol")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult AddCol(string listName, string description, string textColumnName, string numberColumnName)
        {
            var list = _listService.CreateBlankList(listName, description);

            list.AddCol(new TextColumn { Name = textColumnName });
            list.AddCol(new Column { Name = numberColumnName, Type = ColumnType.Number });

            list.AddRow("Row1", 1);
            list.AddRow("Row2", 2);
            list.AddRow("Row3", 3);
            list.AddRow("Row4", 4);

            return Ok(list);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult GetList(Guid id)
        {
            var list = _listService.GetList(id);
            if (list == null)
                return NotFound();

            return Ok(list);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteList(Guid id)
        {
            _listService.DeleteList(id);
            return NoContent();
        }

        [HttpPut("favor/{id}")]
        public IActionResult FavorList(Guid id)
        {
            _listService.CreateBlankList("listName", "description");
            _listService.CreateBlankList("listName1", "description1");

            _listService.FavorList(id);
            return Ok();
        }

        [HttpGet("templates")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult GetTemplates()
        {
            var templates = _listService.GetTemplate();
            return Ok(templates);
        }

        
    }
}
