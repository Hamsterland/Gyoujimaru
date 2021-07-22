using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.WebSocket;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Gyoujimaru.Services.Olympics._2021.Tracker
{
    public class TrackerService
    {
        private readonly SheetsService _sheetsService;
        private readonly DiscordSocketClient _client;
        
        public TrackerService(DiscordSocketClient client)
        {
            _client = client;
            using var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read);

            var credential = GoogleCredential
                .FromStream(stream)
                .CreateScoped();

            _sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Gyoujimaru"
            });
        }

        private const string _spreadSheetId = "10W8igDyYD8kQzgklUmjXt-67hkuHIZtPElSjDuipe4c";

        public async Task AddCharacter(CharacterSubmission submission, int overridePos = -1)
        {
            var columnIndex = 3 * submission.Stage - 3;
            var rowIndex = submission.Id - 1;
            
            if (overridePos != -1)
            {
                rowIndex = overridePos;
            }

            var user = _client.GetUser(submission.ClaimantId);
            
            var cellsRequest = new Request
            {
                UpdateCells = new UpdateCellsRequest
                {
                    Start = new GridCoordinate
                    {
                        ColumnIndex = columnIndex,
                        RowIndex = rowIndex
                    },

                    Rows = new List<RowData>
                    {
                        new()
                        {
                            Values = new List<CellData>
                            {
                                new()
                                {
                                    UserEnteredValue = new ExtendedValue
                                    {
                                        StringValue = $"{user.ToString() ?? "Name not found"} ({user.Id})"
                                    },
                                    
                                    UserEnteredFormat = new CellFormat
                                    {
                                        BackgroundColor = new Color
                                        {
                                            Red = 1f,
                                            Green = 0.949019608f,
                                            Blue = 0.8f,
                                            Alpha = 0
                                        }
                                    }
                                },
                                
                                new()
                                {
                                    UserEnteredValue = new ExtendedValue
                                    {
                                        FormulaValue = $"=HYPERLINK(\"{submission.Url}\", \"{submission.Name}\")"
                                    },
                                    
                                    UserEnteredFormat = new CellFormat
                                    {
                                        BackgroundColor = new Color
                                        {
                                            Red = 1f,
                                            Green = 0.949019608f,
                                            Blue = 0.8f,
                                            Alpha = 0
                                        }
                                    }
                                }
                            }
                        }
                    },

                    Fields = "*"
                },
            };

            var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request> { cellsRequest }
            };

            var request = _sheetsService
                .Spreadsheets
                .BatchUpdate(batchUpdateRequest, _spreadSheetId);

            await request.ExecuteAsync();
        }
        
        // var request = sheetsService
        //     .Spreadsheets
        //     .Get(spreadsheetId);
        //
        // request.IncludeGridData = true;
        // request.Ranges = new[] {"'Selections'!A3:Z300"};
        //
        // return await request.ExecuteAsync();
    }
}