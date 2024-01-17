using LingvoCards.App.ViewModels;

namespace LingvoCards.App.Views;

public partial class PracticePage : ContentPage
{
	public PracticePage( )
	{
		InitializeComponent();
        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<PracticeViewModel>();

        CardBack.IsVisible = false;
    }

    private async void Card_Tapped(object sender, EventArgs e)
    {
        // Assuming front and back views are named CardFront and CardBack
        bool isFrontVisible = CardFront.IsVisible;

        // Start rotation to 90 degrees (halfway)

        if (isFrontVisible)
        {
            TextFront.FadeTo(0, 250, Easing.Linear);
        }
        else
        {
            TextBack.FadeTo(0, 250, Easing.Linear);
        }

        await CardGrid.RotateYTo(90, 250, Easing.Linear);

        // Toggle visibility of front and back views
        CardFront.IsVisible = !isFrontVisible;
        CardBack.IsVisible = isFrontVisible;

        if (!isFrontVisible)
        {
            Task.Delay(150).ContinueWith(c => TextFront.FadeTo(1, 250, Easing.Linear));
        }
        else
        {
            Task.Delay(150).ContinueWith(c => TextBack.FadeTo(1, 250, Easing.Linear));
        }
        


        // Complete rotation to 180 degrees
        await CardGrid.RotateYTo(180, 250, Easing.Linear);

        // Reset rotation to 0 without animation for the next flip
        CardGrid.RotationY = 0;
    }
}