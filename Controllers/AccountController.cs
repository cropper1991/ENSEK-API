using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ENSEK_API.Structures;
using Microsoft.AspNetCore.Http;
using System.IO;
using ENSEK_API.Database;

namespace ENSEK_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //Controller to handle all API requests to do with Meter Readings (Uploading of Meter Reading CSV File)
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/get-accounts")]
        //Entry Point for Getting Account Information by HTTP GET
        public Account[] GetAccounts()
        {
            try
            {
                return DatabaseHandler.GetAccounts(_logger);
            }
            catch (Exception Ex)
            { //Catch any unexpected Exceptions and Log via Logger
                _logger.LogError(Ex, "Account Retrieval Failure");
            }

            return null;
        }
    }
}