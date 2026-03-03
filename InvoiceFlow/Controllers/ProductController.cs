using Application.Interfaces.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InvoiceFlow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            var result = await _productService.AddProductAsync(product);
            if (result) return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
            return BadRequest("Error creating product");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Product product)
        {
            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null) return NotFound();

            product.Id = id;
            var result = await _productService.UpdateProductAsync(product);
            if (result) return NoContent();
            return BadRequest("Error updating product");
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (result) return NoContent();
            return NotFound();
        }
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (product == null)
                return BadRequest("Product data is null");

            var result = await _productService.AddProductAsync(product);
            if (result)
                return Ok("Product added successfully");

            return BadRequest("Failed to add product");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, Product product)
        {
            if (id != product.Id)
                return BadRequest("Product ID mismatch");

            var result = await _productService.UpdateProductAsync(product);
            if (result)
                return Ok("Product updated successfully");

            return BadRequest("Failed to update product");
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}