namespace SongCollector;

public partial class PlayerControl : Grid
{
	bool isPlaying;
	string play = "<ImageButton.Source>\r\n                        <FontImageSource\r\n             FontFamily=\"Icons\"\r\n                 Size=\"15\"\r\n                Glyph=\"&#xE801;\"\r\n                Color=\"{StaticResource Gray400}\" />\r\n                    </ImageButton.Source>";
	string pause = "<ImageButton.Source>\r\n                        <FontImageSource\r\n             FontFamily=\"Icons\"\r\n                 Size=\"15\"\r\n                Glyph=\"&#xE802;\"\r\n                Color=\"{StaticResource Gray400}\" />\r\n                    </ImageButton.Source>";
    public PlayerControl()
	{
		InitializeComponent();
		isPlaying = true;
	}

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
		if (isPlaying)
		{
			Ad.Source.LoadFromXaml(play);
		}
		else
		{
			Ad.Source.LoadFromXaml(play);
		}
    }
}