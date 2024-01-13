using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LingvoCards.Domain.Model;
using System.Collections.ObjectModel;
using LingvoCards.Dal.Repositories;

namespace LingvoCards.App.ViewModels
{
    public partial class TagViewModel : ObservableObject
    {
        private readonly TagRepository _tagRepository;

        [ObservableProperty] 
        private ObservableCollection<Tag> _availableTags;

        [ObservableProperty]
        private Card _selectedCard;

        [ObservableProperty]
        private string _newTagText;

        partial void OnSelectedCardChanging(Card value)
        {
            foreach (var availableTag in AvailableTags)
            {
                if (value.Tags.Select(t => t.Id).Contains(availableTag.Id))
                {
                    availableTag.IsSelected = true;
                }
            }
        }

        public TagViewModel(TagRepository tagRepository)
        {
            _tagRepository = tagRepository;
            LoadTags();
        }

        public void InitializeWithSelectedCard(Card selectedCard)
        {
            SelectedCard = selectedCard;
        }

        private void LoadTags()
        {
            var tags = _tagRepository.GetAll();

            foreach (var tag in tags)
            {
                AvailableTags.Add(tag);    
            }
        }

        [RelayCommand]
        private void AddTag()
        {
            var tag = new Tag()
            {
                Id = Guid.NewGuid(),
                Text = NewTagText,
                IsDefault = false
            };

            _tagRepository.Add(tag);
            _tagRepository.SaveChanges();

            LoadTags();
        }

        [RelayCommand]
        private void Apply()
        {
            // Logic to apply the selected tags
        }
    }
}

