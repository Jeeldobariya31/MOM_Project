# Modern UI Framework Guide

## Overview

This guide explains how to use the Modern UI Framework implemented in the MOM (Meeting of Minutes) system. The framework provides consistent styling, custom animated popups, and interactive components across all pages.

## Files Structure

```
MOM/wwwroot/assets/
├── css/
│   ├── modern-ui.css      # Main modern UI styles
│   ├── style.css          # Base template styles
│   └── custome.css        # Custom overrides
├── js/
│   ├── modern-ui.js       # Modern UI JavaScript framework
│   └── main.js            # Base template scripts
└── docs/
    └── modern-ui-guide.md # This guide
```

## CSS Framework Features

### 1. Color System
The framework uses CSS custom properties for consistent theming:

```css
:root {
    --primary-color: #4154f1;
    --primary-light: #6776f4;
    --success-color: #198754;
    --danger-color: #dc3545;
    --warning-color: #ffc107;
    --info-color: #0dcaf0;
}
```

### 2. Component Classes

#### Cards
```html
<div class="card shadow-hover">
    <div class="card-header">
        <h5 class="mb-0 text-white">Card Title</h5>
    </div>
    <div class="card-body">
        Card content
    </div>
</div>
```

#### Tables
```html
<div class="table-container">
    <div class="table-responsive">
        <table class="table table-hover align-middle">
            <!-- Table content -->
        </table>
    </div>
</div>
```

#### Buttons
```html
<button class="btn btn-primary">Primary Button</button>
<button class="btn btn-outline-secondary">Secondary Button</button>
```

#### Forms
```html
<div class="filter-section">
    <form class="row g-3">
        <div class="col-md-6">
            <label class="form-label">Field Label</label>
            <input type="text" class="form-control" placeholder="Enter value">
        </div>
    </form>
</div>
```

#### Avatars
```html
<div class="avatar bg-primary">
    <i class="bi bi-person"></i>
</div>
<div class="avatar avatar-sm bg-success">JS</div>
<div class="avatar avatar-lg bg-info">
    <i class="bi bi-building"></i>
</div>
```

#### Empty States
```html
<div class="empty-state">
    <i class="bi bi-inbox"></i>
    <h3>No items found</h3>
    <p>Get started by adding your first item</p>
    <button class="btn btn-primary">Add Item</button>
</div>
```

## JavaScript Framework (ModernUI)

### 1. Custom Modals

#### Basic Modal
```javascript
modernUI.showModal({
    title: 'Modal Title',
    content: '<p>Modal content here</p>',
    buttons: [
        {
            text: 'Close',
            class: 'btn-secondary',
            onclick: 'modernUI.closeModal()'
        }
    ]
});
```

#### Confirmation Dialog
```javascript
modernUI.showConfirmation({
    title: 'Confirm Action',
    message: 'Are you sure you want to proceed?',
    confirmText: 'Yes, Continue',
    confirmClass: 'btn-danger',
    onConfirm: () => {
        // Action to perform on confirmation
        console.log('Confirmed!');
    }
});
```

### 2. Toast Notifications

```javascript
// Success toast
modernUI.showToast({
    title: 'Success',
    message: 'Operation completed successfully',
    type: 'success',
    duration: 5000
});

// Error toast
modernUI.showToast({
    title: 'Error',
    message: 'Something went wrong',
    type: 'error'
});

// Info toast
modernUI.showToast({
    title: 'Information',
    message: 'Please note this important information',
    type: 'info'
});

// Warning toast
modernUI.showToast({
    title: 'Warning',
    message: 'Please check your input',
    type: 'warning'
});
```

### 3. Loading States

```javascript
// Show loading on button
modernUI.showLoading('#submitBtn', 'Saving...');

// Hide loading
modernUI.hideLoading('#submitBtn');

// Show loading on any element
const button = document.querySelector('#myButton');
modernUI.showLoading(button, 'Processing...');
```

### 4. Form Validation

```javascript
// Validate entire form
const isValid = modernUI.validateForm('#myForm');

// Show field error
modernUI.showFieldError(inputElement, 'This field is required');

// Clear field error
modernUI.clearFieldError(inputElement);
```

### 5. AJAX Helper

```javascript
modernUI.makeRequest('/api/data', {
    method: 'POST',
    data: { name: 'John', email: 'john@example.com' },
    loadingElement: '#submitBtn',
    successMessage: 'Data saved successfully',
    errorMessage: 'Failed to save data'
})
.then(result => {
    console.log('Success:', result);
})
.catch(error => {
    console.error('Error:', error);
});
```

### 6. Table Utilities

```javascript
// Export table to CSV
modernUI.exportTable('#myTable', 'data-export', 'csv');

// Export table to JSON
modernUI.exportTable('#myTable', 'data-export', 'json');

// Setup live search
modernUI.setupLiveSearch('#searchInput', '#myTable');
```

## Page Templates

### 1. List Page Template

```html
<!-- Page Header -->
<div class="page-header">
    <div class="d-flex justify-content-between align-items-center">
        <div>
            <h1><i class="bi bi-icon me-2"></i>Page Title</h1>
            <nav>
                <ol class="breadcrumb">
                    <li class="breadcrumb-item"><a href="/">Home</a></li>
                    <li class="breadcrumb-item active">Current Page</li>
                </ol>
            </nav>
        </div>
        <a href="/add" class="btn btn-light">
            <i class="bi bi-plus-lg me-1"></i>Add New
        </a>
    </div>
</div>

<!-- Main Content -->
<section class="section">
    <div class="row">
        <div class="col-lg-12">
            <div class="card shadow-hover">
                <div class="card-body">
                    <!-- Header with Stats -->
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <div>
                            <h5 class="card-title mb-1">
                                <i class="bi bi-icon text-primary me-2"></i>
                                Management Title
                            </h5>
                            <p class="text-muted small mb-0">
                                <i class="bi bi-info-circle me-1"></i>
                                Showing X of Y records
                            </p>
                        </div>
                        <div class="d-flex gap-2">
                            <button class="btn btn-outline-secondary btn-sm" onclick="modernUI.exportTable('#table', 'export', 'csv')">
                                <i class="bi bi-file-earmark-excel me-1"></i>Export
                            </button>
                            <a href="/add" class="btn btn-primary">
                                <i class="bi bi-plus-lg me-1"></i>Add New
                            </a>
                        </div>
                    </div>

                    <!-- Search and Filter Section -->
                    <div class="filter-section">
                        <form method="get" class="row g-3">
                            <div class="col-md-6">
                                <label class="form-label">Search</label>
                                <div class="input-group">
                                    <span class="input-group-text">
                                        <i class="bi bi-search"></i>
                                    </span>
                                    <input type="text" name="search" class="form-control" placeholder="Search...">
                                    <button type="submit" class="btn btn-primary">
                                        <i class="bi bi-search me-1"></i>Search
                                    </button>
                                </div>
                            </div>
                        </form>
                    </div>

                    <!-- Data Table -->
                    <div class="table-container">
                        <div class="table-responsive">
                            <table class="table table-hover align-middle" id="dataTable">
                                <thead>
                                    <tr>
                                        <th>Column 1</th>
                                        <th>Column 2</th>
                                        <th class="text-center">Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <!-- Table rows -->
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>
```

### 2. Form Page Template

```html
<!-- Page Header -->
<div class="page-header">
    <div class="d-flex justify-content-between align-items-center">
        <div>
            <h1><i class="bi bi-icon me-2"></i>Form Title</h1>
            <nav>
                <ol class="breadcrumb">
                    <li class="breadcrumb-item"><a href="/">Home</a></li>
                    <li class="breadcrumb-item"><a href="/list">List</a></li>
                    <li class="breadcrumb-item active">Form</li>
                </ol>
            </nav>
        </div>
        <a href="/list" class="btn btn-light">
            <i class="bi bi-arrow-left me-1"></i>Back to List
        </a>
    </div>
</div>

<!-- Form Content -->
<section class="section">
    <div class="row justify-content-center">
        <div class="col-lg-8">
            <div class="card shadow-hover">
                <div class="card-header">
                    <div class="d-flex align-items-center">
                        <div class="avatar bg-primary me-3">
                            <i class="bi bi-icon"></i>
                        </div>
                        <div>
                            <h5 class="mb-0 text-white">Form Title</h5>
                            <p class="mb-0 text-white-50 small">Form description</p>
                        </div>
                    </div>
                </div>
                <div class="card-body">
                    <form id="myForm">
                        <!-- Form fields -->
                        <div class="mb-3">
                            <label class="form-label">Field Label <span class="text-danger">*</span></label>
                            <input type="text" class="form-control" required>
                        </div>
                        
                        <!-- Form actions -->
                        <div class="d-flex justify-content-between align-items-center mt-4">
                            <button type="button" class="btn btn-outline-secondary" onclick="resetForm()">
                                <i class="bi bi-arrow-clockwise me-1"></i>Reset
                            </button>
                            <button type="submit" class="btn btn-primary" id="submitBtn">
                                <i class="bi bi-check-lg me-1"></i>Save
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</section>
```

## Animation Classes

### Fade In
```html
<div class="fade-in">Content with fade in animation</div>
```

### Slide Up
```html
<div class="slide-up">Content with slide up animation</div>
```

### Bounce In
```html
<div class="bounce-in">Content with bounce in animation</div>
```

## Utility Classes

### Text Gradient
```html
<h1 class="text-gradient">Gradient Text</h1>
```

### Shadow Hover
```html
<div class="card shadow-hover">Card with hover shadow effect</div>
```

### Border Gradient
```html
<div class="border-gradient">Element with gradient border</div>
```

## Best Practices

### 1. Consistent Styling
- Always use the provided CSS classes instead of inline styles
- Follow the established color scheme using CSS custom properties
- Use consistent spacing with the predefined spacing variables

### 2. Accessibility
- Always include proper ARIA labels and roles
- Ensure keyboard navigation works properly
- Use semantic HTML elements
- Provide alternative text for icons when they convey meaning

### 3. Performance
- Use the loading states for long-running operations
- Implement proper error handling with toast notifications
- Use the live search feature for better user experience
- Optimize table rendering for large datasets

### 4. Mobile Responsiveness
- All components are mobile-responsive by default
- Test on different screen sizes
- Use the responsive grid system properly

### 5. Form Validation
- Always validate forms both client-side and server-side
- Use the built-in validation helpers
- Provide clear error messages
- Show loading states during form submission

## Browser Support

The Modern UI Framework supports:
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Troubleshooting

### Common Issues

1. **Modals not showing**: Ensure `modernUI` is loaded before calling modal functions
2. **Styles not applying**: Check if `modern-ui.css` is loaded after other CSS files
3. **JavaScript errors**: Ensure all dependencies are loaded in the correct order
4. **Toast notifications not working**: Check if the toast container is properly initialized

### Debug Mode

Enable debug mode by adding this to your page:
```javascript
modernUI.debug = true;
```

This will log additional information to the browser console.

## Contributing

When adding new components or features:
1. Follow the established naming conventions
2. Add proper documentation
3. Test across different browsers and devices
4. Ensure accessibility compliance
5. Update this guide with new features

## Support

For questions or issues with the Modern UI Framework, please refer to the development team or create an issue in the project repository.