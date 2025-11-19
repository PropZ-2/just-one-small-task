namespace just_one_small_task
{
    public partial class MainPage : ContentPage
    {
        private string Language = "en";

        public MainPage()
        {
            InitializeComponent();
        }

        private async void StarGameClicked(object? sender, EventArgs e)
        {
            // Navigate to GamePage using Shell routing
            await Shell.Current.GoToAsync(nameof(GamePage));
        }

        private void CounterBtn_ChildrenReordered(object sender, EventArgs e)
        {

        }
    }
}
