/**
 * Modern UI Framework JavaScript
 * Custom modal, toast, and interactive components
 */

class ModernUI {
    constructor() {
        this.confirmCallbacks = {};
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.initializeComponents();
    }

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

        // Auto-hide alerts
        this.autoHideAlerts();
    }

    initializeComponents() {
        // Initialize any components that need setup
        this.initializeTooltips();
        this.initializeAnimations();
    }

    /**
     * Custom Modal System
     */
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

        // Add to DOM
        document.body.insertAdjacentHTML('beforeend', modalHTML);

        // Show with animation
        setTimeout(() => {
            const overlay = document.getElementById('customModalOverlay');
            const modal = document.getElementById('customModal');
            if (overlay && modal) {
                overlay.classList.add('show');
                modal.classList.add('show');
            }
        }, 10);

        // Callback
        if (onShow) onShow();

        return {
            modal: document.getElementById('customModal'),
            overlay: document.getElementById('customModalOverlay'),
            close: () => this.closeModal(),
            updateContent: (newContent) => {
                const body = document.querySelector('#customModal .custom-modal-body');
                if (body) body.innerHTML = newContent;
            }
        };
    }

    closeModal() {
        const overlay = document.getElementById('customModalOverlay');
        if (overlay) {
            overlay.classList.remove('show');
            setTimeout(() => {
                overlay.remove();
            }, 300);
        }
    }

    /**
     * Confirmation Dialog
     */
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

    handleConfirmAction(callbackId) {
        this.closeModal();
        if (this.confirmCallbacks && this.confirmCallbacks[callbackId]) {
            this.confirmCallbacks[callbackId]();
            delete this.confirmCallbacks[callbackId];
        }
    }

    handleConfirmCancel(callbackId) {
        this.closeModal();
        if (this.confirmCallbacks && this.confirmCallbacks[callbackId]) {
            this.confirmCallbacks[callbackId]();
            delete this.confirmCallbacks[callbackId];
        }
    }

    /**
     * Toast Notification System
     */
    showToast(options) {
        let title, message, type, duration;
        
        // Handle both object and string parameters for backward compatibility
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

    closeToast(toastId) {
        const toast = document.getElementById(toastId);
        if (toast) {
            toast.classList.remove('show');
            setTimeout(() => {
                toast.remove();
            }, 300);
        }
    }

    /**
     * Loading States
     */
    showLoading(element, text = 'Loading...') {
        if (typeof element === 'string') {
            element = document.querySelector(element);
        }
        
        if (element) {
            element.classList.add('btn-loading');
            element.disabled = true;
            element.setAttribute('data-original-text', element.textContent);
            element.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>${text}`;
        }
    }

    hideLoading(element) {
        if (typeof element === 'string') {
            element = document.querySelector(element);
        }
        
        if (element) {
            element.classList.remove('btn-loading');
            element.disabled = false;
            const originalText = element.getAttribute('data-original-text');
            if (originalText) {
                element.textContent = originalText;
                element.removeAttribute('data-original-text');
            }
        }
    }

    /**
     * Form Validation
     */
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

    validateForm(formSelector) {
        const form = document.querySelector(formSelector);
        if (!form) return false;

        let isValid = true;
        const requiredFields = form.querySelectorAll('[required]');

        requiredFields.forEach(field => {
            if (!field.value.trim()) {
                this.showFieldError(field, 'This field is required');
                isValid = false;
            } else {
                this.clearFieldError(field);
            }
        });

        return isValid;
    }

    showFieldError(field, message) {
        this.clearFieldError(field);
        
        field.classList.add('is-invalid');
        const errorDiv = document.createElement('div');
        errorDiv.className = 'invalid-feedback';
        errorDiv.textContent = message;
        field.parentNode.appendChild(errorDiv);
    }

    clearFieldError(field) {
        field.classList.remove('is-invalid');
        const errorDiv = field.parentNode.querySelector('.invalid-feedback');
        if (errorDiv) {
            errorDiv.remove();
        }
    }

    clearFormValidation(formSelector) {
        const form = document.querySelector(formSelector);
        if (!form) return;

        form.querySelectorAll('.is-valid, .is-invalid').forEach(field => {
            field.classList.remove('is-valid', 'is-invalid');
        });

        form.querySelectorAll('.validation-error').forEach(error => {
            error.remove();
        });
    }

    /**
     * AJAX Helper
     */
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

            if (loadingElement) {
                this.hideLoading(loadingElement);
            }

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
            if (loadingElement) {
                this.hideLoading(loadingElement);
            }
            
            this.showToast({
                title: 'Error',
                message: error.message || errorMessage,
                type: 'error'
            });
            
            throw error;
        }
    }

    /**
     * Utility Functions
     */
    /**
     * Generic Details Modal Helper
     */
    showDetailsModal(options) {
        const {
            title,
            data,
            fields,
            editUrl,
            editText = 'Edit',
            additionalButtons = []
        } = options;

        const content = `
            <div class="row g-3">
                ${fields.map(field => `
                    <div class="col-md-${field.colSize || 6}">
                        <div class="card border-0 bg-light">
                            <div class="card-body">
                                <h6 class="card-title text-${field.titleColor || 'primary'} mb-3">
                                    <i class="bi bi-${field.icon || 'info-circle'} me-1"></i>${field.title}
                                </h6>
                                ${field.items.map(item => `
                                    <div class="row mb-2">
                                        <div class="col-sm-${item.labelCol || 5}"><strong>${item.label}:</strong></div>
                                        <div class="col-sm-${item.valueCol || 7}">
                                            ${item.type === 'badge' 
                                                ? `<span class="badge bg-${item.badgeColor || 'primary'}">${this.safeValue(data[item.field], item.defaultValue)}</span>`
                                                : item.type === 'date'
                                                ? this.formatDate(data[item.field])
                                                : item.type === 'email'
                                                ? (data[item.field] ? `<a href="mailto:${data[item.field]}">${data[item.field]}</a>` : 'N/A')
                                                : item.type === 'phone'
                                                ? (data[item.field] ? `<a href="tel:${data[item.field]}">${data[item.field]}</a>` : 'N/A')
                                                : this.safeValue(data[item.field], item.defaultValue)
                                            }
                                        </div>
                                    </div>
                                `).join('')}
                            </div>
                        </div>
                    </div>
                `).join('')}
            </div>
        `;

        const buttons = [
            ...additionalButtons,
            {
                text: editText,
                class: 'btn-primary',
                icon: 'bi bi-pencil',
                onclick: `window.location.href='${editUrl}'`
            },
            {
                text: 'Close',
                class: 'btn-secondary',
                onclick: 'modernUI.closeModal()'
            }
        ];

        return this.showModal({
            title,
            size: 'lg',
            content,
            buttons
        });
    }

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

    autoHideAlerts() {
        setTimeout(() => {
            const alerts = document.querySelectorAll('.alert');
            alerts.forEach(alert => {
                if (window.bootstrap && window.bootstrap.Alert) {
                    const bsAlert = new bootstrap.Alert(alert);
                    bsAlert.close();
                }
            });
        }, 5000);
    }

    initializeTooltips() {
        // Initialize Bootstrap tooltips if available
        if (window.bootstrap && window.bootstrap.Tooltip) {
            const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
            tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        }
    }

    initializeAnimations() {
        // Add fade-in animation to cards
        const cards = document.querySelectorAll('.card');
        cards.forEach((card, index) => {
            card.style.animationDelay = `${index * 0.1}s`;
            card.classList.add('fade-in');
        });
    }

    /**
     * Table Utilities
     */
    exportTable(tableSelector, filename = 'export', format = 'csv') {
        const table = document.querySelector(tableSelector);
        if (!table) {
            this.showToast('Table not found', 'error');
            return;
        }

        let data = [];
        const rows = table.querySelectorAll('tr');
        
        rows.forEach(row => {
            const cells = row.querySelectorAll('th, td');
            const rowData = Array.from(cells).map(cell => cell.textContent.trim());
            data.push(rowData);
        });

        if (format === 'csv') {
            const csv = data.map(row => row.join(',')).join('\n');
            this.downloadFile(csv, `${filename}.csv`, 'text/csv');
        } else if (format === 'json') {
            const headers = data[0];
            const jsonData = data.slice(1).map(row => {
                const obj = {};
                headers.forEach((header, index) => {
                    obj[header] = row[index];
                });
                return obj;
            });
            this.downloadFile(JSON.stringify(jsonData, null, 2), `${filename}.json`, 'application/json');
        }

        this.showToast(`Exporting ${filename}.${format}...`, 'info');
    }

    downloadFile(content, filename, contentType) {
        const blob = new Blob([content], { type: contentType });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }

    /**
     * Search and Filter Utilities
     */
    setupLiveSearch(inputSelector, targetSelector, searchFunction = null) {
        const input = document.querySelector(inputSelector);
        const target = document.querySelector(targetSelector);
        
        if (!input || !target) return;

        let timeout;
        input.addEventListener('input', (e) => {
            clearTimeout(timeout);
            timeout = setTimeout(() => {
                const query = e.target.value.toLowerCase();
                
                if (searchFunction) {
                    searchFunction(query, target);
                } else {
                    // Default search implementation
                    const rows = target.querySelectorAll('tbody tr');
                    rows.forEach(row => {
                        const text = row.textContent.toLowerCase();
                        row.style.display = text.includes(query) ? '' : 'none';
                    });
                }
            }, 300);
        });
    }
}

// Initialize ModernUI
const modernUI = new ModernUI();

// Global helper functions for backward compatibility
function showToast(message, type = 'info', duration = 5000) {
    modernUI.showToast({ message, type, duration });
}

function showConfirmation(message, onConfirm, title = 'Confirm Action') {
    modernUI.showConfirmation({ title, message, onConfirm });
}

function showModal(title, content, buttons = []) {
    return modernUI.showModal({ title, content, buttons });
}

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ModernUI;
}