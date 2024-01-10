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
        private string? _searchTerm;

        partial void OnSearchTermChanged(string value)
        {
            PerformSearch();
        }

        [ObservableProperty]
        private Card? _selectedCard;

        partial void OnSelectedCardChanged(Card? value)
        {
            EditableTerm = value?.Term;
            EditableDescription = value?.Description;
        }

        [ObservableProperty]
        private string? _editableTerm;

        [ObservableProperty] 
        private string? _editableDescription;

        [ObservableProperty] 
        private string? _editableTags;

        private void UpdateAllCards()
        {
            var allCards = _cardRepository.GetAll();
            Cards = new ObservableCollection<Card>(allCards);
        }

        [RelayCommand]
        private void SaveCard()
        {
            if (SelectedCard == null)
            {
                // add new card
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
                return;
            }

            else
            {
                // Update the selected card with editable fields
                SelectedCard.Term = EditableTerm;
                SelectedCard.Description = EditableDescription;
                // ... other fields ...

                _cardRepository.Update(SelectedCard); // Save changes to the database
            }
        }

        private void PerformSearch()
        {
            Cards = string.IsNullOrEmpty(SearchTerm)
                ? new ObservableCollection<Card>(_cardRepository.GetAll())
                : new ObservableCollection<Card>(_cardRepository.GetByTermOrDescription(SearchTerm));
        }
    }
}
