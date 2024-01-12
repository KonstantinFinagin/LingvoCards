using LingvoCards.App.ViewModels;
using LingvoCards.Domain.Model;

namespace LingvoCards.App.Views;

public partial class CardsPage : ContentPage
{
    public CardsPage()
	{
		InitializeComponent();
        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<CardsViewModel>();
    }
}