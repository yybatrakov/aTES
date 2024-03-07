using Confluent.Kafka;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PopugCommon.KafkaMessages.Messages;
using static PopugTaskTracker.DataContext;

namespace PopugTaskTracker.Logic
{
    public class TaskLogic
    {
        private readonly DataContext dataContext;
        
        public TaskLogic(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<List<PopugTask>> Get()
        {
            return await dataContext.PopugTasks.ToListAsync();
        }
        public async Task<PopugTask> Get(int taskId)
        {
            return await dataContext.PopugTasks.Where(t => t.Id == taskId).FirstOrDefaultAsync();
        }
        public async Task<List<PopugTask>> Get(string userId)
        {
            return await dataContext.PopugTasks.Where(t => t.AssignedUserId == userId).ToListAsync();
        }

        public async Task<PopugTask> Add(PopugTask task)
        {
            dataContext.PopugTasks.Add(task);
            dataContext.SaveChanges();

            return await Get(task.Id);
        }
        public async Task<PopugTask> Update(PopugTask task)
        {
            dataContext.PopugTasks.Update(task);
            dataContext.SaveChanges();

            return await Get(task.Id);
        }

        public async Task<PopugTask> CompleteTask(int taskId)
        {
            var task = await Get(taskId);
            task.IsCompleted= true;
            await Update(task);
            await Kafka.Produce(KafkaTopics.TasksStream, task.Id.ToString(), new PopugMessage(task, Messages.Tasks.Stream.Updated, "v1"));
            await Kafka.Produce(KafkaTopics.TasksLifecycle, task.Id.ToString(), new PopugMessage(new TaskCompletedEvent() { PublicId = task.PublicId}, Messages.Tasks.Completed, "v1"));
            return task;
        }

        public async Task<PopugTask> CreateTask(PopugTask task)
        {
            task = (await AssignTasks(new List<PopugTask> { task })).Single();
            await Update(task);
            await Kafka.Produce(KafkaTopics.TasksStream, task.Id.ToString(), new PopugMessage(task, Messages.Tasks.Stream.Created, "v1"));
            await Kafka.Produce(KafkaTopics.TasksLifecycle, task.Id.ToString(), new PopugMessage(new TaskAssignedEvent() { PublicId = task.PublicId, AssignedUserId = task.AssignedUserId }, Messages.Tasks.Assigned, "v1"));
            return task;
        }

        public async Task<List<PopugTask>> ReassignTasks()
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
        private async Task<List<PopugTask>> AssignTasks(List<PopugTask> tasks)
        {
            var user = await GetUsersForAssign();
            var rng = new Random();
            foreach (var task in tasks)
            {
                task.AssignedUserId = user[rng.Next(user.Count)].UserId;
            }
            return tasks;
        }
        private async Task<List<User>> GetUsersForAssign()
        {
            var excludeRoles = new[] { "Admin", "Manager" };

            return await dataContext.Users.Where(u => !excludeRoles.Contains(u.UserRole)).ToListAsync();

        }
        private async Task<List<PopugTask>> GetTasksForAssign()
        {
            return await dataContext.PopugTasks.Where(t=> !t.IsCompleted).ToListAsync();
        }
    }
}
