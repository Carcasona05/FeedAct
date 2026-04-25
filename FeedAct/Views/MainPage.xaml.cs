using System.Collections.ObjectModel;
using FeedAct.Entities;
using FeedAct.Services;

namespace FeedAct.Views;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly ObservableCollection<PostViewModel> _posts = new();

    public MainPage()
    {
        InitializeComponent();

        _db = new DatabaseService();
        FeedCollection.ItemsSource = _posts;

        LoadPosts();
    }

    private async void LoadPosts()
    {
        var posts = await _db.GetPostsAsync();

        _posts.Clear();
        foreach (var post in posts)
        {
            _posts.Add(new PostViewModel(post));
        }
    }

   private async void OnAddPostClicked(object sender, EventArgs e)
{
    var addPage = new AddPostPage();

    await Navigation.PushModalAsync(new NavigationPage(addPage));

    // wait until page closes
    addPage.Disappearing += (s, args) =>
    {
        LoadPosts(); // always refresh when returning
    };
}

    private async void OnCommentsClicked(object sender, TappedEventArgs e)
    {
        if (sender is Label label && label.BindingContext is PostViewModel vm)
        {
            await Navigation.PushModalAsync(new NavigationPage(new PostDetailPage(vm.Post)));
        }
    }
}

public class PostViewModel
{
    public Post Post { get; }

    public string Author => Post.Author;
    public string Content => Post.Content;
    public string? ImagePath => Post.ImagePath;
    public bool HasImage => !string.IsNullOrEmpty(Post.ImagePath);
    public DateTime CreatedAt => Post.CreatedAt;
    public string VisibilityIcon => Post.IsPublic ? "🌐" : "🔒";
    public string TimeAgo => CreatedAt.ToString("MMM dd, yyyy 'at' h:mm tt");

    public PostViewModel(Post post)
    {
        Post = post;
    }
}