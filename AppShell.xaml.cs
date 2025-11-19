namespace just_one_small_task
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register route for GamePage so we can navigate using Shell.Current.GoToAsync(nameof(GamePage))
            Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
        }
    }
}
