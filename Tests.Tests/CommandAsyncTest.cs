using Autodesk.Revit.DB;
using SimpleRevit;
using Tests.Commands;
using Xunit;
using xUnitRevitUtils;
using Autodesk.Revit.UI;

namespace Tests.Tests;

public class CommandAsyncTest : IDisposable
{
    public CommandAsyncTest()
    {
        AppBase.ForceInMainThread = true;
    }

    public void Dispose()
    {
        AppBase.ForceInMainThread = false;
    }

    [Fact]
    public async void CommandAsyncTest2()
    {
        xru.Uiapp.PostCommand(new CommandAsync().CommandId);

        do
        {
            await Task.Delay(100);
        }
        while (CmdBase.IsRunning);

        var wall = xru.Uiapp.ActiveUIDocument.Document.GetInstances(BuiltInCategory.OST_Walls).FirstOrDefault(w => w is Wall) as Wall;

        Assert.Equal(0.2, wall?.GetParameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble() ?? 0, 0.01);
    }
}
