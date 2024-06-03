document.addEventListener('DOMContentLoaded', async () => {
  const languageSelect = document.getElementById('language-select');
  let translations = {};

  async function loadTranslations() {
      try {
          const response = await fetch('translations.json');
          if (!response.ok) throw new Error('Failed to load translations');
          translations = await response.json();
          updateUI();
      } catch (error) {
          console.error('Error loading translations:', error);
      }
  }

  function updateUI() {
      const selectedLanguage = languageSelect.value;
      const elements = document.querySelectorAll('[data-translate]');
      elements.forEach(element => {
          const key = element.dataset.translate;
          if (translations[selectedLanguage] && translations[selectedLanguage][key]) {
              element.textContent = translations[selectedLanguage][key];
          }
      });
  }

  languageSelect.addEventListener('change', updateUI);

  loadTranslations();

  const currentPage = window.location.pathname.split('/').pop();
  const navItems = [
      { id: 'profile', page: 'UserProfilePage.html', translate: 'profile' },
      { id: 'backups', page: 'BackupPage.html', translate: 'backups' },
      { id: 'plants', page: 'PlantPage.html', translate: 'plants' },
      { id: 'tasks', page: 'TaskPage.html', translate: 'tasks' },
      { id: 'workers', page: 'WorkersPage.html', translate: 'workers' },
      { id: 'logout', page: 'LoginPage.html', translate: 'logout' }
  ];

  const navList = document.querySelector('.nav ul');
  navItems.forEach(item => {
      const li = document.createElement('li');
      li.id = item.id;
      li.dataset.translate = item.translate;
      const page = item.page === currentPage ? '#' : item.page; // Prevent link to current page
      li.innerHTML = `<a href="${page}">${translations[languageSelect.value][item.translate]}</a>`;
      if (item.page === currentPage) {
          li.classList.add('nav-selected');
      }
      navList.appendChild(li);
  });

  // Additional checks
  const token = localStorage.getItem('token');
  if (!token) {
      // Redirect to login page if token is not present
      window.location.href = 'pages/LoginPage.html';
  }

});