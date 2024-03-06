using Confluent.Kafka;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PopugCommon.Kafka;
using PopugCommon.KafkaMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Kafka.Produce(KafkaTopics.TasksStream, task.Id.ToString(), new StreamEvent<PopugTask>(task, Operation.Update).ToJson());
            Kafka.Produce(KafkaTopics.TasksLifecycle, task.Id.ToString(), new BussinessEvent<PopugTask>(task, Events.TaskCompleted).ToJson());
            return task;
        }

        public async Task<PopugTask> CreateTask(PopugTask task)
        {
            task = (await AssignTasks(new List<PopugTask> { task })).Single();
            await Update(task);
            Kafka.Produce(KafkaTopics.TasksStream, task.Id.ToString(), new StreamEvent<PopugTask>(task, Operation.Create).ToJson());
            Kafka.Produce(KafkaTopics.TasksLifecycle, task.Id.ToString(), new BussinessEvent<PopugTask>(task, Events.TaskAssigned).ToJson());
            return task;
        }

        public async Task<List<PopugTask>> ReassignTasks()
        {
            var tasks = await AssignTasks(await GetTasksForAssign());
            foreach (var task in tasks)
            {
                await Update(task);
                Kafka.Produce(KafkaTopics.TasksStream, task.Id.ToString(), new StreamEvent<PopugTask>(task, Operation.Update).ToJson());
                Kafka.Produce(KafkaTopics.TasksLifecycle, task.Id.ToString(), new BussinessEvent<PopugTask>(task, Events.TaskAssigned).ToJson());
            }
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
