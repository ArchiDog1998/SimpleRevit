using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace SimpleRevit;

/// <summary>
/// A base command class for MVVM design.
/// </summary>
/// <typeparam name="TView">your view.</typeparam>
/// <typeparam name="TViewModel">your view model.</typeparam>
public abstract class CmdBaseMvvm<TView, TViewModel> : CmdBase
    where TView : Window
    where TViewModel : ObservableObject
{
    /// <summary>
    /// The window.
    /// </summary>
    public TView View { get; private set; }

    /// <summary>
    /// The view model.
    /// </summary>
    public TViewModel ViewModel { get; private set; }

    DateTime _startTime;

    /// <summary>
    /// Close the view after finishing CMD.
    /// </summary>
    protected virtual bool CloseViewWhenFinished => true;

    /// <summary>
    /// How to create the view.
    /// </summary>
    /// <returns></returns>
    public virtual TView CreateView()
    {
        return Activator.CreateInstance<TView>();
    }

    /// <summary>
    /// How to create the view model.
    /// </summary>
    /// <returns></returns>
    public virtual TViewModel CreateViewModel(TView window)
    {
        return Activator.CreateInstance<TViewModel>();
    }

    /// <summary>
    /// The things before <seealso cref="CmdBase.ExecuteMain"/> in main thread.
    /// </summary>
    public override void PreExecute()
    {
        var thread = new Thread(() =>
        {
            View = CreateView();
            View.DataContext = ViewModel = CreateViewModel(View);
            View.ShowDialog();
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        _startTime = DateTime.Now;

        base.PreExecute();
    }

    /// <summary>
    /// How to execute your command. If your <see cref="CmdBase.UseRevitAsync"/> is set to true. Please use <seealse cref="CmdBase.RunAsync(Action)"/> to write data to revit document.
    /// </summary>
    public override void PostExecute()
    {
        if (CloseViewWhenFinished) CloseView();
        base.PostExecute();

        void CloseView()
        {
            for (int i = 0; i < 10; i++)
            {
                if (View != null) break;
                Task.Delay(100).Wait();
            }

            View?.Dispatcher.Invoke(View.Close);
        }
    }


    /// <summary>
    /// Update your calculating percent while running.
    /// </summary>
    /// <param name="percent"></param>
    public void UpdatePercent(double percent)
    {
        if (ViewModel is not SimpleRevitViewModel viewmodel) return;
        if (percent == 0)
        {
            _startTime = DateTime.Now;
        }
        viewmodel.Percent = percent;
        viewmodel.ElapsedTime = DateTime.Now - _startTime;
    }
}