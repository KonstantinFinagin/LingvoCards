using LingvoCards.App.ViewModels;

namespace LingvoCards.App.Views;

public partial class TagPage : ContentPage
{
	public TagPage()
	{
        InitializeComponent();
        BindingContext = Application.Current?.Handler?.MauiContext?.Services.GetService<TagViewModel>();
    }
}