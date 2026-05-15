namespace FilmRentalStore.MVC.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public List<DashboardCardViewModel> Cards { get; set; } = new();
        public List<DashboardLinkViewModel> Links { get; set; } = new();
    }

    public class DashboardCardViewModel
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    public class DashboardLinkViewModel
    {
        public string Text { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = "Index";
    }
}
