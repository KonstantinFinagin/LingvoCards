using System.Collections.ObjectModel;
using System.Windows.Input;
using LingvoCards.Dal.Repositories;
using LingvoCards.Domain.Model;

namespace LingvoCards.App.ViewModels
{
    public class CardsViewModel : BindableObject
    {
        private ObservableCollection<Card> _cards;
        private string _searchTerm;
        private Card? _selectedCard;
        private readonly CardRepository _cardRepository;

        public CardsViewModel(CardRepository cardRepository)
        {
            _cardRepository = cardRepository;
            UpdateAllCards();
        }

        private void UpdateAllCards()
        {
            var allCards = _cardRepository.GetAll();

            foreach (var card in allCards)
            {
                Cards.Add(card);
            }
        }

        public ObservableCollection<Card> Cards
        {
            get => _cards;
            set
            {
                _cards = value;
                OnPropertyChanged();
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
                PerformSearch();
            }
        }

        public string EditableTerm { get; set; }
        public string EditableDescription { get; set; }
        public string EditableTags { get; set; }

        public Card? SelectedCard
        {
            get => _selectedCard;
            set
            {
                _selectedCard = value;
                OnPropertyChanged();

                // Update the editable fields with the selected card details
                // Assuming you have properties like EditableTerm, EditableDescription, etc.
                EditableTerm = _selectedCard?.Term;
                EditableDescription = _selectedCard?.Description;
                // ... other fields ...
            }
        }

        // Command to save changes
        public ICommand SaveCommand => new Command(SaveCard);

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
            if (string.IsNullOrWhiteSpace(_searchTerm))
            {
                // Reset search
                Cards = new ObservableCollection<Card>(_cards);
            }
            else
            {
                // Perform search
                Cards = new ObservableCollection<Card>(_cards.Where(c => c.Term.Contains(_searchTerm) || c.Description.Contains(_searchTerm)));
            }
        }
    }
}
