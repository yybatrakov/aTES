using AuthorizationServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PopugTaskTracker.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Security.Claims;

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
        private string GetUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<List<TaskDb>> Get() => await taskLogic.Get();

        [HttpPost("Create")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName)]
        public async Task<TaskDb> Create(string title, string description) => await taskLogic.CreateTask(new TaskDb() { Title = title, Description = description, JiraId = Guid.NewGuid().ToString(), PublicId = Guid.NewGuid().ToString() });

        [HttpPost("Reassign")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin,Manager")]
        public async Task<List<TaskDb>> Reassign() => await taskLogic.ReassignTasks();

        [HttpPost("Complete")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName)]
        public async Task<TaskDb> Complete(int taskId)
        {
            var task = await taskLogic.Get(taskId);
            if (task.AssignedUserId != GetUserId())
            {
                Unauthorized();
            }
            return await taskLogic.CompleteTask(taskId);
        } 
    }
}
