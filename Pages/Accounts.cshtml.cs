using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Text.Json;
using ENSEK_API.Structures;

namespace Claims_Application.Pages
{
    //Provides Model for the Loss Types View - Viewing of all Loss Types (providing logged in)
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
                {
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; //Skip SSL Verification (would not be in Production Code)
                    using (HttpClient client = new HttpClient(handler))
                    {
                        using (var Response = await client.GetAsync("https://localhost:5001/get-accounts"))
                        {
                            if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                Accounts = JsonSerializer.Deserialize<Account[]> (await Response.Content.ReadAsStringAsync());
                                return Page();
                            }
                            else
                            {
                                ModelState.Clear();
                                ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
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
