using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Linq;
using System;
using Xunit.Sdk;
using static MicrosoftLists.MslTest;
using System.Globalization;

namespace MicrosoftLists
{
    public class MslTest
    {
        private static (ListService service, List list) CreateTestList(string listName = "Test List", string description = "Test Description")
        {
            var service = new ListService();
            var list = service.CreateBlankList(listName, description);
            return (service, list);
        }

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
        public void Test_CreateFromExisting()
        {
            var (service, list) = CreateTestList();

            list.AddCol(new NumberColumn { Name = "Number Column", DefaultValue = 42 });
            list.AddCol(new NumberColumn { Name = "Number Column", DefaultValue = 42 });
            list.AddCol(new NumberColumn { Name = "Number Column", DefaultValue = 42 });

            list.AddRow();

            var newL = service.CreateFromExistingList(list.Id, "existing");

            Assert.Equal(newL.Columns.Count, list.Columns.Count);
            Assert.Equal(newL.Rows[0].Cells.Count, list.Rows[0].Cells.Count);
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
            var (service, list) = CreateTestList();

            service.FavorList(list.Id);

            Assert.True(list.IsFavorited);
        }

        [Fact]
        public void Test_DeleteList()
        {
            var (service, list) = CreateTestList();

            service.DeleteList(list.Id);

            var deletedList = service.GetList(list.Id);
            Assert.Null(deletedList);
        }

        //ADD COLUMNS
        [Fact]
        public void Test_AddTextColumn()
        {
            var list = GetBlankList();

            list.AddCol(new TextColumn { Name = "Text Column" });


            Assert.Contains(list.Columns, c => c.Name == "Text Column" && c is TextColumn);
        }

        [Fact]
        public void Test_AddNumberColumn()
        {
            var list = GetBlankList();

            list.AddCol(new NumberColumn { Name = "Number Column", DefaultValue = 42 });

            var column = list.Columns.First(c => c.Name == "Number Column") as NumberColumn;
            Assert.NotNull(column);
            Assert.Equal(42, column.DefaultValue);
        }

        [Fact]
        public void Test_AddChoiceColumn()
        {
            var list = GetBlankList();

            var choices = new List<Choice>
            {
                new() { Name = "Choice 1", Color = Color.Blue },
                new() { Name = "Choice 2", Color = Color.Green },
                new() { Name = "Choice 3", Color = Color.Yellow }
            };

            list.AddCol(new ChoiceColumn { Name = "Choice Column", Choices = choices, DefaultValue = string.Empty });

            var column = list.Columns.First(c => c.Name == "Choice Column") as ChoiceColumn;
            Assert.NotNull(column);
            Assert.Equal(choices, column.Choices);
        }

        [Fact]
        public void Test_AddDateAndTimeColumn()
        {
            var list = GetBlankList();

            var defaultDate = new DateTime(2023, 7, 15, 0, 0, 0, DateTimeKind.Utc);
            list.AddCol(new DateColumn { Name = "Date Column", DefaultValue = defaultDate });

            var column = list.Columns.First(c => c.Name == "Date Column") as DateColumn;
            Assert.NotNull(column);
            Assert.Equal(defaultDate, column.DefaultValue);
        }

        [Fact]
        public void Test_AddMultipleLinesOfTextColumn()
        {
            var list = GetBlankList();

            list.AddCol(new MultipleLinesOfTextColumn { Name = "Multiple Lines Column", DefaultValue = "Line 1\nLine 2" });

            var column = list.Columns.First(c => c.Name == "Multiple Lines Column") as MultipleLinesOfTextColumn;
            Assert.NotNull(column);
            Assert.Equal("Line 1\nLine 2", column.DefaultValue);
        }

        [Fact]
        public void Test_AddPersonColumn()
        {
            var list = GetBlankList();

            list.AddCol(new PersonColumn { Name = "Person Column", DefaultValue = "John Doe" });

            var column = list.Columns.First(c => c.Name == "Person Column") as PersonColumn;
            Assert.NotNull(column);
            Assert.Equal("John Doe", column.DefaultValue);
        }

        [Fact]
        public void Test_AddYesNoColumn()
        {
            var list = GetBlankList();

            list.AddCol(new YesNoColumn { Name = "Yes/No Column", DefaultValue = true });

            var column = list.Columns.First(c => c.Name == "Yes/No Column") as YesNoColumn;
            Assert.NotNull(column);
            Assert.True(column.DefaultValue);
        }

        [Fact]
        public void Test_AddHyperlinkColumn()
        {
            var list = GetBlankList();
            var url = "http://example.com";

            list.AddCol(new HyperlinkColumn { Name = "Hyperlink Column", DefaultValue = url });

            var column = list.Columns.First(c => c.Name == "Hyperlink Column") as HyperlinkColumn;
            Assert.NotNull(column);
            Assert.Equal(url, column.DefaultValue);
        }

        [Fact]
        public void Test_AddImageColumn()
        {
            var list = GetBlankList();

            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });

            var column = list.Columns.First(c => c.Name == "Image Column") as ImageColumn;
            Assert.NotNull(column);
            Assert.Equal("image.jpg", column.DefaultValue);
        }

        [Fact]
        public void Test_AddLookupColumn()
        {
            var listService = new ListService();
            var list = GetBlankList();

            var lookupList = listService.CreateBlankList("Lookup List", "Lookup List Description");

            lookupList.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });
            list.AddCol(new LookupColumn { Name = "Lookup Column", ListID = lookupList.Id, ColumnID = lookupList.Columns[0].Id });

            var column = list.Columns.First(c => c.Name == "Lookup Column") as LookupColumn;
            Assert.NotNull(column);
            Assert.Equal(lookupList.Id, column.ListID);
            Assert.Equal(lookupList.Columns[0].Id, column.ColumnID);
        }

        [Fact]
        public void Test_AddAverageRatingColumn()
        {
            var list = GetBlankList();

            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });

            var column = list.Columns.First(c => c.Name == "Average Rating Column") as AverageRatingColumn;
            Assert.NotNull(column);
            Assert.Equal(column.Ratings, [4.0]);
        }

        [Fact]
        public void Test_RenameColumn()
        {
            var list = GetBlankList();

            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });

            var column = list.Columns.First(c => c.Name == "Image Column") as ImageColumn;

            ArgumentNullException.ThrowIfNull(column);

            var newName = "new";
            column.Rename(newName);

            Assert.Equal(newName, column.Name);
        }

        [Fact]
        public void Test_HideColumn()
        {
            var list = GetBlankList();

            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });

            var column = list.Columns.First(c => c.Name == "Image Column") as ImageColumn;

            ArgumentNullException.ThrowIfNull(column);

            column.Hide();

            Assert.True(column.IsHidden);
        }

        [Fact]
        public void Test_WidenColumn()
        {
            var list = GetBlankList();

            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });

            var column = list.Columns.First(c => c.Name == "Image Column") as ImageColumn;

            ArgumentNullException.ThrowIfNull(column);

            column.Widen();

            Assert.Equal(MsLConstant.DefaultColWidth + 2, column.Width);

            column.Widen();

            Assert.Equal(MsLConstant.DefaultColWidth + 4, column.Width);
        }

        [Fact]
        public void Test_NarrowColumn()
        {
            var list = GetBlankList();

            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });

            var column = list.Columns.First(c => c.Name == "Image Column") as ImageColumn;

            ArgumentNullException.ThrowIfNull(column);

            //cant be narrowed because meets limit
            column.Narrow();
            Assert.Equal(MsLConstant.DefaultColWidth, column.Width);

            //widen to 10 + 2
            column.Widen();
            Assert.Equal(MsLConstant.DefaultColWidth + 2, column.Width);

            //narrow 
            column.Narrow();
            Assert.Equal(MsLConstant.DefaultColWidth, column.Width);
        }

        [Fact]
        public void Test_AddMultipleCols()
        {
            var list = GetBlankList();

            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });
            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });
            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });
            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });

            list.AddRow();
            list.AddRow();

            var noRows = list.Rows.Count;
            var noCells = list.Rows[0].Cells.Count;

            Assert.Equal(2, noRows);
            Assert.Equal(4, noCells);

            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });
            var newNoCells = list.Rows[0].Cells.Count;

            Assert.Equal(4 + 1, newNoCells);
        }

        [Fact]
        public void Test_SetCellValue()
        {
            var list = GetBlankList();

            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });
            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });
            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });
            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });

            list.AddRow();
            list.AddRow();

            //wrong setting
            void setValueAction() => list.Rows[0].Cells[0].SetValue("image.png");

            Assert.Throws<ArgumentException>(setValueAction);
        }

        [Fact]
        public void Test_MoveColRight()
        {
            var list = GetBlankList();

            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column 1", Ratings = [4.0] });
            list.AddCol(new ImageColumn { Name = "Image Column 1", DefaultValue = "image.jpg" });
            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column 2", Ratings = [4.0] });
            list.AddCol(new ImageColumn { Name = "Image Column 2", DefaultValue = "image.jpg" });

            list.AddRow();
            list.AddRow();

            // Initial positions
            Assert.Equal("Average Rating Column 1", list.Columns[0].Name);
            Assert.Equal("Image Column 1", list.Columns[1].Name);
            Assert.Equal("Average Rating Column 2", list.Columns[2].Name);
            Assert.Equal("Image Column 2", list.Columns[3].Name);

            // Move second column right
            list.Columns[1].MoveRight();

            // New positions
            Assert.Equal("Average Rating Column 1", list.Columns[0].Name);
            Assert.Equal("Average Rating Column 2", list.Columns[1].Name);
            Assert.Equal("Image Column 1", list.Columns[2].Name);
            Assert.Equal("Image Column 2", list.Columns[3].Name);

            // Move third column left
            list.Columns[2].MoveLeft();

            // New positions
            Assert.Equal("Average Rating Column 1", list.Columns[0].Name);
            Assert.Equal("Image Column 1", list.Columns[1].Name);
            Assert.Equal("Average Rating Column 2", list.Columns[2].Name);
            Assert.Equal("Image Column 2", list.Columns[3].Name);
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
                var path = MsLConstant.FilePath;
                ArgumentNullException.ThrowIfNull(path);

                _templates = LoadTemplatesFromJson(path) ?? [];
                ArgumentNullException.ThrowIfNull(_templates);

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
                    Rows = []
                };
                _lists.Add(newList);

                return newList;
            }

            public List CreateBlankList(string listName, string description)
            {
                return CreateBlankList(listName, description, Color.White, "🌟");
            }


            public List CreateFromExistingList(Guid existingID, string newName, string description = "", Color? color = null, string icon = "Smile")
            {
                var existingList = _lists.Find(l => l.Id == existingID);
                ArgumentNullException.ThrowIfNull(existingList);

                var newList = new List
                {
                    Id = Guid.NewGuid(),
                    Name = newName,
                    Description = description,
                    Columns = existingList.Columns,
                    Color = color ?? Color.Transparent, // Use null-coalescing operator to default to Color.Transparent if color is null
                    Icon = icon,
                    Rows = existingList.Rows
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
                    Rows = template.Rows,
                };

                _lists.Add(newList);

                return newList;
            }

            public void DeleteList(Guid id)
            {
                var listToRemove = _lists.First(l => l.Id == id);
                _lists.Remove(listToRemove);
            }

            public List? GetList(Guid id)
            {
                return _lists.Find(l => l.Id == id);
            }



            public void FavorList(Guid id)
            {
                var listToRemove = _lists.First(l => l.Id == id);
                listToRemove.IsFavorited = true;
            }

            public static void HideColumn(Column column)
            {
                ArgumentNullException.ThrowIfNull(column);
                column.IsHidden = true;
            }


        }

        public static class MsLConstant
        {
            public const int DefaultColWidth = 10;
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

            public int Width { get; set; } = MsLConstant.DefaultColWidth;
            public List ParentList { get; set; } = new List();

            public void Hide()
            {
                IsHidden = true;
            }

            public void Widen()
            {
                Width += 2;
            }

            public void Narrow()
            {
                Width = Math.Max(Width - 2, MsLConstant.DefaultColWidth);
            }

            public void Rename(string newName)
            {
                Name = newName;
            }

            public void MoveRight()
            {
                int index = ParentList.Columns.IndexOf(this);
                if (index < ParentList.Columns.Count - 1)
                {
                    ParentList.MoveColumnRight(index);
                }
            }

            public void MoveLeft()
            {
                int index = ParentList.Columns.IndexOf(this);
                if (index > 0)
                {
                    ParentList.MoveColumnLeft(index);
                }
            }
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
            public Color Color { get; set; }
            public List<Row> Rows { get; set; } = [];
            public string Icon { get; set; } = string.Empty;
            public bool IsFavorited { get; set; } = false;
            public bool IsGridView { get; set; } = false;
            public bool IsShared { get; set; } = false;
            public bool IsExported { get; set; } = false;
            public bool Undo { get; set; } = false;
            public bool Redo { get; set; } = false;

            public void AddCol<T>(T col) where T : Column
            {
                col.ParentList = this;
                Columns.Add(col);
                
                // Update existing rows to match the new column count
                foreach (var row in Rows)
                {
                    while (row.Cells.Count < Columns.Count)
                    {
                        row.Cells.Add(new Cell());
                    }
                }
            }

            public void AddRow()
            {
                var newRow = new Row();
                newRow.Cells.AddRange(Columns.Select(column => new Cell()
                {
                    ColumnType = column.Type,
                }));
                Rows.Add(newRow);
            }

            public void MoveColumnLeft(int columnIndex)
            {
                if (columnIndex > 0 && columnIndex < Columns.Count)
                {
                    // Swap columns
                    (Columns[columnIndex - 1], Columns[columnIndex]) = (Columns[columnIndex], Columns[columnIndex - 1]);

                    // Update rows
                    foreach (var row in Rows)
                    {
                        (row.Cells[columnIndex - 1], row.Cells[columnIndex]) = (row.Cells[columnIndex], row.Cells[columnIndex - 1]);
                    }
                }
            }

            public void MoveColumnRight(int index)
            {
                if (index >= 0 && index < Columns.Count - 1)
                {
                    // Swap columns
                    (Columns[index + 1], Columns[index]) = (Columns[index], Columns[index + 1]);

                    // Update rows
                    foreach (var row in Rows)
                    {
                        (row.Cells[index + 1], row.Cells[index]) = (row.Cells[index], row.Cells[index + 1]);
                    }
                }
            }

            public class Row
            {
                public List<Cell> Cells { get; set; } = [];
            }
        }
    }

    public class Cell
    {
        public object Value { get; set; } = string.Empty;
        public ColumnType ColumnType { get; set; }

        private static readonly Dictionary<ColumnType, Func<object, object>> ValueConverters = new()
        {
            { ColumnType.Text, value => value?.ToString() ?? string.Empty },
            { ColumnType.Number, value =>
                {
                    if (value is double v)
                        return v;

                    if (double.TryParse(value?.ToString(), out double d))
                        return d;

                    throw new ArgumentException("Invalid value for Number column");
                }
            },
            { ColumnType.Choice, value => value?.ToString() ?? string.Empty },
            { ColumnType.DateAndTime, value =>
                {
                    if (value is DateTime time)
                        return time;

                    if (DateTime.TryParse(value?.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                        return dt;

                    throw new ArgumentException("Invalid value for Date and Time column");
                }
            },
            { ColumnType.MultipleLinesOfText, value => value?.ToString() ?? string.Empty },
            { ColumnType.Person, value => value?.ToString() ?? string.Empty },
            { ColumnType.YesNo, value =>
                {
                    if (value is bool v)
                        return v;

                    if (bool.TryParse(value?.ToString(), out bool b))
                        return b;

                    throw new ArgumentException("Invalid value for Yes/No column");
                }
            },
            { ColumnType.Hyperlink, value => value?.ToString() ?? string.Empty },
            { ColumnType.Image, value => value?.ToString() ?? string.Empty },
            { ColumnType.Lookup, value => value },
            { ColumnType.AverageRating, value =>
                {
                    if (value is double v)
                        return v;

                    if (double.TryParse(value?.ToString(), out double ar))
                        return ar;

                    throw new ArgumentException("Invalid value for Average Rating column");
                }
            }
        };

        public void SetValue(object value)
        {
            if (ValueConverters.TryGetValue(ColumnType, out var converter))
            {
                Value = converter(value);
            }
            else
            {
                throw new ArgumentException("Unsupported column type");
            }
        }
    }
}

