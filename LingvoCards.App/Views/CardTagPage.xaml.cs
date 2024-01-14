using LingvoCards.App.ViewModels;

namespace LingvoCards.App.Views;

public partial class CardTagPage : ContentPage
{
	public CardTagPage()
	{
        InitializeComponent();
        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<CardTagViewModel>();
    }
}