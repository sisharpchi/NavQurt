
$(document).ready(documentLoaded);

function documentLoaded() {
    document.getElementById("companyListSelect").addEventListener("change", companySelected);
}
function companySelected() {

    let companyToken = document.getElementById("companyListSelect").value;

	let url = "/api/v1/user/replace-claims";


	postData(url, companyToken).then(t => {
		if (t["status"] == 200) {
			window.location.reload();
		}
		console.log(t);
	});
}

