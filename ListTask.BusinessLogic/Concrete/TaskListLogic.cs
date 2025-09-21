using ListTask.BusinessLogic.Abstract;
using ListTask.Core.Const;
using ListTask.Data.Abstract;
using ListTask.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace ListTask.BusinessLogic.Concrete;

public sealed class TaskListLogic(
    IRepository<TaskList> taskListRepository,
    IRepository<TaskListShare> taskListShareRepository) : ITaskListLogic
{
    public async Task<TaskList[]> GetTaskListsByUserIdAsync(int userId, int take, int skip)
    {
        var userShareListIds = await GetTaskListShareIdsByUserIdAsync(userId);
        
        return await taskListRepository.GetAll()
            .Where(x => x.OwnerId == userId || userShareListIds.Contains(x.Id))
            .OrderByDescending(x => x.Created)
            .Skip(skip)
            .Take(take)
            .ToArrayAsync();
    }
    
    public async Task<TaskList> GetTaskListByUniqueIdAsync(Guid taskListUniqueId, int userId)
    {
        var taskList = await taskListRepository.GetAll()
            .Where(x => x.UniqueId == taskListUniqueId)
            .Include(x => x.Shares)
            .FirstOrDefaultAsync();

        if (taskList == null)
        {
            throw new Exception("TaskList not found");
        }
        
        if (!(taskList.OwnerId == userId || (taskList.Shares != null && taskList.Shares.Any(x => x.UserId == userId))))
        {
            throw new Exception("User does not have permission to TaskList");
        }
        
        return taskList;
    }

    public void CreateTaskList(int userId, string name)
    {
        var taskList = new TaskList
        {
            Name = name,
            OwnerId = userId
        };
        
        taskListRepository.Add(taskList);
    }
    
    public async Task UpdateTaskListAsync(int userId, Guid taskListUniqueId, string name)
    {
        var taskList = await GetTaskListByUniqueIdAsync(taskListUniqueId, userId);

        taskList.Name = name;

        taskListRepository.Update(taskList);
    }
        
    public async Task DeleteTaskListAsync(int userId, Guid taskListUniqueId)
    {
        var taskList = await GetTaskListByUniqueIdAsync(taskListUniqueId, userId);

        if (taskList.OwnerId != userId)
        {
            throw new Exception("User is not owner of TaskList");
        }

        taskListRepository.Delete(taskList);
    }

    public async Task ShareTaskListAsync(int currentUserId, Guid taskListUniqueId, int userId)
    {
        var taskList = await GetTaskListByUniqueIdAsync(taskListUniqueId, currentUserId);

        if (taskList.Shares.Count >= ListTaskConst.MaxTaskListShare)
        {
            throw new Exception("Max count of shares reached");
        }
        
        if (taskList.Shares.Any(x => x.UserId == userId))
        {
            throw new Exception("User already has permission to TaskList");
        }

        var taskListShare = new TaskListShare
        {
            TaskListId = taskList.Id,
            UserId = userId
        };
        
        taskListShareRepository.Add(taskListShare);
    }
    
    public async Task DeleteShareTaskListAsync(int currentUserId, Guid taskListUniqueId, int userId)
    {
        var taskList = await GetTaskListByUniqueIdAsync(taskListUniqueId, currentUserId);

        if (taskList.OwnerId == userId)
        {
            throw new Exception("You cannot delete owner");
        }

        var taskListShare = await taskListShareRepository.GetAll()
            .Where(x => x.TaskListId == taskList.Id && x.UserId == userId)
            .FirstOrDefaultAsync();
        
        if (taskListShare == null)
        {
            throw new Exception("TaskListShare not found");
        }
        
        taskListShareRepository.Delete(taskListShare);
    }

    public async Task<int[]> GetTaskListSharedUsersAsync(int userId, Guid taskListUniqueId)
    {
        var taskList = await GetTaskListByUniqueIdAsync(taskListUniqueId, userId);
        
        var sharedUsers = new List<int> { taskList.OwnerId };

        sharedUsers.AddRange(taskList.Shares.Select(x => x.UserId));
        
        return sharedUsers.ToArray();
    }
    
    private async Task<int[]> GetTaskListShareIdsByUserIdAsync(int userId)
    {
        return await taskListShareRepository.GetAll()
            .Where(x => x.UserId == userId)
            .Select(x => x.TaskListId)
            .ToArrayAsync();
    }
}