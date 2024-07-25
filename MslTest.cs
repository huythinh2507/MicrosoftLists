using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Json;
using System.Text.Json;
using CsvHelper;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using MsLServiceLayer;
using PresentationLayer.Controllers;

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

            list.AddRow(42, 42, 42);

            var newL = service.CreateFromExistingList(list.Id, "existing");

            Assert.Equal(newL.Columns.Count, list.Columns.Count);
            Assert.Equal(newL.Rows[0].Cells.Count, list.Rows[0].Cells.Count);
        }

        [Fact]
        public void Test_CreateListFromTemplate()
        {
            var (service, _) = CreateTestList();

            var path = MsLConstant.FilePath;
            ArgumentNullException.ThrowIfNull(path);

            var template = service.GetTemplate()[0];

            var list = service.CreateListFromTemplate(template);

            Assert.NotNull(list);
            Assert.Equal(template.Name, list.Name);
            Assert.Equal(template.Description, list.Description);
            Assert.Equal(template.Columns.Count, list.Columns.Count);
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
        public void Test_ColFilter()
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

            var searchResults = list.Search("Harry");

            Assert.Contains(searchResults, row => row.Cells.Exists(cell =>
            {
                var cellValue = cell.Value?.ToString();
                return cellValue != null && cellValue.Contains("Harry", StringComparison.OrdinalIgnoreCase);
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

        [Fact]
        public void Test_FilterByColumnValues()
        {
            var list = GetBlankList();
            list.AddCol(new TextColumn { Name = "Text Column" });
            list.AddCol(new Column { Name = "Number Column", Type = ColumnType.Number });

            list.AddRow("Harry Kane", 21);
            list.AddRow("Lebron James", 23);
            list.AddRow("Kevin Durant", 13);
            list.AddRow("Anthony Edwards", 11);
            list.AddRow("Stephen Curry", 30);

            var filteredValues = list.Columns[1].FilterBy(value => (int)value > 20);

            Assert.Equal(3, filteredValues.Count);
            Assert.Contains(21, filteredValues);
            Assert.Contains(23, filteredValues);
            Assert.Contains(30, filteredValues);
        }

        [Fact]
        public void Test_EditRow()
        {
            var (_, list) = CreateTestList();
            list.AddCol(new TextColumn { Name = "Text Column" });
            list.AddCol(new Column { Name = "Number Column", Type = ColumnType.Number });

            list.AddRow("Harry Kane", 21);
            list.AddRow("Lebron James", 23);
            list.AddRow("Kevin Durant", 13);
            list.AddRow("Anthony Edwards", 11);
            list.AddRow("Stephen Curry", 30);

            var rowId = list.Rows[0].Id;
            var newValues = new List<object> { "Harry Kane 2", 22 };

            list.EditRow(rowId, newValues);

            var editedRow = list.Rows.First(r => r.Id == rowId);
            Assert.Equal("Harry Kane 2", editedRow.Cells[0].Value);
            Assert.Equal(22, editedRow.Cells[1].Value);
        }

        [Fact]
        public void Test_DeleteRow()
        {
            var (_, list) = CreateTestList();
            list.AddCol(new TextColumn { Name = "Text Column" });
            list.AddCol(new Column { Name = "Number Column", Type = ColumnType.Number });

            list.AddRow("Harry Kane", 21);
            list.AddRow("Lebron James", 23);
            list.AddRow("Kevin Durant", 13);
            list.AddRow("Anthony Edwards", 11);
            list.AddRow("Stephen Curry", 30);

            var initialRowCount = list.Rows.Count;

            var rowID = list.Rows[0].Id;
            list.Delete(list.Rows[0]);

            var finalRowCount = list.Rows.Count;

            // Assert that the row count decreased by one
            Assert.Equal(initialRowCount - 1, finalRowCount);

            // Assert that the row with the specific ID is no longer in the list
            Assert.DoesNotContain(list.Rows, row => row.Id == rowID);
        }

        [Fact]
        public void Test_ExportToJson()
        {
            var (_, list) = CreateTestList();
            list.Name = "New List";

            list.AddCol(new Column { Name = "Text Column", Type = ColumnType.Text });
            list.AddCol(new Column { Name = "Number Column", Type = ColumnType.Number });

            list.AddRow("Harry Kane", 21);
            list.AddRow("Lebron James", 23);
            list.AddRow("Kevin Durant", 13);
            list.AddRow("Anthony Edwards", 11);
            list.AddRow("Stephen Curry", 30);

            string jsonFilePath = MsLConstant.FilePath;

            // Serialize the list to JSON
            var json = ListExporter.ExportToJson(list);
            ListExporter.SaveToJson(json, jsonFilePath);


            // Verify that the JSON file contains the new list
            var loadedTemplates = ListService.LoadTemplatesFromJson(jsonFilePath);
            Assert.NotNull(loadedTemplates);

            var newTemplate = loadedTemplates.Find(t => t.Name == list.Name);
            Assert.NotNull(newTemplate);
            Assert.Equal(list.Name, newTemplate.Name);
            Assert.Equal(list.Columns.Count, newTemplate.Columns.Count);
        }

        [Fact]
        public void Test_Form()
        {
            var (_, list) = CreateTestList();
            list.Name = "New List";

            list.AddCol(new Column { Name = "Text Column", Type = ColumnType.Text });
            list.AddCol(new Column { Name = "Number Column", Type = ColumnType.Number });

            var form = ListService.ToForm(list);

            Assert.Equal(list.Name, form.Name);
            Assert.Equal(list.Description, form.Description);
            Assert.Equal(list.Columns.Count, form.Columns.Count);
        }

        [Fact]
        public void Test_SubmitForm()
        {
            var (_, list) = CreateTestList();
            list.Name = "New List";

            list.AddCol(new Column { Name = "Text Column", Type = ColumnType.Text });
            list.AddCol(new Column { Name = "Number Column", Type = ColumnType.Number });

            var form = ListService.ToForm(list);
            form.AddRow("Harry", 2);

            Assert.Equal(list.Name, form.Name);
            Assert.Equal(list.Description, form.Description);
            Assert.Equal(list.Columns.Count, form.Columns.Count);
        }

        [Fact]
        public void Test_Paging()
        {
            var list = GetBlankList();
            list.PageSize = 2;

            list.AddCol(new TextColumn { Name = "Text Column" });
            list.AddCol(new Column { Name = "Number Column", Type = ColumnType.Number });

            list.AddRow("Row1", 1);
            list.AddRow("Row2", 2);
            list.AddRow("Row3", 3);
            list.AddRow("Row4", 4);

            var totalPages = list.GetTotalPages();
            Assert.Equal(2, totalPages);

            var firstPage = list.GetCurrentPage();
            Assert.Equal(2, firstPage.Count);
            Assert.Equal("Row1", firstPage[0].Cells[0].Value);
            Assert.Equal("Row2", firstPage[1].Cells[0].Value);

            list.NextPage();
            var secondPage = list.GetCurrentPage();
            Assert.Equal(2, secondPage.Count);
            Assert.Equal("Row3", secondPage[0].Cells[0].Value);
            Assert.Equal("Row4", secondPage[1].Cells[0].Value);

            list.PreviousPage();
            Assert.Equal("Row1", firstPage[0].Cells[0].Value);
            Assert.Equal("Row2", firstPage[1].Cells[0].Value);
        }

        [Fact]
        public void Test_AddUser()
        {
            var list = new List();
            var user = new User { Name = "testUser" };

            list.AddAccess(user);

            Assert.True(list.HasAccess(user));
        }

        [Fact]
        public void Test_RemoveUser()
        {
            var list = new List();
            var user = new User { Name = "testUser" };

            list.AddAccess(user);

            Assert.True(list.HasAccess(user));

            list.RemoveAccess(user);
            Assert.False(list.HasAccess(user));
        }

        [Fact]
        public void Test_UserCannotBeAddedTwice()
        {
            var list = new List();
            var user = new User { Name = "testUser" };

            list.AddAccess(user);
            list.AddAccess(user);

            Assert.Single(list.GetUsers().FindAll(u => u.Name == user.Name));
        }

        [Fact]
        public void CreateBlankList_ReturnsOkResult()
        {
            // Arrange
            var mockListService = new Mock<ListService>();
            var controller = new ListController(mockListService.Object);

            // Act
            var result = controller.CreateBlankList("MyList", "Description");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}

