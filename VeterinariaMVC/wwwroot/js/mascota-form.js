document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('formMascota');
    if (!form) return;

    const selectEspecie = document.getElementById('EspecieId');
    const selectRaza = document.getElementById('RazaId');
    const inputCedula = document.getElementById('buscarCedulaDueno');
    const inputDuenoId = document.getElementById('DuenoId');
    const cajaDueno = document.getElementById('duenoSeleccionado');
    const avisoNoEncontrado = document.getElementById('duenoNoEncontrado');
    const modalEl = document.getElementById('modalCrearDueno');
    const formModal = document.getElementById('formCrearDueno');
    const errorModal = document.getElementById('errorModalDueno');

    const seleccionarDueno = (dueno) => {
        inputDuenoId.value = dueno.id;
        inputCedula.value = dueno.cedula;
        document.getElementById('textoDuenoNombre').textContent = dueno.nombre;
        document.getElementById('textoDuenoCedula').textContent = `(Céd. ${dueno.cedula})`;
        cajaDueno.classList.remove('d-none');
        avisoNoEncontrado.classList.add('d-none');
    };

    selectEspecie.addEventListener('change', async () => {
        const especieId = parseInt(selectEspecie.value, 10) || 0;
        selectRaza.innerHTML = '<option value="0">Seleccione una raza...</option>';
        selectRaza.disabled = true;
        if (especieId <= 0) return;

        const res = await fetch(`${form.dataset.urlRazas}?especieId=${especieId}`);
        if (res.ok) {
            const razas = await res.json();
            razas.forEach(r => selectRaza.add(new Option(r.nombre, r.id)));
            selectRaza.disabled = false;
        }
    });

    const buscarDueno = async () => {
        const cedula = inputCedula.value.trim();
        if (!cedula) return;

        const res = await fetch(`${form.dataset.urlBuscarDueno}?cedula=${encodeURIComponent(cedula)}`);
        if (res.ok) {
            seleccionarDueno(await res.json());
        } else {
            inputDuenoId.value = '0';
            cajaDueno.classList.add('d-none');
            avisoNoEncontrado.classList.remove('d-none');
            formModal.elements['Cedula'].value = cedula;
        }
    };

    document.getElementById('btnBuscarDueno').addEventListener('click', buscarDueno);
    inputCedula.addEventListener('keydown', (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            buscarDueno();
        }
    });

    modalEl.addEventListener('show.bs.modal', () => {
        errorModal.classList.add('d-none');
        if (inputCedula.value.trim() && !formModal.elements['Cedula'].value.trim()) {
            formModal.elements['Cedula'].value = inputCedula.value.trim();
        }
    });

    formModal.addEventListener('submit', async (e) => {
        e.preventDefault();
        errorModal.classList.add('d-none');

        const payload = Object.fromEntries(new FormData(formModal));
        const token = form.querySelector('input[name="__RequestVerificationToken"]').value;

        const res = await fetch(form.dataset.urlCrearDueno, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token },
            body: JSON.stringify(payload)
        });

        const datos = await res.json();
        if (!res.ok) {
            errorModal.textContent = datos.mensaje || 'No se pudo registrar el dueño.';
            errorModal.classList.remove('d-none');
            return;
        }

        seleccionarDueno(datos);
        formModal.reset();
        bootstrap.Modal.getInstance(modalEl).hide();
    });
});