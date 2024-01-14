﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LingvoCards.Domain.Model;
using System.Collections.ObjectModel;
using LingvoCards.App.Views;
using LingvoCards.Dal.Repositories;

namespace LingvoCards.App.ViewModels
{
    public partial class CardTagViewModel : ObservableObject
    {
        private readonly TagRepository _tagRepository;
        private readonly CardRepository _cardRepository;

        [ObservableProperty] 
        private ObservableCollection<Tag> _availableTags = new();

        [ObservableProperty]
        private Card? _selectedCard;

        [ObservableProperty]
        private string _newTagText;

        partial void OnSelectedCardChanged(Card? value)
        {
            LoadTags();
            MarkSelectedCardTags(value);
        }

        public CardTagViewModel(TagRepository tagRepository, CardRepository cardRepository)
        {
            _tagRepository = tagRepository;
            _cardRepository = cardRepository;
        }

        public void InitializeWithSelectedCard(Card? selectedCard)
        {
            LoadTags();

            if (selectedCard != null)
            {
                SelectedCard = selectedCard;
            }
        }

        private void LoadTags()
        {
            var tags = _tagRepository.GetAll();
            AvailableTags = new ObservableCollection<Tag>(tags);
        }

        [RelayCommand]
        private async Task Dismiss()
        {
            await ReturnToCardPage();
        }

        [RelayCommand]
        private async Task Apply()
        {
            if (SelectedCard == null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Sorry", "Card is not selected. Selection can only be used for deletion", "Got it!");
            }

            var existingCardIds = SelectedCard.Tags.Select(t => t.Id);
            var newTags = AvailableTags.Where(t => t.IsSelected).Where(t => !existingCardIds.Contains(t.Id));

            foreach (var tag in newTags)
            {
                SelectedCard.Tags.Add(tag);
            }

            _cardRepository.Update(SelectedCard);
            _cardRepository.SaveChanges();

            SelectedCard = null;
            
            await ReturnToCardPage();

        }

        private void MarkSelectedCardTags(Card? card)
        {
            if (card?.Tags == null)
            {
                AvailableTags.All(t => t.IsSelected = false);
                return;
            }

            foreach (var availableTag in AvailableTags)
            {
                availableTag.IsSelected = false;
            }

            foreach (var availableTag in AvailableTags)
            {
                if (card.Tags.Select(t => t.Id).Contains(availableTag.Id))
                {
                    availableTag.IsSelected = true;
                }
            }
        }

        private async Task ReturnToCardPage()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}

