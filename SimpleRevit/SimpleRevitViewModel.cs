using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleRevit;

public partial class SimpleRevitViewModel : ObservableObject
{
    [ObservableProperty]
    double _Percent = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RemainTime))]
    TimeSpan _ElapsedTime;

    /// <summary>
    /// the remain time about this command.
    /// </summary>
    public TimeSpan RemainTime
    {
        get
        {
            if (Percent == 0) return TimeSpan.Zero;

            var ratio = Percent / 100;
            return TimeSpan.FromSeconds(ElapsedTime.TotalSeconds / ratio * (1 - ratio));
        }
    }
}