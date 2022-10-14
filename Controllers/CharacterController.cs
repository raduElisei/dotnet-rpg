using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    // api/[controller] se traduce in acest context ca api/Character dupa numele clasei de tip controller
    // e sinonim cu [Route("[controller]")]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private static Character knight = new();

        [HttpGet]
        public ActionResult<Character> Get()
        {
            return Ok(knight);
        }
    }
}