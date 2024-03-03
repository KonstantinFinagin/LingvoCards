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
        private ObservableCollection<Tag> _availableTags = new();

        [ObservableProperty]
        private Tag? _selectedTag;

        [ObservableProperty]
        private string _newTagText;

        public TagViewModel(TagRepository tagRepository)
        {
            _tagRepository = tagRepository;
            LoadTagsAsync().ConfigureAwait(false);
        }

        private async Task LoadTagsAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            AvailableTags = new ObservableCollection<Tag>(tags);
        }

        [RelayCommand]
        private async Task AddTag()
        {
            if (string.IsNullOrEmpty(NewTagText))
            {
                return;
            }

            var tag = new Tag()
            {
                Id = Guid.NewGuid(),
                Text = NewTagText,
                IsDefault = false
            };

            _tagRepository.Add(tag);
            await _tagRepository.SaveChangesAsync();

            NewTagText = string.Empty;

            await LoadTagsAsync();
        }


        [RelayCommand]
        private async Task DeleteSelected()
        {
            if (SelectedTag == null)
            {
                return;
            }

            var deletionApproved = await Shell.Current.CurrentPage.DisplayAlert("Delete", $"Delete {SelectedTag.Text}?", "Delete", "Cancel");
            if (!deletionApproved)
            {
                return;
            }

            _tagRepository.Remove(SelectedTag);
            await _tagRepository.SaveChangesAsync();

            await LoadTagsAsync();
        }


        [RelayCommand]
        private async Task SaveDefaults()
        {
            var saveApproved = await Shell.Current.CurrentPage.DisplayAlert("Save new defaults", "Save new defaults", "Save", "Cancel");
            if (!saveApproved)
            {
                return;
            }

            foreach (var availableTag in AvailableTags)
            {
                _tagRepository.Update(availableTag);
            }

            await _tagRepository.SaveChangesAsync();
            await LoadTagsAsync();
        }
    }
}

