using AuthorizationServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PopugTaskTracker.Logic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PopugTaskTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskLogic taskLogic;

        public TaskController(TaskLogic taskLogic)
        {
            this.taskLogic = taskLogic;
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<List<PopugTask>> Get() => await taskLogic.Get();

        [HttpPost("Create")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName)]
        public async Task<PopugTask> Create(string description) => await taskLogic.CreateTask(new PopugTask() { Description = description });

        [HttpPost("Reassign")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin,Manager")]
        public async Task<List<PopugTask>> Reassign() => await taskLogic.ReassignTasks();

        [HttpPost("Complete")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName)]
        public async Task<PopugTask> Complete(int taskId) => await taskLogic.CompleteTask(taskId);//TODO, проверить пользователя
    }
}
