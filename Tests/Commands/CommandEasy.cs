using Autodesk.Revit.Attributes;
using SimpleRevit;

namespace Tests.Commands;

[Cmd(Name = "Easy One")]
[Cmd(Panel = "Commands")]
[Transaction(TransactionMode.Manual)]
public class CommandEasy : CmdBase
{
    protected override bool UseRevitAsync => false;

    public override void ExecuteMain()
    {
    }
}
