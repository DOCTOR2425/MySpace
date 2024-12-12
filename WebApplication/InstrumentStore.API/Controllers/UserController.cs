﻿using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UserController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("/register")]
        public async Task<ActionResult<Guid>> Register(RegisterUserRequest request)
        {
            return Ok(await _usersService.Register(request));
        }

        [HttpPost("/login")]
        public async Task<IResult> Login(LoginUserRequest request, HttpContext context)
        {
            string token = await _usersService.Login(request.EMail, request.Password);

            context.Response.Cookies.Append("token-cookies", token);

            return Results.Ok(token);
        }
    }
}
