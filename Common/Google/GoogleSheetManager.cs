using System.Net.Mime;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace Approvers.King.Common;

public static class GoogleSheetManager
{
    private static readonly string[] Scopes = [SheetsService.Scope.SpreadsheetsReadonly];
    private static ICredential? _credential;
    
    public static async Task<IReadOnlyDictionary<string, GoogleSheet>> GetAllSheetsAsync(IEnumerable<string> sheetNames)
    {
        if (_credential == null)
        {
            await using var stream = new FileStream(EnvironmentManager.GoogleCredentialFilePath, FileMode.Open, FileAccess.Read);
            _credential = (await GoogleCredential.FromStreamAsync(stream, CancellationToken.None)).CreateScoped(Scopes).UnderlyingCredential;
        }
        var sheetService = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = _credential
        });
        var request = sheetService.Spreadsheets.Values.BatchGet(EnvironmentManager.GoogleMasterSheetId);
        request.Ranges = sheetNames.ToList();
        var sheet = await request.ExecuteAsync();
        return sheet.ValueRanges.Select(x => new GoogleSheet(x)).ToDictionary(x => x.SheetName, x => x);
    }
}