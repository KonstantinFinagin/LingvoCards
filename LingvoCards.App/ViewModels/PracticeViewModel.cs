﻿using CommunityToolkit.Mvvm.ComponentModel;
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

            _allTags = new ObservableCollection<Tag?>(tagRepository.GetAllAsync().GetAwaiter().GetResult().Append((Tag?)null))!;
        }

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
        private bool _isIKnowButtonVisible = true;

        [ObservableProperty]
        private string _word;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private ELevel _cardLevel = ELevel.Bronze;

        [ObservableProperty] 
        private string _cardLevelText = "";

        partial void OnSelectedLevelChanged(ELevel value)
        {
            ReloadInternalAsync().ConfigureAwait(false);
        }

        partial void OnSelectedTagChanged(Tag? value)
        {
            ReloadInternalAsync().ConfigureAwait(false);
        }

        partial void OnDateFromChanged(DateTime? value)
        {
            ReloadInternalAsync().ConfigureAwait(false);
        }

        partial void OnDateToChanged(DateTime? value)
        {
            ReloadInternalAsync().ConfigureAwait(false);
        }

        partial void OnCurrentCardChanged(Card? oldValue, Card? newValue)
        {
            IsBackVisible = false;
            Word = newValue?.Term ?? "NO CARD";
            Description = newValue?.Description ?? "NO CARD";

            CardLevel = newValue?.Level ?? ELevel.Bronze;
            CardLevelText = GetCardLevelText(newValue);
        }

        [ObservableProperty]
        private int _maxCardsInExercise = 50;

        [ObservableProperty]
        private int _currentIndex;

        [RelayCommand]
        private void Previous()
        {
            if (CurrentIndex > 0) CurrentIndex--;
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
            MaxCardsInExercise = _cards.Count;
        }

        [RelayCommand]
        public async Task Reload()
        {
            DefaultSearchParams();
            await ReloadInternalAsync();
        }

        private void DefaultSearchParams()
        {
            DateFrom = DateTime.Now.AddMonths(-6).Date;
            DateTo = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            SelectedTag = null;
            SelectedLevel = ELevel.Bronze;
        }

        private async Task ReloadInternalAsync()
        {
            CurrentIndex = 0;
            _cards = await _cardRepository.GetFilteredAsync(SelectedTag, SelectedLevel, DateFrom, DateTo, MaxCardsInExercise);

            if (_cards.Count == 0)
            {
                // show alert that default cards will be shown
                await Shell.Current.CurrentPage.DisplayAlert("No cards", "No cards match search criteria. Loading defaults", "Got it!");
                DefaultSearchParams();
                _cards = await _cardRepository.GetDefaultFilteredAsync(MaxCardsInExercise);
            }

            if (_cards.Count != 0)
            {
                SetCardAndButtons();
            }
            else
            {
                CurrentCard = null;
            }
        }

        [RelayCommand]
        private async Task UpgradeLevelAsync()
        {
            if (CurrentCard == null) return;
            if(CurrentCard.Level != ELevel.Diamond) CurrentCard.Level++;

            CurrentCard.SuccessCount++;
            _cardRepository.Update(CurrentCard);
            await _cardRepository.SaveChangesAsync();

            CardLevel = CurrentCard.Level;
            CardLevelText = GetCardLevelText(CurrentCard);
        }

        [RelayCommand]
        private async Task DropLevelAsync()
        {
            if (CurrentCard == null || CurrentCard.Level == ELevel.Bronze) return;
            CurrentCard.Level = ELevel.Bronze;

            CurrentCard.FailureCount++;
            _cardRepository.Update(CurrentCard);
            await _cardRepository.SaveChangesAsync();

            CardLevel = CurrentCard.Level;
            CardLevelText = GetCardLevelText(CurrentCard);
        }

        private string GetCardLevelText(Card? card)
        {
            if (card == null)
            {
                return "no card, S/F: -/-";
            }

            return card.Level + $" S/F: {card.SuccessRateText}";
        }
    }
}
