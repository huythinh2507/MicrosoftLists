
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Drawing;

namespace MicrosoftLists
{
    public class MslTest
    {

        [Fact]
        public void CreateListFromBlank()
        {
            var listName = "Test List";
            var description = "Test Description";

            var result = new ListService().CreateBlankList(listName, description);

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateListFromExisting()
        {
            var listName = "Test List";
            var description = "Test Description";

            var firstList = new ListService().CreateBlankList(listName, description);

            var secondList = new ListService().CreateFromExistingList(firstList);

            Assert.Equal(firstList.Name, secondList.Name);
            Assert.Equal(firstList.Description, secondList.Description);
            Assert.Equal(firstList.Columns.Count, secondList.Columns.Count);
        }

        [Fact]
        public void CreateListFromTemplate()
        {
            var listService = new ListService();
            var template = listService.GetTemplate()[0];

            var result = new ListService().CreateListFromTemplate(template);

            Assert.NotNull(result);
            Assert.Equal(template.Name, result.Name);
            Assert.Equal(template.Description, result.Description);
            Assert.Equal(template.Columns.Count, result.Columns.Count);
            Assert.Equal(template.Columns[0].Name, result.Columns[0].Name);
            Assert.Equal(template.Columns[0].Type, result.Columns[0].Type);
        }

        [Fact]
        public void FavorList()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.FavorList(list.Id);

            Assert.True(list.IsFavorited);
        }

        [Fact]
        public void DeleteList()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.DeleteList(list.Id);

            var deletedList = listService.GetList(list.Id);
            Assert.Null(deletedList); // Ensure the list is deleted
        }

        [Fact]
        public void AddItem()
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


        [Fact]
        public void AddTextColumn()
        {
            var listService = new ListService();
            var listName = "Test List";
            var description = "Test Description";
            var list = listService.CreateBlankList(listName, description);

            listService.AddColumn(list.Id, new TextColumn { Name = "Text Column" });

            Assert.Contains(list.Columns, c => c.Name == "Text Column" && c is TextColumn);
        }

        [Fact]
        public void AddNumberColumn()
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
        public void AddChoiceColumn()
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
        public void AddDateAndTimeColumn()
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
        public void AddMultipleLinesOfTextColumn()
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
        public void AddPersonColumn()
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
        public void AddYesNoColumn()
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
        public void AddHyperlinkColumn()
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
        public void AddImageColumn()
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
        public void AddLookupColumn()
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
        public void AddAverageRatingColumn()
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

            // Use null-conditional operator to access IsHidden property
            Assert.True(column?.IsHidden ?? false);
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
                _templates =
                [
                    new ListTemplate
                    {
                        Name = "Template1",
                        Description = "Template Description 1",
                        Columns =
                        [
                            new() { Name = "Column1", Type = "Text" },
                            new() { Name = "Column2", Type = "Number" }
                        ],
                        Color = Color.LightBlue,
                        Icon = "TemplateIcon1",
                        Items = []
                    },
                    new ListTemplate
                    {
                        Name = "Template2",
                        Description = "Template Description 2",
                        Columns =
                        [
                            new() { Name = "ColumnA", Type = "Choice" },
                            new() { Name = "ColumnB", Type = "Date" }
                        ],
                        Color = Color.LightGreen,
                        Icon = "TemplateIcon2",
                        Items = []
                    }
                ];

                _lists = [];
            }
            public List CreateBlankList(string listName, string description)
            {
                var newList = new List
                {
                    Id = Guid.NewGuid(),
                    Name = listName,
                    Description = description,
                    Columns = [],
                    Color = Color.White,
                    Icon = "Smile",
                    Items = []
                };

                _lists.Add(newList);

                return newList;
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

            public List GetList(Guid id)
            {
                return _lists?.Find(l => l.Id == id);
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

    public class Column
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsHidden { get; set; } = false;
    }

    public class PersonColumn : Column
    {
        public new string Type { get; set; } = "Person";
        public string DefaultValue { get; set; } = string.Empty;

        public bool ShowProfilePic = false;
    }

    public class YesNoColumn : Column
    {
        public new string Type { get; set; } = "Yes/No";
        public bool DefaultValue { get; set; } = false;
    }

    public class HyperlinkColumn : Column
    {
        public new string Type { get; set; } = "Hyperlink";
        public string DefaultValue { get; set; } = string.Empty;
    }

    public class ImageColumn : Column
    {
        public new string Type { get; set; } = "Image";
        public string DefaultValue { get; set; } = string.Empty;
    }

    public class LookupColumn : Column
    {
        public new string Type { get; set; } = "Lookup";
        public Guid ListID { get; set; }
        public Guid ColumnID { get; set; }
    }

    public class AverageRatingColumn : Column
    {
        public new string Type { get; set; } = "Average Rating";
        public List<double> Ratings { get; set; } = [];

        public double GetAverageRating()
        {
            return Ratings.Average();
        }
        
    }

    public class MultipleLinesOfTextColumn : Column
    {
        public new string Type { get; set; } = "Multiple lines of text";
        public string DefaultValue { get; set; } = string.Empty;
    }

    public class TextColumn : Column
    {
        public new string Type { get; set; } = "Text";
        public string DefaultValue { get; set; } = string.Empty;

        public bool CalculatedValue = false;

        public bool AtoZFilter = false;

        public bool ZtoAFilter = false;
    }

    public class NumberColumn : Column
    {
        public new string Type { get; set; } = "Number";
        public double DefaultValue { get; set; } = 0.0;
    }

    public class ChoiceColumn : Column
    {
        public new string Type { get; set; } = "Choice";
        public List<Choice> Choices { get; set; } =
        [
            new() { Name = "Choice 1", Color = Color.Blue },
            new() { Name = "Choice 2", Color = Color.Green },
            new() { Name = "Choice 3", Color = Color.Yellow }
        ];
        public string DefaultValue { get; set; } = string.Empty;
        public bool AddValuesManually { get; set; } = false;
    }

    public class DateColumn : Column
    {
        public new string Type { get; set; } = "Date and time";
        public DateTime DefaultValue { get; set; } = DateTime.Now;
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
