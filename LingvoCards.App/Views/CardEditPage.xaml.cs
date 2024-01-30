using LingvoCards.App.ViewModels;

namespace LingvoCards.App.Views;

public partial class CardEditPage : ContentPage
{
    public CardEditPage()
	{
		InitializeComponent();
        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<CardsViewModel>();
    }
}