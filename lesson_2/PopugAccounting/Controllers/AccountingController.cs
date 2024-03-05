using AuthorizationServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PopugTaskTracker;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PopugAccounting.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountingController : ControllerBase
    {
        public AccountingController()
        {
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<List<PopugTask>> Get()
        {
            return new List<PopugTask> { };
        }
    }
}
