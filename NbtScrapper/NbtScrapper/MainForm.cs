using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using fNbt;
using NbtStudio;
using TryashtarUtils.Nbt;

namespace NbtScrapper;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void inputOpenButton_Click(object sender, EventArgs e)
    {
        var folderBrowserDialog = new FolderBrowserDialog();
        folderBrowserDialog.InitialDirectory = inputPathTextBox.Text;
        var result = folderBrowserDialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            inputPathTextBox.Text = folderBrowserDialog.SelectedPath;
        }
    }

    private void outputOpenButton_Click(object sender, EventArgs e)
    {
        var folderBrowserDialog = new FolderBrowserDialog();
        folderBrowserDialog.InitialDirectory = outputPathTextBox.Text;
        var result = folderBrowserDialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            outputPathTextBox.Text = folderBrowserDialog.SelectedPath;
        }
    }

    private void extractButton_Click(object sender, EventArgs e)
    {
        extractButton.Enabled = false;
        var regionFiles = new List<string>();
        var playerFiles = new List<string>();
        var path = inputPathTextBox.Text;

        RecursiveFolderSearch(path);

        Task.Run(() => FindBooksAndSigns(regionFiles, playerFiles));

        void RecursiveFolderSearch(string path)
        {
            var regionPath = Path.Combine(path, "region");
            if (Directory.Exists(regionPath))
            {
                regionFiles.AddRange(Directory.GetFiles(regionPath));
            }

            var entitiesPath = Path.Combine(path, "entities");
            if (Directory.Exists(entitiesPath))
            {
                regionFiles.AddRange(Directory.GetFiles(entitiesPath));
            }

            var playerDataPath = Path.Combine(path, "playerdata");
            if (Directory.Exists(playerDataPath))
            {
                playerFiles.AddRange(Directory.GetFiles(playerDataPath));
            }

            foreach (var dir in Directory.GetDirectories(path))
            {
                RecursiveFolderSearch(dir);
            }
        }
    }

    private void FindBooksAndSigns(ICollection<string> regionFiles, ICollection<string> playerFiles)
    {
        var signFile = File.Create(Path.Combine(outputPathTextBox.Text, "sign.txt"));
        var bookFile = File.Create(Path.Combine(outputPathTextBox.Text, "book.txt"));
        var signFileWriter = new StreamWriter(signFile);
        var bookFileWriter = new StreamWriter(bookFile);
        var hashes = new List<byte[]>();
        var signCount = 0;
        var bookCount = 0;
        progressBar.InvokeIfRequired(() =>
        {
            progressBar.Maximum = playerFiles.Count;
            progressBar.Value = 0;
        });

        foreach (var file in playerFiles)
        {
            progressBar.InvokeIfRequired(() => { progressBar.Value++; });
            try
            {
                var playerData = new fNbt.NbtFile(file);
                infoTextBox.InvokeIfRequired(() =>
                {
                    infoTextBox.Text =
                        $@"Players: {progressBar.Value}/{playerFiles.Count}{Environment.NewLine}Signs found: {signCount}{Environment.NewLine}Books found: {bookCount}";
                });
                CyclicSearch(playerData.RootTag as NbtContainerTag, SearchMethod);
                GC.Collect();
            }
            catch
            {
                continue;
            }
        }

        progressBar.InvokeIfRequired(() =>
        {
            progressBar.Maximum = regionFiles.Count;
            progressBar.Value = 0;
        });
        foreach (var file in regionFiles)
        {
            progressBar.InvokeIfRequired(() => { progressBar.Value++; });
            try
            {
                var region = new RegionFile(file);
                var j = 0;
                var count = region.AllChunks.Count();

                foreach (var chunk in region.AllChunks)
                {
                    j++;
                    try
                    {
                        infoTextBox.InvokeIfRequired(() =>
                        {
                            infoTextBox.Text =
                                $@"Regions: {progressBar.Value}/{regionFiles.Count}{Environment.NewLine}Current: x={region.Coords.X}, z={region.Coords.Z}{Environment.NewLine}Chunks: {j}/{count}{Environment.NewLine}Signs found: {signCount}{Environment.NewLine}Books found: {bookCount}";
                        });

                        if (chunk == null)
                        {
                            continue;
                        }

                        chunk.Load();
                        CyclicSearch(chunk.Data, SearchMethod);
                        chunk.Remove();
                        GC.Collect();
                    }
                    catch
                    {
                        continue;
                    }
                }

                GC.Collect();
            }
            catch
            {
                continue;
            }
        }

        signFileWriter.Close();
        bookFileWriter.Close();
        extractButton.InvokeIfRequired(() => { extractButton.Enabled = true; });

        void SearchMethod(NbtContainerTag container, NbtTag tag)
        {
            if (tag.Name == "id")
            {
                if ((tag.TagType == NbtTagType.String && tag.StringValue is "minecraft:written_book" or "minecraft:writable_book") || (tag.TagType == NbtTagType.Short && tag.ShortValue is 386 or 387))
                {
                    var book = container["tag"] as NbtCompound;
                    if (book == null)
                    {
                        return;
                    }

                    var pages = book["pages"] as NbtList;

                    if (pages != null && pages.Count != 0)
                    {
                        var text = string.Empty;
                        foreach (var page in pages)
                        {
                            var isSnbt = SnbtParser.ClassicTryParse(page.StringValue, false, out var result);
                            text += isSnbt && result.TagType != NbtTagType.String ? result["text"].StringValue : page.StringValue;
                        }

                        var bookHash = HashString(text);
                        if (hashes.Any(hash => Enumerable.SequenceEqual(hash, bookHash)))
                        {
                            return;
                        }

                        hashes.Add(bookHash);

                        NbtContainerTag inventory = null;
                        NbtTag id = null;
                        if (container.Parent is NbtCompound)
                        {
                            id = container.Parent?["id"];
                            if (id != null)
                            {
                                inventory = container.Parent;
                            }
                            else if (container.Parent?.Parent is NbtCompound)
                            {
                                id = container.Parent?.Parent?["id"];
                                if (id != null)
                                {
                                    inventory = container.Parent.Parent;
                                }
                            }
                        }


                        var coords = string.Empty;
                        if (inventory?["x"] != null)
                        {
                            coords =
                                $"at x:{inventory["x"].IntValue}, y:{inventory["y"].IntValue}, z:{inventory["z"].IntValue}";
                        }

                        bookFileWriter.WriteLine("=====================================");
                        bookFileWriter.WriteLine(
                            $@"Book ""{(book["title"] == null ? "No name" : book["title"].StringValue)}"" by {(book["author"] == null ? "Unknown" : book["author"].StringValue)}");
                        bookFileWriter.WriteLine($"(content hash = {Convert.ToHexString(bookHash)})");
                        bookFileWriter.WriteLine(
                            $"Found {container["Count"].ByteValue}x {tag.StringValue} in slot {(container["Slot"] == null ? "None" : container["Slot"].ByteValue)} {(id == null ? string.Empty : "of block entity " + id.StringValue)} {coords}");
                        bookFileWriter.WriteLine();
                        var i = 0;
                        foreach (var page in pages)
                        {
                            bookFileWriter.WriteLine($"--- Page {i} ---");
                            var isSnbt = SnbtParser.ClassicTryParse(page.StringValue, false, out var result);
                            bookFileWriter.WriteLine(isSnbt && result.TagType != NbtTagType.String ? result["text"].StringValue : page.StringValue);
                            i++;
                        }
                    }

                    bookCount++;
                    return;
                }

                else if ((tag.TagType == NbtTagType.String && tag.StringValue is "minecraft:sign" or "Sign") || (tag.TagType == NbtTagType.Short && tag.ShortValue is 63 or 68))
                {
                    var rows = new List<string>();

                    var oldList = new[] { "Text1", "Text2", "Text3", "Text4" };
                    if (oldList.All(key => container.GetAllTags().Any(t => t.Name == key)))
                    {
                        foreach (var item in oldList)
                        {
                            var isParsed = SnbtParser.ClassicTryParse(container[item].StringValue, false, out var row);
                            if (!isParsed)
                            {
                                rows.Add(string.Empty);
                                continue;
                            }
                            else if (row.TagType == NbtTagType.String)
                            {
                                rows.Add(row.StringValue);
                                continue;
                            }
                            var extra = row["extra"];
                            if (extra?[0] != null)
                            {
                                if (extra[0].TagType == NbtTagType.String)
                                {
                                    rows.Add(extra[0].StringValue);
                                }
                                else
                                {
                                    rows.Add(extra[0]["text"].StringValue);
                                }

                            }
                            else if (row["text"] != null)
                            {
                                rows.Add(row["text"].StringValue);
                            }
                            else
                            {
                                rows.Add(string.Empty);
                            }
                        }
                    }

                    var newList = new[] { "front_text", "back_text" };
                    if (newList.All(key => container.GetAllTags().Any(t => t.Name == key)))
                    {
                        foreach (var item in newList)
                        {
                            var messagesList = container[item]["messages"] as NbtList;

                            if (messagesList == null || messagesList.OfType<NbtString>().All(message => message.StringValue == @"""""" || message.StringValue == @""" """))
                            {
                                continue;
                            }

                            foreach (NbtString message in messagesList)
                            {
                                rows.Add(message.StringValue.Substring(1, message.StringValue.Length - 2));
                            }
                        }
                    }

                    if (rows.All(string.IsNullOrEmpty))
                    {
                        return;
                    }

                    signFileWriter.WriteLine("=====================================");
                    signFileWriter.WriteLine(
                        $"Sign at x:{container["x"].IntValue}, y:{container["y"].IntValue}, z:{container["z"].IntValue}:");
                    foreach (var row in rows)
                    {
                        signFileWriter.WriteLine(row);
                    }

                    signCount++;
                }
            }
        }
    }

    private void RecursiveSearch(NbtContainerTag container, Action<NbtContainerTag, NbtTag> action)
    {
        foreach (var tag in container)
        {
            action(container, tag);
            if (tag is NbtContainerTag cTag)
            {
                if (cTag.Count > 0)
                {
                    RecursiveSearch(cTag, action);
                }
            }
        }
    }

    private void CyclicSearch(NbtContainerTag rootTag, Action<NbtContainerTag, NbtTag> action)
    {
        var stack = new Stack<NbtContainerTag>();
        stack.Push(rootTag);
        while (stack.TryPop(out var container))
        {
            foreach (var tag in container)
            {
                action(container, tag);
                if (tag is NbtContainerTag cTag)
                {
                    if (cTag.Count > 0)
                    {
                        stack.Push(cTag);
                    }
                }
            }
        }
    }

    byte[] HashString(string text, string salt = "")
    {
        using var sha = SHA256.Create();
        var textBytes = Encoding.UTF8.GetBytes(text + salt);
        var hashBytes = sha.ComputeHash(textBytes);

        return hashBytes;
    }
}