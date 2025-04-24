using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.User;
using InstrumentStore.Domain.Services;
using Microsoft.AspNetCore.Mvc;


namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly IJwtProvider _jwtProvider;

        public AccountController(
            IUsersService usersService,
            IMapper mapper,
            IAccountService accountService,
            IJwtProvider jwtProvider,
            IAdminService adminService)
        {
            _usersService = usersService;
            _mapper = mapper;
            _accountService = accountService;
            _jwtProvider = jwtProvider;
            _adminService = adminService;
        }

        [HttpPost("login-first-stage/{email}")]
        public async Task<IActionResult> LoginFirstStage([FromRoute] string email)
        {
            string code = await _accountService.LoginFirstStage(email);
            string codeHash = await _accountService.GenerateCodeHas(email + code);

            HttpContext.Response.Cookies.Append(AccountService.LoginToken, codeHash, new CookieOptions()
            {
                Expires = DateTime.Now.Add(JwtProvider.CookiesLifeTime)
            });

            return Ok();
        }

        [HttpPost("login-second-stage/{email}/{code}")]
        public async Task<IActionResult> LoginSecondStage([FromQuery] string email, [FromQuery] string code)
        {
            string role = await ClientLogin(email, code);

            return Ok(new { role });
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterUserRequest request)
        {
            Guid userId = await _usersService.Register(request);

            string role = await ClientLogin(request.Email, request.Password);

            return Ok(new { role });
        }

        private async Task<string> ClientLogin(string email, string code)
        {
            string codeHash = HttpContext.Request.Headers[AccountService.LoginToken];

            string token = "";

            if (await _adminService.IsAdminEmail(email))
                token = await _accountService.AdminLoginSecondStage(email, code, codeHash);
            else
                token = await _accountService.LoginSecondStage(email, code, codeHash);

            HttpContext.Response.Cookies.Append(JwtProvider.AccessCookiesName, token, new CookieOptions()
            {// выдача пользователю токена в куки файлы
                Expires = DateTime.Now.Add(JwtProvider.CookiesLifeTime)
            });

            string role = (new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken)
                .Claims.First(c => c.Type == ClaimTypes.Role).Value;

            return role;
        }


    }
}
