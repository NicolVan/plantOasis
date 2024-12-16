document.addEventListener('DOMContentLoaded', function () {
    const deliveryCheckbox = document.querySelector('.cb-delivery-address');
    const deliveryAddressDiv = document.querySelector('.delivery-address');

    if (deliveryCheckbox && deliveryAddressDiv) {
        // Initially hide or show the delivery address fields based on checkbox state
        deliveryAddressDiv.style.display = deliveryCheckbox.checked ? 'block' : 'none';

        // Add event listener to toggle visibility
        deliveryCheckbox.addEventListener('change', function () {
            if (this.checked) {
                // Show delivery address and remove 'hidden' class if it's present
                deliveryAddressDiv.classList.remove('hidden');
                deliveryAddressDiv.style.display = 'block';
            } else {
                // Hide delivery address
                deliveryAddressDiv.style.display = 'none';
            }
        });
    }
});
