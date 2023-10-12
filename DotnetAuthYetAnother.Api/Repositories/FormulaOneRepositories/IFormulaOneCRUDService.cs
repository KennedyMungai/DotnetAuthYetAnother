using DotnetAuthYetAnother.Api.Models.Dtos;

namespace DotnetAuthYetAnother.Api.Repositories.FormulaOneRepositories;


public interface IFormulaOneCRUDService
{
    Task<IEnumerable<ReadDataDto>> GetAllData(int page = 1, int pageSize = 10);
    Task<ReadDataDto> GetDataById(int id);
    Task<ReadDataDto> CreateData(CreateDataDto createDataDto);
    Task<ReadDataDto> UpdateData(int id, UpdateDataDto updateDataDto);
    Task<bool> DeleteData(int id);
}