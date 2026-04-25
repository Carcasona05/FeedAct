using System;
using System.Collections.ObjectModel;
using FeedAct.Entities;
using FeedAct.Services;

namespace FeedAct.Views;

public partial class PostDetailPage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly ObservableCollection<Comment> _comments = new();
    private readonly int _postId;

    public PostDetailPage(Post post)
    {
        InitializeComponent();

        _db = new DatabaseService();

        // ✔️ now INT ID (auto-increment)
        _postId = post.Id;

        CommentsCollection.ItemsSource = _comments;

        LoadComments();
    }

    private async void LoadComments()
    {
        // ✔️ FIX: no more ToString()
        var comments = await _db.GetCommentsAsync(_postId);

        _comments.Clear();
        foreach (var c in comments)
        {
            _comments.Add(c);
        }

        UpdateCommentCount();
    }

    private void UpdateCommentCount()
    {
        var count = _comments.Count;

        CommentCountLabel.Text = count switch
        {
            0 => "No comments",
            1 => "1 comment",
            _ => $"{count} comments"
        };
    }

    private async void OnCommentSubmitted(object sender, EventArgs e)
    {
        var text = CommentEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(text))
            return;

        var comment = new Comment
        {
            // ✔️ FIX: int instead of string
            PostId = _postId,
            Text = text
        };

        await _db.SaveCommentAsync(comment);

        CommentEntry.Text = string.Empty;

        LoadComments(); // refresh UI
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}