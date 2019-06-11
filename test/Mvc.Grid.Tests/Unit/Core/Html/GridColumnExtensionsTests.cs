﻿using Microsoft.AspNetCore.Mvc.Rendering;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NonFactors.Mvc.Grid.Tests.Unit
{
    public class GridColumnExtensionsTests
    {
        private IGridColumn<GridModel, String> column;

        public GridColumnExtensionsTests()
        {
            Expression<Func<GridModel, String>> expression = (model) => model.Name;
            column = Substitute.For<IGridColumn<GridModel, String>>();
            column.Expression.Returns(expression);

            column.Filter = new GridColumnFilter<GridModel, String>(column);
        }

        #region RenderedAs<T, TValue>(this IGridColumn<T, TValue> column, Func<T, Int32, Object> value)

        [Fact]
        public void RenderedAs_SetsIndexedRenderValue()
        {
            Func<GridModel, Int32, Object> expected = (model, i) => model.Name;
            Func<GridModel, Int32, Object> actual = column.RenderedAs(expected).RenderValue;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void RenderedAs_ReturnsIndexedColumn()
        {
            Object expected = column;
            Object actual = column.RenderedAs((model, i) => model.Name);

            Assert.Same(expected, actual);
        }

        #endregion

        #region RenderedAs<T, TValue>(this IGridColumn<T, TValue> column, Func<T, Object> value)

        [Fact]
        public void RenderedAs_SetsRenderValue()
        {
            GridModel gridModel = new GridModel { Name = "Test" };

            Object actual = column.RenderedAs(model => model.Name).RenderValue(gridModel, 0);
            Object expected = gridModel.Name;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void RenderedAs_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.RenderedAs(model => model.Name);

            Assert.Same(expected, actual);
        }

        #endregion

        #region UsingFilterOptions<T, TValue>(this IGridColumn<T, TValue> column, IEnumerable<SelectListItem> options)

        [Theory]
        [InlineData(null, "equals")]
        [InlineData("contains", "contains")]
        public void UsingFilterOptions_SetsDefaultFilterMethod(String current, String method)
        {
            column.Filter.DefaultMethod = current;

            String actual = column.UsingFilterOptions(new SelectListItem[0]).Filter.DefaultMethod;
            String expected = method;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UsingFilterOptions_SetsOptions()
        {
            IEnumerable<SelectListItem> expected = new SelectListItem[0];
            IEnumerable<SelectListItem> actual = column.UsingFilterOptions(expected).Filter.Options;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void UsingFilterOptions_EnablesFilter()
        {
            column.Filter.IsEnabled = false;

            Assert.True(column.UsingFilterOptions(new SelectListItem[0]).Filter.IsEnabled);
        }

        [Fact]
        public void UsingFilterOptions_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.UsingFilterOptions(new SelectListItem[0]);

            Assert.Same(expected, actual);
        }

        #endregion

        #region UsingFilterOptions<T, TValue>(this IGridColumn<T, TValue> column)

        [Theory]
        [InlineData(null, "equals")]
        [InlineData("contains", "contains")]
        public void UsingFilterOptions_FromValues_SetsDefaultFilterMethod(String current, String method)
        {
            column.Filter.DefaultMethod = current;

            String actual = column.UsingFilterOptions().Filter.DefaultMethod;
            String expected = method;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UsingFilterOptions_FromValues()
        {
            column.Grid.Source = new[]
            {
                new GridModel { Name = "Test" },
                new GridModel { Name = "Next" },
                new GridModel { Name = "Next" },
                new GridModel { Name = "Last" }
            }.AsQueryable();

            IEnumerator<SelectListItem> actual = column.UsingFilterOptions().Filter.Options.GetEnumerator();
            IEnumerator<SelectListItem> expected = new List<SelectListItem>
            {
                new SelectListItem { Value = null, Text = null },
                new SelectListItem { Value = "Last", Text = "Last" },
                new SelectListItem { Value = "Next", Text = "Next" },
                new SelectListItem { Value = "Test", Text = "Test" }
            }.GetEnumerator();

            while (expected.MoveNext() | actual.MoveNext())
            {
                Assert.Same(expected.Current.Value, actual.Current.Value);
                Assert.Same(expected.Current.Text, actual.Current.Text);
            }
        }

        [Fact]
        public void UsingFilterOptions_FromValues_EnablesFilter()
        {
            column.Filter.IsEnabled = false;

            Assert.True(column.UsingFilterOptions().Filter.IsEnabled);
        }

        [Fact]
        public void UsingFilterOptions_FromValues_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.UsingFilterOptions();

            Assert.Same(expected, actual);
        }

        #endregion

        #region UsingDefaultFilterMethod<T, TValue>(this IGridColumn<T, TValue> column, String method)

        [Fact]
        public void UsingDefaultFilterMethod_SetsDefaultFilterMethod()
        {
            String expected = "test";
            String actual = column.UsingDefaultFilterMethod("test").Filter.DefaultMethod;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void UsingDefaultFilterMethod_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.UsingDefaultFilterMethod("test");

            Assert.Same(expected, actual);
        }

        #endregion

        #region Filterable<T, TValue>(this IGridColumn<T, TValue> column, GridFilterType type)

        [Fact]
        public void Filterable_SetsType()
        {
            GridFilterType? actual = column.Filterable(GridFilterType.Double).Filter.Type;
            GridFilterType? expected = GridFilterType.Double;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Filterable_EnablesFilter()
        {
            column.Filter.IsEnabled = false;

            Assert.True(column.Filterable(GridFilterType.Single).Filter.IsEnabled);
        }

        [Fact]
        public void Filterable_Type_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.Filterable(GridFilterType.Single);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Filterable<T, TValue>(this IGridColumn<T, TValue> column, Boolean isFilterable)

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Filterable_SetsIsEnabled(Boolean isEnabled)
        {
            Boolean? actual = column.Filterable(isEnabled).Filter.IsEnabled;
            Boolean? expected = isEnabled;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Filterable_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.Filterable(true);

            Assert.Same(expected, actual);
        }

        #endregion

        #region FilteredAs<T, TValue>(this IGridColumn<T, TValue> column, String filterName)

        [Fact]
        public void FilteredAs_SetsFilterName()
        {
            String actual = column.FilteredAs("Numeric").Filter.Name;
            String expected = "Numeric";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FilteredAs_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.FilteredAs("Numeric");

            Assert.Same(expected, actual);
        }

        #endregion

        #region InitialSort<T, TValue>(this IGridColumn<T, TValue> column, GridSortOrder order)

        [Fact]
        public void InitialSort_SetsInitialSortOrder()
        {
            GridSortOrder? actual = column.InitialSort(GridSortOrder.Desc).Sort.InitialOrder;
            GridSortOrder? expected = GridSortOrder.Desc;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InitialSort_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.InitialSort(GridSortOrder.Desc);

            Assert.Same(expected, actual);
        }

        #endregion

        #region FirstSort<T, TValue>(this IGridColumn<T, TValue> column, GridSortOrder order)

        [Fact]
        public void FirstSort_SetsFirstOrder()
        {
            GridSortOrder? actual = column.FirstSort(GridSortOrder.Desc).Sort.FirstOrder;
            GridSortOrder? expected = GridSortOrder.Desc;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FirstSort_ReturnsColumn()
        {
            Object actual = column.FirstSort(GridSortOrder.Desc);
            Object expected = column;

            Assert.Same(expected, actual);
        }

        #endregion

        #region Sortable<T, TValue>(this IGridColumn<T, TValue> column, Boolean isSortable)

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Sortable_SetsIsEnabled(Boolean isEnabled)
        {
            Boolean? actual = column.Sortable(isEnabled).Sort.IsEnabled;
            Boolean? expected = isEnabled;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Sortable_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.Sortable(true);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Encoded<T, TValue>(this IGridColumn<T, TValue> column, Boolean isEncoded)

        [Fact]
        public void Encoded_SetsIsEncoded()
        {
            Assert.True(column.Encoded(true).IsEncoded);
        }

        [Fact]
        public void Encoded_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.Encoded(true);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Formatted<T, TValue>(this IGridColumn<T, TValue> column, String format)

        [Fact]
        public void Formatted_SetsFormat()
        {
            String actual = column.Formatted("Format").Format;
            String expected = "Format";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Formatted_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.Formatted("Format");

            Assert.Same(expected, actual);
        }

        #endregion

        #region AppendCss<T, TValue>(this IGridColumn<T, TValue> column, String css)

        [Theory]
        [InlineData("", "", "")]
        [InlineData("", " ", "")]
        [InlineData("", null, "")]
        [InlineData("", "test", "test")]
        [InlineData("", " test", "test")]
        [InlineData("", "test ", "test")]
        [InlineData("", " test ", "test")]

        [InlineData(" ", "", "")]
        [InlineData(" ", " ", "")]
        [InlineData(" ", null, "")]
        [InlineData(" ", "test", "test")]
        [InlineData(" ", " test", "test")]
        [InlineData(" ", "test ", "test")]
        [InlineData(" ", " test ", "test")]

        [InlineData("first", "", "first")]
        [InlineData("first", null, "first")]
        [InlineData("first", "test", "first test")]
        [InlineData("first", " test", "first test")]
        [InlineData("first", "test ", "first test")]
        [InlineData("first", " test ", "first test")]
        [InlineData("first ", " test ", "first  test")]
        [InlineData(" first ", " test ", "first  test")]
        public void AppendsCss_Classes(String current, String toAppend, String css)
        {
            column.CssClasses = current;

            String expected = css;
            String actual = column.AppendCss(toAppend).CssClasses;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AppendsCss_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.AppendCss("column-class");

            Assert.Same(expected, actual);
        }

        #endregion

        #region Titled<T, TValue>(this IGridColumn<T, TValue> column, Object title)

        [Theory]
        [InlineData(1)]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Title")]
        public void Titled_SetsTitle(Object title)
        {
            Object expected = title ?? "";
            Object actual = column.Titled(title).Title;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Titled_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.Titled("Title");

            Assert.Same(expected, actual);
        }

        #endregion

        #region Named<T, TValue>(this IGridColumn<T, TValue> column, String name)

        [Theory]
        [InlineData("", "")]
        [InlineData(" ", "")]
        [InlineData(null, "")]
        [InlineData("Name", "name")]
        [InlineData(" Name ", "name")]
        [InlineData("NameTest", "name-test")]
        [InlineData(" NameTest ", "name-test")]
        [InlineData(" Name_Test ", "name-test")]
        [InlineData("name-test-apply", "name-test-apply")]
        public void Named_SetsName(String rawName, String name)
        {
            String expected = name;
            String actual = column.Named(rawName).Name;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Named_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.Named("Name");

            Assert.Same(expected, actual);
        }

        #endregion

        #region Css<T, TValue>(this IGridColumn<T, TValue> column, String css)

        [Fact]
        public void Css_SetsCssClasses()
        {
            String actual = column.Css(" column-class ").CssClasses;
            String expected = "column-class";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Css_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.Css(null);

            Assert.Same(expected, actual);
        }

        #endregion

        #region Hidden<T, TValue>(this IGridColumn<T, TValue> column)

        [Fact]
        public void Hidden_SetsHidden()
        {
            column.IsHidden = false;

            Assert.True(column.Hidden().IsHidden);
        }

        [Fact]
        public void Hidden_ReturnsColumn()
        {
            Object expected = column;
            Object actual = column.Hidden();

            Assert.Same(expected, actual);
        }

        #endregion
    }
}
