using AutoMapper;

namespace DotnetAuthYetAnother.Api.Models.Dtos.Profiles;


public class FormulaOneProfile : Profile
{
    public FormulaOneProfile()
    {
        CreateMap<TeamModel, ReadDataDto>().ReverseMap();
        CreateMap<TeamModel, CreateDataDto>().ReverseMap();
        CreateMap<TeamModel, UpdateDataDto>().ReverseMap();
    }
}