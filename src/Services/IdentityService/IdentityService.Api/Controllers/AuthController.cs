using IdentityService.Api.Application.Models;
using IdentityService.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequestModel)
    {
        return Ok(await _identityService.Login(loginRequestModel));
    }
}