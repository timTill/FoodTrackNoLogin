﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodTracker.Data;
using FoodTracker.Models;
using FoodTracker.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodTracker.Areas.User.Controllers
{
	[Area("User")]
	public class ShoppingController : Controller
    {
		private readonly ApplicationDbContext _db;
		public ShoppingController(ApplicationDbContext db)
		{
			_db = db;
		}

		[HttpGet]
		public async Task<IActionResult> Shopping()
		{
			IndexViewModel IndexVM = new IndexViewModel()
			{
				FoodItem = await _db.Foods.Where(m => m.QuantityLeft == 0).Include(m => m.Category).
				Include(m => m.SubCategory).ToListAsync(),
				Category = await _db.Category.ToListAsync(),
			};
			return View(IndexVM);
		}
		public IActionResult AddToStock(int id)
		{			
			Food selectedFood = _db.Foods.Where(f => f.ID == id).Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefault();
			return View(selectedFood);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("AddToStock")]
		public async Task<IActionResult> AddToStockPost(Food food)
		{
			Food selectedFood = _db.Foods.Where(f => f.ID == food.ID).Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefault();
			selectedFood.BestBefore = food.BestBefore;
			selectedFood.Description = food.Description;
			selectedFood.QuantityLeft = food.QuantityLeft;
			selectedFood.Unit = food.Unit;

			if (food.BestBefore != null && food.QuantityLeft != 0)
			{			
				await _db.SaveChangesAsync();
				return RedirectToAction("Index","Home");
			}
			return View(selectedFood);
		}
	}
}