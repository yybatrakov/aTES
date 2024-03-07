using AuthorizationServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PopugTaskTracker.Logic;
using System;
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
        public async Task<List<TaskDb>> Get() => await taskLogic.Get();

        [HttpPost("Create")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName)]
        public async Task<TaskDb> Create(string description) => await taskLogic.CreateTask(new TaskDb() { Description = description, PublicId = Guid.NewGuid().ToString() });

        [HttpPost("Reassign")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin,Manager")]
        public async Task<List<TaskDb>> Reassign() => await taskLogic.ReassignTasks();

        [HttpPost("Complete")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName)]
        public async Task<TaskDb> Complete(int taskId) => await taskLogic.CompleteTask(taskId);
    }
}
