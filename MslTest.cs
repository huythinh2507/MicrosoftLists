using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MicrosoftLists
{
    public class MslTest
    {
        public static List GetBlankList()
        {
            var listName = "Test List";
            var description = "Test Description";

            var list = new ListService().CreateBlankList(listName, description);
            return list;
        }

        [Fact]
        public void Test_CreateListFromBlank()
        {
            var list = GetBlankList();

            Assert.NotNull(list);
        }

        [Fact]
        public void Test_CreateListFromExisting()
        {
            var secondList = new ListService().CreateFromExistingList(GetBlankList());

            Assert.Equal(GetBlankList().Name, secondList.Name);
            Assert.Equal(GetBlankList().Description, secondList.Description);
            Assert.Equal(GetBlankList().Columns.Count, secondList.Columns.Count);
        }

        [Fact]
        public void Test_CreateListFromTemplate()
        {
            var listService = new ListService();
            var template = listService.GetTemplate()[0];

            var list = new ListService().CreateListFromTemplate(template);

            Assert.NotNull(list);
            Assert.Equal(template.Name, list.Name);
            Assert.Equal(template.Description, list.Description);
            Assert.Equal(template.Columns.Count, list.Columns.Count);
            Assert.Equal(template.Columns[0].Name, list.Columns[0].Name);
            Assert.Equal(template.Columns[0].Type, list.Columns[0].Type);
        }

        [Fact]
        public void Test_FavorList()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.FavorList(list.Id);

            Assert.True(list.IsFavorited);
        }

        [Fact]
        public void Test_DeleteList()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.DeleteList(list.Id);

            var deletedList = listService.GetList(list.Id);
            Assert.Null(deletedList); 
        }

        //ADD COLUMNS
        [Fact]
        public void Test_AddTextColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.AddColumn(list.Id, new TextColumn { Name = "Text Column" });


            Assert.Contains(list.Columns, c => c.Name == "Text Column" && c is TextColumn);
        }

        [Fact]
        public void Test_AddNumberColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.AddColumn(list.Id, new NumberColumn { Name = "Number Column", DefaultValue = 42 });

            var column = list.Columns.First(c => c.Name == "Number Column") as NumberColumn;
            Assert.NotNull(column);
            Assert.Equal(42, column.DefaultValue);
        }

        [Fact]
        public void Test_AddChoiceColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            var choices = new List<Choice>
            {
                new() { Name = "Choice 1", Color = Color.Blue },
                new() { Name = "Choice 2", Color = Color.Green },
                new() { Name = "Choice 3", Color = Color.Yellow }
            };

            listService.AddColumn(list.Id, new ChoiceColumn { Name = "Choice Column", Choices = choices, DefaultValue = string.Empty });

            var column = list.Columns.First(c => c.Name == "Choice Column") as ChoiceColumn;
            Assert.NotNull(column);
            Assert.Equal(choices, column.Choices);
        }

        [Fact]
        public void Test_AddDateAndTimeColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            var defaultDate = new DateTime(2023, 7, 15, 0, 0, 0, DateTimeKind.Utc);
            listService.AddColumn(list.Id, new DateColumn { Name = "Date Column", DefaultValue = defaultDate });

            var column = list.Columns.First(c => c.Name == "Date Column") as DateColumn;
            Assert.NotNull(column);
            Assert.Equal(defaultDate, column.DefaultValue);
        }

        [Fact]
        public void Test_AddMultipleLinesOfTextColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.AddColumn(list.Id, new MultipleLinesOfTextColumn { Name = "Multiple Lines Column", DefaultValue = "Line 1\nLine 2" });

            var column = list.Columns.First(c => c.Name == "Multiple Lines Column") as MultipleLinesOfTextColumn;
            Assert.NotNull(column);
            Assert.Equal("Line 1\nLine 2", column.DefaultValue);
        }

        [Fact]
        public void Test_AddPersonColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.AddColumn(list.Id, new PersonColumn { Name = "Person Column", DefaultValue = "John Doe" });

            var column = list.Columns.First(c => c.Name == "Person Column") as PersonColumn;
            Assert.NotNull(column);
            Assert.Equal("John Doe", column.DefaultValue);
        }

        [Fact]
        public void Test_AddYesNoColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.AddColumn(list.Id, new YesNoColumn { Name = "Yes/No Column", DefaultValue = true });
            
            var column = list.Columns.First(c => c.Name == "Yes/No Column") as YesNoColumn;
            Assert.NotNull(column);
            Assert.True(column.DefaultValue);
        }

      

        [Fact]
        public void Test_AddHyperlinkColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);
            var url = "http://example.com";

            listService.AddColumn(list.Id, new HyperlinkColumn { Name = "Hyperlink Column", DefaultValue = url });

            var column = list.Columns.First(c => c.Name == "Hyperlink Column") as HyperlinkColumn;
            Assert.NotNull(column);
            Assert.Equal(url, column.DefaultValue);
        }

        [Fact]
        public void Test_AddImageColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.AddColumn(list.Id, new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });

            var column = list.Columns.First(c => c.Name == "Image Column") as ImageColumn;
            Assert.NotNull(column);
            Assert.Equal("image.jpg", column.DefaultValue);
        }

        [Fact]
        public void Test_AddLookupColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            var lookupList = listService.CreateBlankList("Lookup List", "Lookup List Description");

            listService.AddColumn(list.Id, new LookupColumn { Name = "Lookup Column", ListID = lookupList.Id, ColumnID = lookupList.Columns[0].Id });

            var column = list.Columns.First(c => c.Name == "Lookup Column") as LookupColumn;
            Assert.NotNull(column);
            Assert.Equal(lookupList.Id, column.ListID);
            Assert.Equal(lookupList.Columns[0].Id, column.ColumnID);
        }

        [Fact]
        public void Test_AddAverageRatingColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.AddColumn(list.Id, new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });

            var column = list.Columns.First(c => c.Name == "Average Rating Column") as AverageRatingColumn;
            Assert.NotNull(column);
            Assert.Equal(column.Ratings, [4.0]);
        }

        [Fact]
        public void Test_HideColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.AddColumn(list.Id, new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });

            var column = list.Columns.First(c => c.Name == "Average Rating Column") as AverageRatingColumn;

            ListService.HideColumn(column);

            Assert.True(column?.IsHidden ?? false);
        }

        [Fact]
        public void Test_AddMultipleCols()
        {
            var listService = new ListService();
            var list = GetBlankList();

            listService.AddColumn(list.Id, new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });

            listService.AddColumn(list.Id, new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });

            var column = list.Columns.First(c => c.Name == "Average Rating Column") as AverageRatingColumn;
            ListService.HideColumn(column);

            Assert.True(column?.IsHidden ?? false);
        }

        [Fact]
        public void Test_AddItem()
        {
            // Arrange
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            // Act
            var commentContent = "This is a test comment.";
            var itemTitle = "Test Item";
            var comment = new Comment { Content = commentContent };
            listService.AddItem(list.Id, new Item { Title = itemTitle, Comments = [comment] });

            // Assert
            var addedItem = list.Items.First(i => i.Title == itemTitle);
            Assert.NotNull(addedItem);
            Assert.Contains(addedItem.Comments, c => c.Content == commentContent);
        }

       
        //
        //SERVICE LAYER
        //

        public class ListService
        {
            public class ListTemplate : List
            {
                public ListTemplate()
                {
                }
            }

            private readonly List<ListTemplate> _templates;
            private readonly List<List> _lists;

            public ListService()
            {
                _templates = LoadTemplatesFromJson(MsLConstant.FilePath);
                _lists = [];
            }

            public static List<ListTemplate>? LoadTemplatesFromJson(string filePath)
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<ListTemplate>>(json);
            }

            public List CreateBlankList(string listName, string description, Color color, string icon)
            {
                var newList = new List
                {
                    Id = Guid.NewGuid(),
                    Name = listName,
                    Description = description,
                    Columns = [],
                    Color = color,
                    Icon = icon,
                    Items = []
                };
                _lists.Add(newList);

                return newList;
            }

            public List CreateBlankList(string listName, string description)
            {
                return CreateBlankList(listName, description, Color.White, "🌟");
            }


            public List CreateFromExistingList(List list)
            {
                var newList = new List
                {
                    Id = Guid.NewGuid(),
                    Name = list.Name,
                    Description = list.Description,
                    Columns = list.Columns,
                    Color = list.Color,
                    Icon = list.Icon,
                    Items = list.Items,
                };

                _lists.Add(newList);

                return newList;
            }

            public List<ListTemplate> GetTemplate()
            {
                return _templates;
            }

            public List CreateListFromTemplate(ListTemplate template)
            {
                var newList = new List
                {
                    Id = Guid.NewGuid(),
                    Name = template.Name,
                    Description = template.Description,
                    Columns = template.Columns,
                    Color = template.Color,
                    Icon = template.Icon,
                    Items = template.Items,
                };

                _lists.Add(newList);

                return newList;
            }

            public void DeleteList(Guid id)
            {
                var listToRemove = _lists.First(l => l.Id == id);
                _lists.Remove(listToRemove);
            }
            //throw if null
            public List GetList(Guid id)
            {
                return _lists.Find(l => l.Id == id);
            }

            public void FavorList(Guid id)
            {
                var listToRemove = _lists.First(l => l.Id == id);
                listToRemove.IsFavorited = true;
            }

            public void AddColumn<T>(Guid listId, T colType) where T : Column
            {
                var list = GetList(listId);
                list?.Columns.Add(colType);
            }

            public void AddItem(Guid listId, Item item)
            {
                var list = GetList(listId);
                list?.Items.Add(item);
            }

            public static void HideColumn(Column column)
            {
                column.IsHidden = true;
            }
        }
    }

    public static class MsLConstant
    {
        public const string FilePath = "C:\\Users\\thinh\\source\\repos\\MicrosoftLists\\templates.json";
    }

    public class Column
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public ColumnType Type { get; set; } = ColumnType.Text;
        public string Value { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsHidden { get; set; } = false;
    }

    public enum ColumnType
    {
        Text,
        Number,
        Choice,
        DateAndTime,
        MultipleLinesOfText,
        Person,
        YesNo,
        Hyperlink,
        Image,
        Lookup,
        AverageRating
    }

    public class PersonColumn : Column
    {
        public string DefaultValue { get; set; } = string.Empty;
        public bool ShowProfilePic { get; set; } = false;

        public PersonColumn()
        {
            Type = ColumnType.Person;
        }
    }


    public class YesNoColumn : Column
    {
        public bool DefaultValue { get; set; } = false;

        public YesNoColumn()
        {
            Type = ColumnType.YesNo;
        }
    }

    public class HyperlinkColumn : Column
    {
        public string DefaultValue { get; set; } = string.Empty;

        public HyperlinkColumn()
        {
            Type = ColumnType.Hyperlink;
        }
    }

    public class ImageColumn : Column
    {
        public string DefaultValue { get; set; } = string.Empty;

        public ImageColumn()
        {
            Type = ColumnType.Image;
        }
    }

    public class LookupColumn : Column
    {
        public Guid ListID { get; set; }
        public Guid ColumnID { get; set; }

        public LookupColumn()
        {
            Type = ColumnType.Lookup;
        }
    }

    public class AverageRatingColumn : Column
    {
        public List<double> Ratings { get; set; } = [];

        public AverageRatingColumn()
        {
            Type = ColumnType.AverageRating;
        }

        public double GetAverageRating()
        {
            return Ratings.Average();
        }
    }

    public class MultipleLinesOfTextColumn : Column
    {
        public string DefaultValue { get; set; } = string.Empty;

        public MultipleLinesOfTextColumn()
        {
            Type = ColumnType.MultipleLinesOfText;
        }
    }

    public class TextColumn : Column
    {
        public string DefaultValue { get; set; } = string.Empty;
        public bool CalculatedValue { get; set; } = false;
        public bool AtoZFilter { get; set; } = false;
        public bool ZtoAFilter { get; set; } = false;

        public TextColumn()
        {
            Type = ColumnType.Text;
        }
    }

    public class NumberColumn : Column
    {
        public double DefaultValue { get; set; } = 0.0;

        public NumberColumn()
        {
            Type = ColumnType.Number;
        }
    }

    public class ChoiceColumn : Column
    {
        public List<Choice> Choices { get; set; } =
        [
            new Choice { Name = "Choice 1", Color = Color.Blue },
        new Choice { Name = "Choice 2", Color = Color.Green },
        new Choice { Name = "Choice 3", Color = Color.Yellow }
        ];
        public string DefaultValue { get; set; } = string.Empty;
        public bool AddValuesManually { get; set; } = false;

        public ChoiceColumn()
        {
            Type = ColumnType.Choice;
        }
    }

    public class DateColumn : Column
    {
        public DateTime DefaultValue { get; set; } = DateTime.Now;

        public DateColumn()
        {
            Type = ColumnType.DateAndTime;
        }
    }


    public class Choice
    {
        public string Name { get; set; } = string.Empty;
        public Color Color { get; set; }
    }

    public class List
    {
        public Guid Id = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Column> Columns { get; set; } = [];
        public List<Item> Items { get; set; } = [];
        public Color Color { get; set; }
        public string Icon { get; set; } = string.Empty;
        public bool IsFavorited { get; set; } = false;
        public bool IsGridView { get; set; } = false;
        public bool IsShared { get; set; } = false;
        public bool IsExported { get; set; } = false;
        public bool Undo { get; set; } = false;
        public bool Redo { get; set;} = false;
    }

    public class Item
    {
        public string Title { get; internal set; } = string.Empty;
        public List<Comment> Comments { get; set; } = [];
    }

    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty;
        public DateTime Time { get; set; } = DateTime.UtcNow;
    }
}
