
console.log(passeddata);



document.getElementById('TransferButton').disabled = passeddata.listFerramentaria.length > 0 ? false : true;


const selectElement = document.getElementById('transferFerramentariaSelect');


if (passeddata.listFerramentaria.length > 0) {
    document.getElementById('TransferButton').disabled = false;
    document.getElementById('TransferButton').style.display = 'block';

    selectElement.innerHTML = '<option value="">Selecionar...</option>'; 

    passeddata.listFerramentaria.forEach(ferr => {
        const option = document.createElement('option');
        option.value = ferr.id;
        option.textContent = ferr.nome;
        selectElement.appendChild(option);
    });

    selectElement.classList.add('is-invalid');
    document.getElementById('obsTransferTextarea').classList.add('is-invalid');

} else {
    document.getElementById('TransferButton').style.display = 'none';
    document.getElementById('TransferButton').disabled = true;
}



document.getElementById('TransferButton').addEventListener('click', function () {

    var transferDiv = document.getElementById('TransferDiv');
    if (transferDiv.style.display === 'block') {
        transferDiv.style.display = 'none';
        showProductDetails();
    } else {
        hideCancel();
        cleanTransfer();
        hideProductDetails();
        transferDiv.style.display = 'block';
    }

});

document.getElementById('CancelButton').addEventListener('click', function () {

    //var modal = document.getElementById('promptModal');
    //var modalInstance = new bootstrap.Modal(modal);
    //modal.setAttribute('data-bs-instance', modalInstance); // Store the instance in a data attribute
    //modalInstance.show();

    //var modaltitle = modal.querySelector('.modal-title');
    //modaltitle.innerHTML = `<b> Cancel Reservation </b>`;

    //var modalbody = modal.querySelector('.modal-body');
    //modalbody.innerHTML = `<p class='text-danger font-weight-bold'>Tem certeza de que deseja cancelar esta reserva?</p>`;


    //var modalYesButton = modal.querySelector('.modal-footer .btn-success');
    //modalYesButton.setAttribute('data-action', 'Cancel');


    var cancelDiv = document.getElementById('CancelDiv');
    if (cancelDiv.style.display === 'block') {
        cancelDiv.style.display = 'none';
        showProductDetails();
    } else {
        hideTransfer();
        cleanCancel();
        hideProductDetails();
        cancelDiv.style.display = 'block';
    }

});

//document.querySelector('#promptModal .modal-footer .btn-success').addEventListener('click', function () {
//    // Your code here
//    var modal = document.getElementById('promptModal');

//    var modalInstance = bootstrap.Modal.getInstance(modal);

//    var attributevalue = modal.querySelector('.modal-footer .btn-success').getAttribute('data-action');

//    console.log(attributevalue);

//    if (attributevalue === "Cancel") {
//        var cancelDiv = document.getElementById('CancelDiv');
//        if (cancelDiv.style.display === 'block') {
//            cancelDiv.style.display = 'none';
//        } else {
//            hideTransfer();
//            cleanCancel();
//            hideProductDetails();
//            cancelDiv.style.display = 'block';
//        }
//    }

//    modalInstance.hide();

//});

function showProductDetails() {
    var attributevalue = document.getElementById('productList').getAttribute('data-multiple');

    var productInfoDiv = document.getElementById('productInformation');
    productInfoDiv.style.display = 'block';

    if (attributevalue === "yes") {
        var productListDiv = document.getElementById('productList');
        productListDiv.style.display = 'block';
    }

    var attributeCAvalue = document.getElementById('caList').getAttribute('data-withca');
    if (attributeCAvalue === "yes") {
        document.getElementById('caList').style.display = 'block';
    }
}

function hideProductDetails() {

    var attributevalue = document.getElementById('productList').getAttribute('data-multiple');

    var productInfoDiv = document.getElementById('productInformation');
    productInfoDiv.style.display = 'none';

    if (attributevalue === "yes") {
        var productListDiv = document.getElementById('productList');
        productListDiv.style.display = 'none';
        document.getElementById('idProdutoHidden').value = "";
        document.getElementById('afLabel').textContent = "";
        document.getElementById('patLabel').textContent = "";
        document.getElementById('qtyLabel').textContent = "";
        document.getElementById('dataVencimentoLabel').textContent = "";
        document.getElementById('dataPrevistaInput').value = "";
        document.getElementById('observacaoTextarea').value = "";
    }

    var attributeCAvalue = document.getElementById('caList').getAttribute('data-withca');
    if (attributeCAvalue === "yes") {
        document.getElementById('caList').style.display = 'none';
    }

}


function hideTransfer() {
    document.getElementById('TransferDiv').style.display = 'none';
}

function cleanTransfer() {

    selectElement.value = "";
    selectElement.classList.add('is-invalid');

    var textarea = document.getElementById('TransferDiv').querySelector('.ObsArea .text-area');
    textarea.value = "";
    textarea.classList.add('is-invalid');

}


function hideCancel() {
    document.getElementById('CancelDiv').style.display = 'none';
}

function cleanCancel() {
    var textarea = document.getElementById('CancelDiv').querySelector('.ObsArea .text-area');
    textarea.value = "";
    textarea.classList.add('is-invalid');
}




document.getElementById('transferFerramentariaSelect').addEventListener('change', function () {
/*    var selectElement = document.getElementById('transferFerramentariaSelect');*/

    if (selectElement.value !== "") {
        selectElement.classList.remove('is-invalid');
        selectElement.classList.add('is-valid');
    } else {
        selectElement.classList.remove('is-valid');
        selectElement.classList.add('is-invalid');
    }

});

document.querySelectorAll('.text-area').forEach(function (element) {
    element.addEventListener('input', function (event) {
        if (element.value.trim() !== '') {
            element.classList.add('is-valid');
            element.classList.remove('is-invalid');
        } else {
            element.classList.add('is-invalid');
            element.classList.remove('is-valid');
        }
    });
});



if (passeddata.listProduto.length == 1) {
    document.getElementById('productList').style.display = 'none';
    document.getElementById('productList').setAttribute('data-multiple', 'no');
    selectedProduct(passeddata.listProduto[0]);
} else if (passeddata.listProduto.length > 1) {
    document.getElementById('productInformation').style.display = 'none';
    document.getElementById('productList').setAttribute('data-multiple', 'yes');
    makeProductTable(passeddata.listProduto);
} else {

}

function selectedProduct(item) {

    document.getElementById('idProdutoHidden').value = parseInt(item.id,10);

    document.getElementById('afLabel').textContent = item.af;
    document.getElementById('patLabel').textContent = item.pat;
    document.getElementById('qtyLabel').textContent = item.quantidade;

    if (item.dataVencimento != null) {
        document.getElementById('dataVencimentoLabel').textContent = formatDateToDDMMYYYYLabels(item.dataVencimento);
    } else {
        document.getElementById('dataVencimentoLabel').textContent = "";
    }

    const classeCatalog = document.getElementById('classeLabel').textContent;
    const typeCatalog = document.getElementById('typeLabel').textContent;

    if (classeCatalog == "Ferramenta") {

        if (typeCatalog == "PorAferido" && item.dataVencimento != null) {
            document.getElementById('dataPrevistaInput').value = formatDateForInputs(item.dataVencimento);
        } else {
            if (item.dataPrevistaDevolucao != null) {
                document.getElementById('dataPrevistaInput').value = formatDateForInputs(item.dataPrevistaDevolucao);
            } else {
                document.getElementById('dataPrevistaInput').value = "";
            }
        }

    } else if (classeCatalog == "EPI") {

        if (item.dataPrevistaDevolucao != null) {
            document.getElementById('dataPrevistaInput').value = formatDateForInputs(item.dataPrevistaDevolucao);
            document.getElementById('dataPrevistaInput').disabled = true;
        } else {
            document.getElementById('dataPrevistaInput').value = "";
        }

    } else {
        document.getElementById('dataPrevistaInput').value = "";
        document.getElementById('dataPrevistaInput').disabled = true;
    }

    document.getElementById('productList').style.display = 'none';
    document.getElementById('productInformation').style.display = 'block';

    appendAlert(`Selected the product with AF:${item.af ?? ''}  PAT:${item.pat ?? ''} successfully`, 'success');

}

function makeProductTable(products) {

    const tbody = document.getElementById("productListTable").querySelector("tbody");
    tbody.innerHTML = ''; // Clear existing rows

    products.forEach((item, index) => {
        const row = document.createElement('tr');
        row.id = item.id;

        if (item.allowedtoborrow == false) {
            row.style.backgroundColor = 'red';
        }

        const selectCell = document.createElement('td');
        selectCell.className = 'text-center align-middle';
        const selectLink = document.createElement("a");
        selectLink.href = "#";
        selectLink.innerText = "Select";
        selectLink.onclick = function (event) {
            event.preventDefault(); 
            selectedProduct(item); // Assuming message.id is the identifier
        };

        selectCell.appendChild(selectLink);

        const afCell = document.createElement('td');
        afCell.className = 'text-center align-middle AFcolumn';
        afCell.textContent = item.af;

        const patCell = document.createElement('td');
        patCell.className = 'text-center align-middle PATcolumn';
        patCell.textContent = item.pat;

        const qtyCell = document.createElement('td');
        qtyCell.className = 'text-center align-middle';
        qtyCell.textContent = item.quantidade;

        const dataVencimentoCell = document.createElement('td');
        dataVencimentoCell.className = 'text-center align-middle';
        if (item.dataVencimento !== null) {
            dataVencimentoCell.textContent = formatDateToDDMMYYYYLabels(item.dataVencimento);
        }



        const reasonCell = document.createElement('td');

        const img1 = document.createElement('img');
        img1.src = '/img/chat-dots.svg';
        img1.className = 'SwitchingIcons';
        img1.alt = 'dots';

        const popoverLink = document.createElement("a");
        popoverLink.setAttribute("href", "#"); // Avoid jumping to top of page
        popoverLink.setAttribute("data-bs-toggle", "popover");
        popoverLink.setAttribute("data-bs-placement", "right");
        popoverLink.setAttribute("data-bs-title", "Reason");
        popoverLink.setAttribute("data-bs-trigger", "hover");
        if (item.reason != null) {
            popoverLink.setAttribute("data-bs-content", item.reason);
        }
        popoverLink.appendChild(img1);
        reasonCell.appendChild(popoverLink);
        // Initialize the popover manually
        const popover = new bootstrap.Popover(popoverLink, {
            trigger: "hover",
            placement: "right"
        });

        if (item.allowedtoborrow == false) {
            row.style.backgroundColor = '#FA7268';
            selectLink.style.display = 'none';
            popoverLink.style.display = 'block';
        } else {
            popoverLink.style.display = 'none';
        }

        row.appendChild(selectCell);
        row.appendChild(afCell);
        row.appendChild(patCell);
        row.appendChild(qtyCell);
        row.appendChild(dataVencimentoCell);
        row.appendChild(reasonCell);
        tbody.appendChild(row);


    });

}


document.getElementById('searchProduct').addEventListener('keyup', function () {
    const searchValue = this.value.toLowerCase();
    const rows = document.querySelectorAll('#productListTable tbody tr');

    rows.forEach(row => {
        const patCell = row.querySelector('.PATcolumn');
        const afCell = row.querySelector('.AFcolumn');

        const patText = patCell ? patCell.textContent.toLowerCase() : '';
        const afText = afCell ? afCell.textContent.toLowerCase() : '';

        if (patText.includes(searchValue) || afText.includes(searchValue)) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
});

function checkProductDiv() {

}


if (passeddata.intClasse === 2) {
    if (passeddata.listCA.length > 0) {
        document.getElementById('caList').style.display = 'block';
        document.getElementById('caList').setAttribute('data-withca', 'yes');
        populateCAList(passeddata.listCA);
    } else {
        document.getElementById('caList').style.display = 'none';
        document.getElementById('caList').setAttribute('data-withca', 'no');
    }
} else {
    document.getElementById('caList').style.display = 'none';
    document.getElementById('caList').setAttribute('data-withca', 'no');
}

function populateCAList(caList) {

    const divbody = document.getElementById("caListBody");
    divbody.innerHTML = '';

    caList.forEach((item, index) => {

        const caDivBody = document.createElement('div');
        caDivBody.className = 'inline-checkbox';

        const selectbox = document.createElement('input');
        selectbox.type = 'radio';
        selectbox.className = 'form-check-input';
        selectbox.name = 'IdControleCA';
        selectbox.value = item.id;


        // // Set the first radio button to be checked by default
        if (index === 0) {
            selectbox.checked = true;
        }


        const labelradio = document.createElement('label');
        labelradio.className = 'form-check-label';
        labelradio.innerHTML = `<b>CA:</b> ${item.numeroCA}  -  <b>Validade:</b> ${formatDateToDDMMYYYYLabels(item.validade)}`;

        caDivBody.appendChild(selectbox);
        caDivBody.appendChild(labelradio);
        divbody.appendChild(caDivBody);
    });

}


document.getElementById('processReservationForm').addEventListener('submit', function (event) {

    event.preventDefault();
    var form = event.target;
    var isValid = true;

    var idReservation = document.getElementById('idReservationHidden').value;
    console.log(idReservation);
    if (!idReservation) {
        isValid = false;
        appendAlertWithoutAnimation('IdReservation is empty. Please refresh the page or find the Reservation Id First.', 'warning');
        turnOffLoader();
        event.preventDefault(); 
    }

    var idProduto = document.getElementById('idProdutoHidden').value;
    console.log(idProduto);
    if (!idProduto) {
        isValid = false;
        appendAlertWithoutAnimation('No Product is Selected', 'warning');
        turnOffLoader();
        event.preventDefault(); 
    }

    if (passeddata.intClasse === 2) {
        if (passeddata.listCA.length > 0) {
            var idCA = document.querySelector('input[name="IdControleCA"]:checked').value;
            console.log(idCA);
            if (!idCA) {
                isValid = false;
                appendAlertWithoutAnimation('Item is EPI with list of CA, please select the CA first.', 'warning');
                turnOffLoader();
                event.preventDefault(); 
            }
        }   
    }

    var dataprevista = document.getElementById('dataPrevistaInput').value;
    console.log(dataprevista);
    if (dataprevista) {
        var inputDate = new Date(dataprevista);
        var today = new Date();

        // Set the time of today's date to midnight for accurate comparison
        today.setHours(0, 0, 0, 0);

        if (inputDate < today) {
            isValid = false;
            appendAlertWithoutAnimation(`The inputted date ${formatDateToDDMMYYYYLabels(inputDate)} cannot be less than today.`, 'warning');
            document.getElementById('dataPrevistaInput').value = "";
            turnOffLoader();
            event.preventDefault(); 
        } 
    }

    if (isValid) {

        const formContainer = document.getElementById('hiddenInputsContainerReservation');
        formContainer.innerHTML = '';

        formContainer.appendChild(makeHiddenInputForSubmission('IdReservation', idReservation));

        if (dataprevista) {
            formContainer.appendChild(makeHiddenInputForSubmission('dataPrevista', dataprevista));
        }

        if (idCA) {
            formContainer.appendChild(makeHiddenInputForSubmission('IdControleCA', idCA));
        }
     
        /*formContainer.appendChild(makeHiddenInputForSubmission('IdReservation', idReservation));*/

        form.action = 'proceedReservation'; // Replace with your actual action URL
        form.method = 'POST';

        form.submit();

    } 

});

document.getElementById('processCancellationForm').addEventListener('submit', function (event) {

    event.preventDefault();
    var form = event.target;
    var isValid = true;

    var idReservation = document.getElementById('idReservationHidden').value;
    console.log(idReservation);
    if (!idReservation) {
        isValid = false;
        appendAlertWithoutAnimation('IdReservation is empty. Please refresh the page or find the Reservation Id First.', 'warning');
        turnOffLoader();
        event.preventDefault(); 
    }

    var cancelTextArea = document.getElementById('cancelTextarea');
    var textAreaInput = cancelTextArea.value;
    console.log(textAreaInput);
    if (!cancelTextArea.classList.contains('is-valid')) {
        isValid = false;
        turnOffLoader();
        event.preventDefault(); 
    }

    if (isValid) {
        const formContainer = document.getElementById('hiddenInputsContainerCancellation');
        formContainer.innerHTML = '';

        formContainer.appendChild(makeHiddenInputForSubmission('IdReservation', idReservation));

        form.action = 'proceedCancellation'; // Replace with your actual action URL
        form.method = 'POST';

        form.submit();

    }


});


document.getElementById('processTransferForm').addEventListener('submit', function (event) {

    event.preventDefault();
    var form = event.target;
    var isValid = true;



    var idReservation = document.getElementById('idReservationHidden').value;
    console.log(idReservation);
    if (!idReservation) {
        isValid = false;
        appendAlertWithoutAnimation('IdReservation is empty. Please refresh the page or find the Reservation Id First.', 'warning');
        turnOffLoader();
        event.preventDefault();
    }

    var selectFerramentaria = document.getElementById('transferFerramentariaSelect');
    var idFerramentariaToTransfer = selectFerramentaria.value;
    console.log(idFerramentariaToTransfer);
    if (!idFerramentariaToTransfer) {
        isValid = false;
        event.preventDefault(); 
        selectFerramentaria.classList.add('is-invalid');
        turnOffLoader();
    }

    var cancelTextArea = document.getElementById('obsTransferTextarea');
    var textAreaInput = cancelTextArea.value;
    console.log(textAreaInput);
    if (!cancelTextArea.classList.contains('is-valid')) {
        isValid = false;
        turnOffLoader();
        event.preventDefault();
    }

    var ferramentariaOrigin = passeddata.idFerramentaria;
    console.log(ferramentariaOrigin);
    if (!ferramentariaOrigin) {
        isValid = false;
        appendAlertWithoutAnimation('ferramentariaOrigin is empty.', 'warning');
        turnOffLoader();
        event.preventDefault();
    }



    if (isValid) {

        const formContainer = document.getElementById('hiddenInputsContainerTransfer');
        formContainer.innerHTML = '';

        formContainer.appendChild(makeHiddenInputForSubmission('IdReservation', idReservation));
        formContainer.appendChild(makeHiddenInputForSubmission('IdFerramentariaOrigin', idReservation));


        form.action = 'proceedTransfer'; // Replace with your actual action URL
        form.method = 'POST';

        form.submit();

    }


});












