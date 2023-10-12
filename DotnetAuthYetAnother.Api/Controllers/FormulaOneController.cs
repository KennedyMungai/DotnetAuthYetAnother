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
    public async Task<ActionResult<IEnumerable<ReadDataDto>>> GetAllInfo([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var data = await _service.GetAllData(page, pageSize);

        return await Task.FromResult(Ok(data));
    }
}