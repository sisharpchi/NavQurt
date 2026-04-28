
async function postData(url = "", data = {}) {
    // Default options are marked with *
    return await fetch(url, {
        method: "POST", // *GET, POST, PUT, DELETE, etc.
        mode: "cors", // no-cors, *cors, same-origin
        cache: "no-cache", // *default, no-cache, reload, force-cache, only-if-cached
        credentials: "same-origin", // include, *same-origin, omit
        headers: {
            "Content-Type": "application/json",
            "ApiKey":"69fcced8fd87ad8641d07664fb3b5a88358af4b56bcdfd1ee8285064f6d3ce6a"
            // 'Content-Type': 'application/x-www-form-urlencoded',
        },
        redirect: "follow", // manual, *follow, error
        referrerPolicy: "no-referrer", // no-referrer, *client
        body: JSON.stringify(data), // body data type must match "Content-Type" header
    });
    
}

 function disableElement(id,disable){
     let element = document.getElementById(id);
     if (element !== null && element !== undefined)
         element.disabled = disable;
}

function onSignalRMessageReciever(data, successMessage) {
	let containerMessage = document.getElementById("statusMessageContainer");
	if (!data["success"]) {
		containerMessage.innerText = data["errorMessage"];
		containerMessage.setAttribute("class", "bg-danger p-2");
	}
	else {
        containerMessage.innerText = successMessage;
		containerMessage.setAttribute("class", "bg-success p-2");
	}
	document.getElementById("statusSpinner").setAttribute("class", "d-none");
}

function resetStatusContainer() {
	document.getElementById("statusSpinner").setAttribute("class", "");
	let containerMessage = document.getElementById("statusMessageContainer");
	containerMessage.innerText = "В ожидании...";
	containerMessage.setAttribute("class", "bg-secondary p-2");
}
function displayStatusContainer() {
	document.getElementById("statusContainer").setAttribute("class", "");
}
function hideStatusContainer() {
	document.getElementById("statusContainer").setAttribute("class", "d-none");
}