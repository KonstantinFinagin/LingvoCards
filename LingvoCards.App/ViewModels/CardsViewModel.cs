using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LingvoCards.Dal.Repositories;
using LingvoCards.Domain.Model;

namespace LingvoCards.App.ViewModels
{
    public partial class CardsViewModel : ObservableObject
    {
        private readonly CardRepository _cardRepository;

        public CardsViewModel(CardRepository cardRepository)
        {
            _cardRepository = cardRepository;
            UpdateAllCards();
        }

        [ObservableProperty]
        private ObservableCollection<Card> _cards = new();

        [ObservableProperty]
        private ObservableCollection<Tag> _currentTags = new();

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
            CurrentTags = new ObservableCollection<Tag>(value?.Tags ?? new List<Tag>());
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
        private bool _isDeleteVisible;
        
        [RelayCommand]
        private void AddCard()
        {
            // add new card
            if(string.IsNullOrEmpty(EditableTerm) || string.IsNullOrEmpty(EditableDescription))
            {
                Shell.Current.CurrentPage.DisplayAlert("Error", "Term and description cannot be empty.", "Got it!");
                return;
            }

            _cardRepository.Add(new Card()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTimeOffset.Now,
                Term = EditableTerm,
                Description = EditableDescription,
                // TODO tags
            });

            _cardRepository.SaveChanges();
            UpdateAllCards();
        }

        [RelayCommand]
        private void EditCard()
        {
            if (SelectedCard == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(EditableTerm) || string.IsNullOrEmpty(EditableDescription))
            {
                Shell.Current.CurrentPage.DisplayAlert("Error", "Term and description cannot be empty.", "Got it!");
                return;
            }

            SelectedCard.Term = EditableTerm ?? String.Empty;
            SelectedCard.Description = EditableDescription ?? String.Empty;

            _cardRepository.SaveChanges();
            UpdateAllCards();
        }

        [RelayCommand]
        private void DeleteCard()
        {
            if (SelectedCard == null)
            {
                return;
            }

            _cardRepository.Remove(SelectedCard);
            _cardRepository.SaveChanges();
            SelectedCard = null;
            UpdateAllCards();
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
                    IsDeleteVisible = false;
                    break;
                }

                case (false, false, _):  // card is not selected and some info is missing
                case (false, _, false):
                {
                    IsAddNewVisible = false;
                    IsEditVisible = false;
                    IsDeleteVisible = false;
                    break;
                }

                case (true, true, true): // card is selected, and both term and description are filled in - then  we show all buttons
                {
                    IsAddNewVisible = SelectedCard?.Term != EditableTerm || SelectedCard?.Description != EditableDescription && TagsAreTheSame();
                    IsEditVisible = SelectedCard?.Term != EditableTerm || SelectedCard?.Description != EditableDescription && TagsAreTheSame();
                    IsDeleteVisible = true;
                    break;
                }

                case (true, false, _):  // card is selected and some info is missing
                case (true, _, false):
                {
                    IsAddNewVisible = false;
                    IsEditVisible = false;
                    IsDeleteVisible = true;
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

        private void UpdateAllCards()
        {
            var allCards = _cardRepository.GetAll();
            Cards = new ObservableCollection<Card>(allCards);
        }

        private void PerformSearch()
        {
            Cards = string.IsNullOrEmpty(SearchTerm)
                ? new ObservableCollection<Card>(_cardRepository.GetAll())
                : new ObservableCollection<Card>(_cardRepository.GetByTermOrDescription(SearchTerm));

            SelectedCard = Cards.Count == 1 ? Cards.First() : null;
        }


    }
}
