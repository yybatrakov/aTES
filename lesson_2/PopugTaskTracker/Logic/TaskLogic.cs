using Microsoft.EntityFrameworkCore;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopugTaskTracker.Logic
{
    public class TaskLogic
    {
        private readonly DataContext dataContext;
        
        public TaskLogic(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<List<TaskDb>> Get()
        {
            return await dataContext.PopugTasks.ToListAsync();
        }
        public async Task<TaskDb> Get(int taskId)
        {
            return await dataContext.PopugTasks.Where(t => t.Id == taskId).FirstOrDefaultAsync();
        }
        public async Task<List<TaskDb>> Get(string userId)
        {
            return await dataContext.PopugTasks.Where(t => t.AssignedUserId == userId).ToListAsync();
        }

        public async Task<TaskDb> Add(TaskDb task)
        {

            dataContext.PopugTasks.Add(task);
            dataContext.SaveChanges();

            return await Get(task.Id);
        }
        public async Task<TaskDb> Update(TaskDb task)
        {
            dataContext.PopugTasks.Update(task);
            dataContext.SaveChanges();

            return await Get(task.Id);
        }

        public async Task<TaskDb> CompleteTask(int taskId)
        {
            var task = await Get(taskId);
            task.IsCompleted= true;
            await Update(task);
            await Kafka.Produce(KafkaTopics.TasksStream, task.Id.ToString(), new PopugMessage(task, Messages.Tasks.Stream.Updated, "v1"));
            await Kafka.Produce(KafkaTopics.TasksLifecycle, task.Id.ToString(), new PopugMessage(new TaskCompletedEvent() { PublicId = task.PublicId}, Messages.Tasks.Completed, "v1"));
            return task;
        }

        public async Task<TaskDb> CreateTask(TaskDb task)
        {
            task = (await AssignTasks(new List<TaskDb> { task })).Single();
            await Update(task);
            await Kafka.Produce(KafkaTopics.TasksStream, task.Id.ToString(), new PopugMessage(task, Messages.Tasks.Stream.Created, "v1"));
            await Kafka.Produce(KafkaTopics.TasksLifecycle, task.Id.ToString(), new PopugMessage(new TaskAssignedEvent() { PublicId = task.PublicId, AssignedUserId = task.AssignedUserId }, Messages.Tasks.Assigned, "v1"));
            return task;
        }

        public async Task<List<TaskDb>> ReassignTasks()
        {
            var tasks = await AssignTasks(await GetTasksForAssign());
            foreach (var task in tasks)
            {
                await Update(task);
                await Kafka.Produce(KafkaTopics.TasksStream, task.Id.ToString(), new PopugMessage(task, Messages.Tasks.Stream.Updated, "v1"));
            }
            
            var e = new TasksReassignedEvent() { Tasks = tasks.Select(t => new TaskAssignedEvent() { PublicId = t.PublicId, AssignedUserId = t.AssignedUserId }).ToList() };
            await Kafka.Produce(KafkaTopics.TasksLifecycle, DateTime.Now.Ticks.ToString() , new PopugMessage(e, Messages.Tasks.ReAssigned, "v1"));
            return tasks;
        }
        private async Task<List<TaskDb>> AssignTasks(List<TaskDb> tasks)
        {
            var user = await GetUsersForAssign();
            var rng = new Random();
            foreach (var task in tasks)
            {
                task.AssignedUserId = user[rng.Next(user.Count)].PublicId;
            }
            return tasks;
        }
        private async Task<List<UserDb>> GetUsersForAssign()
        {
            var excludeRoles = new[] { "Admin", "Manager" };

            return await dataContext.Users.Where(u => !excludeRoles.Contains(u.UserRole)).ToListAsync();

        }
        private async Task<List<TaskDb>> GetTasksForAssign()
        {
            return await dataContext.PopugTasks.Where(t=> !t.IsCompleted).ToListAsync();
        }
    }
}
