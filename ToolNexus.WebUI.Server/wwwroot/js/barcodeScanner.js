// Html5-QRCode implementacija
let html5QrCode = null;
let isScanning = false;

// Naloži Html5Qrcode preko script taga
async function loadHtml5Qrcode() {
    return new Promise((resolve, reject) => {
        if (window.Html5Qrcode) {
            resolve();
            return;
        }
        
        const script = document.createElement('script');
        script.src = 'https://unpkg.com/html5-qrcode@2.3.8/html5-qrcode.min.js';
        script.onload = resolve;
        script.onerror = reject;
        document.head.appendChild(script);
    });
}

export async function startScanning(dotNetRef, mode = 'auto') {
    try {
        // Najprej naloži knjižnico
        await loadHtml5Qrcode();
        
        const videoContainer = document.getElementById('qr-video-container');
        if (!videoContainer) {
            console.error('Video container not found');
            return;
        }

        // Ustvari Html5-QRCode instanco
        html5QrCode = new window.Html5Qrcode("qr-video-container");
        
        // Nastavi formate glede na način - uporabi številke namesto enum
        let formatsToSupport = [];
        if (mode === 'barcode') {
            formatsToSupport = [
                4, // CODE_128
                2, // CODE_39
                9, // EAN_13
                10, // EAN_8
                12, // UPC_A
                13, // UPC_E
                6, // CODABAR
                3  // ITF
            ];
        } else if (mode === 'qr') {
            formatsToSupport = [
                0, // QR_CODE
                14, // DATA_MATRIX
                8  // PDF_417
            ];
        } else {
            // Auto - vsi formati (pustimo prazno za vse)
            formatsToSupport = undefined;
        }
        
        const config = {
            fps: 10,
            qrbox: function(viewfinderWidth, viewfinderHeight){
                // Dinamično prilagodi velikost viewbox-a glede na velikost zaslona
                let minEdgePercentage = 0.7; // 70% širine ali višine
                let minEdgeSize = Math.min(viewfinderWidth, viewfinderHeight);
                let qrboxSize = Math.floor(minEdgeSize * minEdgePercentage);
                
                // Za črtne kode uporabi pravokotnik
                return {
                    width: Math.min(qrboxSize, 350),
                    height: Math.min(qrboxSize * 0.5, 175) // Polovična višina
                };
            },
            aspectRatio: 1.777 // 16:9 razmerje
        };
        
        // Dodaj formate samo če so definirani
        if (formatsToSupport) {
            config.formatsToSupport = formatsToSupport;
        }
        
        const successCallback = (decodedText, decodedResult) => {
            console.log(`Scanned: ${decodedText}`, decodedResult);
            dotNetRef.invokeMethodAsync('OnCodeDetected', decodedText);
            stopScanning();
        };
        
        const errorCallback = (errorMessage) => {
            // Ignoriraj rutinske napake skeniranja
            if (!errorMessage.includes("NotFoundException") && !errorMessage.includes("No MultiFormat Readers")) {
                console.log(`Scan error: ${errorMessage}`);
            }
        };
        
        // Začni skeniranje
        await html5QrCode.start(
            { facingMode: "environment" }, // Zadnja kamera
            config,
            successCallback,
            errorCallback
        );
        
        isScanning = true;
        console.log('Html5-QRCode scanner started successfully');
        
    } catch (error) {
        console.error('Error starting scanner:', error);
        
        let errorMessage = 'Napaka pri zagonu skenerja: ';
        if (error.name === 'NotAllowedError') {
            errorMessage += 'Dostop do kamere ni dovoljen. Prosim, dovolite dostop do kamere v nastavitvah brskalnika.';
        } else if (error.name === 'NotFoundError') {
            errorMessage += 'Kamera ni bila najdena. Preverite, ali ima vaša naprava kamero.';
        } else if (error.name === 'NotReadableError') {
            errorMessage += 'Kamera je že v uporabi v drugi aplikaciji.';
        } else {
            errorMessage += error.message || error;
        }
        
        alert(errorMessage);
    }
}

export async function stopScanning() {
    if (html5QrCode && isScanning) {
        try {
            await html5QrCode.stop();
            html5QrCode.clear();
            console.log('Scanner stopped');
        } catch (error) {
            console.error('Error stopping scanner:', error);
        }
        isScanning = false;
    }
}

// Cleanup when page unloads
window.addEventListener('beforeunload', () => {
    stopScanning();
});