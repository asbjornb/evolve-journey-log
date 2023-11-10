let API_BASE;
if (window.location.protocol === "file:") {
    API_BASE = 'https://localhost:7274/api/player';
} else {
    API_BASE = 'https://evolvejourneylog.azurewebsites.net/api/player';
}

const playerId = localStorage.getItem('playerId');
if (playerId) {
    document.getElementById('registrationSection').style.display = 'none';
    document.getElementById('playerIdInfo').innerText = `Player ID: ${playerId}`;
}

async function registerPlayer() {
    const playerName = prompt("Enter your player name:");
    if (!playerName) return;

    const payload = {
        playerName: playerName
    };

    const response = await fetch(`${API_BASE}/register`, {
        method: 'POST',
        body: JSON.stringify(payload),
        headers: { 'Content-Type': 'application/json' }
    });

    if (!response.ok) {
        const errorMessage = await response.text();
        alert(`Error: ${errorMessage}`);
        return;
    }

    const playerId = await response.json();

    document.getElementById('playerIdInfo').innerText = `Your Player ID is: ${playerId}. Note this GUID somewhere - if you lose it you won't be able to access your data again.`;

    localStorage.setItem('playerId', playerId);  // Store the playerId in local storage

    // Update the UI after registering
    document.getElementById('registrationSection').style.display = 'none';
    document.getElementById('playerIdInfo').innerText = `Player ID: ${playerId}`;
}

async function uploadSaves() {
    const playerId = localStorage.getItem('playerId');
    if (!playerId) {
        alert("Please register first.");
        return;
    }

    const files = document.getElementById('fileInput').files;
    const rawSaveInput = document.getElementById('rawSaveInput').value;

    // Display the spinner
    document.getElementById('uploadSpinner').style.display = 'inline-block';
    document.getElementById('uploadSuccess').style.display = 'none';  // Hide success message

    let uploadSuccess = true;

    try {
        // Upload from the text input
        if (rawSaveInput) {
            await uploadSingleSave(playerId, rawSaveInput);
        }

        // Upload from file inputs
        for (let file of files) {
            const fileContent = await file.text();
            await uploadSingleSave(playerId, fileContent);
        }
    } catch (error) {
        uploadSuccess = false;
        alert(`Error: ${error}`);
    }

    // Hide the spinner and display success message if applicable
    document.getElementById('uploadSpinner').style.display = 'none';
    if (uploadSuccess) {
        document.getElementById('uploadSuccess').style.display = 'block';
    }
}

async function uploadSingleSave(playerId, content) {
    const payload = {
        RawSave: content
    };

    const response = await fetch(`${API_BASE}/${playerId}/uploadSave`, {
        method: 'POST',
        body: JSON.stringify(payload),
        headers: { 'Content-Type': 'application/json' }
    });

    if (!response.ok) {
        const errorMessage = await response.text();
        throw new Error(errorMessage);
    }

    const result = await response.json();
    console.log(result);
}
