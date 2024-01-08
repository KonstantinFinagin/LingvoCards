using System.Collections.ObjectModel;
using LingvoCards.Domain.Model;

namespace LingvoCards.App.ViewModels
{
    public class CardsViewModel : BindableObject
    {
        private ObservableCollection<Card> _cards;
        private string _searchTerm;

        public CardsViewModel()
        {
            _cards = new ObservableCollection<Card>()
            {
                new Card() { Term = "Hello0", Description = "A Greeting 0", CreatedOn = DateTime.Parse("2022-01-01") },
                new Card() { Term = "Hello1", Description = "A Greeting 1", CreatedOn = DateTime.Parse("2022-01-02") },
                new Card() { Term = "Hello2", Description = "A Greeting 2", CreatedOn = DateTime.Parse("2022-01-03") },
                new Card() { Term = "Hello3", Description = "A Greeting 3", CreatedOn = DateTime.Parse("2022-01-04") },
                new Card() { Term = "Hello4", Description = "A Greeting 4", CreatedOn = DateTime.Parse("2022-01-05") },
            };
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
