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
            EditableTerm = value?.Term ?? string.Empty;
            EditableDescription = value?.Description ?? string.Empty;
            UpdateButtonVisibility();
        }


        [ObservableProperty]
        private string? _editableTerm;

        partial void OnEditableTermChanged(string? value)
        {
            UpdateButtonVisibility();
        }

        [ObservableProperty]
        private string? _editableDescription;

        partial void OnEditableDescriptionChanged(string? value)
        {
            UpdateButtonVisibility();
        }

        [ObservableProperty] 
        private string? _editableTags;
        
        [ObservableProperty]
        private bool _isAddNewVisible;

        [ObservableProperty]
        private bool _isEditVisible;

        [ObservableProperty]
        private bool _isCardSelected;
        
        [RelayCommand]
        private void AddCard()
        {
            // add new card
            if(string.IsNullOrEmpty(EditableTerm) || string.IsNullOrEmpty(EditableDescription))
            {
                Shell.Current.CurrentPage.DisplayAlert("Error", "Term and description cannot be empty.", "Got it!");
                return;
            }

            var card = new Card()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTimeOffset.Now,
                Term = EditableTerm,
                Description = EditableDescription,
                Tags = _tagRepository.GetDefault()
                // TODO tags
            };

            _cardRepository.Add(card);

            _cardRepository.SaveChanges();

            ReloadAllCards();
            SelectedCard = Cards.FirstOrDefault(c => c.Id == card.Id);
        }

        [RelayCommand]
        private void EditCard()
        {
            if (SelectedCard == null) return;

            var card = _cardRepository.GetCard(SelectedCard.Id);
            if (card == null) return;

            if (string.IsNullOrEmpty(EditableTerm) || string.IsNullOrEmpty(EditableDescription))
            {
                Shell.Current.CurrentPage.DisplayAlert("Error", "Term and description cannot be empty.", "Got it!");
                return;
            }

            card.Term = EditableTerm ?? string.Empty;
            card.Description = EditableDescription ?? string.Empty;

            _cardRepository.SaveChanges();
            ReloadAllCards();

            SelectedCard = Cards.First(c => c.Id == card.Id);
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

        [RelayCommand]
        private async Task SelectTags()
        {
            var cardTagViewModel = _serviceProvider.GetService<CardTagViewModel>();
            cardTagViewModel.ModalClosed += ReloadAllCards;
            cardTagViewModel.InitializeWithSelectedCard(SelectedCard);


            var tagSelectionPage = new CardTagPage() { BindingContext = cardTagViewModel };
            await Application.Current.MainPage.Navigation.PushModalAsync(tagSelectionPage);
        }

        [RelayCommand]
        private async Task ManageTags()
        {
            var tagViewModel = _serviceProvider.GetService<TagViewModel>();
            var tagSelectionPage = new TagPage() {BindingContext = tagViewModel };
            await Application.Current.MainPage.Navigation.PushModalAsync(tagSelectionPage);
        }

        private void UpdateButtonVisibility()
        {
            var isCardSelected = SelectedCard != null;
            var isTermFilled = !string.IsNullOrWhiteSpace(EditableTerm);
            var isDescriptionFilled = !string.IsNullOrWhiteSpace(EditableDescription);

            switch (isCardSelected, isTermFilled, isDescriptionFilled)
            {
                case (false, true, true): // card is not selected, but term and description are filled in - add new
                {
                    IsAddNewVisible = true;
                    IsEditVisible = false;
                    IsCardSelected = false;
                    break;
                }

                case (false, false, _):  // card is not selected and some info is missing
                case (false, _, false):
                {
                    IsAddNewVisible = false;
                    IsEditVisible = false;
                    IsCardSelected = false;
                    break;
                }

                case (true, true, true): // card is selected, and both term and description are filled in - then  we show all buttons
                {
                    IsAddNewVisible = SelectedCard?.Term != EditableTerm || SelectedCard?.Description != EditableDescription && TagsAreTheSame();
                    IsEditVisible = SelectedCard?.Term != EditableTerm || SelectedCard?.Description != EditableDescription && TagsAreTheSame();
                    IsCardSelected = true;
                    break;
                }

                case (true, false, _):  // card is selected and some info is missing
                case (true, _, false):
                {
                    IsAddNewVisible = false;
                    IsEditVisible = false;
                    IsCardSelected = true;
                    break;
                }

                default:
                    break;
            }
        }

        private bool TagsAreTheSame()
        {
            // TODO
            return true;
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
