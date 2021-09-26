using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Text.Json;
using ENSEK_API.Structures;

namespace ENSEK_API.Pages
{
    //Provides Model for the Accounts View - Viewing of all Accounts
    public class AccountsModel : PageModel
    {
        private readonly ILogger<AccountsModel> _logger;
        public Account[] Accounts;

        public AccountsModel(ILogger<AccountsModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                { //Create HttpClientHandler - So we can override default SSL Logic/Behaviour
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; //Skip SSL Verification (would not be in Production Code)
                    using (HttpClient client = new HttpClient(handler))
                    { //Create HttpClient
                        using (var Response = await client.GetAsync("https://localhost:5001/get-accounts"))
                        { //Request Data from Web API for Accounts
                            if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                            { //Request Successful - Parse Result
                                Accounts = JsonSerializer.Deserialize<Account[]> (await Response.Content.ReadAsStringAsync());
                                return Page();
                            }
                            else
                            { //Request Failure - Add Data to Model State
                                ModelState.Clear();
                                ModelState.AddModelError("APIResult", "Username or Password is Incorrect");
                                return Page();
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Error during File Upload - FrontEnd");
            }

            return Page();
        }
    }
}
