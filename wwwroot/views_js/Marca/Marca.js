console.log('marca script');

let currentPage = 0;

const pageSize = 10;


document.getElementById('inputSearchMarca').addEventListener('keydown', function (event) {
    if (event.key === 'Enter') {
        event.preventDefault();
        turnOnLoader();
        currentPage = 0;
        handleSearch();
    }
});


document.getElementById('searchMarcaBtn').addEventListener('click', function () {
    currentPage = 0;
    handleSearch();
});

function handleSearch() {

    const filter = document.getElementById("inputSearchMarca").value;
    const status = document.querySelector('input[name="status"]:checked').value;
    const pagination = document.querySelector('input[name="pagination"]:checked').value;

    $.ajax({
        url: '/Marcas/GetMarcaList',
        type: 'GET',
        data: { filter: filter, status: status, pagination: pagination, page: currentPage },
        success: function (data) {
            if (data.success) {
                console.log(data.result);
                renderTable(data.result);
                updatePaginationControls(data.totalItems);
                turnOffLoader();
            } else {
                console.error('No results found:', data.message);
                turnOffLoader();
            }
        },
        error: function (error) {
        }
    });
}

function renderTable(data) {
    const tbody = document.querySelector('#myTable tbody');
    tbody.innerHTML = '';

    data.forEach(item => {

        const row = document.createElement('tr');

        const idCell = document.createElement('td');
        idCell.textContent = item.id;

        const nomeCell = document.createElement('td');
        nomeCell.textContent = item.nome;

        const ativoCell = document.createElement('td');

        const blankCell = document.createElement('td');

        const editLink = document.createElement("a");
        editLink.href = "#";
        editLink.classList.add('btn');
        editLink.classList.add('btn-warning');
        editLink.innerText = "Editar";
        editLink.onclick = function () {
            openEdit(item); // Assuming message.id is the identifier
        };
        blankCell.appendChild(editLink);


        if (item.ativo === 1) {
            ativoCell.textContent = 'Ativo';

            const deleteLink = document.createElement("a");
            deleteLink.href = "#";
            deleteLink.classList.add('btn');
            deleteLink.classList.add('btn-danger');
            deleteLink.classList.add('ms-2');
            deleteLink.innerText = "Inativar";
            deleteLink.onclick = function () {
                openConfirmInactivate(item); // Assuming message.id is the identifier
            };
            blankCell.appendChild(deleteLink);
        } else {
            ativoCell.textContent = 'Inativo';
            row.classList.add('table-danger');
        }

        row.appendChild(idCell);
        row.appendChild(nomeCell);
        row.appendChild(ativoCell);
        row.appendChild(blankCell);
        tbody.appendChild(row);

    });
}

function openEdit(item) {

    var modal = document.getElementById('EditMarca');
    var modalInstance = new bootstrap.Modal(modal);
    modal.setAttribute('data-bs-instance', modalInstance); // Store the instance in a data attribute
    modalInstance.show();



    // Set the values for the input fields
    modal.querySelector('#idMarca').value = item.id;
    modal.querySelector('.nome-input-marca').value = item.nome;
    modal.querySelector('.nome-input-marca').classList.remove('is-invalid');
    modal.querySelector('.nome-input-marca').classList.remove('is-valid');

    modal.querySelector('.spinner-nome-marca').style.display = 'none';
    modal.querySelector('.empty-nome-validate').style.display = 'none';
    modal.querySelector('.duplicate-nome-validate').style.display = 'none';


}

function openConfirmInactivate(item) {
    
    var modal = document.getElementById('inactivateMarcaModal');
    var modalInstance = new bootstrap.Modal(modal);
    modal.setAttribute('data-bs-instance', modalInstance); // Store the instance in a data attribute
    modalInstance.show();

    var modalbody = modal.querySelector('.modal-body');
    modalbody.innerHTML = `Tem certeza de que deseja desativar este item <br> Id:<b>${item.id}</b> <br> Marca:<b>${item.nome}</b>?`

    const input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'Id';
    input.value = item.id;

    modalbody.appendChild(input);
}



function updatePaginationControls(totalItems) {
    const totalPages = Math.ceil(totalItems / pageSize);
    const paginationControls = document.getElementById('paginationControls');
    paginationControls.innerHTML = '';

    const themeClass = document.querySelector("body").dataset.theme === 'dark' ? 'bg-dark text-light' : 'bg-light text-dark';

    // Previous button
    const prevButton = document.createElement('li');
    prevButton.className = 'page-item' + (currentPage === 0 ? ' disabled' : '');
    // prevButton.innerHTML = `<a class="page-link" href="#" aria-label="Previous"><span aria-hidden="true">&laquo;</span></a>`;
    prevButton.innerHTML = `<a class="page-link ${themeClass}" href="#" aria-label="Previous"><span aria-hidden="true">&laquo;</span></a>`;
    prevButton.onclick = function (event) {
        event.preventDefault(); // Prevent default action
        if (currentPage > 0) {
            currentPage--;
            handleSearch();
        }
    };
    paginationControls.appendChild(prevButton);

    // Page number buttons
    for (let i = 0; i < totalPages; i++) {
        const pageItem = document.createElement('li');
        const pageLinkClass = `page-link ${i === currentPage ? '' : themeClass}`;
        pageItem.className = 'page-item' + (i === currentPage ? ' active' : '');
        // pageItem.innerHTML = `<a class="page-link" href="#">${i + 1}</a>`;
        pageItem.innerHTML = `<a class="${pageLinkClass}" href="#">${i + 1}</a>`;
        pageItem.onclick = function (event) {
            event.preventDefault(); // Prevent default action
            currentPage = i;
            handleSearch();
        };
        paginationControls.appendChild(pageItem);
    }

    // Next button
    const nextButton = document.createElement('li');
    nextButton.className = 'page-item' + (currentPage === totalPages - 1 ? ' disabled' : '');
    // nextButton.innerHTML = `<a class="page-link" href="#" aria-label="Next"><span aria-hidden="true">&raquo;</span></a>`;
    nextButton.innerHTML = `<a class="page-link ${themeClass}" href="#" aria-label="Next"><span aria-hidden="true">&raquo;</span></a>`;
    nextButton.onclick = function (event) {
        event.preventDefault(); // Prevent default action
        if (currentPage < totalPages - 1) {
            currentPage++;
            handleSearch();
        }
    };
    paginationControls.appendChild(nextButton);
}
  
document.querySelectorAll('.nome-input-marca').forEach(function (element) {
    element.addEventListener('blur', function (event) {
        var nomeInput = event.target;
        var nomeSpinner = nomeInput.closest('.d-flex').querySelector('.spinner-nome-marca');
        var nomeValidate = nomeInput.closest('.col-9').querySelector('.empty-nome-validate');
        var nomeValidateDiv = nomeInput.closest('.col-9').querySelector('.duplicate-nome-validate');

        var idMarca = document.getElementById("idMarca").value || null;
        console.log(idMarca);

        console.log(nomeInput.value);

        if (nomeInput.value.trim() !== "") {
            nomeInput.disabled = true;
            nomeSpinner.style.display = 'block';
            nomeValidateDiv.style.display = 'block';
            nomeValidateDiv.innerHTML = `Aguarde um momento. Validando se o Nome: <b> ${nomeInput.value} </b> inserido é duplicado.`;

            nomeInput.classList.remove('is-invalid');
            nomeValidate.style.display = 'none';

            $.ajax({
                url: '/Marcas/checkNomeDuplicate',
                type: 'GET',
                data: { nome: nomeInput.value.trim(), idMarca: idMarca },
                success: function (data) {
                    if (data.success) {
                        nomeInput.classList.add('is-valid');
                        nomeValidateDiv.style.display = 'none';
                    } else {
                        nomeValidateDiv.innerHTML = data.message;
                        nomeInput.classList.add('is-invalid');
                    }
                    nomeSpinner.style.display = 'none';
                    nomeInput.disabled = false;
                },
                error: function (error) {
                    console.error('Error fetching items:', error);
                    appendAlertWithoutAnimation(error, 'danger');
                    nomeInput.disabled = false;
                }
            });


        } else {
            nomeInput.classList.add('is-invalid');
            nomeValidate.style.display = 'block';
        }

    });
});

document.getElementById('createMarcaForm').addEventListener('submit', function (event) {
    event.preventDefault();
    var form = event.target;
    var isValid = true;

    var submitBtn = document.getElementById("submitCreateMarca");
    var mainName = document.getElementById("mainSubmitName");
    var loaderName = document.getElementById("submitLoader");

    var nome = document.getElementById('inputNomeMarca');
    var nomeValidate = nome.closest('.col-9').querySelector('.empty-nome-validate');
    var nomeValidateDiv = nome.closest('.col-9').querySelector('.duplicate-nome-validate');

    if (!nome.value) {
        isValid = false;
        nome.classList.add('is-invalid');
        nomeValidate.style.display = 'block';
        nomeValidateDiv.style.display = 'none';
        turnOffLoader();
    } else if (nome.classList.contains('is-invalid')) {
        isValid = false;
        nomeValidate.style.display = 'none';
        nomeValidateDiv.style.display = 'block';
        turnOffLoader();
    } else if (!nome.classList.contains('is-valid')) {
        isValid = false;
        nome.classList.add('is-invalid');
        nomeValidate.style.display = 'none';
        nomeValidateDiv.style.display = 'block';
        nomeValidateDiv.innerText = 'Please verify the inputted name before proceeding on submitting the data.';
        event.preventDefault(); 
        turnOffLoader();
    } else {
        nome.classList.remove('is-invalid');
        nomeValidate.style.display = 'none';
        nomeValidateDiv.style.display = 'none';
    }

    if (isValid) {
        loaderName.style.display = 'block';
        mainName.style.display = 'none';


        form.action = 'CreateMarca'; // Replace with your actual action URL
        form.method = 'POST';

        form.submit();
    } else {
        loaderName.style.display = 'none';
        mainName.style.display = 'block';
    }
});

document.getElementById('editMarcaForm').addEventListener('submit', function (event) {
    event.preventDefault();
    var form = event.target;
    var isValid = true;

    var submitBtn = document.getElementById("submitEditMarca");
    var mainName = document.getElementById("mainSubmitEditName");
    var loaderName = document.getElementById("submitEditLoader");

    var modal = document.getElementById('EditMarca');

    var nome = modal.querySelector('.nome-input-marca');

 /*   var nome = document.getElementById('inputNomeMarca');*/
    var nomeValidate = nome.closest('.col-9').querySelector('.empty-nome-validate');
    var nomeValidateDiv = nome.closest('.col-9').querySelector('.duplicate-nome-validate');

    console.log(nome.value);
    if (!nome.value) {
        isValid = false;
        nome.classList.add('is-invalid');
        nomeValidate.style.display = 'block';
        nomeValidateDiv.style.display = 'none';
        turnOffLoader();
    } else if (nome.classList.contains('is-invalid')) {
        isValid = false;
        nomeValidate.style.display = 'none';
        nomeValidateDiv.style.display = 'block';
        turnOffLoader();
    } else if (!nome.classList.contains('is-valid')) {
        isValid = false;
        nome.classList.add('is-invalid');
        nomeValidate.style.display = 'none';
        nomeValidateDiv.style.display = 'block';
        nomeValidateDiv.innerText = 'Please verify the inputted name before proceeding on submitting the data.';
        event.preventDefault();
        turnOffLoader();
    } else {
        nome.classList.remove('is-invalid');
        nomeValidate.style.display = 'none';
        nomeValidateDiv.style.display = 'none';
    }

    if (isValid) {
        loaderName.style.display = 'block';
        mainName.style.display = 'none';


        form.action = 'EditMarca'; // Replace with your actual action URL
        form.method = 'POST';

        form.submit();
    } else {
        loaderName.style.display = 'none';
        mainName.style.display = 'block';
    }

});

document.getElementById('inactivateMarcaForm').addEventListener('submit', function (event) {
    event.preventDefault();
    var form = event.target;
    var isValid = true;

    var modal = document.getElementById('inactivateMarcaModal');

    var idInactivate = modal.querySelector('input[name="id"]').value;

    console.log(idInactivate);


});



document.getElementById('EditMarca').addEventListener('hidden.bs.modal', function () {
    var idMarcaElement = document.getElementById("idMarca");
    if (idMarcaElement) {
        idMarcaElement.value = null;
    }
});
