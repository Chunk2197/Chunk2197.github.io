using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Statiq.App;
using Statiq.Common;
using Statiq.Core;
using Statiq.Web;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var bootstrapper = Bootstrapper.Factory.CreateWeb(args);

        // FeedGenerator: produce /feed.xml from posts
        var feedPipeline = new Pipeline
        {
            InputModules = { new ReadFiles("posts/*.md") },
            ProcessModules =
            {
                new ExecuteConfig(Config.FromContext(ctx =>
                {
                    var posts = ctx.Inputs.OrderByDescending(d => d.Get<DateTime>("Date")).Take(20).ToList();
                    var siteTitle = ctx.Settings.GetString("title") ?? "Davis DIY";
                    var siteUrl = ctx.Settings.GetString("site_url") ?? "https://davis.diy";

                    var channel = new XElement("channel",
                        new XElement("title", siteTitle),
                        new XElement("link", siteUrl),
                        new XElement("description", "Recent posts from " + siteTitle)
                    );

                    foreach (var post in posts)
                    {
                        var title = post.GetString("Title") ?? post.GetString("title") ?? string.Empty;
                        var destStr = post.Destination.ToString();
                        var dest = string.IsNullOrWhiteSpace(destStr) ? $"/posts/{title.Replace(' ', '-')}.html" : destStr;
                        if (!dest.StartsWith("/")) dest = "/" + dest;
                        if (dest.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                        {
                            dest = dest.Substring(0, dest.Length - 3) + ".html";
                        }
                        var link = siteUrl.TrimEnd('/') + dest;
                        var description = post.GetString("Description") ?? post.GetString("description") ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
                        {
                            // Try to extract from generated HTML if metadata not present
                            var html = post.GetString("Rendered") ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(title))
                            {
                                var h1Start = html.IndexOf("<h1", StringComparison.OrdinalIgnoreCase);
                                if (h1Start >= 0)
                                {
                                    var start = html.IndexOf('>', h1Start);
                                    var end = html.IndexOf("</h1>", start, StringComparison.OrdinalIgnoreCase);
                                    if (start >= 0 && end > start)
                                    {
                                        title = html.Substring(start + 1, end - start - 1).Trim();
                                    }
                                }
                            }

                            if (string.IsNullOrWhiteSpace(description))
                            {
                                var pStart = html.IndexOf("<p>", StringComparison.OrdinalIgnoreCase);
                                var pEnd = pStart >= 0 ? html.IndexOf("</p>", pStart, StringComparison.OrdinalIgnoreCase) : -1;
                                if (pStart >= 0 && pEnd > pStart)
                                {
                                    var p = html.Substring(pStart + 3, pEnd - pStart - 3).Trim();
                                    description = p.Length > 300 ? p.Substring(0, 300) + "..." : p;
                                }
                            }
                        }
                        var date = post.ContainsKey("Date") ? post.Get<DateTime>("Date") : (post.ContainsKey("date") ? post.Get<DateTime>("date") : DateTime.MinValue);
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            var file = System.IO.Path.GetFileNameWithoutExtension(dest).Replace('-', ' ');
                            title = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(file);
                        }
                        var item = new XElement("item",
                            new XElement("title", title),
                            new XElement("link", link),
                            new XElement("guid", link),
                            new XElement("pubDate", date == DateTime.MinValue ? DateTime.UtcNow.ToString("r") : date.ToUniversalTime().ToString("r")),
                            new XElement("description", new XCData(description))
                        );
                        channel.Add(item);
                    }

                    var rss = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("rss", new XAttribute("version", "2.0"), channel));

                    var doc = ctx.CreateDocument(rss.ToString()).Clone(new NormalizedPath("feed.xml"));
                    return new[] { doc };
                }))
            },
            OutputModules = { new WriteFiles() }
        };

        bootstrapper.AddPipeline("FeedGenerator", feedPipeline);

        return await bootstrapper.RunAsync();
    }
}
