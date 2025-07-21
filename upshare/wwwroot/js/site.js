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

document
  .getElementById("profile-upload")
  .addEventListener("change", function (e) {
    const file = e.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = function (event) {
        document.getElementById("profile-preview").src = event.target.result;
      };
      reader.readAsDataURL(file);
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
