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
    //Provides Model for the Loss Types View - Viewing of all Loss Types (providing logged in)
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        //Handle for Postback (File Upload)
        public async Task<IActionResult> OnPost()
        {
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                { //Create HttpClientHandler - So we can override default SSL Logic/Behaviour
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator; //Skip SSL Verification (would not be in Production Code)
                    using (HttpClient client = new HttpClient(handler))
                    { //Create HttpClient
                        using (var form = new MultipartFormDataContent())
                        { //Create MultipartFormDataContent 
                            using (var reader = new BinaryReader(Request.Form.Files[0].OpenReadStream()))
                            { //Use Binary Reader to Read Contents of File
                                using (var fileContent = new ByteArrayContent(reader.ReadBytes((int)Request.Form.Files[0].Length)))
                                { //Create ByteArrayContent to Represent File

                                    //Add File to MultipartFormDataContent and post to Web API
                                    form.Add(fileContent, "file", Request.Form.Files[0].FileName);
                                    using (var Response = await client.PostAsync("https://localhost:5001/meter-reading-uploads", form))
                                    {
                                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                                        { //Request Successful - Retrieve Response
                                            ViewData["UploadResponse"] = JsonSerializer.Deserialize<MeterReadingUploadResult>(await Response.Content.ReadAsStringAsync());
                                            return Page();
                                        }
                                        else
                                        {//Request Failure
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
