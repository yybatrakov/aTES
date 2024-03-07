using AuthorizationServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PopugAnalitics.Logic;
using PopugTaskTracker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static PopugAnalitics.Logic.AnaliticsLogic;

namespace PopugAnalitics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnaliticsController : ControllerBase
    {
        public AnaliticsController(AnaliticsLogic analiticsLogic)
        {
            AnaliticsLogic = analiticsLogic;
        }

        public AnaliticsLogic AnaliticsLogic { get; }

        [HttpGet]
        [Route("expensive_tasks")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<List<ExpensiveTaskResult>> GetExpensiveTasks(DateTime from, DateTime to)
        {
            return await AnaliticsLogic.GetExpensiveTasks(from, to);

        }
        [HttpGet]
        [Route("top_management_money")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<int> GetTopManagementMoneyForToday()
        {
            return await AnaliticsLogic.GetTopManagementMoneyForToday();
        }
        [HttpGet]
        [Route("popugs_in_minus_today")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<List<string>> GetPopugsInMinusForToday()
        {
            return await AnaliticsLogic.GetPopugsInMinusForToday();
        }





    }
}
