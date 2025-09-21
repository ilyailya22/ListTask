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
public sealed class TaskListLogicTests
{
    private ITaskListLogic _taskListLogic;

    private Mock<IRepository<TaskList>> _taskListRepositoryMock;
    private Mock<IRepository<TaskListShare>> _taskListShareRepositoryMock;

    private const int OwnerId = 10;
    private const int AnotherOwnerId = 30;
    private const int SharedUser1 = 20;
    private const int SharedUser2 = 21;

    private static readonly Guid TaskList1Guid = new("2073A6A0-62E6-4BF5-8C4D-BC84E9E9CA1B");
    private static readonly Guid TaskList2Guid = new("F91D05B8-554E-4E79-B3B2-0B1B63CB640B");
    private static readonly Guid TaskList3Guid = new("666A0919-0E0D-4F1A-B19B-33B3C2B9E097");

    private IQueryable<TaskList> _taskLists;
    private IQueryable<TaskListShare> _taskListShares;

    [SetUp]
    public void SetUp()
    {
        var taskList1 = new TaskList
        {
            Id = 1,
            UniqueId = TaskList1Guid,
            OwnerId = OwnerId,
            Name = "TL1",
            Created = new DateTime(2024, 01, 01),
            Shares = new List<TaskListShare>
            {
                new() { Id = 100, TaskListId = 1, UserId = SharedUser1 },
                new() { Id = 101, TaskListId = 1, UserId = SharedUser2 },
            }
        };

        var taskList2 = new TaskList
        {
            Id = 2,
            UniqueId = TaskList2Guid,
            OwnerId = OwnerId,
            Name = "TL2",
            Created = new DateTime(2024, 05, 01),
            Shares = new List<TaskListShare>()
        };

        var taskList3 = new TaskList
        {
            Id = 3,
            UniqueId = TaskList3Guid,
            OwnerId = AnotherOwnerId,
            Name = "TL3",
            Created = new DateTime(2024, 03, 01),
            Shares = new List<TaskListShare>
            {
                new() { Id = 200, TaskListId = 3, UserId = OwnerId }
            }
        };

        _taskLists = new List<TaskList> { taskList1, taskList2, taskList3 }.BuildMock();

        _taskListShares = new List<TaskListShare>
        {
            new() { Id = 300, TaskListId = 3, UserId = OwnerId },
            new() { Id = 1,   TaskListId = 1, UserId = SharedUser2 },
            new() { Id = 100, TaskListId = 1, UserId = SharedUser1 },
            new() { Id = 101, TaskListId = 1, UserId = SharedUser2 },
        }.BuildMock();

        _taskListRepositoryMock = new Mock<IRepository<TaskList>>(MockBehavior.Strict);
        _taskListRepositoryMock.Setup(x => x.GetAll()).Returns(_taskLists);
        _taskListRepositoryMock.Setup(x => x.Add(It.IsAny<TaskList>()));
        _taskListRepositoryMock.Setup(x => x.Update(It.IsAny<TaskList>()));
        _taskListRepositoryMock.Setup(x => x.Delete(It.IsAny<TaskList>()));

        _taskListShareRepositoryMock = new Mock<IRepository<TaskListShare>>(MockBehavior.Strict);
        _taskListShareRepositoryMock.Setup(x => x.GetAll()).Returns(_taskListShares);
        _taskListShareRepositoryMock.Setup(x => x.Add(It.IsAny<TaskListShare>()));
        _taskListShareRepositoryMock.Setup(x => x.Delete(It.IsAny<TaskListShare>()));

        _taskListLogic = new TaskListLogic(_taskListRepositoryMock.Object, _taskListShareRepositoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _taskListRepositoryMock.VerifyNoOtherCalls();
        _taskListShareRepositoryMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetTaskListsByUserIdAsync_ReturnsOwnedAndShared_WithOrderingAndPagination()
    {
        var take = 2;
        var skip = 0;

        var result = await _taskListLogic.GetTaskListsByUserIdAsync(OwnerId, take, skip);

        result.Select(x => x.Id).Should().BeEquivalentTo(new[] { 2, 3 }, opts => opts.WithStrictOrdering());

        _taskListShareRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetTaskListByUniqueIdAsync_SucceedsForOwner()
    {
        var taskList = await _taskListLogic.GetTaskListByUniqueIdAsync(TaskList1Guid, OwnerId);

        taskList.Id.Should().Be(1);

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetTaskListByUniqueIdAsync_Throws_WhenNotFound()
    {
        var guid = new Guid("0FF8B6F2-D707-43DF-B9F9-AF7BCC8B0EC8");
        var action = () => _taskListLogic.GetTaskListByUniqueIdAsync(guid, OwnerId);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("TaskList not found");

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetTaskListByUniqueIdAsync_Throws_WhenUserDoesntHavePermission()
    {
        var action = () => _taskListLogic.GetTaskListByUniqueIdAsync(TaskList3Guid, 12);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("User does not have permission to TaskList");

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Test]
    public void CreateTaskList_AddsEntity()
    {
        _taskListLogic.CreateTaskList(OwnerId, "New List");

        _taskListRepositoryMock.Verify(x => x.Add(Its.EquivalentTo(new TaskList
        {
            Name = "New List",
            OwnerId = OwnerId
        })), Times.Once);
    }

    [Test]
    public async Task UpdateTaskListAsync_UpdatesName_ForOwner()
    {
        await _taskListLogic.UpdateTaskListAsync(OwnerId, TaskList1Guid, "Renamed");

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        _taskListRepositoryMock.Verify(x => x.Update(Its.EquivalentTo(new TaskList
        {
            Id = 1,
            UniqueId = TaskList1Guid,
            OwnerId = OwnerId,
            Name = "Renamed",
            Created = new DateTime(2024, 01, 01),
            Shares = new List<TaskListShare>
            {
                new() { Id = 100, TaskListId = 1, UserId = SharedUser1 },
                new() { Id = 101, TaskListId = 1, UserId = SharedUser2 },
            }
        })), Times.Once);
    }

    [Test]
    public async Task DeleteTaskListAsync_Deletes_ForOwner()
    {
        await _taskListLogic.DeleteTaskListAsync(OwnerId, TaskList1Guid);

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        _taskListRepositoryMock.Verify(x => x.Delete(Its.EquivalentTo(new TaskList
        {
            Id = 1,
            UniqueId = TaskList1Guid,
            OwnerId = OwnerId,
            Name = "TL1",
            Created = new DateTime(2024, 01, 01),
            Shares = new List<TaskListShare>
            {
                new() { Id = 100, TaskListId = 1, UserId = SharedUser1 },
                new() { Id = 101, TaskListId = 1, UserId = SharedUser2 },
            }
        })), Times.Once);
    }

    [Test]
    public async Task DeleteTaskListAsync_Throws_WhenNotOwner()
    {
        var action = () => _taskListLogic.DeleteTaskListAsync(SharedUser1, TaskList1Guid);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("User is not owner of TaskList");

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Test]
    public async Task ShareTaskListAsync_AddsShare_WhenOk()
    {
        await _taskListLogic.ShareTaskListAsync(OwnerId, TaskList2Guid, 999);

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        _taskListShareRepositoryMock.Verify(x => x.Add(Its.EquivalentTo(new TaskListShare
        {
            TaskListId = 2,
            UserId = 999
        })), Times.Once);
    }

    [Test]
    public async Task ShareTaskListAsync_Throws_WhenMaxSharesReached()
    {
        var taskList1 = _taskLists.First(x => x.Id == 1);
        taskList1.Shares.Add(new TaskListShare { Id = 102, TaskListId = 1, UserId = 777 });

        var action = () => _taskListLogic.ShareTaskListAsync(OwnerId, TaskList1Guid, 888);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Max count of shares reached");

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Test]
    public async Task ShareTaskListAsync_Throws_WhenDuplicate()
    {
        var action = () => _taskListLogic.ShareTaskListAsync(OwnerId, TaskList1Guid, SharedUser2);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("User already has permission to TaskList");

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Test]
    public async Task DeleteShareTaskListAsync_DeletesShare_WhenOk()
    {
        await _taskListLogic.DeleteShareTaskListAsync(OwnerId, TaskList1Guid, SharedUser2);

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        _taskListShareRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        _taskListShareRepositoryMock.Verify(x => x.Delete(Its.EquivalentTo(new TaskListShare
        {
            Id = 1,
            TaskListId = 1,
            UserId = SharedUser2
        })), Times.Once);
    }

    [Test]
    public async Task DeleteShareTaskListAsync_Throws_WhenTryingToDeleteOwner()
    {
        var action = () => _taskListLogic.DeleteShareTaskListAsync(OwnerId, TaskList1Guid, OwnerId);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("You cannot delete owner");

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Test]
    public async Task DeleteShareTaskListAsync_Throws_WhenShareNotFound()
    {
        var action = () => _taskListLogic.DeleteShareTaskListAsync(OwnerId, TaskList2Guid, 999);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("TaskListShare not found");

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        _taskListShareRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Test]
    public async Task GetTaskListSharedUsersAsync_ReturnsOwnerAndAllShares()
    {
        var users = await _taskListLogic.GetTaskListSharedUsersAsync(OwnerId, TaskList1Guid);

        users.Should().BeEquivalentTo(new[] { OwnerId, SharedUser1, SharedUser2 });

        _taskListRepositoryMock.Verify(x => x.GetAll(), Times.Once);
    }
}