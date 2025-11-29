using System;
using System.Collections.Generic;
using System.Linq;
using Entity.Dtos.Access;
using Entity.Dtos.Security;
using Entity.Models.Security;

namespace SecurityModel.Tests.Builders
{
    public class UserBuilder
    {
        private readonly User _user;

        public UserBuilder()
        {
            _user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                Password = "hashedPassword123",
                PersonId = 1,
                Person = new Entity.Models.Security.Person
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "User"
                },
                Asset = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public UserBuilder WithId(int id)
        {
            _user.Id = id;
            return this;
        }

        public UserBuilder WithUsername(string username)
        {
            _user.Username = username;
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _user.Email = email;
            return this;
        }

        public UserBuilder WithPassword(string password)
        {
            _user.Password = password;
            return this;
        }

        public UserBuilder WithPersonId(int personId)
        {
            _user.PersonId = personId;
            _user.Person.Id = personId;
            return this;
        }

        public UserBuilder WithPerson(Entity.Models.Security.Person person)
        {
            _user.Person = person;
            return this;
        }

        public UserBuilder WithAsset(bool asset)
        {
            _user.Asset = asset;
            return this;
        }

        public UserBuilder WithNoPerson()
        {
            _user.Person = null!;
            return this;
        }

        public User Build()
        {
            return _user;
        }
    }

    public class PersonBuilder
    {
        private readonly Entity.Models.Security.Person _person;

        public PersonBuilder()
        {
            _person = new Entity.Models.Security.Person
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                DocumentNumber = "123456789",
                Email = "test@example.com"
            };
        }

        public PersonBuilder WithId(int id)
        {
            _person.Id = id;
            return this;
        }

        public PersonBuilder WithFirstName(string firstName)
        {
            _person.FirstName = firstName;
            return this;
        }

        public PersonBuilder WithLastName(string lastName)
        {
            _person.LastName = lastName;
            return this;
        }

        public Entity.Models.Security.Person Build()
        {
            return _person;
        }
    }
}