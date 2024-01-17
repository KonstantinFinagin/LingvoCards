using CommunityToolkit.Mvvm.ComponentModel;
using LingvoCards.Dal.Repositories;
using LingvoCards.Domain.Model;

namespace LingvoCards.App.ViewModels
{
    public partial class PracticeViewModel : ObservableObject
    {
        private readonly CardRepository _cardRepository;

        public PracticeViewModel(CardRepository cardRepository)
        {
            _cardRepository = cardRepository;

            CardLevel = ELevel.Bronze;
        }

        [ObservableProperty]
        private ELevel _cardLevel;
    }
}
