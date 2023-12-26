using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Services;

namespace SongCollector;

public partial class PopUp
{
    public static event EventHandler<NameAddEvent> NameAdded;
    public PopUp()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		MopupService.Instance.PopAllAsync();
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        MopupService.Instance.PopAllAsync();
        NameAdded.Invoke(this,new NameAddEvent(Name.Text));
    }
}