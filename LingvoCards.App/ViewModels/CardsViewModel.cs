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
            ReloadAllCardsAsync().ConfigureAwait(false);
        }

        [ObservableProperty]
        private ObservableCollection<Card> _cards = new();
        

        [ObservableProperty]
        private ObservableCollection<Tag> _defaultTags = new();

        [ObservableProperty]
        private string? _searchTerm;

        // Field to hold a reference to the debounce task
        private Task? _searchDebounceTask = null;

        // The cancellation token source to cancel the previous task when a new character is typed
        private CancellationTokenSource? _cts = null;

        partial void OnSearchTermChanged(string? value)
        {
            // Cancel the previous task if it exists
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            // Replace the previous debounce task with a new one
            _searchDebounceTask = Task.Delay(TimeSpan.FromSeconds(0.5), token).ContinueWith(async t =>
            {
                if (!t.IsCanceled)
                {
                    // Perform the search
                    await PerformSearchAsync();
                }
            }, token, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
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

            cardEditViewModel!.ModalClosed += ReloadAllCardsAsync;
            await cardEditViewModel.InitializeWithSelectedCardAsync(SelectedCard);

            var cardEditPage = new CardEditPage() { BindingContext = cardEditViewModel };
            await Application.Current?.MainPage?.Navigation.PushModalAsync(cardEditPage)!;
        }

        [RelayCommand]
        private async Task DeleteCardAsync()
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
            await _cardRepository.SaveChangesAsync();
            SelectedCard = null;
            await ReloadAllCardsAsync();
        }

        private async Task ReloadAllCardsAsync()
        {
            var allCards = (await _cardRepository.GetAllAsync()).OrderByDescending(c => c.CreatedOn);
            Cards = new ObservableCollection<Card>(allCards);
            SelectedCard = Cards.FirstOrDefault(c => c.Id == SelectedCard?.Id);
        }

        private async Task PerformSearchAsync()
        {
            Cards = string.IsNullOrEmpty(SearchTerm)
                ? new ObservableCollection<Card>(await _cardRepository.GetAllAsync())
                : new ObservableCollection<Card>(await _cardRepository.GetByTermOrDescriptionAsync(SearchTerm));
            
            SelectedCard = null;
        }
    }
}
