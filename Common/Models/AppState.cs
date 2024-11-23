using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Common;

public static class AppStateDbSetExtensions
{
    public async static Task<int?> GetIntAsync(this DbSet<AppState> source, AppStateType type)
    {
        var state = await source.FindAsync(type);
        return int.TryParse(state?.Value ?? string.Empty, out var result) ? result : null;
    }

    public async static Task SetIntAsync(this DbSet<AppState> source, AppStateType type, int value)
    {
        var state = await source.FindAsync(type);
        var valueString = value.ToString();
        if (state == null)
        {
            source.Add(new AppState { Type = type, Value = valueString });
        }
        else
        {
            state.Value = valueString;
        }
    }

    public async static Task<string?> GetStringAsync(this DbSet<AppState> source, AppStateType type)
    {
        var state = await source.FindAsync(type);
        return state?.Value;
    }

    public async static Task SetStringAsync(this DbSet<AppState> source, AppStateType type, string value)
    {
        var state = await source.FindAsync(type);
        var valueString = value;
        if (state == null)
        {
            source.Add(new AppState { Type = type, Value = valueString });
        }
        else
        {
            state.Value = valueString;
        }
    }
}

public class AppState
{
    [Key] public AppStateType Type { get; set; }
    public string Value { get; set; } = null!;
}

public enum AppStateType
{
    RareReplyProbabilityPermillage,
}
