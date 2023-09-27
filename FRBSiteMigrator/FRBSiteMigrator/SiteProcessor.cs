﻿using CsvHelper;
using FRBSiteMigrator.Models;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace FRBSiteMigrator
{
    /// <summary>
    /// This processes a CSV of pages that was created by dumping this query
    /// against the WordPress database via PhpMyAdmin to CSV:
    /// 
    /// SELECT 
    ///     p.ID, p.post_parent,
    ///     p.post_title,
    ///     p.post_content,
    ///     u.display_name as "author",
    ///     p.post_date, p.post_modified,
    ///     p.post_status, p.post_name, p.guid,
    ///     p.post_type,
    ///     p.post_mime_type
    /// FROM wp_posts p
    /// LEFT JOIN wp_users u ON u.ID = p.post_author
    /// WHERE p.post_type != "revision"
    /// ORDER BY ID
    /// 
    /// This is hardcoded to work against the CSV generated by that output.
    /// </summary>

    public class SiteProcessor
    {
        const string CsvFilename = "siteData.csv";
        const string JsonFilename = "siteContents.json";
        const string TempFolder = "temp";
        
        HttpClient client = new HttpClient();
        Site site = new Site();
        Dictionary<string, string> mediaPaths = new Dictionary<string, string>();

        string SiteFolder { get; set; } = "../../../../../";
        string MediaFolder => Path.Combine(SiteFolder, "media");
        string JsonPath => Path.Combine(SiteFolder, JsonFilename);
        string TempPath => Path.Combine(SiteFolder, TempFolder);

        public void Process(string siteUrl)
        {
            Directory.CreateDirectory(SiteFolder);
            Directory.CreateDirectory(MediaFolder);
            Directory.CreateDirectory(TempPath);


            site.SiteUrl = siteUrl;

            // get all of the CSV dump contents and populate the Site object
            GetSiteFromCsv();

            // create full file paths for each file
            Write("Calculating file paths for all content.");
            foreach(var content in site.AllContent)
            {
                content.ProcessedPath = GetUrlRecursive(content);
                Write($"Got url: {content.ProcessedPath}");
            }

            // check if any pages link to media that we don't know about
            Write("Checking for untracked media references.");
            foreach(var page in site.Pages)
            {
                FindAndAddUntrackedMedia(page);
            }
            foreach(var post in site.Posts)
            {
                FindAndAddUntrackedMedia(post);
            }

            // write a backup out so we can see it before we start the
            // slow processing stuff
            WriteSiteBackupToDisk();


            // ==================================================
            // THIS IS WHERE THE HEAVY LIFTING/SLOW PART STARTS!
            // ==================================================

            // we should have a complete list of media, fetch all locally
            // Note that this is
            // intentionally not parallel because the server can't handle it
            Write($"Making local copies of {site.MediaCount} media.");
            foreach (var media in site.Media)
            {
                FetchAndSaveMedia(media);
            }

            // now we fix links and images, convert html to markdown, and save out file
            foreach (var post in site.Posts)
            {
                ProcessPostOrPage(post);
            }
            foreach(var page in site.Pages)
            {
                ProcessPostOrPage(page);
            }

            // write out a backup again
            WriteSiteBackupToDisk();
        }

        void FindAndAddUntrackedMedia(SiteContent content)
        {
            var images = content.Images;
            foreach (var img in images)
            {
                // if the image link does NOT contain "http" it is relative and thus local
                // if the image DOES contain "http" and it contains the site url, it is local
                if (!img.Contains("http") || img.Contains(site.SiteUrl))
                {
                    // we have to strip the protocol because image sources are a mashup
                    // of http and https
                    var srcSansProtocol = img.StripProtocol();
                    var known = site.Media.Any(m => m.Guid.Contains(srcSansProtocol));

                    // if we don't have any matches, this is an untracked image and we
                    // need to add a record for it so we can make a local copy and repoint
                    // all doc URLs
                    if (!known)
                    {
                        var name = Path.GetFileNameWithoutExtension(img);
                        var media = new SiteContent()
                        {
                            Id = -1,
                            Type = "attachment",
                            Author = "SiteProcessor",
                            Created = DateTime.UtcNow,
                            Name = name,
                            Guid = img,
                            ParentId = 0,
                            SiteStatus = "inherit",
                            Title = name,
                            RawContent = "",
                        };
                        media.ProcessedPath = GetUrlRecursive(media);
                        Write($"Found unknown media: {media.ProcessedPath}");
                        site.Media.Add(media);
                    }
                }
            }
        }

        void FetchAndSaveMedia(SiteContent media)
        {
            Uri uri;
            if(media.Guid.Contains("http"))
            {
                uri = new Uri(media.Guid);
            }
            else
            {
                var builder = new UriBuilder("https", site.SiteUrl);
                builder.Path = media.Guid;
                uri = builder.Uri;
            }

            try
            {
                var flattenedFilename = media.ProcessedPath.Trim('/');
                var localPath = Path.Combine(MediaFolder, flattenedFilename);

                if(!File.Exists(localPath))
                {
                    var bytes = client.GetByteArrayAsync(uri).GetAwaiter().GetResult();
                    File.WriteAllBytes(localPath, bytes);
                    Write($"Saved image: {localPath}");
                }
            }
            catch(Exception ex)
            {
                Write($"Failed to fetch media: {media.Guid}.");
                site.BadMediaPaths.Add(media.Guid);
            }
        }

        void ProcessPostOrPage(SiteContent page)
        {
            var content = page.RawContent;

            // convert local links to be relative
            var links = page.Links.ToList();
            foreach (var link in links)
            {
                if(!link.Contains("http") || link.Contains(site.SiteUrl))
                {
                    var relative = link.MakeLinkRelative();
                    if (string.IsNullOrWhiteSpace(Path.GetExtension(link)))
                    {
                        relative += ".md";
                    }
                    // we do this because we don't want to accidentally replace
                    // parts of links that contain other links
                    var strictFind = $"\"{link}\"";
                    var strictReplace = $"\"{relative}\"";
                    content = content.Replace(strictFind, strictReplace);
                }
            }

            // convert image links to their new path, note that we search
            // for the relative path since protocols are mixed between http/s
            // and relative links
            var imgs = page.Images.ToList();
            foreach(var img in imgs)
            {
                if (!img.Contains("http") || img.Contains(site.SiteUrl))
                {
                    var relative = img.MakeLinkRelative();
                    var media = site.Media.Where(m => m.Guid.Contains(relative)).FirstOrDefault();
                    if (media != null)
                    {
                        var newLink = "/media/" + media.ProcessedPath;

                        // we do this because we don't want to accidentally replace
                        // parts of links that contain other links
                        var strictFind = $"\"{img}\"";
                        var strictReplace = $"\"{newLink}\"";
                        content = content.Replace(strictFind, strictReplace);
                    }
                    else
                    {
                        Write("Failed to find a media link in our media collection, this shouldn't happen!");
                    }
                }
            }

            page.ProcessedContent = content;

            // create folder structure for this page
            var dirs = page.ProcessedPath.Trim('/').Split('/');
            var dirPath = SiteFolder;
            for(var i = 0; i < dirs.Length - 1; i++)
            {
                dirPath = Path.Combine(dirPath, dirs[i]);
                Directory.CreateDirectory(dirPath);
            }

            // save out as a temporary html file, required to use pandoc
            var htmlPath = Path.Combine(TempPath, "temp.html");
            File.WriteAllText(htmlPath, page.ProcessedContent);

            // convert to markdown and save
            var markdownPath = Path.Combine(SiteFolder, page.ProcessedPath.Trim('/')) + ".md";
            try
            {
                ConvertHtmlToMarkdown(htmlPath, markdownPath);
                Write($"Processed page: {markdownPath}");
            }
            catch(Exception ex)
            {
                Write($"Failed to convert page: {page.ProcessedPath}");
                site.FailedPageConversions.Add(page.ProcessedPath);
            }
        }

        // NOTE: this method requires a path instead of a string because passing
        // HTML via commandline arguments does not work properly. Pandoc must
        // open, read, convert, and close the files. It's more IO ops but it
        // is the only reliable way to convert via Process Wrapping
        public void ConvertHtmlToMarkdown(string htmlInputPath, string markdownOutputPath)
        {
            var processName = "pandoc";
            var arguments = $"-f html -t gfm-raw_html \"{htmlInputPath}\" -o \"{markdownOutputPath}\"  --wrap=none";

            using (var pandoc = new ProcessWrapper(processName, arguments))
            {
                if (pandoc.Run())
                {
                    // execution worked, nothing to do here!
                }
                else
                {
                    throw new Exception($"Pandoc conversion failed: {pandoc.Error}");
                }
            }
        }

        string GetUrlRecursive(SiteContent content)
        {
            var url = "";

            if(content.Type == "attachment")
            {
                url = content.Guid;
                if(url.Contains("wp-content/uploads"))
                {
                    var index = url.IndexOf("wp-content/uploads");
                    url = url.Substring(index).Replace("wp-content/uploads", "");
                }
                url = url.ToFilenameSafeString();
            }
            else if(content.Type == "post")
            {
                url = "/news/" + content.Name;
            }
            else if(content.Type == "page")
            {
                var myName = content.Name;
                if (content.ParentId != 0)
                {
                    var parent = site.Pages.Where(p => p.Id == content.ParentId).FirstOrDefault();
                    if (parent != null)
                    {
                        // we do this because the api paths are like:
                        // flatredball/flatredball-math/flatredball-math-collision
                        // which results in filenames that are way too long
                        // so this converts that into
                        // flatredball/math/collision
                        if(myName.Contains(parent.Name))
                        {
                            myName = myName.Replace(parent.Name + "-", "");
                        }

                        url += GetUrlRecursive(parent);
                    }
                }
                url += "/" + myName;
            }

            if(url.Length > 100)
            {
                int m = 5;
            }

            return url;
        }

        void GetSiteFromCsv()
        {
            Write($"Processing {CsvFilename}");
            int records = 0;
            using(var reader = new StreamReader(CsvFilename))
            using(var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while(csv.Read())
                {
                    var content = new SiteContent
                    {
                        Id = csv.GetField<int>("ID"),
                        Type = csv.GetField("post_type"),
                        Author = csv.GetField("author"),
                        Created = csv.GetField<DateTime>("post_date"),
                        Name = csv.GetField("post_name"),
                        Guid = csv.GetField("guid"),
                        ParentId = csv.GetField<int>("post_parent"),
                        SiteStatus = csv.GetField("post_status"),
                        Title = csv.GetField("post_title"),
                        RawContent = csv.GetField("post_content"),                        
                    };
                    site.AddContent(content);
                    records++;
                }
            }
            Write($"Found {site.PageCount} pages, {site.MediaCount} media, and {site.PostCount} posts from {records} records.");
        }

        void Write(string msg)
        {
            Console.WriteLine(msg);
        }

        void WriteSiteBackupToDisk()
        {
            Write($"Saving metadata file.");
            var json = JsonConvert.SerializeObject(site, Formatting.Indented);
            File.WriteAllText(JsonPath, json);
            Write($"File saved to {JsonPath}");
        }
    }
}
