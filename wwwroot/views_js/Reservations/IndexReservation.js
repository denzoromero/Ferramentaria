console.log('working js');

/*console.log(testJsValue); */


const codeReader = new ZXing.BrowserQRCodeReader();
const hints = new Map();
hints.set(ZXing.DecodeHintType.TRY_HARDER, true);
hints.set(ZXing.DecodeHintType.PURE_BARCODE, true);

let scanning = false;
let scanInterval;
let videoStream;

document.getElementById('qrCodeModal').addEventListener('shown.bs.modal', function () {
    showVideo();
    scanInterval = setInterval(startScanning, 1000);
    console.log('Scanning started');
});

document.getElementById('qrCodeModal').addEventListener('hidden.bs.modal', function () {
    hideVideo();
    stopScanning();
    console.log('hide');
});

function showVideo() {

    document.getElementById('video-container').style.display = 'block';
    document.getElementById('chaveInput').value = "";

}

function hideVideo() {
    document.getElementById('video-container').style.display = 'none';
}

function startScanning() {
    if (!scanning) {
        scanning = true;
        codeReader.decodeFromVideoDevice(null, 'webcam-preview', (result, err) => {
            if (result) {
                console.log('Found QR code!', result.text);
                // document.getElementById('result').textContent = result.text;

                // Stop scanning immediately after finding a QR code
                stopScanning();
                hideVideo();
                /*getReservedItem(result.text);*/

            }
            if (err) {
                if (err instanceof ZXing.NotFoundException) {
                    console.log('No QR code found.');
                }
                if (err instanceof ZXing.ChecksumException) {
                    console.log('A code was found, but its read value was not valid.');
                }
                if (err instanceof ZXing.FormatException) {
                    console.log('A code was found, but it was in an invalid format.');
                }
            }
        }, hints).then((stream) => {
            videoStream = stream; // Store the stream so we can stop it later
        }).catch(err => {
            appendAlertWithoutAnimation(err, 'danger');
            console.error('error here:', err);
        });
    }
}

function stopScanning() {
    clearInterval(scanInterval); // Stop repeated scanning
    scanning = false;
    console.log('Scanning stopped');

    // Stop the QR code reader
    codeReader.reset(); // This stops the scanning process

    // Stop video stream
    if (videoStream) {
        videoStream.getTracks().forEach(track => track.stop());
        videoStream = null; // Ensure the reference is cleared
    }
}

function getReservedItem(itemId) {

    $.ajax({
        url: '/PrepareReservation/getItemDetail',
        type: 'GET',
        data: { id: itemId },
        success: function (data) {
            if (data.success) {

                window.location.href = `/PrepareReservation/ProcessItem?id=${itemId}`;

            } else {
                appendAlert(data.message, 'danger');
            }
        },
        error: function (error) {
            console.error('Error fetching items:', error);
            appendAlertWithoutAnimation(error, 'danger');
        }
    });
}

document.getElementById("chaveInput").addEventListener("keydown", function (event) {
    if (event.key === "Enter") {
        stopScanning();
        hideVideo();
        const chaveKey = document.getElementById("chaveInput").value;
        getReservedItem(chaveKey);    
    }
});


