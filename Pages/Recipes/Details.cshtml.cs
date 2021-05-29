﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalKulinarny.Areas.Identity.Data;
using PortalKulinarny.Data;
using PortalKulinarny.Models;
using PortalKulinarny.Services;

namespace PortalKulinarny.Pages.Recipes
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseRecipesService _recipesService;
        private readonly VoteService _voteService;
        private readonly FavouritiesService _favouritiesService;
        public readonly UtilsService _utilsService;

        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            DatabaseRecipesService recipesService, VoteService voteService, FavouritiesService favouritiesService, UtilsService utilsService)
        {
            _context = context;
            _userManager = userManager;
            _recipesService = recipesService;
            _voteService = voteService;
            _favouritiesService = favouritiesService;
            _utilsService = utilsService;
        }
        public Recipe Recipe { get; set; }
        public string UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await LoadAsync(id);

            if (Recipe == null)
            {
                return NotFound();
            }

            return Page();
        }
        

        public async Task<IActionResult> OnPostUpVoteAsync(int? id)
        {
            var recipeVoted = await _recipesService.FindByIdAsync(id);

            var userVoting = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _voteService.UpVote(recipeVoted, userVoting);
            }
            catch(Exception)
            {
                //todo better exception handling??
                return RedirectToPage("/Error");
            }

            await LoadAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostDownVoteAsync(int? id, string value)
        {
            var recipeVoted = await _recipesService.FindByIdAsync(id);

            var userVoting = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _voteService.DownVote(recipeVoted, userVoting);
            }
            catch (Exception)
            {
                //todo better exception handling??
                return RedirectToPage("/Error");
            }

            await LoadAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostFavouritiesAsync(int id)
        {
            var recipeAdded = await _recipesService.FindByIdAsync(id);

            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userAdding = await _context.Users.Include(u => u.Favourites).FirstOrDefaultAsync(u => u.Id == userID);

            try
            {
                await _favouritiesService.AddRemoveFav(recipeAdded, userAdding);
            }
            catch (Exception e)
            {
                //todo better exception handling??
                return RedirectToPage("/Error");
            }

            await LoadAsync(id);
            return Page();

        }

        public async Task<string> GetUserName(string userId)
        {
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);
            return applicationUser?.UserName;
        }

        public async Task LoadAsync(int? id)
        {
            Recipe = await _recipesService.FindByIdAsync(id);
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
