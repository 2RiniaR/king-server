using Google.Apis.Sheets.v4.Data;

namespace Approvers.King.Common;

public class GoogleSheet
{
    public string SheetName { get; }
    public IList<IList<object>> Values { get; }
    public int RowCount { get; }
    public int ColumnCount { get; }

    public GoogleSheet(ValueRange valueRange)
    {
        SheetName = valueRange.Range.Split('!')[0].Replace("'", "");
        Values = valueRange.Values;
        RowCount = Values.Count;
        ColumnCount = Values.Count > 0 ? Values[0].Count : 0;
    }

    public string Get(int row, int column)
    {
        return (string)Values[row][column];
    }
}
