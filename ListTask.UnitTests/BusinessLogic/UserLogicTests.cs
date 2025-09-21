using FluentAssertions;
using FluentAssertions.ArgumentMatchers.Moq;
using ListTask.BusinessLogic.Abstract;
using ListTask.BusinessLogic.Concrete;
using ListTask.Core.Model;
using ListTask.Data.Abstract;
using MockQueryable;
using Moq;
using NUnit.Framework;

namespace ListTask.UnitTests.BusinessLogic;

[TestFixture]
public sealed class UserLogicTests
{
    private IUserLogic _userLogic;
    
    private Mock<IRepository<User>> _userRepositoryMock;
    
    private static readonly Guid UserUniqueId1 = new("51AC7B72-AAD9-4FA7-B002-F8EA0E36329A");
    private const int UserId1 = 1;
    private const int UserId4 = 4;
    private const int UserId3 = 3;

    [SetUp]
    public void SetUp()
    {
        var users = new List<User>
        {
            new() { Id = 6, UniqueId = new Guid("E1CA801A-6732-4AF9-ACC4-24BBB74956AF"), Name = "Name6" },
            new() { Id = UserId1, UniqueId = UserUniqueId1, Name = "Name1" },
            new() { Id = UserId3, UniqueId = new Guid("EAEBBBEE-D208-4EC9-8CB0-3F2D234A7806"), Name = "Name3" },
            new() { Id = UserId4, UniqueId = new Guid("F8325FFE-024B-48F1-A551-7DCA2182C51B"), Name = "Name4" },
            new() { Id = 5, UniqueId = new Guid("5A41188D-D831-4621-8883-FEB97D570379"), Name = "Name5" },
        }.BuildMock();
        
        _userRepositoryMock = new Mock<IRepository<User>>(MockBehavior.Strict);
        _userRepositoryMock.Setup(x => x.GetAll()).Returns(users);
        _userRepositoryMock.Setup(x => x.Add(It.IsAny<User>()));
        
        _userLogic = new UserLogic(_userRepositoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _userRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetUserIdByUniqueIdAsync_Succeeded()
    {
        var result = await _userLogic.GetUserIdByUniqueIdAsync(UserUniqueId1);
        
        result.Should().Be(UserId1);
        
        _userRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetUserIdByUniqueIdAsync_ThrowExceptionUniqueIdNull()
    {
       var action = () => _userLogic.GetUserIdByUniqueIdAsync(null); 
       
       await action.Should().ThrowAsync<ArgumentNullException>()
           .WithMessage("Value cannot be null. (Parameter 'uniqueId')");
    }
    
    [Test]
    public async Task GetUserIdByUniqueIdAsync_ThrowExceptionUserNotFound()
    {
        var uniqueId = new Guid("679D7BCC-0381-404F-9192-9E2C9B7195AD");
        var action = () => _userLogic.GetUserIdByUniqueIdAsync(uniqueId); 
       
        await action.Should().ThrowAsync<Exception>()
            .WithMessage($"User with unique id {uniqueId} not found");
                
        _userRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }
    
    [Test]
    public void CreateUser_Succeeded()
    {
        var name = "Name2";
        _userLogic.CreateUser(name);

        _userRepositoryMock.Verify(x => x.Add(Its.EquivalentTo(new User
        {
            Name = name
        })), Times.Once);
    }
    
    [Test]
    public async Task GetUsersAsync_Succeeded([Values] bool paginationHasValue, [Values] bool userIdsNotNull)
    {
        var result = await _userLogic.GetUsersAsync(paginationHasValue ? 2 : null, paginationHasValue ? 1 : null, userIdsNotNull ? [ UserId1, UserId3, UserId4 ] : null);

        var expectedUsers = paginationHasValue
            ? new[]
            {
                new User { Id = 1, UniqueId = UserUniqueId1, Name = "Name1" }
            }
            : userIdsNotNull
                ? new[]
                {
                    new User { Id = UserId1, UniqueId = UserUniqueId1, Name = "Name1" },
                    new User { Id = UserId3, UniqueId = new Guid("EAEBBBEE-D208-4EC9-8CB0-3F2D234A7806"), Name = "Name3" },
                    new User { Id = UserId4, UniqueId = new Guid("F8325FFE-024B-48F1-A551-7DCA2182C51B"), Name = "Name4" },
                }
                : new[]
                {
                    new User { Id = 6, UniqueId = new Guid("E1CA801A-6732-4AF9-ACC4-24BBB74956AF"), Name = "Name6" },
                    new User { Id = UserId1, UniqueId = UserUniqueId1, Name = "Name1" },
                    new User { Id = UserId3, UniqueId = new Guid("EAEBBBEE-D208-4EC9-8CB0-3F2D234A7806"), Name = "Name3" },
                    new User { Id = UserId4, UniqueId = new Guid("F8325FFE-024B-48F1-A551-7DCA2182C51B"), Name = "Name4" },
                    new User { Id = 5, UniqueId = new Guid("5A41188D-D831-4621-8883-FEB97D570379"), Name = "Name5" }
                };
        
        result.Should().BeEquivalentTo(expectedUsers);
        
        _userRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }
}