# JavaScript Documentation - MOM System

## Table of Contents
1. [JavaScript Architecture](#javascript-architecture)
2. [ModernUI Framework](#modernui-framework)
3. [AJAX Operations](#ajax-operations)
4. [Form Validation](#form-validation)
5. [UI Components](#ui-components)
6. [Event Handling](#event-handling)
7. [Utility Functions](#utility-functions)
8. [Page-Specific Scripts](#page-specific-scripts)

## JavaScript Architecture

### File Structure
```
wwwroot/assets/js/
├── modern-ui.js      # Core UI framework and components
├── main.js           # Application-specific functionality
└── page-specific/    # Individual page scripts
```

### Design Patterns Used
- **Module Pattern**: Encapsulation of functionality
- **Singleton Pattern**: Single instance of ModernUI
- **Observer Pattern**: Event-driven architecture
- **Factory Pattern**: Dynamic component creation

## ModernUI Framework

### Core Class Structure
```javascript
class ModernUI {
    constructor() {
        this.confirmCallbacks = {};
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.initializeComponents();
        this.protectGuidelines();
    }
}

// Global instance
const modernUI = new ModernUI();
```

### Key Features
1. **Modal System**: Custom modal dialogs
2. **Toast Notifications**: User feedback system
3. **Form Validation**: Real-time validation
4. **AJAX Helpers**: Simplified API calls
5. **UI Components**: Reusable interface elements

## AJAX Operations

### 1. Generic AJAX Helper
```javascript
async makeRequest(url, options = {}) {
    const {
        method = 'GET',
        data = null,
        headers = {},
        loadingElement = null,
        successMessage = null,
        errorMessage = 'An error occurred'
    } = options;

    if (loadingElement) {
        this.showLoading(loadingElement);
    }

    try {
        const fetchOptions = {
            method,
            headers: {
                'Content-Type': 'application/json',
                ...headers
            }
        };

        if (data && method !== 'GET') {
            fetchOptions.body = JSON.stringify(data);
        }

        const response = await fetch(url, fetchOptions);
        const result = await response.json();

        if (response.ok) {
            if (successMessage) {
                this.showToast({
                    title: 'Success',
                    message: successMessage,
                    type: 'success'
                });
            }
            return result;
        } else {
            throw new Error(result.message || errorMessage);
        }
    } catch (error) {
        this.showToast({
            title: 'Error',
            message: error.message || errorMessage,
            type: 'error'
        });
        throw error;
    }
}
```

### 2. Meeting Operations

#### View Meeting Details
```javascript
function viewMeeting(meetingId) {
    const button = document.querySelector(`button[onclick="viewMeeting(${meetingId})"]`);
    if (button) {
        modernUI.setButtonLoading(button, 'Loading...');
    }
    
    fetch(`/Meeting/GetMeetingDetails?id=${meetingId}`)
        .then(response => response.json())
        .then(data => {
            if (button) {
                modernUI.resetButton(button);
            }
            
            if (data.success) {
                const meeting = data.data;
                modernUI.showModal({
                    title: `Meeting Details`,
                    size: 'lg',
                    content: generateMeetingDetailsHTML(meeting),
                    buttons: [
                        {
                            text: 'Edit Meeting',
                            class: 'btn-primary',
                            onclick: `window.location.href='/Meeting/MeetingAddEdit/${meetingId}'`
                        },
                        {
                            text: 'Close',
                            class: 'btn-secondary',
                            onclick: 'modernUI.closeModal()'
                        }
                    ]
                });
            }
        })
        .catch(error => {
            if (button) {
                modernUI.resetButton(button);
            }
            modernUI.showToast('Failed to load meeting details', 'error');
        });
}
```

#### Cancel Meeting
```javascript
function cancelMeeting(meetingId) {
    modernUI.showModal({
        title: 'Cancel Meeting',
        content: `
            <div class="alert alert-warning">
                <strong>Are you sure you want to cancel this meeting?</strong>
            </div>
            <form id="cancelMeetingForm">
                <div class="mb-3">
                    <label class="form-label">
                        Cancellation Reason <span class="text-muted">(Optional)</span>
                    </label>
                    <textarea name="reason" class="form-control" rows="3" maxlength="250"
                              placeholder="Provide a reason for cancellation (optional)..." 
                              oninput="updateCharacterCount(this)"></textarea>
                    <div class="d-flex justify-content-between">
                        <div class="form-text">
                            Providing a reason helps participants understand the cancellation.
                        </div>
                        <small class="text-muted" id="charCount">0/250</small>
                    </div>
                </div>
            </form>
        `,
        buttons: [
            {
                text: 'Cancel Meeting',
                class: 'btn-warning',
                onclick: `submitCancelMeeting(${meetingId})`
            },
            {
                text: 'Keep Meeting',
                class: 'btn-secondary',
                onclick: 'modernUI.closeModal()'
            }
        ]
    });
}

function submitCancelMeeting(meetingId) {
    const form = document.getElementById('cancelMeetingForm');
    const reason = form.reason.value.trim();
    
    const submitButton = document.querySelector('button[onclick*="submitCancelMeeting"]');
    if (submitButton) {
        modernUI.setButtonLoading(submitButton, 'Cancelling...');
    }

    const formData = new FormData();
    formData.append('id', meetingId);
    formData.append('reason', reason);

    fetch('/Meeting/CancelMeeting', {
        method: 'POST',
        headers: {
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (submitButton) {
            modernUI.resetButton(submitButton);
        }
        
        if (data.success) {
            modernUI.closeModal();
            modernUI.showToast('Meeting cancelled successfully', 'success');
            setTimeout(() => location.reload(), 1500);
        } else {
            modernUI.showToast(data.message || 'Error cancelling meeting', 'error');
        }
    })
    .catch(error => {
        if (submitButton) {
            modernUI.resetButton(submitButton);
        }
        modernUI.showToast('Error cancelling meeting', 'error');
    });
}
```

#### Delete Meeting
```javascript
function deleteMeeting(meetingId) {
    modernUI.showConfirmation({
        title: 'Delete Meeting',
        message: 'Are you sure you want to delete this meeting? This action cannot be undone.',
        confirmText: 'Delete Meeting',
        confirmClass: 'btn-danger',
        onConfirm: () => {
            fetch('/Meeting/Delete', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: `id=${meetingId}`
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    modernUI.showToast('Meeting deleted successfully', 'success');
                    setTimeout(() => location.reload(), 1500);
                } else {
                    modernUI.showToast(data.message || 'Failed to delete meeting', 'error');
                }
            })
            .catch(error => {
                modernUI.showToast('Failed to delete meeting', 'error');
            });
        }
    });
}
```

## Form Validation

### 1. Form Validation System
```javascript
initializeFormValidation(formSelector, rules) {
    const form = document.querySelector(formSelector);
    if (!form) return;

    Object.keys(rules).forEach(fieldName => {
        const field = form.querySelector(`[name="${fieldName}"]`);
        if (field) {
            field.addEventListener('input', () => {
                this.validateField(field, rules[fieldName]);
            });
            field.addEventListener('blur', () => {
                this.validateField(field, rules[fieldName]);
            });
        }
    });
}

validateField(field, rules) {
    const value = field.value.trim();
    let isValid = true;
    let errorMessage = '';

    // Required validation
    if (rules.required && !value) {
        isValid = false;
        errorMessage = rules.messages?.required || 'This field is required';
    }

    // Min length validation
    if (isValid && rules.minLength && value.length < rules.minLength) {
        isValid = false;
        errorMessage = rules.messages?.minLength || `Minimum ${rules.minLength} characters required`;
    }

    // Max length validation
    if (isValid && rules.maxLength && value.length > rules.maxLength) {
        isValid = false;
        errorMessage = rules.messages?.maxLength || `Maximum ${rules.maxLength} characters allowed`;
    }

    // Pattern validation
    if (isValid && rules.pattern && !rules.pattern.test(value)) {
        isValid = false;
        errorMessage = rules.messages?.pattern || 'Invalid format';
    }

    // Update field appearance
    if (isValid) {
        field.classList.remove('is-invalid');
        field.classList.add('is-valid');
    } else {
        field.classList.remove('is-valid');
        field.classList.add('is-invalid');
    }

    // Show/hide error message
    let errorElement = field.parentNode.querySelector('.validation-error');
    if (!isValid) {
        if (!errorElement) {
            errorElement = document.createElement('div');
            errorElement.className = 'validation-error text-danger small mt-1';
            field.parentNode.appendChild(errorElement);
        }
        errorElement.textContent = errorMessage;
    } else if (errorElement) {
        errorElement.remove();
    }

    return isValid;
}
```

### 2. Meeting Form Validation Example
```javascript
modernUI.initializeFormValidation('#meetingForm', {
    'MeetingDate': {
        required: true,
        messages: {
            required: 'Meeting date and time is required'
        }
    },
    'DepartmentID': {
        required: true,
        messages: {
            required: 'Please select a department'
        }
    },
    'MeetingTypeID': {
        required: true,
        messages: {
            required: 'Please select a meeting type'
        }
    },
    'MeetingVenueID': {
        required: true,
        messages: {
            required: 'Please select a meeting venue'
        }
    },
    'MeetingDescription': {
        maxLength: 250,
        messages: {
            maxLength: 'Description cannot exceed 250 characters'
        }
    },
    'CancellationReason': {
        required: false,
        maxLength: 250,
        messages: {
            maxLength: 'Cancellation reason cannot exceed 250 characters'
        }
    }
});
```

## UI Components

### 1. Modal System
```javascript
showModal(options) {
    const {
        title = 'Modal',
        content = '',
        size = 'md',
        showCloseButton = true,
        buttons = [],
        onShow = null
    } = options;

    // Remove existing modal
    this.closeModal();

    // Create modal HTML
    const modalHTML = `
        <div class="modal-overlay" id="customModalOverlay">
            <div class="custom-modal custom-modal-${size}" id="customModal">
                <div class="custom-modal-header">
                    <h5 class="custom-modal-title">${title}</h5>
                    ${showCloseButton ? '<button type="button" class="custom-modal-close" onclick="modernUI.closeModal()"><i class="bi bi-x"></i></button>' : ''}
                </div>
                <div class="custom-modal-body">
                    ${content}
                </div>
                ${buttons.length > 0 ? `
                    <div class="custom-modal-footer">
                        ${buttons.map(btn => `
                            <button type="button" class="btn ${btn.class || 'btn-secondary'}" 
                                    onclick="${btn.onclick || 'modernUI.closeModal()'}"
                                    ${btn.attributes || ''}>
                                ${btn.icon ? `<i class="${btn.icon} me-1"></i>` : ''}
                                ${btn.text}
                            </button>
                        `).join('')}
                    </div>
                ` : ''}
            </div>
        </div>
    `;

    // Add to DOM and show with animation
    document.body.insertAdjacentHTML('beforeend', modalHTML);
    
    setTimeout(() => {
        const overlay = document.getElementById('customModalOverlay');
        const modal = document.getElementById('customModal');
        if (overlay && modal) {
            overlay.classList.add('show');
            modal.classList.add('show');
        }
    }, 10);

    if (onShow) onShow();
}
```

### 2. Toast Notification System
```javascript
showToast(options) {
    let title, message, type, duration;
    
    // Handle both object and string parameters
    if (typeof options === 'string') {
        message = options;
        type = arguments[1] || 'info';
        duration = arguments[2] || 5000;
        title = '';
    } else {
        ({
            title = '',
            message = '',
            type = 'info',
            duration = 5000
        } = options);
    }

    // Create toast container if it doesn't exist
    let container = document.getElementById('toastContainer');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toastContainer';
        container.className = 'toast-container';
        document.body.appendChild(container);
    }

    // Create toast
    const toastId = 'toast_' + Date.now();
    const iconMap = {
        success: 'bi-check-circle-fill text-success',
        error: 'bi-x-circle-fill text-danger',
        warning: 'bi-exclamation-triangle-fill text-warning',
        info: 'bi-info-circle-fill text-info'
    };

    const toastHTML = `
        <div class="custom-toast ${type}" id="${toastId}">
            <i class="toast-icon ${iconMap[type]}"></i>
            <div class="toast-content">
                ${title ? `<div class="toast-title">${title}</div>` : ''}
                <div class="toast-message">${message}</div>
            </div>
            <button type="button" class="toast-close" onclick="modernUI.closeToast('${toastId}')">
                <i class="bi bi-x"></i>
            </button>
        </div>
    `;

    container.insertAdjacentHTML('beforeend', toastHTML);

    // Show with animation
    setTimeout(() => {
        const toast = document.getElementById(toastId);
        if (toast) toast.classList.add('show');
    }, 10);

    // Auto-hide
    if (duration > 0) {
        setTimeout(() => {
            this.closeToast(toastId);
        }, duration);
    }

    return toastId;
}
```

### 3. Confirmation Dialog
```javascript
showConfirmation(options) {
    const {
        title = 'Confirm Action',
        message = 'Are you sure?',
        confirmText = 'Confirm',
        cancelText = 'Cancel',
        confirmClass = 'btn-danger',
        onConfirm = null,
        onCancel = null
    } = options;

    const confirmId = 'confirm-' + Date.now();
    const cancelId = 'cancel-' + Date.now();

    // Store callbacks
    this.confirmCallbacks[confirmId] = onConfirm;
    this.confirmCallbacks[cancelId] = onCancel;

    return this.showModal({
        title,
        size: 'sm',
        content: `
            <div class="text-center py-3">
                <i class="bi bi-exclamation-triangle text-warning" style="font-size: 3rem; margin-bottom: 1rem;"></i>
                <p class="mb-0 fs-6">${message}</p>
            </div>
        `,
        buttons: [
            {
                text: cancelText,
                class: 'btn-secondary',
                onclick: `modernUI.handleConfirmCancel('${cancelId}')`
            },
            {
                text: confirmText,
                class: confirmClass,
                onclick: `modernUI.handleConfirmAction('${confirmId}')`
            }
        ]
    });
}
```

## Event Handling

### 1. Global Event Listeners
```javascript
setupEventListeners() {
    // Close modals when clicking overlay
    document.addEventListener('click', (e) => {
        if (e.target.classList.contains('modal-overlay')) {
            this.closeModal();
        }
    });

    // Close modals with Escape key
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            this.closeModal();
        }
    });
}
```

### 2. Form Event Handling
```javascript
// Form submission with loading state
const form = document.getElementById('meetingForm');
const submitBtn = document.getElementById('submitBtn');

form.addEventListener('submit', function(e) {
    if (!modernUI.validateForm('#meetingForm')) {
        e.preventDefault();
        return false;
    }
    
    // Show loading state
    modernUI.setButtonLoading(submitBtn, 'Saving...');
});
```

### 3. Dynamic Content Events
```javascript
// Toggle cancellation details
document.getElementById('isCancelledSwitch').addEventListener('change', function() {
    const cancellationDetails = document.getElementById('cancellationDetails');
    const statusLabel = document.getElementById('statusLabel');
    
    if (this.checked) {
        cancellationDetails.style.display = 'block';
        statusLabel.textContent = 'Cancelled';
        statusLabel.className = 'text-danger';
    } else {
        cancellationDetails.style.display = 'none';
        statusLabel.textContent = 'Active';
        statusLabel.className = 'text-success';
    }
});
```

## Utility Functions

### 1. Loading States
```javascript
setButtonLoading(button, text = 'Loading...') {
    if (!button) return;
    
    button.disabled = true;
    button.dataset.originalText = button.innerHTML;
    button.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>${text}`;
}

resetButton(button) {
    if (!button) return;
    
    button.disabled = false;
    if (button.dataset.originalText) {
        button.innerHTML = button.dataset.originalText;
    }
}
```

### 2. Character Counter
```javascript
initializeCharacterCounter(textarea, maxLength) {
    const counter = document.createElement('div');
    counter.className = 'character-counter text-end text-muted small mt-1';
    textarea.parentNode.appendChild(counter);

    const updateCounter = () => {
        const count = textarea.value.length;
        counter.innerHTML = `${count}/${maxLength} characters`;
        
        if (count > maxLength * 0.9) {
            counter.className = 'character-counter text-end text-warning small mt-1';
        } else {
            counter.className = 'character-counter text-end text-muted small mt-1';
        }
    };

    textarea.addEventListener('input', updateCounter);
    updateCounter();
}
```

### 3. Data Formatting
```javascript
safeValue(value, defaultValue = 'N/A') {
    if (value === null || value === undefined || value === '') {
        return defaultValue;
    }
    return value;
}

formatDate(dateString, format = 'dd MMM yyyy') {
    if (!dateString) return 'N/A';
    try {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) return 'Invalid Date';
        
        const options = {
            year: 'numeric',
            month: 'short',
            day: '2-digit'
        };
        
        if (format.includes('hh:mm')) {
            options.hour = '2-digit';
            options.minute = '2-digit';
            options.hour12 = true;
        }
        
        return date.toLocaleDateString('en-US', options);
    } catch (error) {
        return 'Invalid Date';
    }
}
```

## Page-Specific Scripts

### 1. Meeting List Page
```javascript
// Character count helper for cancellation reason
function updateCharacterCount(textarea) {
    const charCount = document.getElementById('charCount');
    if (charCount) {
        const count = textarea.value.length;
        charCount.textContent = `${count}/250`;
        
        if (count > 200) {
            charCount.className = 'text-warning';
        } else if (count > 240) {
            charCount.className = 'text-danger';
        } else {
            charCount.className = 'text-muted';
        }
    }
}

// Initialize live search
document.addEventListener('DOMContentLoaded', function() {
    modernUI.setupLiveSearch('input[name="search"]', '#meetingTable');
});
```

### 2. Meeting Add/Edit Page
```javascript
document.addEventListener('DOMContentLoaded', function() {
    // Initialize modern form validation
    modernUI.initializeFormValidation('#meetingForm', validationRules);

    // Form submission with loading state
    const form = document.getElementById('meetingForm');
    const submitBtn = document.getElementById('submitBtn');
    
    form.addEventListener('submit', function(e) {
        if (!modernUI.validateForm('#meetingForm')) {
            e.preventDefault();
            return false;
        }
        
        modernUI.setButtonLoading(submitBtn, 'Saving...');
    });

    // Character counter for description
    const descriptionField = document.querySelector('textarea[name="MeetingDescription"]');
    if (descriptionField) {
        modernUI.initializeCharacterCounter(descriptionField, 250);
    }

    // File upload enhancement
    const fileInput = document.querySelector('input[type="file"]');
    if (fileInput) {
        fileInput.addEventListener('change', function() {
            const file = this.files[0];
            if (file) {
                // Check file size (10MB limit)
                if (file.size > 10 * 1024 * 1024) {
                    modernUI.showToast('File size cannot exceed 10MB', 'error');
                    this.value = '';
                    return;
                }
                
                // Show file info
                const fileInfo = document.createElement('div');
                fileInfo.className = 'alert alert-success mt-2';
                fileInfo.innerHTML = `
                    <i class="bi bi-file-earmark-check me-1"></i>
                    <strong>Selected:</strong> ${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)
                `;
                
                this.parentNode.appendChild(fileInfo);
            }
        });
    }
});
```

### 3. Dashboard Page
```javascript
document.addEventListener('DOMContentLoaded', function() {
    // Initialize charts
    initializeDashboardCharts();
    
    // Auto-refresh data every 5 minutes
    setInterval(refreshDashboardData, 300000);
    
    // Initialize tooltips
    modernUI.initializeTooltips();
});

function initializeDashboardCharts() {
    // Meeting by Department Chart
    const departmentChart = new ApexCharts(document.querySelector("#departmentChart"), {
        series: departmentData.values,
        chart: {
            type: 'donut',
            height: 350
        },
        labels: departmentData.labels,
        colors: ['#0d6efd', '#198754', '#ffc107', '#dc3545', '#6f42c1']
    });
    departmentChart.render();
    
    // Monthly Meetings Chart
    const monthlyChart = new ApexCharts(document.querySelector("#monthlyChart"), {
        series: [{
            name: 'Meetings',
            data: monthlyData.values
        }],
        chart: {
            type: 'line',
            height: 350
        },
        xaxis: {
            categories: monthlyData.labels
        }
    });
    monthlyChart.render();
}
```

This comprehensive JavaScript documentation covers all the key aspects of the client-side functionality in the MOM system, including the custom ModernUI framework, AJAX operations, form validation, UI components, and page-specific implementations.