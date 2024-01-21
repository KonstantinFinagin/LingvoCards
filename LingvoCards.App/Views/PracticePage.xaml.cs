using LingvoCards.App.ViewModels;

namespace LingvoCards.App.Views;

public partial class PracticePage : ContentPage
{
	public PracticePage( )
	{
		InitializeComponent();
        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<PracticeViewModel>();

        CardBack.IsVisible = false;
        TextBack.Opacity = 0;
    }

    private async void Card_Tapped(object sender, EventArgs e)
    {
        var vm = BindingContext as PracticeViewModel;
        if (vm == null) throw new Exception("Binding context not set");

        // Assuming front and back views are named CardFront and CardBack
        bool isFrontVisible = CardFront.IsVisible;

        // disable I know button
        vm.IsIKnowButtonVisible = false;


        CardLevelTextFront.Opacity = 0;
        CardLevelTextBack.Opacity = 0;
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
        if (isFrontVisible)
        {
            vm.DropLevelCommand.Execute(null);
        }

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

        CardLevelTextFront.Opacity = 0.5;
        CardLevelTextBack.Opacity = 0.5;

        // Reset rotation to 0 without animation for the next flip
        CardGrid.RotationY = 0;
    }

    private async void Button_Previous_OnClicked(object? sender, EventArgs e)
    {
        var vm = BindingContext as PracticeViewModel;
        if (vm == null) throw new Exception("Binding context not set");

        // Animate current card flying to the right
        await CardGrid.TranslateTo(this.Width, 0, 250, Easing.Linear);
        CardGrid.TranslationX = -this.Width; // Reset position offscreen to the left

        vm.IsIKnowButtonVisible = true;
        CardFront.IsVisible = true;
        CardBack.IsVisible = false;
        TextFront.Opacity = 1;
        TextBack.Opacity = 0;

        // Load previous card content here...
        vm.PreviousCommand.Execute(null);
        // call previous command

        // Animate previous card coming in from the left
        await CardGrid.TranslateTo(0, 0, 250, Easing.Linear);
    }

    private async void Button_Next_OnClicked(object? sender, EventArgs e)
    {
        var vm = BindingContext as PracticeViewModel;
        if (vm == null) throw new Exception("Binding context not set");


        // Animate current card flying to the left
        await CardGrid.TranslateTo(-this.Width, 0, 250, Easing.Linear);
        CardGrid.TranslationX = this.Width; // Reset position offscreen to the right

        vm.IsIKnowButtonVisible = true;
        CardFront.IsVisible = true;
        CardBack.IsVisible = false;
        TextFront.Opacity = 1;
        TextBack.Opacity = 0;

        // Load next card content here...
        // call next command
        vm.NextCommand.Execute(null);

        // Animate next card coming in from the right
        await CardGrid.TranslateTo(0, 0, 250, Easing.Linear);
    }

    private async void Button_IKnow_OnClicked(object? sender, EventArgs e)
    {
        var vm = BindingContext as PracticeViewModel;
        if (vm == null) throw new Exception("Binding context not set");

        vm.IsIKnowButtonVisible = false;

        // Animation: scale the card down to zero in 100 ms
        await CardGrid.ScaleTo(0, 100, Easing.Linear);

        // Updating the level of the card
        vm.UpgradeLevelCommand.Execute(null);

        // Animation: scale the card back to original size (1) in 100 ms
        await CardGrid.ScaleTo(1, 100, Easing.Linear);
    }

}