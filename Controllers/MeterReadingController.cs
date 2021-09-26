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
    public class MeterReadingController : ControllerBase
    {
        private readonly ILogger<MeterReadingController> _logger;

        public MeterReadingController(ILogger<MeterReadingController> logger)
        {
            _logger = logger;
        }

        //Internal Handler for Processing a given Meter Reading CSV file
        private MeterReadingUploadResult MeterReadingHandler(IFormFile File)
        {
            string Line; //Stores Individual Line in File
            string[] Split; //Stores Split of CSV Line
            int AccountID; //Used to Temporarily Store AccountID during Parsing
            DateTime Date; //Used to Temporarily Store Date during Parsing
            MeterReading Reading = new MeterReading(); //Represents a new Meter Reading
            MeterReadingUploadResult Result = new MeterReadingUploadResult(); //Provides the End Count of Successful / Failures

            using (Stream Stream = File.OpenReadStream())
            { //Open File Stream
                using (StreamReader Reader = new StreamReader(Stream))
                { //Open Reader on File Stream
                    Reader.ReadLine(); //Skip Header Line of CSV file
                    while (!Reader.EndOfStream)
                    { //While not Reached End of Stream - Continue Reading File
                        try
                        {
                            Line = Reader.ReadLine(); //Read Line in File
                            Split = Line.Split(','); //Split on Comma (CSV)
                            if (Split.Length == 3)
                            { //Valid Line (3 Values Supplied - AccountID, Date, MeterReading)
                                if (Split[2].Length == 5 && Split[2].All(Char.IsDigit))
                                { //Proivided in Format NNNNN
                                    if (int.TryParse(Split[0], out AccountID) && DateTime.TryParse(Split[1], out Date))
                                    { //Able to Parse Values of AccountID and Date - Continue Processing
                                        Reading.AccountID = AccountID;
                                        Reading.Date = Date;
                                        Reading.Reading = Split[2];

                                        if (DatabaseHandler.AddMeterReading(Reading, _logger))
                                        {
                                            Result.SuccessfulCount += 1;
                                        }
                                        else
                                        {
                                            Result.FailedCount += 1;
                                        }
                                    }
                                    else
                                    { //Unable to Parse Values - Skip Record and treat as Failure
                                        Result.FailedCount += 1;
                                    }
                                }
                                else
                                { //Invalid Format Provided - Skip Line
                                    Result.FailedCount += 1;
                                }
                            }
                            else
                            { //Skip Reading - Not in Valid Format
                                Result.FailedCount += 1;
                            }
                        }
                        catch (Exception Ex)
                        { //Capture any Unexpected Exceptions and Log via Logger - Marking as a Failure
                            _logger.LogError(Ex, "Meter Reading Upload Failure");
                            Result.FailedCount += 1;
                        }
                    }
                }
            }

            return Result;
        }


        [HttpPost("/meter-reading-uploads")]
        //Entry Point for Meter Reading Uploads via POST
        public MeterReadingUploadResult UploadMeterReadings(IFormFile File)
        {
            try
            {
                if (Path.GetExtension(File.FileName) == ".csv")
                { //Correct Extension on File - Continue Processing
                    return MeterReadingHandler(File);
                }
                else
                { //Reject File as it is assumed it's not a CSV File
                    throw new Exception("Invalid File Type Uploaded!");
                }
            }
            catch (Exception Ex)
            { //Catch any unexpected Exceptions and Log via Logger
                _logger.LogError(Ex, "Meter Reading Upload Failure");
            }

            return new MeterReadingUploadResult();
        }
    }
}