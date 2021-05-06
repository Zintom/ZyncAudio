using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace ZyncAudio.PlaylistReaders
{
    public class WPLReader : PlaylistReader
    {

        public override ExternalPlaylist Read(Stream stream, string? playlistFilePath)
        {
            ExternalPlaylist playlist = new ExternalPlaylist();

            XmlReader xmlReader = XmlReader.Create(stream);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    string elementName = xmlReader.Name.ToLower();

                    if (elementName == "title")
                    {
                        playlist.Title = xmlReader.ReadElementContentAsString();
                    }
                    else if (elementName == "meta")
                    {
                        string? metaName = xmlReader.GetAttribute("name")?.ToLower();// ?? throw new FormatException("A <meta> element must have a `name` attribute.");
                        string? metaContent = xmlReader.GetAttribute("content");// ?? throw new FormatException("A <meta> element must have a `content` attribute.");

                        if (string.IsNullOrEmpty(metaName) || string.IsNullOrEmpty(metaContent))
                        {
                            continue;
                        }

                        if (metaName == "generator")
                        {
                            playlist.Generator = metaContent;
                        }
                        else if (metaName == "averagerating")
                        {
                            playlist.AverageRating = int.Parse(metaContent);
                        }
                        else if (metaName == "totalduration")
                        {
                            playlist.TotalDuration = int.Parse(metaContent);
                        }
                        else if (metaName == "itemcount")
                        {
                            playlist.ItemCount = int.Parse(metaContent);
                        }
                    }
                    else if (elementName == "media")
                    {
                        string? mediaFilePath = xmlReader.GetAttribute("src");

                        if (string.IsNullOrEmpty(mediaFilePath))
                        {
                            Debug.WriteLine("<media> element did not have a source file, skipping.");
                            continue;
                        }

                        // Convert all relative paths to absolute paths.
                        if (!string.IsNullOrEmpty(playlistFilePath) && mediaFilePath.StartsWith("..\\"))
                        {
                            string playlistFileParentDirectory = new FileInfo(playlistFilePath)?.Directory?.FullName ?? "";
                            mediaFilePath = Path.GetFullPath(playlistFileParentDirectory + "\\" + mediaFilePath);
                        }

                        playlist.MediaItems.Add(mediaFilePath);
                    }
                }
            }

            return playlist;
        }
    }
}
