using Business.Interfaces.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Implementations.Menu
{
    [Authorize]

    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuBusiness _menuBusiness;

        public MenuController(IMenuBusiness menuBusiness)
        {
            _menuBusiness = menuBusiness;
        }

        [HttpGet("by-user/{userId}/parking/{parkingId}")]
        public async Task<IActionResult> GetUserMenu(int userId, int parkingId)
        {
            var result = await _menuBusiness.GetUserMenuAsync(userId, parkingId);
            if (result == null || result.Roles.Count == 0)
                return NotFound("El usuario no tiene roles o accesos en este parqueadero.");

            return Ok(result);
        }
    }
}
