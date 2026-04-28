function checkWorkerBinding(organizationId) {
    return new Promise(async (resolve, reject) => {
        const response = await fetch(`/api/v1/user/worker-binding?organizationId=${organizationId}`);
        const data = await response.json();
        let resolved = false;

        if (data.isBound) {
            resolve(); // уже привязан — продолжаем
        } else {
            // Показываем модалку
            const select = document.getElementById("workerSelect");
            select.innerHTML = "";
            data.workers.forEach(worker => {
                const option = document.createElement("option");
                option.value = worker.id;
                option.textContent = worker.name;
                select.appendChild(option);
            });

            const modalElement = document.getElementById('workerBindingModal');
            const modal = new bootstrap.Modal(modalElement);

            modalElement.addEventListener('hidden.bs.modal', () => {
                if (!resolved) reject("Модалка закрыта без выбора");

            }, { once: true });

            modal.show();

            $(select).select2({
                dropdownParent: $('#workerBindingModal'),
                width: '100%' // чтобы не было проблем с размерами
            });

            document.getElementById("bindWorkerBtn").onclick = async function () {
                const selectedWorker = select.value;

                const response = await fetch(`/api/v1/user/worker-binding?organizationId=${organizationId}`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ workerId: selectedWorker })
                });

                const result = await response.json();
                if (result.success) {
                    resolved = true;

                    modal.hide();
                    resolve();  
                } else {
                    alert(result.error);
                }
            };
        }
    });
}

