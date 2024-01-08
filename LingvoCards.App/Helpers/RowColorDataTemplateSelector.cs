namespace LingvoCards.App.Helpers
{
    public class RowColorDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EvenRowTemplate { get; set; } = null!;
        public DataTemplate OddRowTemplate { get; set; } = null!;

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (container is CollectionView collectionView)
            {
                var index = collectionView.ItemsSource.Cast<object>().ToList().IndexOf(item);
                return index % 2 == 0 ? EvenRowTemplate : OddRowTemplate;
            }

            throw new ArgumentException(nameof(container));
        }
    }
}
