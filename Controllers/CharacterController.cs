using Microsoft.AspNetCore.Mvc;
using dotnet_rpg.Services.CharacterService;
using dotnet_rpg.Dtos.Character;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    // api/[controller] se traduce in acest context ca api/Character dupa numele clasei de tip controller
    // e sinonim cu [Route("[controller]")]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("GetAll")]
        // poate fi scris si separat:
        // [HttpGet]
        // [Route("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int id)
        {
            return Ok(await _characterService.GetCharacterById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<Character>>>> AddCharacter(
            AddCharacterDto newCharacter
        )
        {
            return Ok(await _characterService.AddCharacter(newCharacter));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> UpdateCharacter(
            UpdateCharacterDto updatedCharacter
        )
        {
            var serviceResponse = await _characterService.UpdateCharacter(updatedCharacter);
            if (serviceResponse.Data == null)
            {
                return NotFound(serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Delete(int id)
        {
            var serviceResponse = await _characterService.DeleteCharacter(id);
            if (serviceResponse.Data == null)
            {
                return NotFound(serviceResponse);
            }
            return Ok(serviceResponse);
        }
    }
}
