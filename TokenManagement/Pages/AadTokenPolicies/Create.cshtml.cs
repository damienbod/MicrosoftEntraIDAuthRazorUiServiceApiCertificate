﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using TokenManagement.AadTokenPolicies;

namespace TokenManagement.Pages.AadTokenPolicies;

[AuthorizeForScopes(Scopes = ["Policy.Read.All", "Policy.ReadWrite.ApplicationConfiguration"])]
public class CreateModel : PageModel
{
    private readonly TokenLifetimePolicyGraphApiService _tokenLifetimePolicyGraphApiService;

    public CreateModel(TokenLifetimePolicyGraphApiService tokenLifetimePolicyGraphApiService)
    {
        _tokenLifetimePolicyGraphApiService = tokenLifetimePolicyGraphApiService;
    }


    [BindProperty]
    public TokenLifetimePolicyDto TokenLifetimePolicyDto { get; set; } = new();

    public IActionResult OnGet()
    {
        TokenLifetimePolicyDto = new TokenLifetimePolicyDto
        {
            Definition = "{\"TokenLifetimePolicy\":{\"Version\":1,\"AccessTokenLifetime\":\"00:30:00\"}}"
        };

        return Page();
    }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var tokenLifetimePolicy = new TokenLifetimePolicy
        {
            Definition = new List<string>()
            {
                TokenLifetimePolicyDto.Definition
            },
            DisplayName = TokenLifetimePolicyDto.DisplayName,
            IsOrganizationDefault = TokenLifetimePolicyDto.IsOrganizationDefault
        };

        await _tokenLifetimePolicyGraphApiService.CreatePolicy(tokenLifetimePolicy);

        return RedirectToPage("./Index");
    }
}