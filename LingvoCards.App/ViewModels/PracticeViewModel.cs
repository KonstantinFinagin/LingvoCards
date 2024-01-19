using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LingvoCards.Dal.Repositories;
using LingvoCards.Domain.Model;
using System.Collections.ObjectModel;

namespace LingvoCards.App.ViewModels
{
    public partial class PracticeViewModel : ObservableObject
    {
        private readonly CardRepository _cardRepository;
        private readonly TagRepository _tagRepository;

        public PracticeViewModel(CardRepository cardRepository, TagRepository tagRepository)
        {
            _cardRepository = cardRepository;
            _tagRepository = tagRepository;

            _allTags = new ObservableCollection<Tag>(tagRepository.GetAll());
            _selectedTag = null;
        }

        [ObservableProperty]
        private ELevel _cardLevel = ELevel.Bronze;

        [ObservableProperty]
        private ObservableCollection<ELevel> _eLevels = new()
        {
            ELevel.Bronze,
            ELevel.Silver,
            ELevel.Gold,
            ELevel.Diamond
        };

        [ObservableProperty]
        private ELevel _selectedLevel = ELevel.Bronze;

        [ObservableProperty]
        private ObservableCollection<Tag> _allTags;

        [ObservableProperty]
        private Tag? _selectedTag;

        [ObservableProperty]
        private DateTime? _dateFrom;

        [ObservableProperty]
        private DateTime? _dateTo;

        private List<Card> _cards;

        [ObservableProperty]
        private Card? _currentCard;

        [ObservableProperty]
        private bool _isBackVisible;

        [ObservableProperty]
        private bool _isNextButtonVisible = true;

        [ObservableProperty]
        private bool _isPreviousButtonVisible = false;

        [ObservableProperty]
        private string _word;

        [ObservableProperty]
        private string _description;

        partial void OnSelectedLevelChanged(ELevel oldValue, ELevel newValue)
        {
            Reload();
        }

        partial void OnSelectedTagChanged(Tag value)
        {
            Reload();
        }

        partial void OnDateFromChanged(DateTime? oldValue, DateTime? newValue)
        {
            Reload();
        }

        partial void OnDateToChanged(DateTime? value)
        {
            Reload();
        }

        partial void OnCurrentCardChanged(Card? oldValue, Card newValue)
        {
            IsBackVisible = false;
            Word = newValue.Term;
            Description = newValue.Description;
            CardLevel = newValue.Level;
        }

        private int _currentIndex;

        [RelayCommand]
        private void Previous()
        {
            if (_currentIndex > 0) _currentIndex--;
            SetCardAndButtons();
        }


        [RelayCommand]
        private void Next()
        {
            if (_currentIndex < _cards.Count - 1) _currentIndex++;
            SetCardAndButtons();
        }

        private void SetCardAndButtons()
        {
            IsNextButtonVisible = _currentIndex < _cards.Count - 1;
            IsPreviousButtonVisible = _currentIndex > 0;
            CurrentCard = _cards.ElementAtOrDefault(_currentIndex);
        }

        [RelayCommand]
        private void Reload()
        {
            _currentIndex = 0;
            _cards = _cardRepository.GetFiltered(SelectedTag, SelectedLevel, DateFrom, DateTo, 50);
            CurrentCard = _cards.ElementAtOrDefault(_currentIndex);
        }
    }
}
