
using ISP.Database;
using ISP.Database.Models;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace ISP.Pages {
    public sealed partial class BlogPage : Page {
        private List<BlogModel> blogPosts;
        public BlogPage() {
            this.InitializeComponent();
            this.UpdateBlog();
        }

        private async void UpdateBlog() {
            try {
                this.blogPosts = await DatabaseAccess.FetchBlogs();
                PostListContainer.Children.Clear();

                foreach (var post in blogPosts) {
                    var container = CreatePostContainer(post);
                    PostListContainer.Children.Add(container);
                }
            } catch (Exception exception) {

            }
        }

        private StackPanel CreatePostContainer(BlogModel post) {
            var panel = new StackPanel {
                Style = (Style) Resources["PostContainerStyle"]
            };

            var postBody = new StackPanel {
                Style = (Style) Resources["PostBodyStyle"]
            };

            var titleElement = new TextBlock();
            titleElement.Text = post.Title;
            titleElement.Style = (Style) Resources["PostTitleStyle"];

            var richTextBlock = new RichTextBlock();
            var paragraph = new Paragraph();
            var run = new Run();
            richTextBlock.IsTextSelectionEnabled = true;
            richTextBlock.TextWrapping = TextWrapping.Wrap;
            run.Text = post.Content;
            richTextBlock.Width = 200;
            paragraph.Inlines.Add(run);
            richTextBlock.Blocks.Add(paragraph);

            postBody.Children.Add(titleElement);
            postBody.Children.Add(richTextBlock);

            panel.Children.Add(postBody);

            var postButtons = new StackPanel();
            postButtons.Style = (Style) Resources["PostAppBarButtonsStyle"];

            return panel;
        }
    }
}
