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
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; //Skip SSL Verification (would not be in Production Code)
                    using (HttpClient client = new HttpClient(handler))
                    {
                        using (var form = new MultipartFormDataContent())
                        {
                            using (var reader = new BinaryReader(Request.Form.Files[0].OpenReadStream()))
                            {
                                using (var fileContent = new ByteArrayContent(reader.ReadBytes((int)Request.Form.Files[0].Length)))
                                {
                                    MultipartFormDataContent multiContent = new MultipartFormDataContent();
                                    multiContent.Add(fileContent, "file", Request.Form.Files[0].FileName);
                                    using (var Response = await client.PostAsync("https://localhost:5001/meter-reading-uploads", multiContent))
                                    {
                                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                                        {
                                            String ResponseText = await Response.Content.ReadAsStringAsync();
                                            return Page();
                                        }
                                        else
                                        {
                                            ModelState.Clear();
                                            ModelState.AddModelError("AccountStatus", "Failed to Retrieve Accounts");
                                            return Page();
                                        }
                                    }
                                }
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
