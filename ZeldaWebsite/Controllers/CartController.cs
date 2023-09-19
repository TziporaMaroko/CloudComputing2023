using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using ZeldaWebsite.Data;
using ZeldaWebsite.Models;

namespace ZeldaWebsite.Controllers;

public class CartController : Controller
{
    public string ShoppingCartId { get; set; }

    public ZeldaWebsiteContext _db = new ZeldaWebsiteContext();

    public const string CartSessionKey = "CartId";

    public CartController(ZeldaWebsiteContext context)
    {
        _db = context;
    }

    // GET: Flavours1
    public async Task<IActionResult> Index()
    {
        var cartItems = GetCartItems();
        var flavours = new List<Flavour>();

        foreach (var item in cartItems)
        {
            var flavour = GetFlavourById(item.FlavourId);
            flavours.Add(flavour);
        }

        var model = new CartView
        {
            CartItems = cartItems,
            Flavours = flavours
        };

        return View(model);
    }
    public async Task<IActionResult> Checkout()
    {
        var cartItems = GetCartItems();
        var flavours = new List<Flavour>();

        foreach (var item in cartItems)
        {
            var flavour = GetFlavourById(item.FlavourId);
            flavours.Add(flavour);
        }

        var model = new CartView
        {
            CartItems = cartItems,
            Flavours = flavours
        };

        return View(model);
    }

    public IActionResult ThankYou()
    {
        return View();
    }

    public async Task AddToCart(int id, double size)
    {
        ShoppingCartId = GetCartId();

        var cartItem = await _db.ShoppingCartItems.SingleOrDefaultAsync(
            c => c.CartId == ShoppingCartId && c.FlavourId == id);

        if (cartItem == null)
        {
			// Create a new cart item if it doesn't exist.

			cartItem = new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                FlavourId = id,
                CartId = ShoppingCartId,
                Quantity = 1,
                DateCreated = DateTime.Now,
                Size= size,
                Price= size*GetFlavourById(id).Price
            };

            _db.ShoppingCartItems.Add(cartItem);
        }
        else
        {
            // If the item exists in the cart, increment the quantity.
            cartItem.Size+=size;
            cartItem.Price += size * GetFlavourById(id).Price;
            cartItem.Price.ToString("F3");
        }
        try { await _db.SaveChangesAsync(); }
        catch(Exception ex) { }
        
    }

    // Dispose of the database context properly.
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _db.Dispose();
        }
        base.Dispose(disposing);
    }
   

    public string GetCartId()
    {
        if (HttpContext.Session.Get(CartSessionKey) == null)
        {
            if (!string.IsNullOrWhiteSpace(User.Identity.Name))
            {
                var bytes1 = Encoding.UTF8.GetBytes(User.Identity.Name);
                HttpContext.Session.Set(CartSessionKey, bytes1);
            }
            else
            {
                // Generate a new random GUID using System.Guid class.
                Guid tempCartId = Guid.NewGuid();
                var bytes1 = Encoding.UTF8.GetBytes(tempCartId.ToString());
                HttpContext.Session.Set(CartSessionKey, bytes1);
            }
        }
        var bytes= HttpContext.Session.Get(CartSessionKey);
		return Encoding.UTF8.GetString(bytes);
	}

    public List<CartItem> GetCartItems()
    {
        ShoppingCartId = GetCartId();

        return _db.ShoppingCartItems.Where(
            c => c.CartId == ShoppingCartId).ToList();
    }

    public Flavour GetFlavourById(int id)
    {
        return _db.Flavour.SingleOrDefault(p => p.Id == id);
    }



}


