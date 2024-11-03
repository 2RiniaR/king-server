using Approvers.King.Common;
using Microsoft.EntityFrameworkCore;

namespace Approvers.King.Events;

public class MonthlyResetPresenter : SchedulerJobPresenterBase
{
    protected override async Task MainAsync()
    {
        await using var app = AppService.CreateSession();
        
        // ToDo: 課金額ランキング出したい
        
        await app.Users.ForEachAsync(user => user.ResetMonthlyPurchase());
        await app.SaveChangesAsync();
    }
}
