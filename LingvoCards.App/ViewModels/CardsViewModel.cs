using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LingvoCards.App.Views;
using LingvoCards.Dal.Repositories;
using LingvoCards.Domain.Model;

namespace LingvoCards.App.ViewModels
{
    public partial class CardsViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CardRepository _cardRepository;
        private readonly TagRepository _tagRepository;

        public CardsViewModel(CardRepository cardRepository, TagRepository tagRepository, IServiceProvider serviceProvider)
        {
            _cardRepository = cardRepository;
            _tagRepository = tagRepository;
            _serviceProvider = serviceProvider;
            ReloadAllCards();
        }

        [ObservableProperty]
        private ObservableCollection<Card> _cards = new();
        

        [ObservableProperty]
        private ObservableCollection<Tag> _defaultTags = new();

        [ObservableProperty]
        private string? _searchTerm;

        partial void OnSearchTermChanged(string? value)
        {
            PerformSearch();
        }

        [ObservableProperty]
        private Card? _selectedCard;
        
        partial void OnSelectedCardChanged(Card? value)
        {
            IsCardSelected = value != null;
            AddButtonText = value == null ? "Add new" : "View/Edit";
        }

        [ObservableProperty]
        private string _addButtonText = "Add new";

        [ObservableProperty] 
        private string? _editableTags;

        [ObservableProperty]
        private bool _isCardSelected;

        [RelayCommand]
        private void ClearSelection()
        {
            SelectedCard = null;
        }

        [RelayCommand]
        private async Task AddOrUpdateCard()
        {
            var cardEditViewModel = _serviceProvider.GetService<CardEditViewModel>();

            cardEditViewModel!.ModalClosed += ReloadAllCards;
            cardEditViewModel.InitializeWithSelectedCard(SelectedCard);

            var cardEditPage = new CardEditPage() { BindingContext = cardEditViewModel };
            await Application.Current?.MainPage?.Navigation.PushModalAsync(cardEditPage)!;
        }

        [RelayCommand]
        private async Task DeleteCard()
        {
            if (SelectedCard == null)
            {
                return;
            }
            
            var deletionApproved = await Shell.Current.CurrentPage.DisplayAlert("Delete", "Delete selected?", "Delete", "Cancel");
            if (!deletionApproved)
            {
                return;
            }

            _cardRepository.Remove(SelectedCard);
            _cardRepository.SaveChanges();
            SelectedCard = null;
            ReloadAllCards();
        }

        private void ReloadAllCards()
        {
            var allCards = _cardRepository.GetAll().OrderByDescending(c => c.CreatedOn);
            Cards = new ObservableCollection<Card>(allCards);
            SelectedCard = Cards.FirstOrDefault(c => c.Id == SelectedCard?.Id);
        }

        private void PerformSearch()
        {
            Cards = string.IsNullOrEmpty(SearchTerm)
                ? new ObservableCollection<Card>(_cardRepository.GetAll())
                : new ObservableCollection<Card>(_cardRepository.GetByTermOrDescription(SearchTerm));
            
            SelectedCard = null;
        }
    }
}
