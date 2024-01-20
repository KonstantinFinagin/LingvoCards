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
            SelectedTag = null;
            SelectedLevel = _eLevels.First();

            Reload();
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
        private ELevel _selectedLevel;

        partial void OnSelectedLevelChanged(ELevel value)
        {
            Reload();
        }

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

        [ObservableProperty]
        private int _maxCardsInExercise = 50;

        [ObservableProperty]
        private int _currentIndex;

        [RelayCommand]
        private void Previous()
        {
            if (_currentIndex > 0) CurrentIndex--;
            SetCardAndButtons();
        }

        [RelayCommand]
        private void Next()
        {
            if (CurrentIndex < _cards.Count - 1) CurrentIndex++;
            SetCardAndButtons();
        }

        private void SetCardAndButtons()
        {
            IsBackVisible = false;
            IsNextButtonVisible = CurrentIndex < _cards.Count - 1;
            IsPreviousButtonVisible = CurrentIndex > 0;
            CurrentCard = _cards.ElementAtOrDefault(CurrentIndex);
        }

        [RelayCommand]
        private void Reload()
        {
            CurrentIndex = 0;
            _cards = _cardRepository.GetFiltered(SelectedTag, SelectedLevel, DateFrom, DateTo, MaxCardsInExercise);
            CurrentCard = _cards.ElementAtOrDefault(CurrentIndex);
            MaxCardsInExercise = _cards.Count;
        }
    }
}
