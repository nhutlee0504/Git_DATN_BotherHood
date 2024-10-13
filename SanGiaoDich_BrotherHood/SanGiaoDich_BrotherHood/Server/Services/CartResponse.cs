﻿
using Microsoft.EntityFrameworkCore;
using SanGiaoDich_BrotherHood.Server.Data;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class CartResponse : ICart
    {
        private readonly ApplicationDbContext _context;
        public CartResponse(ApplicationDbContext context) => _context = context;
        public async Task<Cart> AddCart(Cart cart)
        {
            try
            {
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
                return cart;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<Cart> DeleteCart(int IDCart)
        {
            try
            {
                var cart = await _context.Carts.FindAsync(IDCart);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                return cart;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<IEnumerable<Cart>> GetCartsByUserName(string userName)
        {
            return await _context.Carts.Where(x => x.UserName == userName).ToListAsync();
        }

        public async Task<Cart> UpdateCart(int IDCart, Cart cart)
        {
            try
            {
                var cartUpdate = await _context.Carts.FindAsync(IDCart);
                if (cartUpdate == null)
                    return null;
                cartUpdate.UserName = cart.UserName;
                cartUpdate.IDProduct = cart.IDProduct;
                cartUpdate.Quantity = cart.Quantity;
                _context.Carts.Update(cartUpdate);
                await _context.SaveChangesAsync();
                return cart;
            }
            catch (System.Exception)
            {

                return null;
            }
        }
    }
}
