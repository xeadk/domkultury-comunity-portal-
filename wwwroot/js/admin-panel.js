// Pobierz wszystkie pola z nagłówka tabeli (poza "Akcje")
function getFields(table) {
    const ths = table.querySelectorAll('thead th');
    const fields = [];
    ths.forEach(th => {
        const name = th.innerText.trim();
        if (name !== "Akcje" && name !== "") fields.push(name);
    });
    return fields;
}

// Zapisz (dodaj/edytuj) wiersz
async function zapiszWiersz(button) {
    const row = button.closest('tr');
    const table = row.closest('table');
    const entityType = table.dataset.type;
    const id = row.dataset.id;
    const data = {};

    // Pobierz dane z komórek
    row.querySelectorAll('[data-field]').forEach(cell => {
        let value = cell.innerText.trim();
        const field = cell.dataset.field;
        // Skip navigation properties (nie wysyłaj nawigacji)
        if (["Kategoria", "Instruktor", "Uczestnicy", "Wydarzenia", "Zajecia"].includes(field)) return;
        // NIE dodawaj Id jeśli puste (przy tworzeniu)
        if (field === "Id" && (value === "" || value === "0")) return;
        // Konwersja bool
        if (field === "Status") value = value === "true" || value === "1";
        // Konwersja daty
        if (
            field.toLowerCase().includes("data") ||
            field.toLowerCase().includes("termin")
        ) {
            if (value === "") {
                value = null;
            } else {
                // Spróbuj rozpoznać polski format DD.MM.YYYY HH:mm:ss
                let m = value.match(/^(\d{2})\.(\d{2})\.(\d{4})\s+(\d{2}):(\d{2}):(\d{2})$/);
                if (m) {
                    // Zamień na ISO: YYYY-MM-DDTHH:mm:ss
                    value = `${m[3]}-${m[2]}-${m[1]}T${m[4]}:${m[5]}:${m[6]}`;
                }
                let parsed = Date.parse(value);
                value = isNaN(parsed) ? null : new Date(parsed).toISOString();
            }
        }
        // Konwersja liczb całkowitych
        if (["KategoriaId", "InstruktorId", "MaksymalnaLiczbaUczestnikow"].includes(field)) {
            value = value === "" ? null : parseInt(value, 10);
        }
        // Konwersja cena na float
        if (field === "Cena") {
            value = value === "" ? null : parseFloat(value.replace(",", "."));
        }
        data[field] = value;
    });

    // Usuń pole "Id" jeśli to dodawanie (POST)
    if (!id) {
        delete data["Id"];
    }

    const method = id ? 'PUT' : 'POST';
    const url = id ? `/AdminApi/Update/${entityType}/${id}` : `/AdminApi/Create/${entityType}`;

    const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });

    if (response.ok) {
        // Odśwież tylko tabelę (AJAX)
        await odswiezTabele(entityType, table);
        alert("Zapisano.");
    } else {
        alert("Błąd zapisu.");
    }
}

// Usuń wiersz
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
            await odswiezTabele(entityType, table);
            alert("Usunięto.");
        } else {
            alert("Błąd usuwania.");
        }
    }
}

// Dodaj nowy wiersz (puste pola)
function dodajNowyWiersz(entityType) {
    const table = document.querySelector(`table[data-type='${entityType}'] tbody`);
    const fields = getFields(table.closest('table'));
    const row = document.createElement('tr');
    row.innerHTML = fields
        .filter(f => f !== "Id") // pomiń Id
        .map(f => {
            let val = "";
            if (f === "Status") val = "false";
            return `<td contenteditable="true" data-field="${f}">${val}</td>`;
        }).join('') +
        `<td>
        <button class="btn btn-sm btn-success" onclick="zapiszWiersz(this)">Zapisz</button>
        <button class="btn btn-sm btn-danger" onclick="usunWiersz(this)">Usuń</button>
    </td>`;
    table.appendChild(row);
}

// Odśwież tabelę po operacji (AJAX)
async function odswiezTabele(entityType, table) {
    const response = await fetch(`/AdminApi/TablePartial/${entityType}`);
    if (response.ok) {
        const html = await response.text();
        table.parentElement.innerHTML = html;
    }
}
