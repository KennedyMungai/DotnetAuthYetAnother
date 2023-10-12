using DotnetAuthYetAnother.Api.Models.Dtos;
using DotnetAuthYetAnother.Api.Repositories.FormulaOneRepositories;
using Microsoft.AspNetCore.Mvc;


namespace DotnetAuthYetAnother.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class FormulaOneController : ControllerBase
{
    private readonly IFormulaOneCRUDService _service;

    public FormulaOneController(IFormulaOneCRUDService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ReadDataDto>))]
    public async Task<ActionResult<IEnumerable<ReadDataDto>>> GetAllInfo([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var data = await _service.GetAllData(page, pageSize);

        return await Task.FromResult(Ok(data));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadDataDto>> GetOneEntry(int id)
    {
        var item = _service.GetDataById(id);

        if (item is null)
        {
            return await Task.FromResult(NotFound("The data entry with id " + id + " was not found"));
        }

        return await Task.FromResult(Ok(item));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReadDataDto>> CreateOneEntry(CreateDataDto data)
    {
        var item = _service.CreateData(data);

        if (!(item.GetType() == typeof(ReadDataDto)))
        {
            return await Task.FromResult(BadRequest("The data could not be entered"));
        }

        return await Task.FromResult(CreatedAtAction("GetOneEntry", new { id = item.Id }, item));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadDataDto>> UpdateOneEntry(int id, UpdateDataDto data)
    {
        var item = await _service.GetDataById(id);

        if (item is null)
        {
            return await Task.FromResult(NotFound("The item of id " + id + " was not found"));
        }

        var updatedItem = _service.UpdateData(id, data);

        if (updatedItem is null)
        {
            return await Task.FromResult(BadRequest("Failed to update entry of id " + id));
        }

        return await Task.FromResult(Accepted(updatedItem));
    }
}