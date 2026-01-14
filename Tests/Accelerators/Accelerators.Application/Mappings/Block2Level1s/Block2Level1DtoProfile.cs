using Accelerators.Domain.Entities.DTOFieldSync;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace Accelerators.Application.Block2Level1s
{
    public class Block2Level1DtoProfile : Profile
    {
        public Block2Level1DtoProfile()
        {
            CreateMap<Block2Level1, Block2Level1Dto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.BlockId));
        }
    }

    public static class Block2Level1DtoMappingExtensions
    {
        public static Block2Level1Dto MapToBlock2Level1Dto(this Block2Level1 projectFrom, IMapper mapper) => mapper.Map<Block2Level1Dto>(projectFrom);

        public static List<Block2Level1Dto> MapToBlock2Level1DtoList(
            this IEnumerable<Block2Level1> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToBlock2Level1Dto(mapper)).ToList();
    }
}