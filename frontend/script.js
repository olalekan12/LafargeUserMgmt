// Adjust to your API origin
const API = 'http://localhost:5169/api/users';

document.getElementById('createForm').addEventListener('submit', async (e) => {
  e.preventDefault();
  const form = e.target;
  const data = new FormData(form);
  const res = await fetch(API, { method: 'POST', body: data });
  const json = await res.json();
  alert('Created user #' + json.id);
  form.reset();
});

document.getElementById('btnSearch').addEventListener('click', async () => {
  const q = document.getElementById('q').value;
  const res = await fetch(API + '?q=' + encodeURIComponent(q));
  const json = await res.json();
  const results = document.getElementById('results');
  results.innerHTML = '';
  json.items.forEach(u => {
    const div = document.createElement('div');
    div.className = 'card';
    div.innerHTML = `<strong>${u.firstName} ${u.lastName}</strong> (${u.role})<br>
      ${u.email} | ${u.phone || ''}<br>
      ${u.pictureUrl ? '<img src="' + API.replace('/api/users','') + u.pictureUrl + '">' : ''}
      <div class="actions">
        <button data-id="${u.id}" class="delete">Delete</button>
      </div>`;
    results.appendChild(div);
  });

  document.querySelectorAll('.delete').forEach(btn => {
    btn.addEventListener('click', async () => {
      const id = btn.getAttribute('data-id');
      if (confirm('Delete user #' + id + '?')) {
        await fetch(API + '/' + id, { method: 'DELETE' });
        btn.closest('.card').remove();
      }
    });
  });
});
