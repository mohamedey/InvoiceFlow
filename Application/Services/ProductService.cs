using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Domain.Entities;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;

        public ProductService(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.ToList();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            try
            {
                await _productRepository.AddAsync(product);
                await _productRepository.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product != null)
                {
                    await _productRepository.DeleteAsync(product);
                    await _productRepository.SaveAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}