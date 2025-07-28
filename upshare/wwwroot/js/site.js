function handlePasswordResetRequest() {
  const hash = window.location.hash.substring(1);
  const params = new URLSearchParams(hash);

  const accessToken = params.get("access_token");
  const type = params.get("type");
  console.log("Access Token:", accessToken);
  console.log("Type:", type);

  if (accessToken && type === "recovery") {
    window.location.href = `/Login/EnterNewPassword?access_token=${encodeURIComponent(
      accessToken
    )}&type=${encodeURIComponent(type)}`;
  } else if (type === "signup") {
    window.location.href = `/Login/SignUpConfirmSuccess`;
  } else {
    window.location.href = "/Login/Auth";
  }
}

document.addEventListener("DOMContentLoaded", function () {
  if (document.body.classList.contains("request-handling-page")) {
    handlePasswordResetRequest();
  }
});

function toggleEditMode() {
  const viewModeElements = document.querySelectorAll(".view-mode");
  const editModeElements = document.querySelectorAll(".edit-mode");
  console.log("Edit mode activated, reverting to view mode.");

  viewModeElements.forEach((el) => el.classList.add("d-none"));
  editModeElements.forEach((el) => el.classList.remove("d-none"));
}

function cancelEdit() {
  const viewModeElements = document.querySelectorAll(".view-mode");
  const editModeElements = document.querySelectorAll(".edit-mode");

  editModeElements.forEach((el) => el.classList.add("d-none"));
  viewModeElements.forEach((el) => el.classList.remove("d-none"));
  console.log("Edit cancelled, reverting to view mode.");

  document.getElementById("profileForm").reset();

  const originalSrc = "@imageUrl";
  document.getElementById("profile-preview").src = originalSrc;
}

document.addEventListener('DOMContentLoaded', function() {
    const profileImageInput = document.getElementById('profileImageInput');
    const profileImagePreview = document.getElementById('profileImagePreview');
    
    if (profileImageInput && profileImagePreview) {
        profileImageInput.addEventListener('change', function(event) {
            const file = event.target.files[0];
            console.log('File selected:', file);
            if (file) {
                const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
                if (!allowedTypes.includes(file.type)) {
                    alert('Please select a valid image file (JPEG, PNG, or GIF)');
                    this.value = '';
                    return;
                }
                
                // Validate file size (5MB max)
                const maxSize = 5 * 1024 * 1024; // 5MB in bytes
                if (file.size > maxSize) {
                    alert('File size must be less than 5MB');
                    this.value = '';
                    return;
                }
                
                // Show preview
                const reader = new FileReader();
                reader.onload = function(e) {
                    profileImagePreview.src = e.target.result;
                };
                reader.readAsDataURL(file);
            }
        });
    }
});

document.getElementById('profileForm').addEventListener('submit', function(e) {
    const saveButton = document.getElementById('saveButton');
    const originalText = saveButton.innerHTML;
    
    const formData = new FormData(this);
    console.log('Form data entries:');
    for (let [key, value] of formData.entries()) {
        console.log(key + ':', value);
    }
    
    saveButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Saving...';
    saveButton.disabled = true;
    

    const firstName = document.querySelector('input[name="firstname"]');
    const lastName = document.querySelector('input[name="lastname"]');
    
    if (firstName && firstName.value.trim() === '') {
        e.preventDefault();
        alert('First name is required');
        saveButton.innerHTML = originalText;
        saveButton.disabled = false;
        return;
    }
    
    if (lastName && lastName.value.trim() === '') {
        e.preventDefault();
        alert('Last name is required');
        saveButton.innerHTML = originalText;
        saveButton.disabled = false;
        return;
    }
});
