// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function validateField(input, type) {
    const errorElement = document.getElementById(`${input.id}-error`);

    // נקה שגיאות קודמות
    input.classList.remove('invalid');
    errorElement.style.display = 'none';

    // בדוק אם השדה ריק
    if (!input.value.trim()) {
        showError(input, errorElement, 'שדה זה הוא חובה');
        return false;
    }

    // בדיקות ספציפיות לכל סוג שדה
    switch (type) {
        case 'name':
            if (input.value.length < 2) {
                showError(input, errorElement, 'השם חייב להכיל לפחות 2 תווים');
                return false;
            }
            break;

        case 'username':
            if (input.value.length < 4) {
                showError(input, errorElement, 'שם משתמש חייב להכיל לפחות 4 תווים');
                return false;
            }
            break;

        case 'email':
            if (!input.value.toLowerCase().endsWith('gmail.com')) {
                showError(input, errorElement, 'נא להזין כתובת gmail בלבד');
                return false;
            }
            break;

        case 'password':
            if (input.value.length < 6) {
                showError(input, errorElement, 'סיסמה חייבת להכיל לפחות 6 תווים');
                return false;
            }
            break;
    }

    return true;
}

function showError(input, errorElement, message) {
    input.classList.add('invalid');
    errorElement.textContent = message;
    errorElement.style.display = 'block';
}

function validateLoginForm() {
    let isValid = true;

    isValid = validateField(document.getElementById('loginUsername'), 'username') && isValid;
    isValid = validateField(document.getElementById('loginPassword'), 'password') && isValid;

    return isValid;
}

function validateRegisterForm() {
    let isValid = true;

    isValid = validateField(document.getElementById('fName'), 'name') && isValid;
    isValid = validateField(document.getElementById('lName'), 'name') && isValid;
    isValid = validateField(document.getElementById('uName'), 'username') && isValid;
    isValid = validateField(document.getElementById('email'), 'email') && isValid;
    isValid = validateField(document.getElementById('password'), 'password') && isValid;

    if (isValid) {
        showThankYouMessage(
            document.getElementById('fName').value,
            document.getElementById('lName').value,
            document.getElementById('uName').value,
            document.getElementById('email').value,
            document.querySelector('input[name="gender"]:checked').value
        );
    }

    return false;
}

function showForm(formType) {
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');
    const thankYouSection = document.getElementById('thankYouSection');

    document.querySelectorAll('.error-message').forEach(error => error.style.display = 'none');
    document.querySelectorAll('input').forEach(input => input.classList.remove('invalid'));

    thankYouSection.classList.add('hidden');

    if (formType === 'login') {
        loginForm.classList.remove('hidden');
        registerForm.classList.add('hidden');
    } else {
        registerForm.classList.remove('hidden');
        loginForm.classList.add('hidden');
    }
}

function showThankYouMessage(fName, lName, uName, email, gender) {
    document.getElementById('registerForm').classList.add('hidden');
    document.getElementById('tableFName').textContent = fName;
    document.getElementById('tableLName').textContent = lName;
    document.getElementById('tableUName').textContent = uName;
    document.getElementById('tableEmail').textContent = email;
    document.getElementById('tableGender').textContent = gender;
    document.getElementById('thankYouSection').classList.remove('hidden');
}