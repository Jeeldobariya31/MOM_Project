/**
 * Simple Popup System - Lightweight and secure
 * Replaces modern-ui.js with minimal, non-blocking functionality
 */

(function(window) {
    'use strict';

    // Simple popup system
    const SimplePopup = {
        // Show modal dialog
        showModal: function(options) {
            const {
                title = 'Modal',
                content = '',
                size = 'md',
                buttons = []
            } = options;

            // Remove existing modal
            this.closeModal();

            // Create modal HTML
            const modalHTML = `
                <div class="modal fade" id="simpleModal" tabindex="-1" data-bs-backdrop="static">
                    <div class="modal-dialog modal-${size}">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">${title}</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body">
                                ${content}
                            </div>
                            ${buttons.length > 0 ? `
                                <div class="modal-footer">
                                    ${buttons.map(btn => `
                                        <button type="button" class="btn ${btn.class || 'btn-secondary'}" 
                                                onclick="${btn.onclick || 'SimplePopup.closeModal()'}"
                                                ${btn.dismiss ? 'data-bs-dismiss="modal"' : ''}>
                                            ${btn.icon ? `<i class="${btn.icon} me-1"></i>` : ''}
                                            ${btn.text}
                                        </button>
                                    `).join('')}
                                </div>
                            ` : ''}
                        </div>
                    </div>
                </div>
            `;

            // Add to DOM
            document.body.insertAdjacentHTML('beforeend', modalHTML);

            // Show modal using Bootstrap
            const modal = new bootstrap.Modal(document.getElementById('simpleModal'));
            modal.show();

            return modal;
        },

        // Close modal
        closeModal: function() {
            const existingModal = document.getElementById('simpleModal');
            if (existingModal) {
                const modal = bootstrap.Modal.getInstance(existingModal);
                if (modal) {
                    modal.hide();
                }
                setTimeout(() => {
                    if (existingModal.parentNode) {
                        existingModal.remove();
                    }
                }, 300);
            }
        },

        // Show confirmation dialog
        showConfirmation: function(options) {
            const {
                title = 'Confirm Action',
                message = 'Are you sure?',
                confirmText = 'Confirm',
                cancelText = 'Cancel',
                confirmClass = 'btn-danger',
                onConfirm = null,
                onCancel = null
            } = options;

            return this.showModal({
                title,
                size: 'sm',
                content: `
                    <div class="text-center py-3">
                        <i class="bi bi-exclamation-triangle text-warning" style="font-size: 3rem; margin-bottom: 1rem;"></i>
                        <div class="mb-0">${message}</div>
                    </div>
                `,
                buttons: [
                    {
                        text: cancelText,
                        class: 'btn-secondary',
                        dismiss: true,
                        onclick: onCancel ? `(${onCancel.toString()})(); SimplePopup.closeModal();` : 'SimplePopup.closeModal()'
                    },
                    {
                        text: confirmText,
                        class: confirmClass,
                        onclick: onConfirm ? `(${onConfirm.toString()})(); SimplePopup.closeModal();` : 'SimplePopup.closeModal()'
                    }
                ]
            });
        },

        // Show toast notification
        showToast: function(message, type = 'info', duration = 5000) {
            // Create toast container if it doesn't exist
            let container = document.getElementById('toastContainer');
            if (!container) {
                container = document.createElement('div');
                container.id = 'toastContainer';
                container.className = 'toast-container position-fixed top-0 end-0 p-3';
                container.style.zIndex = '9999';
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
                <div class="toast" id="${toastId}" role="alert">
                    <div class="toast-header">
                        <i class="bi ${iconMap[type]} me-2"></i>
                        <strong class="me-auto">${type.charAt(0).toUpperCase() + type.slice(1)}</strong>
                        <button type="button" class="btn-close" data-bs-dismiss="toast"></button>
                    </div>
                    <div class="toast-body">
                        ${message}
                    </div>
                </div>
            `;

            container.insertAdjacentHTML('beforeend', toastHTML);

            // Show toast using Bootstrap
            const toastElement = document.getElementById(toastId);
            const toast = new bootstrap.Toast(toastElement, {
                delay: duration
            });
            toast.show();

            // Remove after hiding
            toastElement.addEventListener('hidden.bs.toast', () => {
                toastElement.remove();
            });

            return toast;
        },

        // Show loading state on button
        setButtonLoading: function(button, text = 'Loading...') {
            if (!button) return;
            
            button.disabled = true;
            button.setAttribute('data-original-text', button.innerHTML);
            button.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>${text}`;
        },

        // Reset button from loading state
        resetButton: function(button) {
            if (!button) return;
            
            button.disabled = false;
            const originalText = button.getAttribute('data-original-text');
            if (originalText) {
                button.innerHTML = originalText;
                button.removeAttribute('data-original-text');
            }
        },

        // Export table data
        exportTable: function(tableSelector, filename = 'export', format = 'csv') {
            const table = document.querySelector(tableSelector);
            if (!table) {
                this.showToast('Table not found', 'error');
                return;
            }

            let data = [];
            const rows = table.querySelectorAll('tr');
            
            rows.forEach(row => {
                const cells = row.querySelectorAll('th, td');
                const rowData = Array.from(cells).map(cell => {
                    // Clean cell text (remove extra whitespace and HTML)
                    return cell.textContent.trim().replace(/\s+/g, ' ');
                });
                if (rowData.some(cell => cell.length > 0)) { // Only add non-empty rows
                    data.push(rowData);
                }
            });

            if (format === 'csv') {
                const csv = data.map(row => 
                    row.map(cell => `"${cell.replace(/"/g, '""')}"`).join(',')
                ).join('\n');
                this.downloadFile(csv, `${filename}.csv`, 'text/csv');
            }

            this.showToast(`Exporting ${filename}.${format}...`, 'info');
        },

        // Download file
        downloadFile: function(content, filename, contentType) {
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
    };

    // Make it globally available
    window.SimplePopup = SimplePopup;

    // Backward compatibility aliases
    window.showToast = function(message, type, duration) {
        SimplePopup.showToast(message, type, duration);
    };

    window.showConfirmation = function(message, onConfirm, title) {
        SimplePopup.showConfirmation({ title, message, onConfirm });
    };

    window.showModal = function(title, content, buttons) {
        return SimplePopup.showModal({ title, content, buttons });
    };

})(window);