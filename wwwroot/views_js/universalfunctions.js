function formatDateToDDMMYYYYLabels(dateString) {
    const date = new Date(dateString);

    // Extract the date components
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Months are zero-based
    const year = date.getFullYear();

    // Format the date as dd-mm-yyyy
    return `${day}-${month}-${year}`;
}

function formatDateForInputs(dateString) {
    const date = new Date(dateString);

    // Extract the date components
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Months are zero-based
    const day = date.getDate().toString().padStart(2, '0');

    // Format the date as yyyy-mm-dd
    return `${year}-${month}-${day}`;
}

function makeHiddenInputForSubmission(name, value) {
    const input = document.createElement('input');
    input.type = 'hidden';
    input.name = name;
    input.value = value;
    return input;
}

const appendAlert = (message, type) => {
    const alertPlaceholder = document.getElementById('liveAlertPlaceholder');
    if (!alertPlaceholder) {
        console.error('Alert placeholder not found');
        return;
    }

    const wrapper = document.createElement('div');
    wrapper.innerHTML = [
        `<div class="alert alert-${type} alert-dismissible" role="alert">`,
        `   <div>${message}</div>`,
        '   <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>',
        '</div>'
    ].join('');

    alertPlaceholder.append(wrapper);

    // Automatically close the alert after 3 seconds with fade-out effect
    setTimeout(() => {
        wrapper.classList.add('fade-out');
        setTimeout(() => {
            wrapper.remove();
        }, 500); // Match this duration with the fadeOut animation duration
    }, 3000);
};

const appendAlertWithoutAnimation = (message, type) => {
    const alertPlaceholder = document.getElementById('liveAlertPlaceholder');
    if (!alertPlaceholder) {
        console.error('Alert placeholder not found');
        return;
    }

    const wrapper = document.createElement('div');
    wrapper.innerHTML = [
        `<div class="alert alert-${type} alert-dismissible" role="alert">`,
        `   <div>${message}</div>`,
        '   <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>',
        '</div>'
    ].join('');

    alertPlaceholder.append(wrapper);
};
