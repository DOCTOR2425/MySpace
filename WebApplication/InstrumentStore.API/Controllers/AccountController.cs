using AutoMapper;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;

namespace InstrumentStore.API.Controllers
{
    public class AccountController
    {
        private readonly InstrumentStoreDBContext _dbContext;
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public AccountController(
            IUsersService usersService,
            IMapper mapper,
            InstrumentStoreDBContext dbContext)
        {
            _usersService = usersService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        //[HttpPost("login/{email}")]
        //public async Task LoginFirstStage([FromRoute] string email)
        //{
        //    _emailService.SendMail(email, GenerateLoginCode())
        //}
    }
}
