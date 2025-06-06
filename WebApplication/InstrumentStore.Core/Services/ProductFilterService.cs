﻿using System.Globalization;
using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.Contracts.Filters;
using InstrumentStore.Domain.DataBase.Models;

namespace InstrumentStore.Domain.Services
{
    public class ProductFilterService : IProductFilterService
    {
        private readonly IProductPropertyService _productPropertyService;

        public ProductFilterService(IProductPropertyService productPropertyService)
        {
            _productPropertyService = productPropertyService;
        }

        public async Task<List<Product>> GetAllWithFilters(
            Guid categoryId,
            FilterRequest filter,
            List<Product> productsForFilter)
        {
            List<Product> products = await FilterByStaticProperties(filter, productsForFilter);
            products = await FilterByUnStaticProperties(categoryId, filter, products);

            return products;
        }

        private async Task<List<Product>> FilterByUnStaticProperties(
            Guid categoryId,
            FilterRequest filter,
            List<Product> productsForFilter)
        {
            FilterRequest unStaticFilter = DeleteStaticFilters(filter);

            productsForFilter = await FilterByUnStaticRanges(
                categoryId,
                unStaticFilter.RangeFilters,
                productsForFilter);

            productsForFilter = await FilterByUnStaticCollection(
                categoryId,
                unStaticFilter.CollectionFilters,
                productsForFilter);

            return productsForFilter;
        }

        private FilterRequest DeleteStaticFilters(FilterRequest filter)
        {
            RangeFilter[] rangeFilters = (from rf in filter.RangeFilters
                                          where rf.Property.ToLower() != "цена"
                                          select rf).ToArray();

            CollectionFilter[] collectionFilters = (from cf in filter.CollectionFilters
                                                    where cf.Property.ToLower() != "бренд" &&
                                                        cf.Property.ToLower() != "страна"
                                                    select cf).ToArray();

            return new FilterRequest(rangeFilters, collectionFilters);
        }

        private async Task<List<Product>> FilterByUnStaticCollection(
            Guid categoryId,
            CollectionFilter[] collectionFilters,
            List<Product> productsForFilter)
        {
            if (collectionFilters.Length == 0)
                return productsForFilter;

            List<ProductPropertyValue> values =
                await _productPropertyService.GetValuesByCategoryId(categoryId);
            List<Product> filteredProducts = new List<Product>();

            var filtersGroupedByProperty = collectionFilters
                .GroupBy(f => f.Property)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var product in productsForFilter)
            {
                bool matchesAllFilters = true;

                foreach (var property in filtersGroupedByProperty.Keys)
                {
                    var filtersForProperty = filtersGroupedByProperty[property];
                    var allValuesForProperty = values
                        .Where(v => v.ProductProperty.Name == property)
                        .Select(v => v.Value)
                        .Distinct()
                        .ToList();

                    bool allFiltersSelected = filtersForProperty
                        .Select(f => f.PropertyValue)
                        .OrderBy(v => v)
                        .SequenceEqual(allValuesForProperty.OrderBy(v => v));

                    if (allFiltersSelected)
                        continue;

                    bool matchesCurrentProperty = filtersForProperty.Any(filter =>
                        values.Any(v =>
                            v.Product.ProductId == product.ProductId &&
                            v.ProductProperty.Name == filter.Property &&
                            v.Value == filter.PropertyValue));

                    if (!matchesCurrentProperty)
                    {
                        matchesAllFilters = false;
                        break;
                    }
                }

                if (matchesAllFilters)
                    filteredProducts.Add(product);
            }

            return filteredProducts.DistinctBy(p => p.ProductId).ToList();
        }

        private async Task<List<Product>> FilterByUnStaticRanges(
            Guid categoryId,
            RangeFilter[] rangeFilters,
            List<Product> productsForFilter)
        {
            if (rangeFilters.Length == 0)
                return productsForFilter;

            List<ProductPropertyValue> values =
                await _productPropertyService.GetValuesByCategoryId(categoryId);
            List<Product> products = new List<Product>();

            NumberFormatInfo formatInfo = new NumberFormatInfo()
            {
                NumberDecimalSeparator = "."
            };

            foreach (var f in rangeFilters)
            {
                foreach (var v in values)
                {
                    if (v.ProductProperty.Name == f.Property &&
                        decimal.Parse(v.Value, formatInfo) >= f.MinValue &&
                        decimal.Parse(v.Value, formatInfo) <= f.MaxValue &&
                        productsForFilter.FirstOrDefault(
                            p => p.ProductId == v.Product.ProductId) != null)
                    {
                        products.Add(v.Product);
                    }
                }
            }
            return products.DistinctBy(p => p.ProductId).ToList();
        }

        private async Task<List<Product>> FilterByStaticProperties(FilterRequest filter, List<Product> productsForFilter)
        {
            List<Product> products = await FilterByPrice(filter, productsForFilter);
            products = await FilterByBrand(filter, products);
            products = await FilterByCountry(filter, products);

            return products;
        }

        private async Task<List<Product>> FilterByPrice(FilterRequest filter, List<Product> productsForFilter)
        {
            RangeFilter? priceRange = filter.RangeFilters
                .FirstOrDefault(f => f.Property.ToLower() == "цена");

            if (priceRange == null)
                return productsForFilter;

            List<Product> products = new List<Product>();
            foreach (var product in productsForFilter)
                if (product.Price >= priceRange.MinValue &&
                        product.Price <= priceRange.MaxValue)
                    products.Add(product);

            return products;
        }

        private async Task<List<Product>> FilterByBrand(FilterRequest filter, List<Product> productsForFilter)
        {
            List<CollectionFilter> collectionFilters = filter.CollectionFilters
                .Where(f => f.Property.ToLower() == "бренд")
                .ToList();

            if (collectionFilters.Count == 0)
                return productsForFilter;

            List<Product> productsAfterFilter = new List<Product>();

            foreach (var f in collectionFilters)
                foreach (var p in productsForFilter)
                    if (p.Brand.Name.ToLower() == f.PropertyValue.ToLower())
                        productsAfterFilter.Add(p);

            return productsAfterFilter;
        }

        private async Task<List<Product>> FilterByCountry(FilterRequest filter, List<Product> productsForFilter)
        {
            List<CollectionFilter> collectionFilters = filter.CollectionFilters
                .Where(f => f.Property.ToLower() == "country")
                .ToList();

            if (collectionFilters.Count == 0)
                return productsForFilter;

            List<Product> productsAfterFilter = new List<Product>();

            foreach (var f in collectionFilters)
                foreach (var p in productsForFilter)
                    if (p.Country.Name.ToLower() == f.PropertyValue.ToLower())
                        productsAfterFilter.Add(p);

            return productsAfterFilter;
        }

    }
}
