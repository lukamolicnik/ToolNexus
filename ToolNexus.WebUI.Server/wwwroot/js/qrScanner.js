let activeScanner = null;
let scannerType = null;
let videoElement = null;
let quaggaRunning = false;

// Naloži Quagga preko script taga
async function loadQuagga() {
    return new Promise((resolve, reject) => {
        if (window.Quagga) {
            resolve();
            return;
        }
        
        const script = document.createElement('script');
        script.src = 'https://cdnjs.cloudflare.com/ajax/libs/quagga/0.12.1/quagga.min.js';
        script.onload = resolve;
        script.onerror = reject;
        document.head.appendChild(script);
    });
}

// Uporabi Quagga za črtne kode
async function startWithQuagga(dotNetRef) {
    try {
        await loadQuagga();
        
        const videoContainer = document.getElementById('qr-video-container');
        
        Quagga.init({
            inputStream: {
                name: "Live",
                type: "LiveStream",
                target: videoContainer, // Uporabi container namesto video elementa
                constraints: {
                    width: { min: 640 },
                    height: { min: 480 },
                    facingMode: "environment",
                    aspectRatio: { min: 1, max: 2 }
                },
                area: { // Omejitev območja skeniranja
                    top: "0%",
                    right: "0%",
                    left: "0%",
                    bottom: "0%"
                }
            },
            frequency: 10, // Kako pogosto poskuša dekodirati
            decoder: {
                readers: [
                    "code_128_reader",
                    "ean_reader",
                    "ean_8_reader",
                    "code_39_reader",
                    "upc_reader",
                    "upc_e_reader"
                ],
                debug: {
                    showCanvas: true,
                    showPatches: false,
                    showFoundPatches: false,
                    showSkeleton: false,
                    showLabels: false,
                    showPatchLabels: false,
                    showRemainingPatchLabels: false,
                    boxFromPatches: {
                        showTransformed: false,
                        showTransformedBox: false,
                        showBB: false
                    }
                }
            },
            locate: true,
            numOfWorkers: 4,
            locator: {
                halfSample: true,
                patchSize: "medium", // Velikost vzorca
                debug: {
                    showCanvas: false,
                    showPatches: false,
                    showFoundPatches: false,
                    showSkeleton: false,
                    showLabels: false,
                    showPatchLabels: false,
                    showRemainingPatchLabels: false,
                    boxFromPatches: {
                        showTransformed: false,
                        showTransformedBox: false,
                        showBB: false
                    }
                }
            }
        }, function(err) {
            if (err) {
                console.error('Quagga init error:', err);
                startWithQrScanner(dotNetRef);
                return;
            }
            
            Quagga.start();
            quaggaRunning = true;
            scannerType = 'quagga';
            console.log('Quagga started successfully');
            
            // Nastavi video element stil
            const video = videoContainer.querySelector('video');
            if (video) {
                video.style.width = '100%';
                video.style.height = 'auto';
            }
        });
        
        let lastCode = '';
        let codeCount = 0;
        
        Quagga.onDetected(function(result) {
            if (result.codeResult && result.codeResult.code) {
                const code = result.codeResult.code;
                
                // Preveri, če je ista koda zaznana vsaj 3x zapored za boljšo zanesljivost
                if (code === lastCode) {
                    codeCount++;
                } else {
                    lastCode = code;
                    codeCount = 1;
                }
                
                if (codeCount >= 3) {
                    console.log('Quagga detected barcode:', code, 'Format:', result.codeResult.format);
                    dotNetRef.invokeMethodAsync('OnCodeDetected', code);
                    
                    // Ponastavi
                    lastCode = '';
                    codeCount = 0;
                    
                    // Ustavi po detekciji
                    stopScanning();
                }
            }
        });
        
    } catch (error) {
        console.error('Error with Quagga, falling back to QrScanner:', error);
        await startWithQrScanner(dotNetRef);
    }
}

// QR Scanner za QR kode
async function startWithQrScanner(dotNetRef) {
    try {
        const QrScanner = (await import('https://unpkg.com/qr-scanner@1.4.2/qr-scanner.min.js')).default;
        
        QrScanner.WORKER_PATH = 'https://unpkg.com/qr-scanner@1.4.2/qr-scanner-worker.min.js';

        activeScanner = new QrScanner(
            videoElement,
            result => {
                console.log('QrScanner detected code:', result.data);
                dotNetRef.invokeMethodAsync('OnCodeDetected', result.data);
            },
            {
                returnDetailedScanResult: true,
                highlightScanRegion: true,
                highlightCodeOutline: true,
                maxScansPerSecond: 5,
                preferredCamera: 'environment'
            }
        );

        await activeScanner.start();
        scannerType = 'qrscanner';
        console.log('QrScanner started successfully');
    } catch (error) {
        console.error('QrScanner error:', error);
        throw error;
    }
}

export async function startScanning(dotNetRef, scanMode = 'auto') {
    try {
        // Počakaj, da se element video pojavi v DOM
        await new Promise(resolve => setTimeout(resolve, 100));
        
        videoElement = document.getElementById('qr-video');
        
        if (!videoElement) {
            console.error('Video element not found');
            return;
        }

        // Izberi skener glede na način
        if (scanMode === 'barcode') {
            await startWithQuagga(dotNetRef);
        } else if (scanMode === 'qr') {
            await startWithQrScanner(dotNetRef);
        } else {
            // Auto mode - poskusi oba
            await startWithQuagga(dotNetRef);
        }
        
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
            errorMessage += error.message;
        }
        
        alert(errorMessage);
    }
}

export function stopScanning() {
    if (scannerType === 'quagga' && window.Quagga) {
        Quagga.stop();
        quaggaRunning = false;
        console.log('Quagga stopped');
    } else if (scannerType === 'qrscanner' && activeScanner) {
        activeScanner.stop();
        activeScanner.destroy();
        activeScanner = null;
        console.log('QrScanner stopped');
    }
    scannerType = null;
}

// Preklopi med načini skeniranja
export async function switchScanMode(dotNetRef, mode) {
    stopScanning();
    await new Promise(resolve => setTimeout(resolve, 100));
    
    if (mode === 'barcode') {
        await startWithQuagga(dotNetRef);
    } else if (mode === 'qr') {
        await startWithQrScanner(dotNetRef);
    } else {
        await startScanning(dotNetRef);
    }
}

// Cleanup when page unloads
window.addEventListener('beforeunload', () => {
    stopScanning();
});