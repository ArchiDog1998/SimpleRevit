using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Serilog;
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
            wall.CreateSharedParameter("TestOne", SpecTypeId.Int.Integer, BuiltInParameterGroup.INVALID);
        }

        trans.Commit();
    }
}
