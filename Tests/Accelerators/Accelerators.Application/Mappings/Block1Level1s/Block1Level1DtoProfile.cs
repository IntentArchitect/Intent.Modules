using Accelerators.Domain.Entities.DTOFieldSync;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace Accelerators.Application.Block1Level1s
{
    public class Block1Level1DtoProfile : Profile
    {
        public Block1Level1DtoProfile()
        {
            CreateMap<Block1Level1, Block1Level1Dto>();
        }
    }

    public static class Block1Level1DtoMappingExtensions
    {
        public static Block1Level1Dto MapToBlock1Level1Dto(this Block1Level1 projectFrom, IMapper mapper) => mapper.Map<Block1Level1Dto>(projectFrom);

        public static List<Block1Level1Dto> MapToBlock1Level1DtoList(
            this IEnumerable<Block1Level1> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToBlock1Level1Dto(mapper)).ToList();
    }
}