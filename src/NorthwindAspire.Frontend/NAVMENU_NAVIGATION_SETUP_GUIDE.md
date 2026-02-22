# NavMenu.razor Navigation Setup Guide

## Overview

This guide explains how to add navigation links to your `NavMenu.razor` file to provide access to all your CRUD pages in the Blazor frontend application.

## Current NavMenu Structure

Your `NavMenu.razor` currently contains:
```razor
<MudNavMenu>
    <MudNavLink Href="" Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Home">Home</MudNavLink>
    <MudNavLink Href="counter" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Add">Counter</MudNavLink>
    <MudNavLink Href="weather" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.List">Weather</MudNavLink>
</MudNavMenu>
```

## Adding CRUD Navigation Links

### Step 1: Update NavMenu.razor with CRUD Links

Add navigation links for your CRUD pages inside the `<MudNavMenu>` component:

**File:** `src/NorthwindAspire.Frontend/Components/Layout/NavMenu.razor`

```razor
<MudNavMenu>
    <MudNavLink Href="" Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Home">Home</MudNavLink>
    
    <!-- Original Pages -->
    <MudNavLink Href="counter" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Add">Counter</MudNavLink>
    <MudNavLink Href="weather" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.List">Weather</MudNavLink>
    
    <!-- Separator for Organization -->
    <MudDivider Class="my-2" />
    <MudText Typo="Typo.body2" Class="px-4 py-2 text-disabled">Data Management</MudText>
    
    <!-- CRUD Navigation Links -->
    <MudNavLink Href="categories" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Category">
        Categories
    </MudNavLink>
    
    <MudNavLink Href="products" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Shopping">
        Products
    </MudNavLink>
    
    <MudNavLink Href="customers" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.People">
        Customers
    </MudNavLink>
</MudNavMenu>
```

## MudNavLink Component Reference

### Basic Syntax
```razor
<MudNavLink Href="[route]" Match="NavLinkMatch.[Type]" Icon="@Icons.Material.Filled.[Icon]">
    [Display Text]
</MudNavLink>
```

### Attributes

| Attribute | Purpose | Example |
|-----------|---------|---------|
| `Href` | Route path | `"categories"` |
| `Match` | When to highlight the link | `NavLinkMatch.Prefix` or `NavLinkMatch.All` |
| `Icon` | Icon to display | `@Icons.Material.Filled.Category` |
| Text Content | Display label | `Categories` |

### Match Types

- **`NavLinkMatch.All`** - Only match if the current URL exactly matches
- **`NavLinkMatch.Prefix`** - Match if the URL starts with the specified href (recommended for CRUD pages)

## Available Material Icons for CRUD

Use these Material Design icons for your data management sections:

```csharp
// Entities
@Icons.Material.Filled.Category          // Categories
@Icons.Material.Filled.ShoppingCart      // Products
@Icons.Material.Filled.People            // Customers
@Icons.Material.Filled.Person            // Employees
@Icons.Material.Filled.Building          // Suppliers/Companies
@Icons.Material.Filled.Receipt           // Orders
@Icons.Material.Filled.LocalShipping     // Shippers
@Icons.Material.Filled.Map               // Regions/Territories

// Actions/States
@Icons.Material.Filled.Home              // Home
@Icons.Material.Filled.Settings          // Settings
@Icons.Material.Filled.Info              // Information
@Icons.Material.Filled.Help              // Help
@Icons.Material.Filled.Search            // Search
@Icons.Material.Filled.Edit              // Edit
@Icons.Material.Filled.Delete            // Delete
@Icons.Material.Filled.Add               // Add/Create
```

## Complete Example with All CRUD Entities

Here's an extended example with all typical Northwind entities:

```razor
<MudNavMenu>
    <!-- Home and Utilities -->
    <MudNavLink Href="" Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Home">
        Home
    </MudNavLink>
    
    <MudNavLink Href="counter" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Add">
        Counter
    </MudNavLink>
    
    <MudNavLink Href="weather" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.List">
        Weather
    </MudNavLink>
    
    <!-- Data Management Section -->
    <MudDivider Class="my-2" />
    <MudText Typo="Typo.body2" Class="px-4 py-2 text-disabled">Data Management</MudText>
    
    <!-- Core Data -->
    <MudNavLink Href="categories" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Category">
        Categories
    </MudNavLink>
    
    <MudNavLink Href="products" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.ShoppingCart">
        Products
    </MudNavLink>
    
    <MudNavLink Href="customers" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.People">
        Customers
    </MudNavLink>
    
    <!-- Business Operations -->
    <MudDivider Class="my-2" />
    <MudText Typo="Typo.body2" Class="px-4 py-2 text-disabled">Operations</MudText>
    
    <MudNavLink Href="employees" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Person">
        Employees
    </MudNavLink>
    
    <MudNavLink Href="orders" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Receipt">
        Orders
    </MudNavLink>
    
    <!-- Suppliers and Logistics -->
    <MudDivider Class="my-2" />
    <MudText Typo="Typo.body2" Class="px-4 py-2 text-disabled">Suppliers & Logistics</MudText>
    
    <MudNavLink Href="suppliers" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Building">
        Suppliers
    </MudNavLink>
    
    <MudNavLink Href="shippers" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.LocalShipping">
        Shippers
    </MudNavLink>
    
    <!-- Geographic Data -->
    <MudDivider Class="my-2" />
    <MudText Typo="Typo.body2" Class="px-4 py-2 text-disabled">Reference Data</MudText>
    
    <MudNavLink Href="regions" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Map">
        Regions
    </MudNavLink>
</MudNavMenu>
```

## Navigation Structure Best Practices

### 1. Organize by Domain
Group related entities together logically:
- **Core Data** - Categories, Products, Customers
- **Operations** - Employees, Orders, Order Details
- **Suppliers & Logistics** - Suppliers, Shippers
- **Reference Data** - Regions, Territories

### 2. Use Dividers and Labels
```razor
<!-- Separator -->
<MudDivider Class="my-2" />

<!-- Section Label -->
<MudText Typo="Typo.body2" Class="px-4 py-2 text-disabled">
    Section Name
</MudText>
```

### 3. Icon Selection
- Use descriptive, related icons
- Keep style consistent (all Material Design)
- Consider user familiarity with icons

### 4. Link Ordering
- Home link first
- Utility pages (Counter, Weather) near top
- Main CRUD operations grouped by domain
- Less frequently used items at bottom

## Styling and Customization

### Adding CSS Classes

```razor
<MudNavLink Href="categories" 
            Match="NavLinkMatch.Prefix" 
            Icon="@Icons.Material.Filled.Category"
            Class="custom-nav-link">
    Categories
</MudNavLink>
```

### Custom Styling Example

Add to your `app.css`:

```css
/* Custom navigation link styling */
.mud-nav-link {
    margin-bottom: 4px;
}

.mud-nav-link.active {
    background-color: rgba(63, 81, 181, 0.1);
    font-weight: 600;
}

.text-disabled {
    opacity: 0.6;
    font-size: 0.85rem;
}
```

## Conditional Navigation

You can conditionally show/hide links based on user roles or permissions:

```razor
@if (IsAdmin)
{
    <MudNavLink Href="settings" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Settings">
        Settings
    </MudNavLink>
}

@code {
    private bool IsAdmin { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        // Get user role from authentication service
        IsAdmin = await AuthService.IsUserAdminAsync();
    }
}
```

## Dynamic Navigation from Code

For more complex scenarios, you can generate navigation links dynamically:

```razor
@foreach (var entity in NavigationItems)
{
    <MudNavLink Href="@entity.Route" 
                Match="NavLinkMatch.Prefix" 
                Icon="@entity.Icon">
        @entity.Label
    </MudNavLink>
}

@code {
    private List<NavItem> NavigationItems = new()
    {
        new NavItem { Route = "categories", Icon = Icons.Material.Filled.Category, Label = "Categories" },
        new NavItem { Route = "products", Icon = Icons.Material.Filled.Shopping, Label = "Products" },
        new NavItem { Route = "customers", Icon = Icons.Material.Filled.People, Label = "Customers" },
    };
    
    private class NavItem
    {
        public string Route { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }
}
```

## Responsive Navigation

MudBlazor's NavMenu is responsive by default. For additional customization:

```razor
<MudNavMenu Dense="@Dense">
    <!-- Navigation items -->
</MudNavMenu>

@code {
    private bool Dense { get; set; }
    
    protected override void OnInitialized()
    {
        // Set Dense=true for mobile, false for desktop
        Dense = !string.IsNullOrEmpty(HttpContext?.Request.Headers["User-Agent"]);
    }
}
```

## Complete Step-by-Step Instructions

### 1. Open NavMenu.razor
Navigate to `src/NorthwindAspire.Frontend/Components/Layout/NavMenu.razor`

### 2. Add Data Management Section
Insert after the existing navigation links:
```razor
<!-- Separator for Organization -->
<MudDivider Class="my-2" />
<MudText Typo="Typo.body2" Class="px-4 py-2 text-disabled">Data Management</MudText>
```

### 3. Add CRUD Links
Add links for each entity:
```razor
<MudNavLink Href="categories" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Category">
    Categories
</MudNavLink>

<MudNavLink Href="products" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Shopping">
    Products
</MudNavLink>

<MudNavLink Href="customers" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.People">
    Customers
</MudNavLink>
```

### 4. Save and Test
1. Save the file
2. Run the application
3. Verify navigation links appear in the sidebar
4. Click links to verify they navigate to correct pages

## Troubleshooting

### Links Not Appearing
- **Issue**: Navigation items not visible
- **Solution**: Check MudNavMenu is rendering correctly, ensure component is in layout

### Wrong Active State
- **Issue**: Links not highlighting when active
- **Solution**: Verify `Match="NavLinkMatch.Prefix"` and route paths match exactly

### Icons Not Showing
- **Issue**: Empty icon placeholder appears
- **Solution**: Ensure icon name is correct (e.g., `Icons.Material.Filled.Category`)

### Navigation Doesn't Work
- **Issue**: Clicking links doesn't navigate
- **Solution**: Verify route pages exist at correct paths (e.g., `/pages/Categories.razor`)

## Testing Navigation

### Manual Testing Checklist
- [ ] All navigation links appear in sidebar
- [ ] Clicking a link navigates to correct page
- [ ] Active link is highlighted with current route
- [ ] Icons display correctly
- [ ] Section dividers and labels are visible
- [ ] Navigation works on mobile (responsive)
- [ ] No console errors

### Automated Testing
You can add unit tests for navigation:

```csharp
[Test]
public void NavMenu_ContainsCategoriesLink()
{
    // Arrange
    var cut = RenderComponent<NavMenu>();
    
    // Act
    var categoryLink = cut.Find("a[href='categories']");
    
    // Assert
    Assert.IsNotNull(categoryLink);
    Assert.That(categoryLink.TextContent, Contains.Substring("Categories"));
}
```

## Performance Considerations

### Optimization Tips
1. **Lazy Load Icons** - MudBlazor loads icons on demand
2. **Minimize DOM Elements** - Each link adds minimal overhead
3. **Use KeyedCollection** - For large navigation hierarchies
4. **Memoization** - Cache navigation items to prevent recalculation

## Accessibility

### Best Practices
- Navigation links have semantic HTML (`<a>` tags)
- Icons have appropriate ARIA labels
- Keyboard navigation works (Tab to focus, Enter to activate)
- Color contrast meets WCAG standards
- Active links have clear visual indicator

### Adding ARIA Labels
```razor
<MudNavLink Href="categories" 
            Match="NavLinkMatch.Prefix" 
            Icon="@Icons.Material.Filled.Category"
            aria-label="Navigate to Categories">
    Categories
</MudNavLink>
```

## Summary

### What You've Learned
? How to add navigation links to NavMenu.razor
? Available MudNavLink attributes and options
? Best practices for organizing navigation
? Icons and styling options
? How to handle dynamic and conditional navigation
? Testing and troubleshooting approaches

### Next Steps
1. Update NavMenu.razor with CRUD links
2. Test navigation to verify it works
3. Customize styling if desired
4. Add links for additional entities as you create them

### Quick Reference

**Basic Navigation Link:**
```razor
<MudNavLink Href="categories" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Category">
    Categories
</MudNavLink>
```

**With Section:**
```razor
<MudDivider Class="my-2" />
<MudText Typo="Typo.body2" Class="px-4 py-2 text-disabled">Section Name</MudText>
<MudNavLink Href="route" Match="NavLinkMatch.Prefix" Icon="@Icons.Material.Filled.Icon">
    Label
</MudNavLink>
```

---

**Reference**: MudBlazor v6+, .NET 10, Blazor Interactive Server
**Last Updated**: 2024
