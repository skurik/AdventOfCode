<Query Kind="Program" />

void Main()
{
    var input = File.ReadAllText("07_input");

    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
    using (var reader = new StreamReader(stream))
    {
        var root = ReadDirectory(reader);
        var totalSizeOfDirsUnder100k = root.AllDirectories.Where(d => d.TotalSize <= 100_000).Sum(d => d.TotalSize);
        totalSizeOfDirsUnder100k.Dump();
        
        var totalDiskSize = 70_000_000;
        var freeSpaceRequired = 30_000_000;
        var freeSpace = totalDiskSize - root.TotalSize;
        var spaceToFree = freeSpaceRequired - freeSpace;
        var smallestDirToRemove = root.AllDirectories.OrderBy(d => d.TotalSize).First(d => d.TotalSize >= spaceToFree);        
        smallestDirToRemove.Dump();
    }
}

public Directory ReadDirectory(StreamReader inputReader)
{
    var dir = new Directory();
    while (!inputReader.EndOfStream)
    {
        var line = inputReader.ReadLine();
        if (line.StartsWith("$ cd .."))
        {
            return dir;
        }        
        else if (line.StartsWith("$ cd "))
        {
            var nested = ReadDirectory(inputReader);
            dir.Directories.Add(nested);
        }        
        else if (char.IsDigit(line[0]))
        {
            var fileSize = int.Parse(line.Split(' ').First());
            dir.FileSizes.Add(fileSize);
        }
    }
    
    return dir;
}

public class Directory
{
    public Directory()
    {
        Directories = new List<Directory>();
        FileSizes = new List<int>();
    }

    public List<Directory> Directories { get; }
    public List<int> FileSizes { get; }
    
    public IEnumerable<Directory> AllDirectories => this.ToEnumerable().Concat(Directories.SelectMany(d => d.AllDirectories));    
    public int TotalSize => FileSizes.Sum() + Directories.Sum(d => d.TotalSize);
}