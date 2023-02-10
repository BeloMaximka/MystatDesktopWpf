using MystatAPI.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Services
{

    internal static class HomeworkEventsService
    {
        static List<HomeworkNode> nodes;
        private static readonly string directoryName = @"%appdata%\Mystat";
        static readonly string fileName = Environment.ExpandEnvironmentVariables(@$"{directoryName}\hwdata");

        static HomeworkEventsService()
        {
            if(!File.Exists(fileName))
            {
                nodes = new();
                return;
            }

            string content = File.ReadAllText(fileName);
            nodes = JsonSerializer.Deserialize<List<HomeworkNode>>(content) ?? throw new InvalidDataException("Homework nodes are not in correct format");
        }

        public static void Save(Dictionary<HomeworkStatus, HomeworkCollection> dict)
        {
            var allHomeworks = dict.Values.Select(x => x.Items).SelectMany(x => x.Select(i => HomeworkNode.From(i))).ToArray();
            File.WriteAllText(fileName, JsonSerializer.Serialize(allHomeworks));
        }

        public static HomeworkEvent[] GetEvents(Dictionary<HomeworkStatus, HomeworkCollection> dict, bool dontSaveItems = false)
        {
            var allHomeworks = dict.Values.Select(x => x.Items).SelectMany(x => x).ToArray();
            var events = GetEvents(allHomeworks);

            if (!dontSaveItems)
            {
                Save(dict);
            }

            return events.ToArray();
        }

        static HomeworkEvent[] GetEvents(Homework[] items)
        {
            List<HomeworkEvent> events = new();

            if (nodes.Count == 0) return events.ToArray();

            for(int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var node = nodes.FirstOrDefault(n => n.Id == item.Id);

                if(node is null)
                {
                    events.Add(new HomeworkEvent(HomeworkNode.From(item), item.Status, true));
                }
                else if(node.Status != item.Status)
                {
                    events.Add(new HomeworkEvent(node, item.Status));
                }
            }

            return events.ToArray();
        }
    }

    class HomeworkNode
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public HomeworkStatus Status { get; set; }

        public static HomeworkNode From(Homework item)
        {
            return new HomeworkNode()
            {
                Id = item.Id,
                Status = item.Status,
                SubjectName = item.SubjectName
            };
        }
    }

    class HomeworkEvent
    {
        public HomeworkNode Node { get; set; }
        public HomeworkStatus NewStatus { get; set; }
        public bool IsRecentlyAdded { get; set; }

        public HomeworkEvent(HomeworkNode node, HomeworkStatus newStatus, bool isRecentlyAdded = false)
        {
            Node = node;
            NewStatus = newStatus;
            IsRecentlyAdded = isRecentlyAdded;
        }
    }
}
