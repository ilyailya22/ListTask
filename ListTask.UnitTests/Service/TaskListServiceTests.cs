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
public sealed class TaskListServiceTests
{
    private ITaskListService _taskListService;

    private Mock<ITaskListLogic> _taskListLogicMock;
    private Mock<IUserLogic> _userLogicMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;

    private const int CurrentUserId = 1;
    private const int TargetUserId = 3;

    private static readonly TaskList TaskList1 = new()
    {
        UniqueId = new Guid("19D28DD5-C293-42DC-B7A9-573688466941"),
        Name = "TaskList1"
    };
    
    private static readonly TaskList TaskList2 = new()
    {
        UniqueId = new Guid("DF8C31C8-EDE4-43C9-A875-5FF22E28CD58"),
        Name = "TaskList2"
    };
    
    private static readonly User User1 = new()
    {
        UniqueId = new Guid("5AA02EB5-76F6-4E2A-BA8D-D2B39F53D041"),
        Name = "User1"
    };
    
    private static readonly User User2 = new()
    {
        UniqueId = new Guid("E8D0A121-9C26-4356-9428-443D734E15EB"),
        Name = "User2"
    };

    [SetUp]
    public void SetUp()
    {
        _taskListLogicMock = new Mock<ITaskListLogic>(MockBehavior.Strict);
        _userLogicMock = new Mock<IUserLogic>(MockBehavior.Strict);
        _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);

        _userLogicMock.SetupSequence(x => x.GetUserIdByUniqueIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(CurrentUserId)
            .ReturnsAsync(TargetUserId);

        _userLogicMock.Setup(x => x.GetUsersAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int[]>()))
            .ReturnsAsync([ User1, User2 ]);

        _taskListLogicMock.Setup(x => x.GetTaskListByUniqueIdAsync(It.IsAny<Guid>(), It.IsAny<int>()))
            .ReturnsAsync(TaskList1);
        
        _taskListLogicMock.Setup(x => x.GetTaskListsByUserIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync([TaskList1, TaskList2]);

        _taskListLogicMock.Setup(x => x.CreateTaskList(It.IsAny<int>(), It.IsAny<string>()));
        _taskListLogicMock.Setup(x => x.UpdateTaskListAsync(
                It.IsAny<int>(), 
                It.IsAny<Guid>(), 
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _taskListLogicMock.Setup(x => x.DeleteTaskListAsync(It.IsAny<int>(), It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        _taskListLogicMock.Setup(x => x.ShareTaskListAsync(
                It.IsAny<int>(), 
                It.IsAny<Guid>(), 
                It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        _taskListLogicMock.Setup(x => x.DeleteShareTaskListAsync(
                It.IsAny<int>(), 
                It.IsAny<Guid>(), 
                It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        _taskListLogicMock.Setup(x => x.GetTaskListSharedUsersAsync(It.IsAny<int>(), It.IsAny<Guid>()))
            .ReturnsAsync([CurrentUserId, TargetUserId]);

        _unitOfWorkMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        _taskListService = new TaskListService(
            _taskListLogicMock.Object,
            _userLogicMock.Object,
            _unitOfWorkMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _taskListLogicMock.VerifyNoOtherCalls();
        _userLogicMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetTaskListsByUserIdAsync_Succeeded()
    {
        var request = new TaskListsByUserIdRequest
        {
            UserUniqueId = User1.UniqueId,
            Take = 2,
            Skip = 0
        };

        var response = await _taskListService.GetTaskListsByUserIdAsync(request);

        response.TaskLists.Should().BeEquivalentTo(new[]
        {
            new TaskListInfo { UniqueId = TaskList1.UniqueId, Name = TaskList1.Name },
            new TaskListInfo { UniqueId = TaskList2.UniqueId, Name = TaskList2.Name }
        });

        _userLogicMock.Verify(x => x.GetUserIdByUniqueIdAsync(User1.UniqueId), Times.Once);
        _taskListLogicMock.Verify(x => x.GetTaskListsByUserIdAsync(CurrentUserId, request.Take!.Value, request.Skip!.Value), Times.Once);
    }

    [Test]
    public async Task GetTaskListByUniqueIdAsync_Succeeded()
    {
        var request = new TaskListByUniqueIdRequest
        {
            UserUniqueId = User1.UniqueId,
            TaskListUniqueId = TaskList1.UniqueId,
        };

        var response = await _taskListService.GetTaskListByUniqueIdAsync(request);

        response.TaskList.Should().BeEquivalentTo(new TaskListInfo
        {
            UniqueId = TaskList1.UniqueId,
            Name = TaskList1.Name,
        });

        _userLogicMock.Verify(x => x.GetUserIdByUniqueIdAsync(User1.UniqueId), Times.Once);
        _taskListLogicMock.Verify(x => x.GetTaskListByUniqueIdAsync(TaskList1.UniqueId, CurrentUserId), Times.Once);
    }

    [Test]
    public async Task CreateTaskListAsync_Succeeded()
    {
        var request = new CreateTaskListRequest
        {
            UserUniqueId = User1.UniqueId,
            Name = "New List"
        };

        await _taskListService.CreateTaskListAsync(request);

        _userLogicMock.Verify(x => x.GetUserIdByUniqueIdAsync(User1.UniqueId), Times.Once);
        _taskListLogicMock.Verify(x => x.CreateTaskList(CurrentUserId, "New List"), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateTaskListAsync_Succeeded()
    {
        var request = new UpdateTaskListRequest
        {
            UserUniqueId = User1.UniqueId,
            TaskListUniqueId = TaskList1.UniqueId,
            Name = "Updated"
        };

        await _taskListService.UpdateTaskListAsync(request);

        _userLogicMock.Verify(x => x.GetUserIdByUniqueIdAsync(User1.UniqueId), Times.Once);
        _taskListLogicMock.Verify(x => x.UpdateTaskListAsync(CurrentUserId, TaskList1.UniqueId, "Updated"), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteTaskListAsync_Succeeded()
    {
        var request = new DeleteTaskListRequest
        {
            UserUniqueId = User1.UniqueId,
            TaskListUniqueId = TaskList1.UniqueId,
        };

        await _taskListService.DeleteTaskListAsync(request);

        _userLogicMock.Verify(x => x.GetUserIdByUniqueIdAsync(User1.UniqueId), Times.Once);
        _taskListLogicMock.Verify(x => x.DeleteTaskListAsync(CurrentUserId, TaskList1.UniqueId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Test]
    public async Task ShareTaskListAsync_Succeeded()
    {
        var request = new ShareTaskListRequest
        {
            CurrentUserUniqueId = User1.UniqueId,
            UserUniqueId = User2.UniqueId,
            TaskListUniqueId = TaskList1.UniqueId,
        };

        await _taskListService.ShareTaskListAsync(request);

        _userLogicMock.Verify(x => x.GetUserIdByUniqueIdAsync(User1.UniqueId), Times.Once);
        _userLogicMock.Verify(x => x.GetUserIdByUniqueIdAsync(User2.UniqueId), Times.Once);
        _taskListLogicMock.Verify(x => x.ShareTaskListAsync(CurrentUserId, TaskList1.UniqueId, TargetUserId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteShareTaskListAsync_Succeeded()
    {
        var request = new DeleteShareTaskListRequest
        {
            CurrentUserUniqueId = User1.UniqueId,
            UserUniqueId = User2.UniqueId,
            TaskListUniqueId = TaskList1.UniqueId
        };

        await _taskListService.DeleteShareTaskListAsync(request);

        _userLogicMock.Verify(x => x.GetUserIdByUniqueIdAsync(User1.UniqueId), Times.Once);
        _userLogicMock.Verify(x => x.GetUserIdByUniqueIdAsync(User2.UniqueId), Times.Once);
        _taskListLogicMock.Verify(x => x.DeleteShareTaskListAsync(CurrentUserId, TaskList1.UniqueId, TargetUserId), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Test]
    public async Task GetTaskListSharedUsersAsync_Succeeded()
    {
        var request = new TaskListSharedUsersRequest
        {
            UserUniqueId = User1.UniqueId,
            TaskListUniqueId = TaskList1.UniqueId
        };

        var response = await _taskListService.GetTaskListSharedUsersAsync(request);

        response.TaskListSharedUsers.Should().BeEquivalentTo(new[]
        {
            new UserInfo { UniqueId = User1.UniqueId, Name = User1.Name },
            new UserInfo { UniqueId = User2.UniqueId,  Name = User2.Name }
        });

        _userLogicMock.Verify(x => x.GetUserIdByUniqueIdAsync(User1.UniqueId), Times.Once);
        _taskListLogicMock.Verify(x => x.GetTaskListSharedUsersAsync(CurrentUserId, TaskList1.UniqueId), Times.Once);
        _userLogicMock.Verify(x => x.GetUsersAsync(null, null, new[]{CurrentUserId, TargetUserId}), Times.Once);
    }
}