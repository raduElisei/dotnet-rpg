using AutoMapper;
using dotnet_rpg.Dtos;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Services;

namespace dotnet_rpg
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<AddWeaponDto, Weapon>();
        }
    }
}