using System.Security.Claims;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos;
using dotnet_rpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace dotnet_rpg.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeaponService(
            DataContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId && c.User.Id == int.Parse(
                                _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                            )
                );
                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }
                
                Weapon weapon = new Weapon {
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character
                };

                _context.Weapons.Add(weapon);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
