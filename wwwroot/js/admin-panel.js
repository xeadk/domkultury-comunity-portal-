async function zapiszWiersz(button) {
    const row = button.closest('tr');
    const table = row.closest('table');
    const entityType = table.dataset.type;
    const id = row.dataset.id;
    const data = {};

    row.querySelectorAll('[data-field]').forEach(cell => {
        data[cell.dataset.field] = cell.innerText.trim();
    });

    const method = id ? 'PUT' : 'POST';
    const url = id ? `/AdminApi/Update/${entityType}/${id}` : `/AdminApi/Create/${entityType}`;

    const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });

    if (response.ok) {
        alert("Zapisano.");
        location.reload();
    } else {
        alert("Błąd zapisu.");
    }
}

async function usunWiersz(button) {
    const row = button.closest('tr');
    const table = row.closest('table');
    const entityType = table.dataset.type;
    const id = row.dataset.id;

    if (!id) {
        row.remove();
        return;
    }

    if (confirm("Na pewno usunąć?")) {
        const response = await fetch(`/AdminApi/Delete/${entityType}/${id}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            row.remove();
            alert("Usunięto.");
        } else {
            alert("Błąd usuwania.");
        }
    }
}

function dodajNowyWiersz(entityType) {
    const table = document.querySelector(`table[data-type='${entityType}'] tbody`);
    const row = document.createElement('tr');
    row.innerHTML = `
        <td contenteditable="true" data-field="Id"></td>
        <td contenteditable="true" data-field="Nazwa"></td>
        <td contenteditable="true" data-field="Opis"></td>
        <td contenteditable="true" data-field="Data"></td>
        <td contenteditable="true" data-field="Organizator"></td>
        <td contenteditable="true" data-field="Lokalizacja"></td>
        <td contenteditable="true" data-field="Status">false</td>
        <td>
            <button class="btn btn-sm btn-success" onclick="zapiszWiersz(this)">Zapisz</button>
            <button class="btn btn-sm btn-danger" onclick="usunWiersz(this)">Usuń</button>
        </td>
    `;
    table.appendChild(row);
}