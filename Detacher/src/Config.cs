using System.Text.Json;

namespace Detacher
{
    internal class Config
    {
        public string? ComputerName { get; set; }
        public List<string>? DiskPaths { get; set; }
        public List<string>? RDSList { get; set; }

        public void Init()
        {
            var json = File.ReadAllText("config.json");
            var config = JsonSerializer.Deserialize<Config>(json);

            if (config == null) return;
            ComputerName = config.ComputerName;
            DiskPaths = config.DiskPaths;
            RDSList = config.RDSList;
        }
    }
}
