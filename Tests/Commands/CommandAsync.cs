using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using SimpleRevit;
using Tests.ViewModels;
using Tests.Views;

namespace Tests.Commands;

[Cmd(Name = "Async One")]
[Cmd(Image = "Resources/Icons/RibbonIcon16.png")]
[Cmd(LargeImage = "Resources/Icons/RibbonIcon32.png")]
[Cmd(Panel = "Commands")]
[Transaction(TransactionMode.Manual)]
public class CommandAsync : CmdBaseMvvm<TestsView, TestsViewModel>
{
    protected override bool UseRevitAsync => true;

    public override void ExecuteMain()
    {
        Task.WaitAll(Document.GetInstances(BuiltInCategory.OST_Walls)
            .Select(wall => wall.GetParameter(BuiltInParameter.WALL_BASE_OFFSET).SetAsync(0.2))
            //.Select(wall => wall.CreateSharedParameterAsync("TestOne", SpecTypeId.Int.Integer, BuiltInParameterGroup.INVALID))
            .ToArray());
    }
}