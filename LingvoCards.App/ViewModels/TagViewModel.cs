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
            LoadTags();
        }

        private void LoadTags()
        {
            var tags = _tagRepository.GetAll();
            AvailableTags = new ObservableCollection<Tag>(tags);
        }

        [RelayCommand]
        private void AddTag()
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
            _tagRepository.SaveChanges();

            NewTagText = string.Empty;

            LoadTags();
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
            _tagRepository.SaveChanges();

            LoadTags();
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

            _tagRepository.SaveChanges();

            LoadTags();
        }

        [RelayCommand]
        private async Task Dismiss()
        {
            await ReturnToCardPage();
        }

        private async Task ReturnToCardPage()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

    }
}

