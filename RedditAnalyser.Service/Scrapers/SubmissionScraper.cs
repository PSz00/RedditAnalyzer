using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace RedditAnalyzer.Service.Scrapers;

public class SubmissionScraper
{
    private readonly SeleniumClient _seleniumClient;

    public SubmissionScraper()
    {
        _seleniumClient = new SeleniumClient();
        _seleniumClient.LoginToReddit();
        _seleniumClient.AcceptCookies();
    }

    public List<SubmissionInfo> GetSubmissionsInformation(List<string> submissionUrls)
    {
        var submissionsInformation = new List<SubmissionInfo>();

        foreach (var submissionUrl in submissionUrls)
        {
            _seleniumClient.NavigateTo(submissionUrl);

            var submissionBaseInfo = GetSubmissionInfo();

            if (submissionBaseInfo == null) continue;

            var comments = GetCommentsInfo(submissionUrl);

            if (comments == null) continue;

            var submissionInfo = new SubmissionInfo(
                submissionUrl, 
                submissionBaseInfo.Author, 
                submissionBaseInfo.Title, 
                submissionBaseInfo.Date, 
                comments);

            submissionsInformation.Add(submissionInfo);
        }

        return submissionsInformation;
    }

    private SubmissionBaseInfo? GetSubmissionInfo()
    {
        var html = _seleniumClient.GetCurrentHtml();

        var postContent = html.DocumentNode.SelectSingleNode(".//body//div[@data-test-id='post-content']");

        var title = postContent?
            .SelectSingleNode(".//div[@data-adclicklocation='title']//h1")?
            .InnerText;

        if (postContent is null || title is null) return null;

        var date = GetDate(postContent);

        if (date is null) return null;

        var authorName = GetSubmissionsAuthor(postContent);

        if (authorName is null) return null;

        return new(authorName, title, (DateTime)date);
    }

    private void ExpandAllComments()
    {
        var html = _seleniumClient.GetCurrentHtml();
        var comments = GetCommentList(html);

        var expandButtons = comments?
            .SelectMany(n => n.SelectNodes(".//i[contains(@class, 'icon') and contains(@class, 'icon-expand')]"))
            .Select(n => n.ParentNode)
            .ToList();

        if (expandButtons is null || expandButtons.Count == 0) return;

        var groupedByClass = expandButtons
            .Select(n => n.Attributes["class"].Value.Split(" ")[2])
            .GroupBy(n => n)
            .OrderBy(g => g.Count())
            .ToList();

        if (groupedByClass.Count > 2) 
            throw new Exception("Expanding failed either order of classes changed, or the way comments are expanded");

        if (groupedByClass.Count != 2) return;

        var minorityGroup = groupedByClass.FirstOrDefault();

        if (minorityGroup == null) return;

        var expandableButtons = expandButtons
            .Where(n => n.Attributes["class"].Value.Split(" ")[2] == minorityGroup.Key)
            .ToList();

        foreach (var button in expandableButtons)
        {
            _seleniumClient.ClickElementByXPath(button.XPath);
        }
    }

    private List<CommentInfo>? GetCommentsInfo(string url)
    {
        _seleniumClient.MaxScrollToEnd();

        ExpandAllComments();

        var currentHtml = _seleniumClient.GetCurrentHtml();
        var commentsList = GetCommentList(currentHtml)?.ToList();
     
        if (commentsList is null) return null;

        var information = commentsList
            .Select(GetInfoFromComment)
            .Where(info => info != null)
            .Select(info => info!)
            .ToList();
        //22 vs 29 => HOVER GetDate 
        if (information.Count != commentsList.Count)
            throw new Exception($"Comments mismatch: Errors are happening SEND HELP to {url}");

        return information;
    }

    private CommentInfo? GetInfoFromComment(HtmlNode commentElement)
    {
        var author = commentElement.SelectSingleNode(".//a[@data-testid='comment_author_link']")?.InnerText;

        if (author is null) return null;

        Console.WriteLine(author);
        var date = GetDate(commentElement);

        if (date is null)
        {
            Console.WriteLine("Date NOT loaded");
            return null;
        }
        Console.WriteLine("Date LOADED");
        var text = GetCommentsText(commentElement);

        if (text is null) return null;

        return new(author, text, (DateTime)date);
    }

    private string? GetCommentsText(HtmlNode commentElement)
    {
        var commentContent = commentElement.SelectSingleNode($".//div[@data-testid='comment']");

        if (commentContent is null) return null;

        var commentText = new StringBuilder();

        foreach (var commentPart in commentContent.ChildNodes)
        {
            if (commentPart is null) continue;

            GetNestedText(commentPart, commentText);
        }

        return commentText.ToString();
    }

    private void GetNestedText(HtmlNode commentPart, StringBuilder stringBuilder)
    {
        var children = commentPart.ChildNodes;

        foreach (var child in children)
        {
            if (child is null) continue;

            if (child.NodeType == HtmlNodeType.Text)
            {
                stringBuilder.Append(child.InnerText.Trim());
                stringBuilder.Append(' ');
                stringBuilder.AppendLine();
            }

            GetNestedText(child, stringBuilder);
        }
    }

    private HtmlNodeCollection? GetCommentList(HtmlDocument html)
    {
        var commentsTree = html.DocumentNode.SelectSingleNode(".//body//div[@data-scroller-first]")?.ParentNode.ChildNodes;

        if (commentsTree is null) return null;

        return commentsTree;
    }

    private DateTime? GetDate(HtmlNode parentElement)
    {
        var timestamp = GetTimestampElement(parentElement);

        if (timestamp is null) return null;

        _seleniumClient.HoverElementByXPath(timestamp.XPath);

        var dateElement = GetDateElement();

        if (dateElement is null) return null;

        var utcDate = ParseInnerTextToDate(dateElement.InnerText);

        return utcDate;
    }
    
    private string? GetSubmissionsAuthor(HtmlNode parentNode)
    {
        var authorName = parentNode
            .SelectSingleNode(".//a[@data-testid='post_author_link']")?
            .InnerText;

        if (authorName is null) return null;

        if (authorName[..2] == "u/")
            authorName = authorName[2..];

        return authorName;
    }

    private HtmlNode? GetTimestampElement(HtmlNode parentElement)
    {
        var timestampA = parentElement.SelectSingleNode(".//a[@data-testid='comment_timestamp']");
        var timestampSpan = parentElement.SelectSingleNode(".//span[@data-testid='post_timestamp']");

        return timestampA ?? timestampSpan;
    }

    private string GetDateRegexPattern()
    {
        var dayOfWeekPattern = @"(Sun|Mon|Tue|Wed|Thu|Fri|Sat)";
        var monthPattern = @"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)";
        var datePattern = @"\d{2}";
        var yearPattern = @"\d{4}";
        var timePattern = @"((0?[1-9]|1[012]):[0-5][0-9]:[0-5][0-9] (AM|PM))";
        var timeZonePattern = @"( .*)?$";

        return $"^{dayOfWeekPattern}, {monthPattern} {datePattern}, {yearPattern}, {timePattern}{timeZonePattern}";
    }

    private HtmlNode? GetDateElement()
    {
        var html = _seleniumClient.GetCurrentHtml();
        var potentialDateElements = html.DocumentNode.SelectNodes(".//body//div[@class='subredditvars-r-csharp']");

        if (potentialDateElements is null) return null;

        var pattern = GetDateRegexPattern();

        var dateElement = potentialDateElements
            .SelectMany(n => n.ChildNodes)
            .SingleOrDefault(n => Regex.IsMatch(n.InnerText, pattern));

        return dateElement;
    }

    private DateTime ParseInnerTextToDate(string innerText)
    {
        var dateParts = innerText.Split(' ');
        var joinedDate = string.Join(" ", dateParts.Take(6));

        var date = DateTime.Parse(joinedDate);
        var utcDate = date.ToUniversalTime();

        return utcDate;
    }

    public class CommentInfo
    {
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public CommentInfo(string author, string text, DateTime date)
        {
            Author = author;
            Text = text;
            Date = date;
        }
    }

    public class SubmissionInfo
    {
        public string Url { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public List<CommentInfo> Comments { get; set; }

        public SubmissionInfo(string url, string author, string title, DateTime date, List<CommentInfo> comments)
        {
            Url = url;
            Author = author;
            Title = title;
            Date = date;
            Comments = comments;
        }
    }

    private class SubmissionBaseInfo
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }

        public SubmissionBaseInfo(string author, string title, DateTime date)
        {
            Author = author;
            Title = title;
            Date = date;
        }
    }

    ~SubmissionScraper()
    {
        _seleniumClient.Dispose();
    }
}
