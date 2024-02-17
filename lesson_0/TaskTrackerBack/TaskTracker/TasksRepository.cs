using LinqToDB;
using tasks.Api.Data;

namespace TaskTracker
{
    public class TasksRepository
    {
        private readonly IConfiguration configuration;

        public TasksRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public List<tasks.Api.Data.Task> GetTasks()
        {
            using var db = new tasksDb(configuration);
            return db.Tasks.ToList();
        }
        public tasks.Api.Data.Task GetTask(int taskId)
        {
            using var db = new tasksDb(configuration);
            return db.Tasks.Where(t=>t.TaskId == taskId).Single();
        }
        public tasks.Api.Data.Task AddTask(tasks.Api.Data.Task task)
        {
            using var db = new tasksDb(configuration);
            return GetTask(db.InsertWithInt32Identity(task));

        }
    }
}
