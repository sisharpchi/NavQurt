function companySelected() {
	var e = document.getElementById("companyListSelect");
	var value = e.value;

	let url = `/api/v1/user/replace-claims`;

	postData(url, value).then(t => {
        if (t.ok) {
			location.reload();
        }
	});
}
