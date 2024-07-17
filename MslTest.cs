using CsvHelper.Configuration;
using CsvHelper;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text.Json;
using static MicrosoftLists.MslTest;
using static MicrosoftLists.MslTest.List;

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

            // Add a row with specific values
            list.AddRow(3.5, "image1.jpg", 4.0, "image2.jpg");
            list.AddRow(4.0, "image3.jpg", 4.5, "image4.jpg");

            Assert.Equal(3.5, list.Columns[0].CellValues[0]);

            Assert.Equal(3.5, list.Rows[0].Cells[0].Value);

            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });
            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });
            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });
            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });

            list.AddRow(3.5, "image1.jpg", 4.0, "image2.jpg", 3.5, "image1.jpg", 4.0, "image2.jpg");

            Assert.Equal(3.5, list.Rows[0].Cells[0].Value);
        }

        [Fact]
        public void Test_MoveRightCol()
        {
            var list = GetBlankList();
            var url = "http://example.com";

            list.AddCol(new HyperlinkColumn { Name = "Hyperlink Column", DefaultValue = url });
            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });
            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });
            list.AddCol(new YesNoColumn { Name = "Yes/No Column", DefaultValue = true });

            // Add a row with specific values
            list.AddRow(3.5, "image1.jpg", 4.0, "image2.jpg");
            list.AddRow(4.0, "image3.jpg", 4.5, "image4.jpg");

            list.Columns[0].MoveRight();
            Assert.Equal("Hyperlink Column", list.Columns[1].Name);

            list.Columns[1].MoveRight();
            Assert.Equal("Hyperlink Column", list.Columns[1 + 1].Name);
        }

        [Fact]
        public void Test_MoveLeftCol()
        {
            var list = GetBlankList();
            var url = "http://example.com";

            list.AddCol(new HyperlinkColumn { Name = "Hyperlink Column", DefaultValue = url });
            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });
            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });
            list.AddCol(new YesNoColumn { Name = "Yes/No Column", DefaultValue = true });

            // Add a row with specific values
            list.AddRow(3.5, "image1.jpg", 4.0, "image2.jpg");
            list.AddRow(4.0, "image3.jpg", 4.5, "image4.jpg");

            list.Columns[0].MoveLeft();
            Assert.Equal("Hyperlink Column", list.Columns[0].Name);

            list.Columns[1].MoveLeft();
            Assert.Equal("Hyperlink Column", list.Columns[1].Name);
            Assert.Equal("Image Column", list.Columns[0].Name);
        }

        [Fact]
        public void Test_HideCol()
        {
            var list = GetBlankList();
            var url = "http://example.com";

            list.AddCol(new HyperlinkColumn { Name = "Hyperlink Column", DefaultValue = url });
            list.AddCol(new ImageColumn { Name = "Image Column", DefaultValue = "image.jpg" });
            list.AddCol(new AverageRatingColumn { Name = "Average Rating Column", Ratings = [4.0] });
            list.AddCol(new YesNoColumn { Name = "Yes/No Column", DefaultValue = true });

            list.Columns[0].Hide();

            Assert.True(list.Columns[0].IsHidden);
        }

        [Fact]
        public void Test_ColFilterByAtoZ()
        {
            var list = GetBlankList();

            list.AddCol(new TextColumn { Name = "Text Column" });

            list.AddRow("b");
            list.AddRow("a");
            list.AddRow("d");
            list.AddRow("e");
            list.AddRow("c");

            list.Columns[0].AtoZ();
            var result = new List<object> { "a", "b", "c", "d", "e" };
            Assert.Equal(result, list.Columns[0].CellValues);

            list.Columns[0].ZtoA();
            var result1 = new List<object> { "e", "d", "c", "b", "a" };
            Assert.Equal(result1, list.Columns[0].CellValues);
        }

        [Fact]
        public void Test_SearchFunction()
        {
            var list = GetBlankList();

            list.AddCol(new TextColumn { Name = "Text Column" });
            list.AddCol(new Column { Name = "Number Column", Type = ColumnType.Number });
            
            list.AddRow("Harry Kane", 21);
            list.AddRow("Lebron James", 23);
            list.AddRow("Kevin Durant", 13);
            list.AddRow("Anthony Edwards", 11);
            list.AddRow("Stephen Curry", 30);

            var searchValue = "Harry";
            var searchResults = list.Search(searchValue);
            
            Assert.Contains(searchResults, row => row.Cells.Exists(cell =>
            {
                var cellValue = cell.Value?.ToString();
                return cellValue != null && cellValue.Contains(searchValue, StringComparison.OrdinalIgnoreCase);

            }));
        }

        [Fact]
        public void Test_ExportToCSV()
        {
            var list = GetBlankList();

            list.AddCol(new TextColumn { Name = "Text Column" });
            list.AddCol(new Column { Name = "Number Column", Type = ColumnType.Number });

            list.AddRow("Harry Kane", 21);
            list.AddRow("Lebron James", 23);
            list.AddRow("Kevin Durant", 13);
            list.AddRow("Anthony Edwards", 11);
            list.AddRow("Stephen Curry", 30);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "export.csv");

            ListExporter.ExportToCsv(list, filePath);

            Assert.True(File.Exists(filePath));
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

        }

        public static class MsLConstant
        {
            public const int DefaultColWidth = 10;
            public const string FilePath = "C:\\Users\\thinh\\source\\repos\\MicrosoftLists\\templates.json";
            public const int WidthIncrement = 2;
        }

        public class Column
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Name { get; set; } = string.Empty;
            public ColumnType Type { get; set; } = ColumnType.Text;
            public List<object> CellValues { get; set; } = [];
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
                Width += MsLConstant.WidthIncrement;
            }

            public void Narrow()
            {
                Width = Math.Max(Width - MsLConstant.WidthIncrement, MsLConstant.DefaultColWidth);
            }

            public void Rename(string newName)
            {
                Name = newName;
            }

            public void MoveRight()
            {
                int index = ParentList.Columns.IndexOf(this);
                ParentList.MoveColumnRight(index);
            }

            public void MoveLeft()
            {
                int index = ParentList.Columns.IndexOf(this);
                ParentList.MoveColumnLeft(index);
            }

            public void AddCellValue(object value)
            {
                CellValues.Add(value);
            }

            public void AtoZ()
            {
                var sortedValues = CellValues.OfType<string>()
                                             .OrderBy(val => val, StringComparer.Ordinal)
                                             .Cast<object>()
                                             .ToList();

                UpdateCellValues(sortedValues);
            }

            public void ZtoA()
            {
                var sortedValues = CellValues.OfType<string>()
                                             .OrderByDescending(val => val, StringComparer.Ordinal)
                                             .Cast<object>()
                                             .ToList();

                UpdateCellValues(sortedValues);
            }

            private void UpdateCellValues(List<object> sortedValues)
            {
                int sortedIndex = 0;
                CellValues = CellValues.Select(val => val is string ? sortedValues[sortedIndex++] : val).ToList();
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
            public Guid Id { get; set; } = Guid.NewGuid();
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

            public void AddRow(params object[] values)
            {
                if (values.Length != Columns.Count)
                {
                    throw new ArgumentException("Number of values must match the number of columns.");
                }

                var newRow = new Row();
                int index = 0;
                foreach (var value in values)
                {
                    var column = Columns[index];
                    var cell = new Cell
                    {
                        ColumnType = column.Type,
                        Value = value
                    };
                    newRow.Cells.Add(cell);
                    column.AddCellValue(value);
                    index++;
                }

                Rows.Add(newRow);
            }


            public void MoveColumnLeft(int index)
            {
                // Ensure index is within valid bounds
                if (index <= 0 || index >= Columns.Count) return;

                // Swap columns
                (Columns[index - 1], Columns[index]) = (Columns[index], Columns[index - 1]);

                // Update rows
                foreach (var row in Rows)
                {
                    (row.Cells[index - 1], row.Cells[index]) = (row.Cells[index], row.Cells[index - 1]);
                }
            }


            public void MoveColumnRight(int index)
            {
                // Ensure index is within valid bounds
                index = Math.Max(index, 0);
                index = Math.Min(index, Columns.Count - 2);

                // Swap columns
                (Columns[index + 1], Columns[index]) = (Columns[index], Columns[index + 1]);

                // Update rows
                foreach (var row in Rows)
                {
                    (row.Cells[index + 1], row.Cells[index]) = (row.Cells[index], row.Cells[index + 1]);
                }
            }

            public List<Row> Search(string query)
            {
                return Rows.Where(row => row.Cells.Exists(cell =>
                {
                    var cellValue = cell.Value?.ToString();
                    return cellValue != null && cellValue.Contains(query, StringComparison.OrdinalIgnoreCase);
                })).ToList();
            }


            public class Row
            {
                public List<Cell> Cells { get; set; } = [];
            }
        }
    }

    internal class ListExporter : List
    {
        public ListExporter()
        {
        }

        public static void ExportToCsv(List list, string filePath)
        {
            // Configure CSV writer options
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Adjust settings as needed, e.g., delimiter, quoting, etc.
                Delimiter = ",",
                HasHeaderRecord = true,
                IgnoreBlankLines = true
            };

            // Write CSV file
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, config);
            // Write header based on column names
            foreach (var col in list.Columns)
            {
                csv.WriteField(col.Name);
            }
            csv.NextRecord();

            // Write rows
            foreach (var row in list.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    csv.WriteField(cell.Value?.ToString() ?? string.Empty);
                }
                csv.NextRecord();
            }
        }
    }

    public class Cell
    {
        public object Value { get; set; } = string.Empty;
        public ColumnType ColumnType { get; set; }

        private readonly static Dictionary<ColumnType, Func<object, object>> ValueConverters = new()
        {
            [ColumnType.Text] = ConvertToString,
            [ColumnType.Number] = ConvertToNumber,
            [ColumnType.Choice] = ConvertToString,
            [ColumnType.DateAndTime] = ConvertToDateTime,
            [ColumnType.MultipleLinesOfText] = ConvertToString,
            [ColumnType.Person] = ConvertToString,
            [ColumnType.YesNo] = ConvertToYesNo,
            [ColumnType.Hyperlink] = ConvertToString,
            [ColumnType.Image] = ConvertToString,
            [ColumnType.Lookup] = value => value,
            [ColumnType.AverageRating] = ConvertToAverageRating
        };

        private static string ConvertToString(object value) => value?.ToString() ?? string.Empty;

        private static object ConvertToNumber(object value) => value switch
        {
            double v => v,
            string s when double.TryParse(s, out double d) => d,
            _ => throw new ArgumentException("Invalid value for Number column")
        };

        private static object ConvertToDateTime(object value) => value switch
        {
            DateTime time => time,
            string s when DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt) => dt,
            _ => throw new ArgumentException("Invalid value for Date and Time column")
        };

        private static object ConvertToYesNo(object value) => value switch
        {
            bool v => v,
            string s when bool.TryParse(s, out bool b) => b,
            _ => throw new ArgumentException("Invalid value for Yes/No column")
        };

        private static object ConvertToAverageRating(object value) => value switch
        {
            double v => v,
            string s when double.TryParse(s, out double ar) => ar,
            _ => throw new ArgumentException("Invalid value for Average Rating column")
        };

        public void SetValue(object value)
        {
            Value = ValueConverters.TryGetValue(ColumnType, out var converter)
                ? converter(value)
                : throw new ArgumentException("Unsupported column type");
        }

    }
}

