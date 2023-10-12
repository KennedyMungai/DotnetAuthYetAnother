using AutoMapper;
using DotnetAuthYetAnother.Api.Data;
using DotnetAuthYetAnother.Api.Models;
using DotnetAuthYetAnother.Api.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DotnetAuthYetAnother.Api.Repositories.FormulaOneRepositories;


public class FormulaOneCRUDService : IFormulaOneCRUDService
{
    private readonly FormulaOneDbContext _context;
    private readonly IMapper _mapper;

    public FormulaOneCRUDService(FormulaOneDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ReadDataDto> CreateData(CreateDataDto createDataDto)
    {
        await _context.TeamModels.AddAsync(_mapper.Map<TeamModel>(createDataDto));
        await _context.SaveChangesAsync();

        return await Task.FromResult(_mapper.Map<ReadDataDto>(_mapper.Map<TeamModel>(createDataDto)));
    }

    public async Task<bool> DeleteData(int id)
    {
        try
        {
            var data = await _context.TeamModels.FirstOrDefaultAsync(x => x.Id == id);

            _context.TeamModels.Remove(data!);

            await _context.SaveChangesAsync();

            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return await Task.FromResult(false);
            throw;
        }

    }

    public async Task<IEnumerable<ReadDataDto>> GetAllData(int page = 1, int pageSize = 10)
    {
        var data = await _context.TeamModels.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return await Task.FromResult(_mapper.Map<IEnumerable<ReadDataDto>>(data));
    }

    public async Task<ReadDataDto> GetDataById(int id)
    {
        var data = await _context.TeamModels.FirstOrDefaultAsync(x => x.Id == id);

        return await Task.FromResult(_mapper.Map<ReadDataDto>(data));
    }

    public async Task<ReadDataDto> UpdateData(int id, UpdateDataDto updateDataDto)
    {
        var data = await _context.TeamModels.FirstOrDefaultAsync(x => x.Id == id);

        data!.Name = updateDataDto.Name;
        data!.Country = updateDataDto.Country;
        data!.TeamPrinciple = updateDataDto.TeamPrinciple;

        await _context.SaveChangesAsync();

        return await Task.FromResult(_mapper.Map<ReadDataDto>(data));
    }
}