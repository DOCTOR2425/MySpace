﻿using InstrumentStore.Domain.DataBase.Models;
using InstrumentStore.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using InstrumentStore.Domain.Contracts.Products;
using AutoMapper;
using InstrumentStore.Domain.Services;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductCard>>> GetAllProductsCards()
        {
            Console.WriteLine(HttpContext.Request.Cookies[JwtProvider.AccessCookiesName]);
            Console.WriteLine(JwtProvider.AccessCookiesName);
            List<Product> products = await _productService.GetAll();
            List<ProductCard> productsCards = new List<ProductCard>();

            foreach (var p in products)
            {
                p.Image = "https://localhost:7295/images/" + p.Image;
                productsCards.Add(_mapper.Map<ProductCard>(p));
            }

            return Ok(productsCards);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> GetProduct([FromRoute] Guid id)
        {
            var product = await _productService.GetById(id);
            product.Image = "https://localhost:7295/images/" + product.Image;
            var response = _mapper.Map<ProductResponse>(product);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] ProductRequest productRequest)
        {
            return Ok(await _productService.Create(productRequest));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateProduct(Guid id, [FromBody] ProductRequest productRequest)
        {
            return Ok(await _productService.Update(id, productRequest));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteProduct(Guid id)
        {
            return Ok(await _productService.Delete(id));
        }
    }
}
/*
{
  "name": "Test Product Name",
  "description": "Test Product description long",
  "price": 240,
  "quantity": 1230,
  "image": "hammer.jpg",
  "productTypeId": "dd6995d4-30fe-4637-a34d-1f7d8ef2f53b",
  "brandId": "f0c2afe2-70d3-4a4f-b9c9-490a8020730f",
  "countryId": "7a0e0584-7c19-403f-bf9f-20753e9cd175"
}
*/