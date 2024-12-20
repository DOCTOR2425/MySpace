﻿using InstrumentStore.Domain.Contracts.Users;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Abstractions
{
    public interface IUsersService
    {
        string GeneratePasswordHas(string password);
        Task<User> GetByEMail(string email);
        Task<string> Login(string email, string password);
        Task<Guid> Register(RegisterUserRequest registerUserRequest);
        bool Verify(string password, string passwordHash);
        Task<User> GetById(Guid id);
    }
}