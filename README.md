# Simple Revit

A toolkit for simplifying your Revit plugin development.

## Installation

1. Install [Revit Templates](https://github.com/Nice3point/RevitTemplates).

2. Add this as a submodule to your project:

   ```powershell
   git submodule add https://github.com/ArchiDog1998/SimpleRevit
   ```

3. Add it to your plugin's CSProj file:

   ```xml
   <ItemGroup>
   	<ProjectReference Include="..\SimpleRevit\SimpleRevit\SimpleRevit.csproj"/>
   </ItemGroup>
   ```

## Features

Here is an [example]((https://github.com/ArchiDog1998/SimpleRevit/Tests)) of this repo.

### External command

All these commands are inherited from [ExternalCommand](https://github.com/Nice3point/RevitToolkit/blob/develop/Nice3point.Revit.Toolkit/External/ExternalCommand.cs). So for further usage, please check this [wiki](https://github.com/Nice3point/RevitToolkit#external-command).

#### CmdBase

CmdBase contains some definitions of [Revit.Async](https://github.com/KennanChan/Revit.Async). If you want to use it, please set `UseRevitAsync` to true.

```c#
using Autodesk.Revit.Attributes;
using SimpleRevit;

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
```

If your `UseRevitAsync` is true, please make all your writing stuff to `RevitTask.RunAsync`.

#### CmdBaseMvvm<TView, TViewModel>****

CmdBaseMvvm is an mvvm-ready command class. The View it created will not be irresponsible while the command is running.  Please don't forget to define your view and your view model.

```c#
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Serilog;
using SimpleRevit;
using Tests.ViewModels;
using Tests.Views;

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
            var param = wall.GetParameter(BuiltInParameter.WALL_BASE_OFFSET);
            param.Set(0.2);
            UpdatePercent(100);

            Task.Delay(1000).Wait();
            Log.Debug("Succeed!");
        }

        trans.Commit();
    }
}
```

### External application

All these commands are inherited from [ExternalApplication](https://github.com/Nice3point/RevitToolkit/blob/develop/Nice3point.Revit.Toolkit/External/ExternalApplication.cs). So for further usage, please check this [wiki](https://github.com/Nice3point/RevitToolkit#external-application).

#### AppBase

Almost everything is down here, don't forget to create one application that is inherited from this.

### Attributes

There are two attributes for you to create the ribbon ui easily.

The `Cmd` attribute is designed for ui showing in the ribbon panel. And the `Priority` attribute is designed for changing the order of the loading button in the ribbon panel.

### Extensions

There are also some extensions for parameter writing and creating. Try them by yourself!