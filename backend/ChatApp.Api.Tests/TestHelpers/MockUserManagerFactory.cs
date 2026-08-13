using ChatApp.Api.Models;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ChatApp.Api.Tests.TestHelpers;

public class MockUserManagerFactory
{
    public static Mock<UserManager<User>> GetUserManager()
    {
        return new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null!, null!, null!, null!, null!, null!, null!, null!);
    }
}