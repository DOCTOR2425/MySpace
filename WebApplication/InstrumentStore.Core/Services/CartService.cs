﻿using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentStore.Domain.Services
{
    public class CartService : ICartService
    {
        private InstrumentStoreDBContext _dbContext;
        private IUsersService _usersService;
        private IProductService _productService;
        private IPaidOrderService _paidOrderService;

        public CartService(InstrumentStoreDBContext dbContext,
            IUsersService usersService,
            IProductService productService,
            IPaidOrderService paidOrderService)
        {
            _dbContext = dbContext;
            _usersService = usersService;
            _productService = productService;
            _paidOrderService = paidOrderService;
        }

        public async Task<List<CartItem>> GetAllCart(Guid userId)
        {
            return await _dbContext.CartItem
                .Include(c => c.User)
                .Include(c => c.Product)
                .Where(c => c.User.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PaidOrderItem>> GetAllOrders(Guid userId)
        {
            return await _dbContext.PaidOrderItem
                .Include(i => i.Product)
                .Include(i => i.PaidOrder)
                .Where(i => i.PaidOrder.User.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Guid> AddToCart(Guid userId, Guid productId, int quantity)
        {
            CartItem? targetCartItem = _dbContext.CartItem
                .Include(c => c.User)
                .Include(c => c.Product)
                .FirstOrDefault(c =>
                    c.User.UserId == userId &&
                    c.Product.ProductId == productId);

            if (targetCartItem != null)
                return await ChangeItemQuantity(targetCartItem.CartItemId, quantity);

            CartItem cartItem = new CartItem()
            {
                CartItemId = Guid.NewGuid(),
                User = await _usersService.GetById(userId),
                Product = await _productService.GetById(productId),
                Quantity = quantity
            };

            await _dbContext.CartItem.AddAsync(cartItem);
            await _dbContext.SaveChangesAsync();

            return cartItem.CartItemId;
        }

        public async Task<Guid> RemoveFromCart(Guid cartItemId)
        {
            await _dbContext.CartItem
                .Where(c => c.CartItemId == cartItemId)
                .ExecuteDeleteAsync();

            return cartItemId;
        }

        public async Task<Guid> OrderCart(Guid userId, Guid deliveryMethodId, Guid paymentMethodId)
        {
            Guid paidOrderId = await _paidOrderService.Create(userId, deliveryMethodId, paymentMethodId);

            foreach (CartItem i in await this.GetAllCart(userId))
            {
                await _dbContext.PaidOrderItem.AddAsync(new PaidOrderItem()
                {
                    PaidOrderItemId = Guid.NewGuid(),
                    PaidOrder = await _dbContext.PaidOrder.FindAsync(paidOrderId),
                    Product = await _productService.GetById(i.Product.ProductId),
                    Quantity = i.Quantity
                });

                await _dbContext.CartItem.Where(ci => ci == i).ExecuteDeleteAsync();
            }

            await _dbContext.SaveChangesAsync();

            return paidOrderId;
        }

        public async Task<Guid> OrderProduct(Guid userId,
            Guid productId,
            Guid deliveryMethodId,
            Guid paymentMethodId)
        {
            Guid paidOrderId = await _paidOrderService.Create(userId, deliveryMethodId, paymentMethodId);

            await _dbContext.PaidOrderItem.AddAsync(new PaidOrderItem()
            {
                PaidOrder = await _paidOrderService.GetById(paidOrderId),
                Product = await _productService.GetById(productId),
                Quantity = 1,
            });

            await _dbContext.SaveChangesAsync();

            return paidOrderId;
        }

        private async Task<Guid> ChangeItemQuantity(Guid cartItemId, int quantity)
        {
            (await _dbContext.CartItem.FirstOrDefaultAsync(c =>
                c.CartItemId == cartItemId))
                .Quantity = quantity;

            await _dbContext.SaveChangesAsync();

            return cartItemId;
        }
    }
}