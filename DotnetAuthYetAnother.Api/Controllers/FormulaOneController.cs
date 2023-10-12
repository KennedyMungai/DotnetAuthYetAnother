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
    public async Task<ActionResult<IEnumerable<ReadDataDto>>> GetAllInfo()
    {
        var data = await _service.GetAllData();

        return await Task.FromResult(Ok(data));
    }
}