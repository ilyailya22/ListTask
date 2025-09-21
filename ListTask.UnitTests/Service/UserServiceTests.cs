using FluentAssertions;
using ListTask.BusinessLogic.Abstract;
using ListTask.Core.Model;
using ListTask.Data.Abstract;
using ListTask.Service.Abstract;
using ListTask.Service.Concrete;
using ListTask.WebApi.Model;
using Moq;
using NUnit.Framework;

namespace ListTask.UnitTests.Service;

[TestFixture]
public sealed class UserServiceTests
{
    private IUserService _userService;
    
    private Mock<IUserLogic> _userLogicMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;

    private static readonly UserInfo UserInfo1 = new()
    {
        Name = "UserName1",
        UniqueId = new Guid("01531DB3-1B88-4118-AE54-03B96A7AA800")
    };
    
    private static readonly UserInfo UserInfo2 = new()
    {
        Name = "UserName1",
        UniqueId = new Guid("398D27DE-3247-4EFF-8BBA-16995C523FFA")
    };

    [SetUp]
    public void SetUp()
    {
        _userLogicMock = new Mock<IUserLogic>(MockBehavior.Strict);
        _userLogicMock.Setup(x => x.CreateUser(It.IsAny<string>()));
        _userLogicMock.Setup(x => x.GetUsersAsync(
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.IsAny<int[]>()))
            .ReturnsAsync([
                new User
                {
                    UniqueId = UserInfo1.UniqueId,
                    Name = UserInfo1.Name
                },
                new User
                {
                    UniqueId = UserInfo2.UniqueId,
                    Name = UserInfo2.Name
                }
            ]);
        
        _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);
        
        _userService = new UserService(_userLogicMock.Object, _unitOfWorkMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _userLogicMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task CreateUserAsync_Success()
    {
        const string name = "UserName";

        await _userService.CreateUserAsync(new CreateUserRequest { Name = name });
        
        _userLogicMock.Verify(x => x.CreateUser(name), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Test]
    public async Task CreateUserAsync_Fail()
    {
        const int skip = 1;
        const int take = 2;
        var result = await _userService.GetUsersAsync(new UsersRequest
        {
            Skip = skip,
            Take = take
        });

        result.Should().BeEquivalentTo(new UsersResponse
        {
            Users = [UserInfo1, UserInfo2]
        });
        
        _userLogicMock.Verify(x => x.GetUsersAsync(take, skip, null), Times.Once);
    }
}