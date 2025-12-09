using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Entity.Models;
using Entity.Models.Security;

namespace SecurityModel.Tests.Helpers
{
    public static class MockSetup
    {
        #region Generic Repository Mock Setup

        public static void SetupGetById<T, TDto>(Mock<IRepositoryBusiness<T, TDto>> mock, T entity) 
            where T : BaseModel 
            where TDto : class
        {
            mock.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync(entity);
        }

        public static void SetupGetByIdReturnsNull<T, TDto>(Mock<IRepositoryBusiness<T, TDto>> mock) 
            where T : BaseModel 
            where TDto : class
        {
            mock.Setup(x => x.GetById(It.IsAny<int>()))
                .ReturnsAsync((T?)null);
        }

        public static void SetupExistsAsync<T>(Mock<IRepositoryBusiness<T, UserDto>> mock, bool exists, Expression<Func<T, bool>> predicate)
        {
            mock.Setup(x => x.ExistsAsync(predicate))
                .ReturnsAsync(exists);
        }

        #endregion

        #region User Data Mock Setup

        public static void SetupGetUserByUsername(Mock<IUserData> mock, User? user)
        {
            mock.Setup(x => x.GetUserByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
        }

        public static void SetupGetUserByEmail(Mock<IUserData> mock, User? user)
        {
            mock.Setup(x => x.GetUserByEmailsync(It.IsAny<string>()))
                .ReturnsAsync(user);
        }

        public static void SetupGetUserAccess(Mock<IUserData> mock, UserAccessDto accessDto)
        {
            mock.Setup(x => x.GetUserAccessAsync(It.IsAny<int>()))
                .ReturnsAsync(accessDto);
        }

        public static void SetupGetUserRoles(Mock<IUserData> mock, List<UserRoleByParkingDto> roles)
        {
            mock.Setup(x => x.GetUserRolesAsync(It.IsAny<int>()))
                .ReturnsAsync(roles);
        }

        public static void SetupGetClientWithVehiclesByPersonId(Mock<IClientData> mock, Entity.Models.Parameter.Client? client)
        {
            mock.Setup(x => x.GetClientWithVehiclesByPersonIdAsync(It.IsAny<int>()))
                .ReturnsAsync(client);
        }

        #endregion

        #region Authentication Service Mock Setup

        public static void SetupVerifyPassword(Mock<IPasswordHasher> mock, bool isValid, string hashedPassword)
        {
            mock.Setup(x => x.VerifyPassword(hashedPassword, It.IsAny<string>()))
                .Returns(isValid);
        }

        public static void SetupHashPassword(Mock<IPasswordHasher> mock, string hashedPassword)
        {
            mock.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns(hashedPassword);
        }

        public static void SetupGenerateToken(Mock<IJwtAuthenticationService> mock, string token, User user, List<string> roles)
        {
            mock.Setup(x => x.GenerarToken(user, roles))
                .Returns(token);
        }

        public static void SetupGenerateTokenWithExtraClaims(Mock<IJwtAuthenticationService> mock, string token, User user, List<string> roles, Dictionary<string, string> extraClaims)
        {
            mock.Setup(x => x.GenerarToken(user, roles, extraClaims))
                .Returns(token);
        }

        #endregion

        #region Email Service Mock Setup

        public static void SetupSendEmail(Mock<IEmailService> mock, bool success = true)
        {
            if (success)
            {
                mock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);
            }
            else
            {
                mock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ThrowsAsync(new Exception("Email service error"));
            }
        }

        #endregion

        #region Rol Business Mock Setup

        public static void SetupGetByName(Mock<IRolBusiness> mock, Entity.Models.Security.Rol? rol)
        {
            mock.Setup(x => x.GetByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(rol);
        }

        public static void SetupExistsAsync(Mock<IRolParkingUserBusiness> mock, bool exists)
        {
            mock.Setup(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(exists);
        }

        #endregion

        #region Verification Business Mock Setup

        public static void SetupGenerateAndSendCode(Mock<IUserVerificationBusiness> mock, bool success = true, string message = "Code sent")
        {
            var result = new ApiResponse<object>(null, success, message, null);
            mock.Setup(x => x.GenerateAndSendCodeAsync(It.IsAny<int>()))
                .ReturnsAsync(result);
        }

        public static void SetupVerifyCode(Mock<IUserVerificationBusiness> mock, bool success = true, string message = "Valid code")
        {
            var result = new ApiResponse<object>(null, success, message, null);
            mock.Setup(x => x.VerifyCodeAsync(It.IsAny<VerificationRequestDto>()))
                .ReturnsAsync(result);
        }

        #endregion

        #region AutoMapper Mock Setup

        public static void SetupMap<T, TTarget>(Mock<IMapper> mock, TSource source, TTarget result)
        {
            mock.Setup(x => x.Map<TTarget>(source))
                .Returns(result);
        }

        public static void SetupMapToEnumerable<T, TTarget>(Mock<IMapper> mock, IEnumerable<TSource> source, IEnumerable<TTarget> result)
        {
            mock.Setup(x => x.Map<IEnumerable<TTarget>>(source))
                .Returns(result);
        }

        #endregion

        #region Password Reset Mock Setup

        public static void SetupCountRequestsSince(Mock<IPasswordReset> mock, int count)
        {
            mock.Setup(x => x.CountRequestsSinceAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(count);
        }

        public static void SetupAddPasswordReset(Mock<IPasswordReset> mock)
        {
            mock.Setup(x => x.Add(It.IsAny<PasswordReset>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupGetValidCode(Mock<IPasswordReset> mock, PasswordReset? reset)
        {
            mock.Setup(x => x.GetValidCode(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(reset);
        }

        public static void SetupMarkAsUsed(Mock<IPasswordReset> mock)
        {
            mock.Setup(x => x.MarkAsUsed(It.IsAny<PasswordReset>()))
                .Returns(Task.CompletedTask);
        }

        #endregion
    }

    #region Mock Interfaces (needed for compilation)

    public interface IRepositoryBusiness<T, D> where T : BaseModel where D : class
    {
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<D> GetById(int id);
    }

    public interface IUserData
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailsync(string email);
        Task<UserAccessDto> GetUserAccessAsync(int userId);
        Task<List<UserRoleByParkingDto>> GetUserRolesAsync(int userId);
    }

    public interface IClientData
    {
        Task<Entity.Models.Parameter.Client?> GetClientWithVehiclesByPersonIdAsync(int personId);
    }

    public interface IRolBusiness
    {
        Task<Rol?> GetByNameAsync(string name);
    }

    public interface IRolParkingUserBusiness
    {
        Task<bool> ExistsAsync(int userId, int rolId);
        Task<RolParkingUserDto> Save(RolParkingUserDto dto);
    }

    public interface IUserVerificationBusiness
    {
        Task<ApiResponse<object>> GenerateAndSendCodeAsync(int userId);
        Task<ApiResponse<object>> VerifyCodeAsync(VerificationRequestDto dto);
    }

    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string plainPassword);
    }

    public interface IJwtAuthenticationService
    {
        string GenerarToken(User user, List<string> roles, Dictionary<string, string>? extraClaims = null);
    }

    public interface IEmailService
    {
        Task SendEmailAsync(string to, string message);
    }

    public interface IPasswordReset
    {
        Task<int> CountRequestsSinceAsync(int userId, DateTime since);
        Task Add(PasswordReset reset);
        Task<PasswordReset?> GetValidCode(int userId, string code);
        Task MarkAsUsed(PasswordReset reset);
    }

    #endregion
}