using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using SimpleRevit;
using Tests.ViewModels;
using Tests.Views;

namespace Tests.Commands;

[Cmd(Name = "Normal One")]
[Cmd(Image = "Resources/Icons/RibbonIcon16.png")]
[Cmd(LargeImage = "Resources/Icons/RibbonIcon32.png")]
[Cmd(Panel = "Commands")]
[Transaction(TransactionMode.Manual)]
public class CommandNormal : CmdBaseMvvm<TestsView, TestsViewModel>
{
    protected override bool UseRevitAsync => false;

    public override void ExecuteMain()
    {
        using var trans = new Transaction(Document);
        trans.Start("Test");
        foreach (var wall in Document.GetInstances(BuiltInCategory.OST_Walls))
        {
            wall.GetParameter(BuiltInParameter.WALL_BASE_OFFSET).Set(0.2);
            wall.CreateSharedParameter("TestOne", ParameterType.Integer, BuiltInParameterGroup.INVALID);
        }
        trans.Commit();

        for (int i = 0; i < 100; i++) 
        {
            Task.Delay(100).Wait();
            UpdatePercent(i);
        }
    }
}
