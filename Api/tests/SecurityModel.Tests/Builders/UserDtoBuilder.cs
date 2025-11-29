using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entity.Dtos.Access;
using Entity.Dtos.Security;
using Entity.Models.Security;

namespace SecurityModel.Tests.Builders
{
    public class UserDtoBuilder
    {
        private readonly UserDto _userDto;

        public UserDtoBuilder()
        {
            _userDto = new UserDto
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123",
                PersonId = 1,
                Asset = true
            };
        }

        public UserDtoBuilder WithId(int id)
        {
            _userDto.Id = id;
            return this;
        }

        public UserDtoBuilder WithUsername(string username)
        {
            _userDto.Username = username;
            return this;
        }

        public UserDtoBuilder WithEmail(string email)
        {
            _userDto.Email = email;
            return this;
        }

        public UserDtoBuilder WithPassword(string password)
        {
            _userDto.Password = password;
            return this;
        }

        public UserDtoBuilder WithPersonId(int personId)
        {
            _userDto.PersonId = personId;
            return this;
        }

        public UserDtoBuilder WithAsset(bool asset)
        {
            _userDto.Asset = asset;
            return this;
        }

        public UserDtoBuilder WithExistingEmail(int id)
        {
            _userDto.Id = id;
            return this;
        }

        public UserDtoBuilder WithEmptyUsername()
        {
            _userDto.Username = string.Empty;
            return this;
        }

        public UserDtoBuilder WithEmptyEmail()
        {
            _userDto.Email = string.Empty;
            return this;
        }

        public UserDtoBuilder WithZeroPersonId()
        {
            _userDto.PersonId = 0;
            return this;
        }

        public UserDto Build()
        {
            return _userDto;
        }
    }
}