using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LingvoCards.Dal.Repositories;
using LingvoCards.Domain.Model;
using System.Collections.ObjectModel;

namespace LingvoCards.App.ViewModels;

public partial class CardEditViewModel : ObservableObject
{
    private readonly CardRepository _cardRepository;
    private readonly TagRepository _tagRepository;

    public CardEditViewModel(CardRepository cardRepository, TagRepository tagRepository)
    {
        _cardRepository = cardRepository;
        _tagRepository = tagRepository;
    }

    public void InitializeWithSelectedCard(Card? selectedCard)
    {
        LoadTags();

        if (selectedCard != null)
        {
            SelectedCard = selectedCard;
            Term = selectedCard.Term;
            Description = selectedCard.Description;

            foreach (var availableTag in AvailableTags)
            {
                if (selectedCard.Tags != null && selectedCard.Tags.Select(t => t.Id).Contains(availableTag.Id))
                {
                    availableTag.IsSelected = true;
                }
            }
        }
        else // selected card == null
        {
            foreach (var availableTag in AvailableTags)
            {
                if (availableTag.IsDefault)
                {
                    availableTag.IsSelected = true;
                }
            }
        }
    }

    private void LoadTags()
    {
        var tags = _tagRepository.GetAll();
        AvailableTags = new ObservableCollection<Tag>(tags);
    }

    public event Action ModalClosed;

    [ObservableProperty]
    private Card? _selectedCard;

    [ObservableProperty]
    private string _term;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private ObservableCollection<Tag> _availableTags = new();

    [RelayCommand]
    private async Task Dismiss()
    {
        await ReturnToCardsViewPage();
    }

    [RelayCommand]
    private async Task SaveCard()
    {
        if (SelectedCard != null)
        {
            await EditCard();
        }
        else
        {
            await AddCard();
        }

        await ReturnToCardsViewPage();
    }

    private async Task AddCard()
    {
        // add new card
        if (string.IsNullOrEmpty(Term) || string.IsNullOrEmpty(Description))
        {
            await Shell.Current.CurrentPage.DisplayAlert("Error", "Term and description cannot be empty.", "Got it!");
            return;
        }

        var card = new Card()
        {
            Id = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            Term = Term,
            Description = Description,
            Tags = AvailableTags.Where(t => t.IsSelected).ToList()
        };

        _cardRepository.Add(card);
        _cardRepository.SaveChanges();
    }

    private async Task EditCard()
    {
        if (SelectedCard == null) return;

        var card = _cardRepository.GetCard(SelectedCard.Id);
        if (card == null) return;

        if (string.IsNullOrEmpty(Term) || string.IsNullOrEmpty(Description))
        {
            await Shell.Current.CurrentPage.DisplayAlert("Error", "Term and description cannot be empty.", "Got it!");
            return;
        }

        card.Term = Term;
        card.Description = Description;
        card.Tags = AvailableTags.Where(t => t.IsSelected).ToList();

        _cardRepository.SaveChanges();
    }

    private async Task ReturnToCardsViewPage()
    {
        Term = string.Empty;
        Description = string.Empty;

        await Application.Current.MainPage.Navigation.PopModalAsync(true);
        ModalClosed?.Invoke();
    }
}